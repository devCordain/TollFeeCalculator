using System;

namespace TollFeeCalculator
{
    class TollFeeInterval
    {
        public TimeSpan StartInterval { get; }
        public TimeSpan EndInterval { get; }
        public int Fee { get; }
        public TollFeeInterval(TimeSpan startInterval, TimeSpan endInterval, int fee)
        {
            StartInterval = startInterval;
            EndInterval = endInterval;
            Fee = fee;
        }
    }
}
