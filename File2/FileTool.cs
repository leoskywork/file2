using File2.Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace File2
{
    class FileTool
    {
        //todo: release task once complete
        private readonly ConcurrentDictionary<string, FileTask<string>> _taskDictionary = new ConcurrentDictionary<string, FileTask<string>>();

        public static string AggregateFile(string sourceFolder, string targetFolder, Action<string> progress, CancellationTokenSource token = null)
        {
            //todo: doesn't call File.Move(..) if source and target have different drive volumes
            //ref: https://stackoverflow.com/questions/187768/can-i-show-file-copy-progress-using-fileinfo-copyto-in-net
            //https://stackoverflow.com/questions/882686/asynchronous-file-copy-move-in-c-sharp
            int filesMoved = 0;
            int filesMoveFiled = 0;
            int filesSkipped = 0;

            var files = Directory.EnumerateFiles(sourceFolder, "*.*", SearchOption.AllDirectories).ToList();
            files.Sort();

            foreach (var file in files)
            {
                System.Diagnostics.Debug.WriteLine("---> moving " + file);
                if (token?.IsCancellationRequested == true)
                {
                    System.Diagnostics.Debug.WriteLine("---> task canceled");
                    progress("task canceled");
                    Thread.Sleep(500);
                    break;
                }
                var count = filesMoved + filesMoveFiled + filesSkipped;
                var name = Path.GetFileName(file);
                string nameInfo = GetDisplayFileName(name);
                progress($"moving {count + 1}/{files.Count} {nameInfo}({(new FileInfo(file)).Length.ToFriendlyFileSize()})");
                //progress($"moving 999/999 {(new FileInfo(file)).Length.ToFriendlyFileSize()} ({nameInfo})...");
                string targetFile = Path.Combine(targetFolder, Path.GetFileName(file));

                if (File.Exists(targetFile))
                {
                    if (file == targetFile) //if same file
                    {
                        filesSkipped++;
                    }
                    //override file if they have same name and size, otherwise skip
                    else if (file != targetFile && (new FileInfo(file)).Length == (new FileInfo(targetFile)).Length)
                    {
                        File.Delete(targetFile);
                        File.Move(file, targetFile);
                        filesMoved++;
                    }
                    else
                    {
                        filesSkipped++;
                    }
                }
                else
                {
                    try
                    {
                        File.Move(file, targetFile);
                        filesMoved++;
                    }
                    catch (Exception)
                    {
                        filesMoveFiled++;
                    }
                }
                System.Diagnostics.Debug.WriteLine("---> done or skip " + file);
            }

            return $"Files moved: {filesMoved}{(filesMoveFiled > 0 ? ", failed: " + filesMoveFiled : "")}{(filesSkipped > 0 ? ", files skipped: " + filesSkipped : "")}";
        }

        private static string GetDisplayFileName(string name)
        {
            int maxFileNameLength = 24;//28;

            return name.Length > maxFileNameLength ? name.Substring(0, maxFileNameLength) + "..." : name;
        }

        public FileTask<string> AggregateFileAsync(string source, string target, Action<string> progress)
        {

            var tokenSource = new CancellationTokenSource();
            var fileTask = new FileTask<string>(() => AggregateFile(source, target, progress, tokenSource), tokenSource, "agt");
            _taskDictionary.TryAdd(fileTask.Key, fileTask);


            return fileTask;
        }

        public void Cancel(string taskKey)
        {
            if (taskKey == null) return;

            if (_taskDictionary.TryGetValue(taskKey, out FileTask<string> fileTask))
            {
                fileTask.TokenSource.Cancel();
                this.Cleanup(taskKey);
            }
        }

        public void Abort(string taskKey)
        {
            if (taskKey == null) return;

            if (Environment.MachineName == "LEO-ASUS") return;

            if (_taskDictionary.TryGetValue(taskKey, out FileTask<string> fileTask))
            {
                fileTask.TokenSource.Token.Register(() => { System.Diagnostics.Debug.WriteLine("abort cxl token callback"); });
                fileTask.TokenSource.Cancel();

                if (fileTask.Task.Status == TaskStatus.Running)
                {
                    //kill the task or return the task result right now
                    //https://stackoverflow.com/questions/4359910/is-it-possible-to-abort-a-task-like-aborting-a-thread-thread-abort-method
                    //https://stackoverflow.com/questions/4783865/how-do-i-abort-cancel-tpl-tasks

                    //Should not call Thread.Abort(), but do it for now

                }

                this.Cleanup(taskKey);
            }
        }

        public void Cleanup(string taskKey)
        {
            if (taskKey == null) return;

            _taskDictionary.TryRemove(taskKey, out _);
        }

        public FileTask<SubFolderResult> GetSubFolderInfoAsync(string path, long minSizeByte, Action<string> progress)
        {
            var tokenSource = new CancellationTokenSource();

            var fileTask = new FileTask<SubFolderResult>(() =>
            {
                var result = new SubFolderResult();// new List<Tuple<string, long>>();

                //get error with following api when test on c/user/leo, so try to manually loop through
                //var files = Directory.EnumerateFiles(path, "*.*", SearchOption.AllDirectories).ToList();

                tryTrackDirectory(path, tokenSource, minSizeByte, progress, result);

                var nullCount = result.FilesAndSizes.Where(i => i == null).ToList();

                if(nullCount.Count > 0)
                {
                    result.Errors.Add(new Exception("null item found in result.FilesAndSizes, count: " + nullCount.Count));
                    result.FilesAndSizes.RemoveAll(f => f == null);
                }


                //result.FilesAndSizes.Sort((a, b) => b.Item2 - a.Item2 > 0 ? 1 : -1); //fixme, null reference exception?? (when sizing c/user/leo)
                result.FilesAndSizes.Sort((a, b) => (b?.Item2 ?? 0) - (a?.Item2 ?? 0) > 0 ? 1 : -1);

                var size = result.FilesAndSizes.Select(f => f.Item2).Sum().ToFriendlyFileSize();
                progress($"file count: {result.Count.ToString("#,###")} total size: {size} (size > {minSizeByte.ToFriendlyFileSize()})");

                return result;
            }, tokenSource, "get-size");

            return fileTask;
        }

        private void tryTrackDirectory(string path, CancellationTokenSource token, long minSizeByte, Action<string> progress, SubFolderResult result)
        {
            try
            {

                var firstInnerFiles = Directory.GetFiles(path);
                trackFileSizes(firstInnerFiles, token, minSizeByte, progress, result);

                var firstInnerDirs = Directory.GetDirectories(path);

                foreach (var dir in firstInnerDirs)
                {
                    tryTrackDirectory(dir, token, minSizeByte, progress, result);
                }
            }
            catch (Exception ex)
            {
                //don't exit
                progress("skipping error: " + ex.Message);
                result.Errors.Add(ex);
            }
        }

        private void trackFileSizes(string[] files, CancellationTokenSource token, long minSizeByte, Action<string> progress, SubFolderResult result)
        {
            foreach (var file in files)
            {
                System.Diagnostics.Debug.WriteLine("---> sizing " + file);
                if (token.IsCancellationRequested)
                {
                    System.Diagnostics.Debug.WriteLine("---> task canceled");
                    progress("task canceled");
                    Thread.Sleep(500);
                    break;
                }
                result.Count++;

                var fileInfo = new FileInfo(file);
                var fileSize = fileInfo.Length;
                progress($"sizing {result.Count.ToString("#,###")} {GetDisplayFileName(fileInfo.Name)}({ fileSize.ToFriendlyFileSize()})");

                if (fileSize > minSizeByte)
                {
                    result.FilesAndSizes.Add(Tuple.Create(file, fileSize, result.Count));
                }

                //System.Diagnostics.Debug.WriteLine("---> done " + file);
            }
        }

    }

   
}
