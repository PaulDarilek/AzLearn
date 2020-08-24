using System;
using System.Linq;

namespace AzLearn.Core.Vision
{

    /// <summary>Delay schedule for polling Results from Vision API</summary>
    internal class PollingSchedule
    {
        private double BeforeElapsedSeconds;
        private int DelayMilliseconds;

        private static readonly PollingSchedule[] Delays =
        {
                new PollingSchedule{ BeforeElapsedSeconds = 2.5, DelayMilliseconds = 500 },
                new PollingSchedule{ BeforeElapsedSeconds = 5.0, DelayMilliseconds = 1000 },
                new PollingSchedule{ BeforeElapsedSeconds = 10.0, DelayMilliseconds = 2000 },
                new PollingSchedule{ BeforeElapsedSeconds = 15.0, DelayMilliseconds = 3000 },
                new PollingSchedule{ BeforeElapsedSeconds = 60.0, DelayMilliseconds = 5000 },
            };

        /// <summary>Maximum time to wait while Polling for a result</summary>
        public static TimeSpan Timeout { get; } = TimeSpan.FromSeconds(Delays.Last().BeforeElapsedSeconds);

        public static int GetDelayMilliseconds(TimeSpan elapsed)
            =>
            Delays
            .Where(x => elapsed.TotalSeconds <= x.BeforeElapsedSeconds)
            .Select(x => (int?)x.DelayMilliseconds)
            .FirstOrDefault()
            ?? Delays.Last().DelayMilliseconds;
    }
}
