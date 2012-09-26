using System;
using System.Collections.Generic;
using System.Linq;

namespace Fitbit.Models
{
    public class SleepLog
    {
        public bool IsMainSleep { get; set; }
        public long LogId { get; set; }
        public int Efficiency { get; set; }
        public DateTime StartTime { get; set; }
        public int Duration { get; set; }
        public int MinutesToFallAsleep { get; set; }
        public int MinutesAwake { get; set; }
        public int MinutesAfterWakeup { get; set; }
        public int AwakeningsCount { get; set; }
        public int TimeInBed { get; set; }
    }
}