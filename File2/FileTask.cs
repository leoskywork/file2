using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace File2
{
    class FileTask<T> : IDisposable
    {
        //fixme: ensure multi-thread safe
        private static int _Count = 0;
        private Thread _thread; //for hard cancel

        public Task<T> Task { get; }
        public CancellationTokenSource TokenSource { get; }
        public string Key { get; }

       


        public FileTask(Func<T> function, CancellationTokenSource source, string name)
        {
            _Count++;
            this.Key = $"{name}{_Count}_{Guid.NewGuid()}";

            this.TokenSource = source;
            this.Task = new Task<T>(() =>
            {
                _thread = Thread.CurrentThread;
                return function();
            }, source.Token);
        }


        public void Dispose()
        {
            this.Task?.Dispose();
            this.TokenSource.Dispose();
            _thread = null;
        }
    }

    class SubFolderResult
    {
        public List<Tuple<string, long, int, string>> FilesAndSizes { get; } = new List<Tuple<string, long, int,string>>();
        public List<Exception> Errors { get; } = new List<Exception>();
        public int Count { get; set; }
    }
}
