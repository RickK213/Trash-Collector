using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TrashCollector.Models
{
    public class UserProfile
    {
        public int UserProfileId { get; set; }
        public ICollection<Address> Addresses { get; set; }
        public ICollection<TrashCollection> TrashCollections { get; set; }
    }
}