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

        private Task<T> _Task;
        private DateTime _StartTime;
        private DateTime _FirstTaskEndTime;

        public CancellationTokenSource TokenSource { get; private set; }
        public string Key { get; }

        public T Result { get { return _Task.Result; } }
        public AggregateException Exception { get { return _Task.Exception; } }
        public TaskStatus Status { get { return _Task.Status; } }

        public TimeSpan FirstTaskSpent { get { return _FirstTaskEndTime - _StartTime; } }

        public FileTask(Func<T> function, CancellationTokenSource source, string namePrefix)
        {
            _Count++;
            this.Key = $"{namePrefix}{_Count}_{Guid.NewGuid()}";

            this.TokenSource = source;
            this._Task = new Task<T>(() =>
            {
                _thread = Thread.CurrentThread;
                return function();
            }, source.Token);
        }


        public void Dispose()
        {
            this._Task?.Dispose();
            this._Task = null;
            this.TokenSource.Dispose();
            this.TokenSource = null;
            _thread = null;
        }

        public void Start()
        {
            this._Task.Start();
            this._StartTime = DateTime.Now;
        }

        public Task ContinueWith(Action<Task<T>> action, TaskScheduler scheduler)
        {
            return this._Task.ContinueWith((preTask) =>
            {
                this._FirstTaskEndTime = DateTime.Now;
                action(preTask);
            }, scheduler);
        }

        public string GetUserFriendlySpent()
        {
            var spent = (int)this.FirstTaskSpent.TotalSeconds;

            if (spent < 1)
            {
                return "1s";
            }

            if (spent <= 60)
            {
                return $"{spent}s";
            }

            return $"{spent / 60}min{spent % 60}s";
        }
       
    }

    class SubFolderResult
    {
        public List<Tuple<string, long, int, string>> FilesAndSizes { get; } = new List<Tuple<string, long, int,string>>();
        public List<Exception> Errors { get; } = new List<Exception>();
        public int Count { get; set; }
    }
}
