using System;

namespace File2
{
    class TimeRange
    {
        public DateTime Start { get; set; }
        public DateTime? End { get; set; }
        public DateTime? CancellationTime { get; set; }

        public bool Canceled => CancellationTime.HasValue;

        public TimeRange()
        {
            this.Start = DateTime.UtcNow;
        }

        public TimeRange(DateTime start)
        {
            this.Start = start;
        }

        public TimeSpan GetPeriod()
        {
            return (this.End ?? DateTime.UtcNow) - this.Start;
        }

        public TimeSpan? GetCancellationPeriod()
        {
            return (this.End ?? DateTime.UtcNow) - this.CancellationTime;
        }

    }
}
