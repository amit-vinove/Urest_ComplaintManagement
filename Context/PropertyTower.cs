using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UrestComplaintManagement.Context
{
    public class PropertyTower
    {
        public int PropertyTowerId { get; set; }

        public string Towername { get; set; }
    }
    public class PropertyOwnerDetails
    {
        public string name { get; set; }
        public string Resident { get; set; }

    }
}