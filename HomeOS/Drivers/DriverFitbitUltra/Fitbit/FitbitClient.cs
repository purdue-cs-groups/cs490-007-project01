using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fitbit.Models;
using RestSharp;
using RestSharp.Authenticators;
using System.Xml.Linq;
using System.Web;
using System.Runtime.InteropServices;
using System.Net;

namespace Fitbit.Api
{
    public class FitbitClient
    {
        private string consumerKey;
        private string consumerSecret;
        private string accessToken;
        private string accessSecret;
        private IRestClient restClient;

        private string baseApiUrl = "https://api.fitbit.com";

        #region Constructors

        public FitbitClient(IRestClient restClient)
        {
            this.restClient = restClient;
            restClient.Authenticator = OAuth1Authenticator.ForProtectedResource(this.consumerKey, this.consumerSecret, this.accessToken, this.accessSecret);
        }

        public FitbitClient(string consumerKey, string consumerSecret)
        {
            this.consumerKey = consumerKey;
            this.consumerSecret = consumerSecret;
        }

        public FitbitClient(string consumerKey, string consumerSecret, string accessToken, string accessSecret)
        {
            this.consumerKey = consumerKey;
            this.consumerSecret = consumerSecret;
            this.accessToken = accessToken;
            this.accessSecret = accessSecret;
            this.restClient = new RestClient(baseApiUrl);

            restClient.Authenticator = OAuth1Authenticator.ForProtectedResource(this.consumerKey, this.consumerSecret, this.accessToken, this.accessSecret);
        }

        public FitbitClient(string consumerKey, string consumerSecret, string accessToken, string accessSecret, IRestClient restClient)
        {
            this.consumerKey = consumerKey;
            this.consumerSecret = consumerSecret;
            this.accessToken = accessToken;
            this.accessSecret = accessSecret;
            this.restClient = restClient;

            restClient.Authenticator = OAuth1Authenticator.ForProtectedResource(this.consumerKey, this.consumerSecret, this.accessToken, this.accessSecret);
        }

        #endregion

        #region Methods

        public UserProfile GetUserProfile()
        {
            return GetUserProfile(null);
        }

        public UserProfile GetUserProfile(string encodedUserId)
        {
            string apiCall;

            if (string.IsNullOrWhiteSpace(encodedUserId))
                apiCall = "/1/user/-/profile.xml";
            else
                apiCall = string.Format("/1/user/{0}/profile.xml", encodedUserId);

            RestRequest request = new RestRequest("/1/user/-/profile.xml");
            request.RootElement = "user";

            var response = restClient.Execute<UserProfile>(request);

            HandleResponseCode(response.StatusCode);

            return response.Data;
        }

        public ActivitySummary GetDayActivitySummary(DateTime activityDate)
        {
            string apiCall = GetActivityApiExtentionURL(activityDate);

            RestRequest request = new RestRequest(apiCall);
            request.RootElement = "summary";

            var response = restClient.Execute<Fitbit.Models.ActivitySummary>(request);

            HandleResponseCode(response.StatusCode);

            return response.Data;
        }

        public Activity GetDayActivity(DateTime activityDate)
        {
            string apiCall = GetActivityApiExtentionURL(activityDate);

            RestRequest request = new RestRequest(apiCall);

            var response = restClient.Execute<Fitbit.Models.Activity>(request);

            HandleResponseCode(response.StatusCode);

            return response.Data;
        }

        public SleepSummary GetDaySleepSummary(DateTime activityDate)
        {
            string apiCall = GetSleepApiExtentionURL(activityDate);

            RestRequest request = new RestRequest(apiCall);
            request.RootElement = "summary";

            var response = restClient.Execute<Fitbit.Models.SleepSummary>(request);

            HandleResponseCode(response.StatusCode);

            return response.Data;
        }

        public Sleep GetDaySleep(DateTime activityDate)
        {
            string apiCall = GetSleepApiExtentionURL(activityDate);

            RestRequest request = new RestRequest(apiCall);

            var response = restClient.Execute<Fitbit.Models.Sleep>(request);

            HandleResponseCode(response.StatusCode);

            return response.Data;
        }

        public Body GetDayBodyMeasurements(DateTime measurementsDate)
        {
            string apiCall = GetBodyApiExtentionURL(measurementsDate);

            RestRequest request = new RestRequest(apiCall);

            var response = restClient.Execute<Fitbit.Models.Body>(request);

            HandleResponseCode(response.StatusCode);

            return response.Data;
        }

        public List<UserProfile> GetFriends()
        {
            RestRequest request = new RestRequest("/1/user/-/friends.xml");
            request.RootElement = "friends";

            var response = restClient.Execute<List<Friend>>(request);

            HandleResponseCode(response.StatusCode);

            List<UserProfile> userProfiles = new List<UserProfile>();

            foreach (Friend friend in response.Data)
            {
                userProfiles.Add(friend.User);
            }

            return userProfiles;
        }

        public List<Device> GetDevices()
        {
            RestRequest request = new RestRequest("/1/user/-/devices.xml");

            request.OnBeforeDeserialization = resp =>
            {
                resp.ContentType = "application/xml";
            };

            var response = restClient.Execute<List<Device>>(request);

            HandleResponseCode(response.StatusCode);

            return response.Data;
        }

        public TimeSeriesDataList GetTimeSeries(TimeSeriesResourceType timeSeriesResourceType, DateTime startDate, DateTime endDate)
        {
            return GetTimeSeries(timeSeriesResourceType, startDate, endDate, null);
        }

