using System;

namespace File2
{
    static class Utility
    {
        public static string ToFriendlyFileSize(this long bytes, string format = "n1", bool alwaysInMB = false)
        {
            if (alwaysInMB)
            {
                return $"{bytes * 1.0 / 1024.0 / 1024.0} MB";
            }
            else
            {
                if(bytes == 0)
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
    }
}
