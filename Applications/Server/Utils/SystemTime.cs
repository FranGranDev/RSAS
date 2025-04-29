using System;

namespace Application.Utils
{
    public static class SystemTime
    {
        private static DateTime? _customTime;

        public static DateTime Now => _customTime ?? DateTime.Now;
        public static DateTime UtcNow => _customTime?.ToUniversalTime() ?? DateTime.UtcNow;

        public static void SetCustomTime(DateTime? customTime)
        {
            _customTime = customTime;
        }

        public static void Reset()
        {
            _customTime = null;
        }
    }
} 