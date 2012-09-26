using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fitbit.Models
{
    public enum IntradayResourceType
    {
        [StringValue("/activities/calories")]
        CaloriesOut,
        [StringValue("/activities/steps")]
        Steps,
        [StringValue("/activities/floors")]
        Floors,
        [StringValue("/activities/elevation")]
        Elevation
    }
}
