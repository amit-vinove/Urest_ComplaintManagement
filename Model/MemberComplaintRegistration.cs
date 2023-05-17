using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UrestComplaintManagement.Model
{
    public class MemberComplaintRegistration
    {
        public int Id { get; set; }
        public string Mobile { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }
}