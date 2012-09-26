using System;
using System.Collections.Generic;
using System.Linq;

namespace Fitbit.Models
{
    public class SleepSummary
    {
        public int TotalMinutesAsleep { get; set; }
        public int TotalSleepRecords { get; set; }
        public int TotalTimeInBed { get; set; }
    }
}