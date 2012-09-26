using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fitbit.Models
{
    public class Sleep
    {
        public SleepSummary Summary { get; set; }
        public List<SleepLog> Sleeps { get; set; }
    }
}