        public TimeSeriesDataList GetTimeSeries(TimeSeriesResourceType timeSeriesResourceType, DateTime startDate, DateTime endDate, string userId)
        {
            string userSignifier = "-";
            if (!string.IsNullOrWhiteSpace(userId))
                userSignifier = userId;

            string requestUrl = string.Format("/1/user/{0}/{1}/date/{2}/{3}.xml", userSignifier, StringEnum.GetStringValue(timeSeriesResourceType), startDate.ToString("yyyy-MM-dd"), endDate.ToString("yyyy-MM-dd"));
            RestRequest request = new RestRequest(requestUrl);

            request.OnBeforeDeserialization = resp =>
            {
                XDocument doc = XDocument.Parse(resp.Content);
                var rootElement = doc.Descendants("result").FirstOrDefault().Descendants().FirstOrDefault();

                request.RootElement = rootElement.Name.LocalName;
            };

            var response = restClient.Execute<TimeSeriesDataList>(request);


            HandleResponseCode(response.StatusCode);

            return response.Data;
        }

        public TimeSeriesDataListInt GetTimeSeriesInt(TimeSeriesResourceType timeSeriesResourceType, DateTime startDate, DateTime endDate)
        {
            return GetTimeSeriesInt(timeSeriesResourceType, startDate, endDate, null);
        }

        public TimeSeriesDataListInt GetTimeSeriesInt(TimeSeriesResourceType timeSeriesResourceType, DateTime startDate, DateTime endDate, string userId)
        {
            string userSignifier = "-";
            if (!string.IsNullOrWhiteSpace(userId))
                userSignifier = userId;

            string requestUrl = string.Format("/1/user/{0}/{1}/date/{2}/{3}.xml", userSignifier, StringEnum.GetStringValue(timeSeriesResourceType), startDate.ToString("yyyy-MM-dd"), endDate.ToString("yyyy-MM-dd"));
            RestRequest request = new RestRequest(requestUrl);

            request.OnBeforeDeserialization = resp =>
            {
                XDocument doc = XDocument.Parse(resp.Content);
                var rootElement = doc.Descendants("result").FirstOrDefault().Descendants().FirstOrDefault();

                request.RootElement = rootElement.Name.LocalName;
            };

            var response = restClient.Execute<TimeSeriesDataListInt>(request);


            HandleResponseCode(response.StatusCode);

            return response.Data;
        }


        public IntradayData GetIntraDayTimeSeries(IntradayResourceType timeSeriesResourceType, DateTime dayAndStartTime, TimeSpan intraDayTimeSpan)
        {
            string requestUrl = null;

            if (intraDayTimeSpan > new TimeSpan(0, 1, 0) &&
                dayAndStartTime.Day == dayAndStartTime.Add(intraDayTimeSpan).Day)
            {
                requestUrl = string.Format("/1/user/-/{0}/date/{1}/1d/time/{2}/{3}.xml",
                    StringEnum.GetStringValue(timeSeriesResourceType),
                    dayAndStartTime.ToString("yyyy-MM-dd"),
                    dayAndStartTime.ToString("HH:mm"),
                    dayAndStartTime.Add(intraDayTimeSpan).ToString("HH:mm"));
            }
            else
            {
                requestUrl = string.Format("/1/user/-/{0}/date/{1}/1d.xml",
                                        StringEnum.GetStringValue(timeSeriesResourceType),
                                        dayAndStartTime.ToString("yyyy-MM-dd"));
            }

            RestRequest request = new RestRequest(requestUrl);

            request.OnBeforeDeserialization = resp =>
            {
                XDocument doc = XDocument.Parse(resp.Content);
                var rootElement = doc.Descendants("result").FirstOrDefault().Descendants().Where(t => t.Name.LocalName.Contains("-intraday")).FirstOrDefault();

                request.RootElement = rootElement.Name.LocalName;
            };

            var response = restClient.Execute<IntradayData>(request);

            HandleResponseCode(response.StatusCode);

            for (int i = 0; i < response.Data.DataSet.Count; i++)
            {
                response.Data.DataSet[i].Time = new DateTime(
                    dayAndStartTime.Year,
                    dayAndStartTime.Month,
                    dayAndStartTime.Day,
                    response.Data.DataSet[i].Time.Hour,
                    response.Data.DataSet[i].Time.Minute,
                    response.Data.DataSet[i].Time.Second);
            }

            return response.Data;
        }

        #endregion

        #region Helper Methods

        private void HandleResponseCode(System.Net.HttpStatusCode httpStatusCode)
        {
            if (httpStatusCode == System.Net.HttpStatusCode.OK ||
                httpStatusCode == System.Net.HttpStatusCode.Created ||
                httpStatusCode == System.Net.HttpStatusCode.NoContent)
            {
                return;
            }
            else
            {
                Console.WriteLine("HttpError:" + httpStatusCode.ToString());

                throw new FitbitException("Http Error:" + httpStatusCode.ToString(), httpStatusCode);
            }
        }

        private string GetActivityApiExtentionURL(DateTime activityDate)
        {
            const string ApiExtention = "/1/user/-/activities/date/{0}-{1}-{2}.xml";
            return string.Format(ApiExtention, activityDate.Year.ToString(), activityDate.Month.ToString(), activityDate.Day.ToString());
        }

        private string GetSleepApiExtentionURL(DateTime activityDate)
        {
            const string ApiExtention = "/1/user/-/sleep/date/{0}-{1}-{2}.xml";
            return string.Format(ApiExtention, activityDate.Year.ToString(), activityDate.Month.ToString(), activityDate.Day.ToString());
        }

        private string GetBodyApiExtentionURL(DateTime activityDate)
        {
            const string ApiExtention = "/1/user/-/body/date/{0}-{1}-{2}.xml";
            return string.Format(ApiExtention, activityDate.Year.ToString(), activityDate.Month.ToString(), activityDate.Day.ToString());
        }

        #endregion
    }
}
