using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TrashCollector.Models
{
    public class Pickup
    {
        public int PickupId { get; set; }
        public bool IsComplete { get; set; }
        public DateTime DateCompleted { get; set; }
        public Address Address { get; set; }
        public int AddressId { get; set; }
    }
}