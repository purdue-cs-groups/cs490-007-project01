using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fitbit.Models
{
    public class Device
    {
        public string Battery { get; set; }
        public string Id { get; set; }
        public DateTime LastSyncTime { get; set; }
        public DeviceType Type { get; set; }
        public string DeviceVersion { get; set; }
    }
}
