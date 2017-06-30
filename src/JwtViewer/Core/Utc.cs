using System;

namespace JwtViewer.Core
{
    public static class Utc
    {
        public static DateTimeOffset Now => DateTimeOffset.UtcNow;
        public static readonly DateTimeOffset Epoch = new DateTimeOffset(1970, 01, 01, 0, 0, 0, TimeSpan.Zero);
    }
}