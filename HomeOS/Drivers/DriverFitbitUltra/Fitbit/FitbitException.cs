using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace Fitbit.Api
{
    public class FitbitException : Exception
    {
        public HttpStatusCode HttpStatusCode { get; set; }

        public FitbitException(string message, HttpStatusCode statusCode) : base(message)
        {
            this.HttpStatusCode = statusCode;
        }
    }
}
