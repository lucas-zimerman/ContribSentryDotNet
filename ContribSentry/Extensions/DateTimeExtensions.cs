using System;

namespace ContribSentry.Extensions
{
    internal static class DateTimeExtensions
    {
        private static readonly DateTime _epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        internal static double TotalUtcSeconds(this DateTime date)
        {
            return (date - _epoch).TotalSeconds;
        }

        internal static double TotalUtcMiliseconds(this DateTime date)
        {
            return (date - _epoch).TotalMilliseconds;
        }
    }
}
