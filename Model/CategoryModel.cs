using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace UrestComplaintManagement.Model
{
    public class CategoryModel
    {
        public CategoryModel()
        {
            this.TaskCategory = new List<SelectListItem>();

            this.TaskSubCategory = new List<SelectListItem>();
        }

        public List<SelectListItem> TaskCategory { get; set; }

        public List<SelectListItem> TaskSubCategory { get; set; }

        public int CategoryId { get; set; }

        public int SubCategoryId { get; set; }
    }

}