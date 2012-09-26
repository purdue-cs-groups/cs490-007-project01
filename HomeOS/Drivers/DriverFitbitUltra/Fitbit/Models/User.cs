using System;
using System.Collections.Generic;
using System.Linq;

namespace Fitbit.Models
{
	public class User
	{
        public DateTime DateOfBirth { get; set; }
        public string DisplayName { get; set; }
        public string EncodedId { get; set; }
        public string Gender { get; set; }
        public decimal Height { get; set; }
        public long OffsetFromUTCMillis { get; set; }
        public decimal Weight { get; set; }
	}
}