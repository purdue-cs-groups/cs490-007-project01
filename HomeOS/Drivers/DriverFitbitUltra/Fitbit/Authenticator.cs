using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RestSharp;
using RestSharp.Authenticators;
using RestSharp.Contrib;
using Fitbit.Models;

namespace Fitbit.Api
{
    public class Authenticator
    {
        private string ConsumerKey;
        private string ConsumerSecret;
        private string RequestTokenUrl;
        private string AccessTokenUrl;
        private string AuthorizeUrl; 

        public Authenticator(string ConsumerKey, string ConsumerSecret, string RequestTokenUrl, string AccessTokenUrl, string AuthorizeUrl)
        {
            this.ConsumerKey = ConsumerKey;
            this.ConsumerSecret = ConsumerSecret;
            this.RequestTokenUrl = RequestTokenUrl;
            this.AccessTokenUrl = AccessTokenUrl;
            this.AuthorizeUrl = AuthorizeUrl;
        }

        public string GetAuthUrlToken()
        {
			var baseUrl = "https://api.fitbit.com";
			var client = new RestClient(baseUrl);
			client.Authenticator = OAuth1Authenticator.ForRequestToken(this.ConsumerKey, this.ConsumerSecret);
            
            var request = new RestRequest("oauth/request_token", Method.POST);
			var response = client.Execute(request);

            if(response.StatusCode != System.Net.HttpStatusCode.OK)
                throw new Exception("Request Token Step Failed");

			var qs = HttpUtility.ParseQueryString(response.Content);
			var oauth_token = qs["oauth_token"];
			var oauth_token_secret = qs["oauth_token_secret"];

			request = new RestRequest("oauth/authorize");
			request.AddParameter("oauth_token", oauth_token);
			var url = client.BuildUri(request).ToString();

            return url;
        }

        public AuthCredential ProcessApprovedAuthCallback(string TempAuthToken, string Verifier)
        {
            var baseUrl = "https://api.fitbit.com";
            var client = new RestClient(baseUrl);
            client.Authenticator = OAuth1Authenticator.ForRequestToken(this.ConsumerKey, this.ConsumerSecret);

            var request = new RestRequest("oauth/access_token", Method.POST);            

            client.Authenticator = OAuth1Authenticator.ForAccessToken(
                this.ConsumerKey, this.ConsumerSecret, TempAuthToken, "123456", Verifier
            );
            
            var response = client.Execute(request);

            var qs = HttpUtility.ParseQueryString(response.Content);
            var oauth_token = qs["oauth_token"];
            var oauth_token_secret = qs["oauth_token_secret"];
            var encoded_user_id = qs["encoded_user_id"];

            return new AuthCredential()
            {
                AuthToken = oauth_token,
                AuthTokenSecret = oauth_token_secret,
                UserId = encoded_user_id
            };
        }
    }
}
