﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace File2
{
    class FileTool
    {
        //fixme: ensure multi-thread safe
        private static int _aggregatingCount = 0;
        //todo: release task once complete
        private readonly ConcurrentDictionary<string, Tuple<Task<string>, CancellationTokenSource>> _taskDictionary = new ConcurrentDictionary<string, Tuple<Task<string>, CancellationTokenSource>>();

        public static string AggregateFile(string sourceFolder, string targetFolder, Action<string> progress)
        {
            int filesMoved = 0;
            int filesMoveFiled = 0;
            int filesSkipped = 0;
            int maxFileNameLength = 28;

            var files = Directory.EnumerateFiles(sourceFolder, "*.*", SearchOption.AllDirectories).ToList();
            files.Sort();

            foreach (var file in files)
            {
                System.Diagnostics.Debug.WriteLine("---> moving " + file);
                var count = filesMoved + filesMoveFiled + filesSkipped;
                var name = Path.GetFileName(file);
                var nameInfo = name.Length > maxFileNameLength ? name.Substring(0, maxFileNameLength) : name;
                progress($"moving {count + 1}/{files.Count} {(new FileInfo(file)).Length.ToFriendlyFileSize()} ({nameInfo})...");
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

        public KeyValuePair<string, Task<string>> AggregateFileAsync(string source, string target, Action<string> progress)
        {
            _aggregatingCount++;
            var cancel = new CancellationTokenSource();
            var task = new Task<string>(() => AggregateFile(source, target, progress));
            var key = $"agt{_aggregatingCount}_{Guid.NewGuid()}";
            _taskDictionary.TryAdd(key, Tuple.Create(task, cancel));
            return new KeyValuePair<string, Task<string>>(key, task);
        }

        public void Cancel(string taskKey)
        {
             
        }
    }
}
