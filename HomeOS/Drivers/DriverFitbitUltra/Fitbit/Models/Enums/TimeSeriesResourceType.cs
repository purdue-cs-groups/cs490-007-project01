using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fitbit.Models
{
    public enum TimeSeriesResourceType
    {
        [StringValue("activities/calories")]
        CaloriesOut,
        [StringValue("activities/steps")]
        Steps,
        [StringValue("activities/distance")]
        Distance,
        [StringValue("activities/minutesSedentary")]
        MinutesSedentary,
        [StringValue("activities/minutesLightlyActive")]
        MinutesLightlyActive,
        [StringValue("activities/minutesFairlyActive")]
        MinutesFairlyActive,
        [StringValue("activities/minutesVeryActive")]
        MinutesVeryActive,
        [StringValue("activities/activeScore")]
        ActiveScore,
        [StringValue("activities/activityCalories")]
        ActivityCalories,
        [StringValue("activities/floors")]
        Floors,
        [StringValue("activities/elevation")]
        Elevation,
        [StringValue("activities/tracker/calories")]
        CaloriesOutTracker,
        [StringValue("activities/tracker/steps")]
        StepsTracker,
        [StringValue("activities/tracker/distance")]
        DistanceTracker,
        [StringValue("activities/tracker/activeScore")]
        ActiveScoreTracker,
        [StringValue("/activities/tracker/activityCalories")]
        ActivityCaloriesTracker,
        [StringValue("activities/tracker/floors")]
        FloorsTracker,
        [StringValue("activities/tracker/elevation")]
        ElevationTracker,
        [StringValue("activities/tracker/minutesSedentary")]
        MinutesSedentaryTracker,
        [StringValue("activities/tracker/minutesLightlyActive")]
        MinutesLightlyActiveTracker,
        [StringValue("activities/tracker/minutesFairlyActive")]
        MinutesFairlyActiveTracker,
        [StringValue("activities/tracker/minutesVeryActive")]
        MinutesVeryActiveTracker,
        [StringValue("sleep/minutesAsleep")]
        MinutesAsleep,
        [StringValue("sleep/minutesAwake")]
        MinutesAwake,
        [StringValue("sleep/awakeningsCount")]
        AwakeningsCount,
        [StringValue("sleep/timeInBed")]
        TimeInBed,
        [StringValue("sleep/minutesToFallAsleep")]
        MinutesToFallAsleep,
        [StringValue("sleep/minutesAfterWakeup")]
        MinutesAfterWakeup,
        [StringValue("sleep/startTime")]
        TimeEnteredBed,
        [StringValue("sleep/efficiency")]
        SleepEfficiency,
        [StringValue("body/weight")]
        Weight,
        [StringValue("body/bmi")]
        BMI,
        [StringValue("body/fat")]
        Fat
    }    
}
