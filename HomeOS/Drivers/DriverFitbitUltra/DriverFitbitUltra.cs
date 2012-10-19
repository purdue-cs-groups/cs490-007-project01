using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;
using System.IO;
using Common;

using Fitbit.Api;
using Fitbit.Models;

namespace DriverNotifications
{
    [System.AddIn.AddIn("DriverFitbitUltra")]
    public class DriverFitbitUltra : Common.ModuleBase
    {
        // Create a Fitbit API application online and then follow the authentication
        // steps at http://term.ie/oauth/example/client.php to request your access 
        // token and secret.

        string consumerKey;
        string consumerSecret;
        string accessToken;
        string accessSecret;

        public override void Start()
        {            
            string[] words = moduleInfo.Args();

            consumerKey = words[0];
            consumerSecret = words[1];
            accessToken = words[2];
            accessSecret = words[3];

            View.VPortInfo portInfo = GetPortInfoFromPlatform("fitbitUltra");
            Port port = InitPort(portInfo);

            var roleList = new List<View.VRole>() { new RoleFitbitUltra() };

            BindRoles(port, roleList, OnOperationInvoke);

            RegisterPortWithPlatform(port);
        }

        public IList<View.VParamType> OnOperationInvoke(string roleName, String opName, IList<View.VParamType> parameters)
        {
            List<View.VParamType> retVals = new List<View.VParamType>();

            FitbitClient client = new FitbitClient(consumerKey, consumerSecret, accessToken, accessSecret);
            
            switch (opName)
            {
                case RoleFitbitUltra.OpGetActiveScore:
                    {
                        DateTime activityDate = (DateTime)parameters[0].Value();
                        ActivitySummary data = client.GetDayActivitySummary(activityDate);

                        int result = data.ActiveScore;
                        retVals.Add(new ParamType(ParamType.SimpleType.integer, "int", result, "result"));
                    }
                    break;

                case RoleFitbitUltra.OpGetCaloriesOut:
                    {
                        DateTime activityDate = (DateTime)parameters[0].Value();
                        ActivitySummary data = client.GetDayActivitySummary(activityDate);

                        int result = data.CaloriesOut;
                        retVals.Add(new ParamType(ParamType.SimpleType.integer, "int", result, "result"));
                    }
                    break;

                case RoleFitbitUltra.OpGetDistance:
                    {
                        DateTime activityDate = (DateTime)parameters[0].Value();
                        ActivitySummary data = client.GetDayActivitySummary(activityDate);

                        float result = data.Distances.Sum(z => z.Distance);
                        retVals.Add(new ParamType(ParamType.SimpleType.binary, "float", result, "result"));
                    }
                    break;

                case RoleFitbitUltra.OpGetSteps:
                    {
                        DateTime activityDate = (DateTime)parameters[0].Value();
                        ActivitySummary data = client.GetDayActivitySummary(activityDate);

                        int result = data.Steps;
                        retVals.Add(new ParamType(ParamType.SimpleType.integer, "int", result, "result"));
                    }
                    break;

                case RoleFitbitUltra.OpGetStepsGoal:
                    {
                        DateTime activityDate = (DateTime)parameters[0].Value();
                        Activity data = client.GetDayActivity(activityDate);

                        int result = data.Goals.Steps;
                        retVals.Add(new ParamType(ParamType.SimpleType.integer, "int", result, "result"));
                    }
                    break;

                case RoleFitbitUltra.OpGetTotalMinutesAsleep:
                    {
                        DateTime activityDate = (DateTime)parameters[0].Value();
                        SleepSummary data = client.GetDaySleepSummary(activityDate);

                        int result = data.TotalMinutesAsleep;
                        retVals.Add(new ParamType(ParamType.SimpleType.integer, "int", result, "result"));
                    }
                    break;

                case RoleFitbitUltra.OpGetTotalSleepRecords:
                    {
                        DateTime activityDate = (DateTime)parameters[0].Value();
                        SleepSummary data = client.GetDaySleepSummary(activityDate);

                        int result = data.TotalSleepRecords;
                        retVals.Add(new ParamType(ParamType.SimpleType.integer, "int", result, "result"));
                    }
                    break;

                case RoleFitbitUltra.OpGetTotalTimeInBed:
                    {
                        DateTime activityDate = (DateTime)parameters[0].Value();
                        SleepSummary data = client.GetDaySleepSummary(activityDate);

                        int result = data.TotalTimeInBed;
                        retVals.Add(new ParamType(ParamType.SimpleType.integer, "int", result, "result"));
                    }
                    break;

                case RoleFitbitUltra.OpGetDevicePresence:
                    {
                        List<Fitbit.Models.Device> data = client.GetDevices();
                        
                        bool result = data.Where(z => z.Type == DeviceType.Tracker).FirstOrDefault().LastSyncTime >= DateTime.Now.AddMinutes(-15);
                        retVals.Add(new ParamType(ParamType.SimpleType.binary, "bool", result, "result"));
                    }
                    break;

                case RoleFitbitUltra.OpGetHasRecentActivity:
                    {
                        Activity data = client.GetDayActivity(DateTime.Now);

                        bool result = data.Activities.Where(z => z.HasStartTime == true &&
                                                                 Convert.ToDateTime(z.StartTime).AddSeconds(z.Duration / 1000) >= DateTime.Now.AddMinutes(-30)).Count() > 0;
                        retVals.Add(new ParamType(ParamType.SimpleType.binary, "bool", result, "result"));
                    }
                    break;

                default:
                    logger.Log("Invalid operation {0}", opName);
                    break;
            }


            return retVals;
        }

        public override void Stop() { }
        public override void PortRegistered(View.VPort port) { }
        public override void PortDeregistered(View.VPort port) { }
    }
}
