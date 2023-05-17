using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UrestComplaintManagement.Context;
using UrestComplaintManagement.Model;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace UrestComplaintManagement.Controllers
{
    public class CategoryController : Controller
    {
        string constr = string.Empty;

        // GET: Category

        public CategoryController()
        {
            constr = ConfigurationManager.ConnectionStrings["adoConnectionstring"].ConnectionString;
        }

        //public ActionResult Index()
        //{
        //    UfirmApp_ProductionEntities7 entities = new UfirmApp_ProductionEntities7();
        //    CategoryModel model = new CategoryModel();
        //    foreach (var category in entities.TaskCategoryMasters)
        //    {
        //        model.TaskCategory.Add(new SelectListItem { Text = category.CategoryName, Value = category.CategoryId.ToString() });
        //    }
        //    return View(model);
        //}

        //[HttpPost]
        //public ActionResult Index(int? categoryId, int? subCategoryId)
        //{
        //    CategoryModel model = new CategoryModel();
        //    UfirmApp_ProductionEntities7 entities = new UfirmApp_ProductionEntities7();
        //    foreach (var item in entities.TaskCategoryMasters)
        //    {
        //        model.TaskCategory.Add(new SelectListItem { Text = item.CategoryName, Value = item.CategoryId.ToString() });
        //    }

        //    if (categoryId.HasValue)
        //    {
        //        var subCategories = (from item in entities.TaskSubCategoryMasters
        //                             where item.CategoryId == categoryId.Value
        //                             select item).ToList();
        //        foreach (var subCat in subCategories)
        //        {
        //            model.TaskSubCategory.Add(new SelectListItem { Text = subCat.SubCategoryName, Value = subCat.SubCategoryId.ToString() });
        //        }
        //    }
        //    return View(model);
        //}

        public JsonResult GetCategory()
        {
            List<SelectListItem> Categoryname = new List<SelectListItem>();
            DataSet ds = CategoryList();
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                Categoryname.Add(new SelectListItem { Text = ds.Tables[0].Rows[i][0].ToString(), Value = ds.Tables[0].Rows[i][1].ToString() });
            }
            return Json(Categoryname, JsonRequestBehavior.AllowGet);
        }

        public DataSet CategoryList()
        {
            SqlConnection connection;
            SqlDataAdapter adapter;
            SqlCommand command = new SqlCommand();
            //  SqlParameter param;
            DataSet ds = new DataSet();
            connection = new SqlConnection(constr);
            connection.Open();
            command.Connection = connection;
            command.CommandType = CommandType.Text;
            command.CommandText = "select CategoryName,CategoryId from TaskCategoryMaster ";
            command.CommandText = "select CategoryName,ScheduleCategoryId CategoryId from [calendar].Category order by CategoryName";

            // param = new SqlParameter();
            // param.Direction = ParameterDirection.Input;
            // param.DbType = DbType.String;
            // command.Parameters.Add(param);
            adapter = new SqlDataAdapter(command);
            adapter.Fill(ds);
            return ds;
        }

        public JsonResult GetSubCategory(int CategoryId)
        {
            List<SelectListItem> subcategoryname = new List<SelectListItem>();
            DataSet ds = SubCategoryList(CategoryId);
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                subcategoryname.Add(new SelectListItem { Text = ds.Tables[0].Rows[i][0].ToString(), Value = ds.Tables[0].Rows[i][1].ToString() });
            }
            return Json(subcategoryname, JsonRequestBehavior.AllowGet);
        }

        public DataSet SubCategoryList(int Id)
        {
            SqlConnection connection;
            SqlDataAdapter adapter;
            SqlCommand command = new SqlCommand();
            SqlParameter param;
            DataSet ds = new DataSet();
            connection = new SqlConnection(constr);
            connection.Open();
            command.Connection = connection;
            command.CommandType = CommandType.Text;
            command.CommandText = "select SubCategoryName,SubCategoryId from SubCategory  where CategoryId = @Id ";
            param = new SqlParameter("@Id", Id);
            // param = new SqlParameter();
            param.Direction = ParameterDirection.Input;
            param.DbType = DbType.String;
            command.Parameters.Add(param);
            adapter = new SqlDataAdapter(command);
            adapter.Fill(ds);
            return ds;
        }
            
        IEnumerable<TaskTransactionModel> TaskTrans(int CategoryId, int SubCategoryId) 
        {
            try
            {
                SqlConnection connection;
                SqlDataAdapter adapter;
                SqlCommand command = new SqlCommand();
                SqlParameter param;
                DataSet ds = new DataSet();
                connection = new SqlConnection(constr);
                connection.Open();
                command.Connection = connection;
                command.CommandType = CommandType.Text;
                //  command.CommandText = "select Id,CategoryId,SubCategoryId,Name, Description,DateFrom,Remarks,Occurence from [dbo].[TaskTransaction] where categoryId = @Id and SubcategoryId = @SId";
                // command.CommandText = @"select cm.CategoryName + ' / '+ sm.SubCategoryName as [Category],t.Name+ '/'+ isnull(t.Description,'') as Name ,t.Occurence from TaskTransaction t left join  TaskCategoryMaster cm on t.CategoryId=cm.CategoryId  left join TaskSubCategoryMaster sm on t.SubCategoryId=sm.SubCategoryId where t.categoryId = @Id and t.SubcategoryId = @SId";
                command.CommandText = "select cm.CategoryName + ' / '+ sm.SubCategoryName as [Category],t.Name+ '/'+ isnull(t.Description,'') as Name ,t.Occurence,isnull(t.Remarks,'') as Remarks from TaskMaster t left join [calendar].Category cm on t.CategoryId=cm.ScheduleCategoryId  left join SubCategory sm on t.SubCategoryId=sm.SubCategoryId where 1=1 ";

                if (CategoryId > 0)
                {
                    command.Parameters.AddWithValue("@Id", CategoryId);
                    command.CommandText = command.CommandText + " and t.categoryId = @Id";
                }
                if (SubCategoryId > 0)
                {
                    command.Parameters.AddWithValue("@SId", SubCategoryId);
                    command.CommandText = command.CommandText + " and t.SubcategoryId = @SId";
                }
                adapter = new SqlDataAdapter(command);
                adapter.Fill(ds);

                connection.Close();

                var data = (ds.Tables[0].AsEnumerable().Select(dataRow => new TaskTransactionModel
                {
                    CategoryName = dataRow.Field<string>("Category"),
                    Name = dataRow.Field<string>("Name"),
                    Occurence = dataRow.Field<string>("Occurence"),
                    Remarks = dataRow.Field<string>("Remarks"),
                    TaskCategoryId = CategoryId,
                    TaskSubCategoryId = SubCategoryId,
                    categories = new List<Category>() { new Category() {  Id=8, name= "Plumbing" }, new Category() {  Id=9, name="Electrical"} }
                })).ToList();

                if (data.Count == 0)
                {
                data=new List<TaskTransactionModel>() { new TaskTransactionModel() {                    categories = new List<Category>() { new Category() {  Id=8, name= "Plumbing" }, new Category() {  Id=9, name="Electrical"} }
                } };
                }

                return data;
            }
            catch(Exception ex)
            { string error = ex.Message; }

            return null;
        }

        public ActionResult GetTaskTransaction(int CategoryId, int SubCategoryId)
        {
            var taskTransactionList = TaskTrans(CategoryId, SubCategoryId);

            return Json(taskTransactionList, JsonRequestBehavior.AllowGet);
        }

        private dynamic ToSelectList(DataTable dt, string v1, string v2)
        {
            throw new NotImplementedException();
        }

        public ActionResult TaskTransaction()
        {
            if (TempData["CategoryID"] != null)
            {
                ViewData["CategoryID"] = TempData["CategoryID"];
                ViewData["SubCategoryID"] = TempData["SubCategoryID"];
            }
            else
            {
                ViewData["CategoryID"] = 0;
                ViewData["SubCategoryID"] = 0;
            }
            ViewBag.Data = TaskTrans(Convert.ToInt32(ViewData["CategoryID"]), Convert.ToInt32(ViewData["SubCategoryID"]));
            return View(ViewBag.Data);
        }

        public ActionResult TaskTransactionRefresh(int catId, int subCatID)
        {
            TempData["CategoryID"] = catId;
            TempData["SubCategoryID"] = subCatID;
            return RedirectToAction("TaskTransactionRefreshData");
        }

        public ActionResult TaskTransactionRefreshData(string CategoryId)
        {
            ViewData["CategoryID"] = CategoryId;// TempData["CategoryID"];
            ViewData["SubCategoryID"] = TempData["SubCategoryID"];

            ViewBag.Data = TaskTrans(Convert.ToInt32(ViewData["CategoryID"]), Convert.ToInt32(ViewData["SubCategoryID"]));
            return View("TaskTransaction", ViewBag.Data);
        }

        [ChildActionOnly]
        public ActionResult TaskTransactionData()
        {
            ViewBag.Data = TaskTrans(Convert.ToInt32(ViewData["CategoryID"]), Convert.ToInt32(ViewData["SubCategoryID"]));
            //return Json(ViewBag.Data, JsonRequestBehavior.AllowGet);
            return PartialView("TaskTransactionData", ViewBag.Data);
        }

        public ActionResult SaveTaskWiseTran(List<TaskTransactionModel> taskTransactionModels,
            string Remarks_1, string transDate, string catID, string subCatID, string remarks, string taskName, string occurance, string action
            )
        {
            string strSQL = "INSERT INTO[dbo].[TaskWiseTransaction] ([TransactionDate],[CategoryID],[SubCategoryID],[Remarks],[TaskName],[Occurance],[Action]) VALUES ('" + transDate + "', '" + catID + "', '" + subCatID + "', '" + remarks + "', '" + taskName + "', '" + occurance + "', '" + action + "'";

            //SqlConnection connection;
            //SqlCommand command = new SqlCommand();
            //DataSet ds = new DataSet();
            //connection = new SqlConnection(constr);
            //connection.Open();
            //command.Connection = connection;
            //command.CommandType = CommandType.Text;
            //command.CommandText = strSQL;

            //command.ExecuteNonQuery();

            //connection.Close();

            ViewBag.Data = TaskTrans(Convert.ToInt32(ViewData["CategoryID"]), Convert.ToInt32(ViewData["SubCategoryID"]));
            return RedirectToAction("TaskTransaction");
        }

    }

}