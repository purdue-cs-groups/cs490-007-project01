using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fitbit.Models
{
    public class UpdatedResource
    {
        public APICollectionType CollectionType { get; set; }
        public DateTime Date { get; set; }
        public string OwnerId { get; set; }
        public ResourceOwnerType OwnerType { get; set; }
        public string SubscriptionId { get; set; }
    }
}
