using System;

namespace File2.Common
{
    static class Utility
    {
        public static string ToFriendlyFileSize(this long bytes, string format = "n1", bool alwaysInMB = false)
        {
            if (alwaysInMB)
            {
                return bytes == 0 ? "0 MB" : FormatFileSize(bytes, 2.0, format, "MB");
            }
            else
            {
                if (bytes == 0)
                {
                    return "0 Byte";
                }
                else if (bytes > Math.Pow(1024.0, 4.0))
                {
                    return FormatFileSize(bytes, 4.0, format, "TB");
                }
                else if (bytes > Math.Pow(1024.0, 3.0))
                {
                    return FormatFileSize(bytes, 3.0, format, "GB");
                }
                else if (bytes > Math.Pow(1024.0, 2.0))
                {
                    return FormatFileSize(bytes, 2.0, format, "MB");
                }
                else if (bytes > 1024.0)
                {
                    return FormatFileSize(bytes, 1.0, format, "KB");
                }
                else
                {
                    return FormatFileSize(bytes, 0, format, bytes > 1 ? "Bytes" : "Byte");
                }
            }
        }

        private static string FormatFileSize(long bytes, double powerBy1024, string format, string unit)
        {
            return (bytes * 1.0 / Math.Pow(1024.0, powerBy1024)).ToString(format) + unit;
        }

        public static string ToFriendlyString(this TimeSpan span)
        {
            if (span.TotalMilliseconds >= 0)
            {
                return ToFriendlyStringPositiveValue(span);
            }
            else
            {
                return ToFriendlyStringPositiveValue(TimeSpan.FromMilliseconds(span.TotalMilliseconds * -1));
            }
        }

        private static string ToFriendlyStringPositiveValue(TimeSpan span)
        {
            if (span.TotalDays >= 1)
            {
                return $"{(int)span.TotalDays}d{(span.Hours > 0 ? " " + span.Hours + "h" : "")}";
            }
            else if (span.TotalHours >= 1)
            {
                return $"{(int)span.TotalHours}h{(span.Minutes > 0 ? " " + span.Minutes + "m" : "")}";
            }
            else if (span.TotalMinutes >= 1)
            {
                return $"{(int)span.TotalMinutes}m{(span.Seconds > 0 ? " " + span.Seconds + "s" : "")}";
            }
            else if (span.TotalSeconds >= 1)
            {
                return $"{(int)span.TotalSeconds}s";
            }
            else
            {
                return span.TotalSeconds == 0 ? "0s" : "1s";
            }
        }
    }
}
