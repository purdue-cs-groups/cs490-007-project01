using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fitbit.Models
{
    public class ActivityLog
    {
        public long ActivityId {get; set;}
        public long ActivityParentId { get; set; }
        public int Calories { get; set; }
        public string Description { get; set; }
        public float Distance { get; set; }
        public long Duration { get; set; }
        public bool HasStartTime { get; set; }
        public bool IsFavorite { get; set; }
        public long LogId { get; set; }
        public string Name { get; set; }
        public string StartTime { get; set; }
        public int Steps { get; set; }
    }
}
