using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UrestComplaintManagement.Model
{
    public class TaskSubCategoryModel
    {
        public int Id { get; set; }

        public int TaskCategoryId { get; set; }

        public string Name { get; set; }
    }

}