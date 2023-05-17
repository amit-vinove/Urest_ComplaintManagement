using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UrestComplaintManagement.Context;

namespace UrestComplaintManagement.Model
{
    public class TaskCategoryModel
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public List<TaskSubCategoryModel> TaskSubCategoryModel { get; set; }

        public List<TaskTransactionModel> TaskTransactionModel { get; set; }
    }

}