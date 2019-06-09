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

        public static string AggregateFile(string sourceFolder, string targetFolder, Action<string> progress, CancellationToken? token = null)
        {
            //todo: doesn't call File.Move(..) if source and target have different drive volumes
            //ref: https://stackoverflow.com/questions/187768/can-i-show-file-copy-progress-using-fileinfo-copyto-in-net
            //https://stackoverflow.com/questions/882686/asynchronous-file-copy-move-in-c-sharp
            int filesMoved = 0;
            int filesMoveFiled = 0;
            int filesSkipped = 0;
            int maxFileNameLength = 28;

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
                var nameInfo = name.Length > maxFileNameLength ? name.Substring(0, maxFileNameLength) + "..." : name;
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

        public FileTask<string> AggregateFileAsync(string source, string target, Action<string> progress)
        {
           
            var tokenSource = new CancellationTokenSource();
            var fileTask = new FileTask<string>(() => AggregateFile(source, target, progress, tokenSource.Token), tokenSource, "agt");
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


    }
}
