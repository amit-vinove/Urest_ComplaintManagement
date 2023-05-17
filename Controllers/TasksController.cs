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
using System.Drawing;
using ZXing;
using System.Web.Configuration;
using ZXing.QrCode.Internal;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Data.Common;

namespace UrestComplaintManagement.Controllers
{
    public class TasksController : Controller
    {
        string constr = string.Empty;

        public TasksController()
        {
            constr = ConfigurationManager.ConnectionStrings["adoConnectionstring"].ConnectionString;
            ViewData["CategoryID"] = "0";
            ViewData["SubCategoryID"] = "0";
            ViewData["Occurance"] = "";
            ViewData["TransDate"] = "";
            ViewData["TaskID"] = "0";
            ViewData["MobileNo"] = "";
        }

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
            command.CommandText = "select CategoryName,ScheduleCategoryId CategoryId from [calendar].Category order by CategoryName";

            // param = new SqlParameter();
            // param.Direction = ParameterDirection.Input;
            // param.DbType = DbType.String;
            // command.Parameters.Add(param);
            adapter = new SqlDataAdapter(command);
            adapter.Fill(ds);
            connection.Close();

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
            ViewData["SubCategories"] = subcategoryname;
            return Json(subcategoryname, JsonRequestBehavior.AllowGet);
        }

        public ActionResult EventList(int month, int year)
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

                command.CommandText = "select eve.EventId, seve.SubEventId, eve.Title EventName, eve.Repeat, seve.eventNumber, seve.startdate, seve.EndDate from [calendar].[Event] eve inner join [calendar].[SubEvent] seve on seve.EventId=eve.EventId where month(seve.StartDate)=@month and year(seve.startdate)=@year and eve.IsActive=1";

                param = new SqlParameter("@month", month);param.Direction = ParameterDirection.Input;param.DbType = DbType.String;command.Parameters.Add(param);
                param = new SqlParameter("@year", year); param.Direction = ParameterDirection.Input; param.DbType = DbType.String; command.Parameters.Add(param);

                adapter = new SqlDataAdapter(command);
                adapter.Fill(ds);

                List<EventList> eList = (ds.Tables[0].AsEnumerable().Select(dataRow => new EventList
                {
                    EventId = dataRow["EventId"] == DBNull.Value ? 0 : dataRow.Field<int>("EventId"),
                    SubEventId = dataRow["SubEventId"] == DBNull.Value ? 0 : dataRow.Field<int>("SubEventId"),
                    EventNumber = dataRow["EventNumber"] == DBNull.Value ? "" : dataRow.Field<string>("EventNumber"),
                    EventName = dataRow["EventName"] == DBNull.Value ? "" : dataRow.Field<string>("EventName"),
                    Repeat = dataRow["Repeat"] == DBNull.Value ? "" : dataRow.Field<string>("Repeat"),
                    Startdate = dataRow["Startdate"] == DBNull.Value ? null : dataRow.Field<DateTime?>("Startdate"),
                    EndDate = dataRow["EndDate"] == DBNull.Value ? null : dataRow.Field<DateTime?>("EndDate"),
                    StartdateS = dataRow["Startdate"] == DBNull.Value ? "" : dataRow.Field<DateTime>("Startdate").ToString("dd/MM/yy"),
                    EndDateS = dataRow["EndDate"] == DBNull.Value ? "" : dataRow.Field<DateTime>("EndDate").ToString("dd/MM/yy")
                })).ToList();

                return Json(eList, JsonRequestBehavior.AllowGet);
            }
            catch
            { return null; }
        }

        public ActionResult AssetsList()
        {
            SqlConnection connection;
            connection = new SqlConnection(constr);
            connection.Open();
            try
            {
                SqlDataAdapter adapter;
                SqlCommand command = new SqlCommand();
                SqlParameter param;
                DataSet ds = new DataSet();
                command.Connection = connection;
                command.CommandType = CommandType.Text;

                command.CommandText = "select * from [dbo].[AssetMaster] where (isdeleted is null or isdeleted=0) order by name";

                adapter = new SqlDataAdapter(command);
                adapter.Fill(ds);

                List<AssetsList> eList = (ds.Tables[0].AsEnumerable().Select(dataRow => new AssetsList
                {
                    Id = dataRow["Id"] == DBNull.Value ? 0 : dataRow.Field<int>("Id"),
                    Name = dataRow["name"] == DBNull.Value ? "" : dataRow.Field<string>("name"),
                    Description = dataRow["Description"] == DBNull.Value ? "" : dataRow["Description"].ToString(),
                    QRCode = dataRow["QRCode"] == DBNull.Value ? "" : dataRow["QRCode"].ToString(),
                    Isdeleted = dataRow["isdeleted"] == DBNull.Value ? 0 : dataRow.Field<int>("isdeleted")
                })).ToList();
                connection.Close();

                return Json(eList, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                connection.Close();
                return null;
            }

        }

        public ActionResult SpotVisit(string MobileNo)
        {
            ViewData["MobileNo"] = MobileNo;

            DataSet ds = new DataSet();
            using (SqlConnection con = new SqlConnection(constr))
            {
                con.Open();

                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = con;
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "select Id, SpotName, Latitude, Longitude, CreatedBy, CreatedOn from spotpointmaster order by SpotName";
                    cmd.ExecuteNonQuery();

                    DataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(ds);

                }
                con.Close();
            }

            var data = (ds.Tables[0].AsEnumerable().Select(dataRow => new SpotPointMaster
            {
                Id = dataRow.Field<int>("Id"),
                SpotName = dataRow.Field<string>("SpotName"),
                Latitude = dataRow.Field<decimal>("Latitude")
            })).ToList();


            return View();        
        }

        public List<SelectListItem> SupervisorList()
        {
            SqlConnection connection;
            connection = new SqlConnection(constr);
            connection.Open();
            try
            {
                SqlDataAdapter adapter;
                SqlCommand command = new SqlCommand();
                SqlParameter param;
                DataSet ds = new DataSet();
                command.Connection = connection;
                command.CommandType = CommandType.Text;

                command.CommandText = "select fac.name, fac.MobileNumber, fac.FacilityMemberId, fac.FacilityMasterId, fac.IsActive, fac.IsDeleted from [App].[FacilityMember] fac inner join [dbo].EmployeeList emp on fac.MobileNumber=emp.MobileNo where emp.Designation like '%superv%' and fac.IsActive=1 order by fac.name";

                adapter = new SqlDataAdapter(command);
                adapter.Fill(ds);

                List<SelectListItem> eList = (ds.Tables[0].AsEnumerable().Select(dataRow => new SelectListItem
                {
                    Text = dataRow["name"] == DBNull.Value ? "" : dataRow.Field<string>("name"),
                    Value = dataRow["MobileNumber"] == DBNull.Value ? "0" : dataRow["MobileNumber"].ToString()
                })).ToList();
                connection.Close();

                return eList;
            }
            catch
            {
                connection.Close();
                return null; 
            }
        }

        public JsonResult GetAssignToList()
        {
            List<SelectListItem> assignToList = new List<SelectListItem>();
            DataSet ds = AssignToList();
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                assignToList.Add(new SelectListItem { Text = ds.Tables[0].Rows[i][1].ToString(), Value = ds.Tables[0].Rows[i][0].ToString() });
            }
            ViewData["AssignToList"] = assignToList;
            return Json(assignToList, JsonRequestBehavior.AllowGet);
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
            command.CommandText = "select SubCategoryName,SubCategoryId from [calendar].[SubCategory] where CategoryId = @Id ";
            param = new SqlParameter("@Id", Id);
            // param = new SqlParameter();
            param.Direction = ParameterDirection.Input;
            param.DbType = DbType.String;
            command.Parameters.Add(param);
            adapter = new SqlDataAdapter(command);
            adapter.Fill(ds);
            return ds;
        }

        public DataSet AssignToList()
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
            command.CommandText = "select distinct fm.facilitymemberid, CONCAT(fm.name, ' - ', fm.mobilenumber, ' - ', fmas.FacilityName) name, fm.IsActive, fm.facilitymasterid from app.facilitymember fm inner join [App].[FacilityMaster] fmas on fmas.FacilityMasterId=fm.FacilityMasterId where 1=1 and fm.IsActive=1 order by name";
            adapter = new SqlDataAdapter(command);
            adapter.Fill(ds);
            return ds;
        }

        IEnumerable<TaskTransactionModel> TaskTrans(int CategoryId, int SubCategoryId, string Occurance, string transDate, string assignto, string qrCode, string MobileNo="", int month=0, int year=0)
        {
            try
            {
                SqlConnection connection;
                SqlDataAdapter adapter;
                DataSet ds = new DataSet();
                connection = new SqlConnection(constr);
                connection.Open();
                SqlCommand command = new SqlCommand();

                command.Connection = connection;
                command.CommandType = CommandType.Text;
                //command.CommandText = "Select id from [dbo].EmployeeList where MobileNo = @MobileNo";//[App].[FacilityMember]
                command.CommandText = "Select FacilityMemberId from [App].[FacilityMember] where MobileNumber = @MobileNo and isactive=1";//
                command.Parameters.AddWithValue("@MobileNo", MobileNo);
                var empID = command.ExecuteScalar();
                empID = empID == DBNull.Value ? 0 : empID;

                command = new SqlCommand();
                command.Connection = connection;
                command.CommandType = CommandType.Text;

                string strSQL = "select cm.CategoryName + ' / '+ sm.SubCategoryName as [Category],(t.Name+ '/'+ isnull(t.Description,'')) as Name, (trim(t.Occurence) + ' (' + cast(t.TimeFrom as varchar(8)) + ' - ' + cast(t.TimeTo as varchar(8)) + ')') as Occurence, isnull(t.Remarks,'') as Remarks, t.id as taskID, tm.modify, t.QRCode from TaskMaster t left join [calendar].Category cm on t.CategoryId=cm.ScheduleCategoryId  left join [calendar].[SubCategory] sm on t.SubCategoryId=sm.SubCategoryId left outer join (select distinct tm.id taskid, 1 as modify from TaskWiseTransaction t inner join taskmaster tm on t.TaskName=tm.Name) tm on t.id=tm.taskid where 1=1";

                if (CategoryId > 0)
                {
                    command.Parameters.AddWithValue("@Id", CategoryId);
                    strSQL += " and t.categoryId = @Id";
                }
                if (SubCategoryId > 0)
                {
                    command.Parameters.AddWithValue("@SId", SubCategoryId);
                    strSQL += command.CommandText + " and t.SubcategoryId = @SId";
                }
                if (Occurance == "0")
                {
                    command.Parameters.AddWithValue("@Occurance", "W");
                    strSQL += command.CommandText + " and t.Occurence = @Occurance and ([dbo].IsTransDateWeekly(t.DateFrom, '" + Convert.ToDateTime(transDate).ToString("MM/dd/yyyy") + "')) >0";
                }
                else if (Occurance == "1")
                {
                    command.Parameters.AddWithValue("@Occurance", "D");
                    strSQL += command.CommandText + " and t.Occurence = @Occurance";
                }
                else if (Occurance == "2")
                {
                    command.Parameters.AddWithValue("@Occurance", "M");
                    strSQL += command.CommandText + " and t.Occurence = @Occurance and datepart(day,t.datefrom) = " + Convert.ToDateTime(transDate).Day + "";
                }
                if (qrCode.Length > 0)
                {
                    command.Parameters.AddWithValue("@qrCode", qrCode);
                    strSQL += command.CommandText + " and t.QRCode = @qrCode";
                }
                if (Convert.ToInt32(empID) > 0)
                {
                    command.Parameters.AddWithValue("@assignto", empID);
                    strSQL += command.CommandText + " and t.assignto = @assignto";
                }
                else
                {
                    if (assignto.Length > 0)
                    {
                        if (Convert.ToInt32(assignto) > 0)
                        {
                            command.Parameters.AddWithValue("@assignto", assignto);
                            strSQL += command.CommandText + " and t.assignto = @assignto";
                        }
                    }
                }
                if (month > 0)
                {
                    command.Parameters.AddWithValue("@month", month);
                    strSQL += command.CommandText + " and month(t.DateFrom) = @month";
                }
                if (year > 0)
                {
                    command.Parameters.AddWithValue("@year", year);
                    strSQL += command.CommandText + " and year(t.DateFrom) = @year";
                }

                command.CommandText = strSQL;

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
                    TaskId = dataRow.Field<int>("taskID"),
                    EntryType = dataRow["modify"] == DBNull.Value ? 0 : dataRow.Field<int>("modify"),
                    QRCode = dataRow["QRCode"] == DBNull.Value ? "" : dataRow.Field<string>("QRCode"),
                    categories = new List<Category>() { new Category() { Id = 8, name = "Plumbing" }, new Category() { Id = 9, name = "Electrical" } }
                })).ToList();

                if (data.Count == 0)
                {
                    data = new List<TaskTransactionModel>()
                    { new TaskTransactionModel()
                        {
                            categories = new List<Category>()
                            {
                                new Category() {Id=8, name= "Plumbing" }, new Category() {  Id=9, name="Electrical"}
                            }
                        }
                    };
                }

                return data;
            }
            catch (Exception ex)
            { string error = ex.Message; }

            return null;
        }

        public ActionResult GetTaskTransaction(int CategoryId, int SubCategoryId, string Occurance, string TransDate, string Assignto, string MobileNo="", int month=0, int year=0)
        {
            TempData["SelectedCatID"] = CategoryId;
            TempData["SelectedSubCatID"] = SubCategoryId;
            TempData["SelectedOccurance"] = Occurance;

            var taskTransactionList = TaskTrans(CategoryId, SubCategoryId, Occurance, TransDate, Assignto, "", MobileNo, month, year);
            return Json(taskTransactionList, JsonRequestBehavior.AllowGet);
        }

        //
        public ActionResult GetTaskTransactionByQRCode(string qrCode)
        {
            var taskTransactionList = TaskTrans(0, 0, "", "", "", qrCode);

            return Json(taskTransactionList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult NotificationList(string MobileNo)
        {
            ViewBag.NotificationData = NotificationListData();
            ViewData["MobileNo"] = MobileNo;
            return View(NotificationListData());
        }

        public ActionResult NotificationData(string MobileNo = "", int notiID=0)
        {
            var NotificationData = NotificationListData(MobileNo, notiID);
            return Json(NotificationData, JsonRequestBehavior.AllowGet);
        }

        public ActionResult NotiDetails(int notiID)
        {
            UpdateNotification(notiID);
            var NotificationData = NotificationListData("",notiID);
            return PartialView("NotiDetails", NotificationData);
        }

        IEnumerable<Notification> NotificationListData(string MobileNo="", int notiID=0)
        {
            try
            {
                SqlConnection connection;
                SqlDataAdapter adapter;
                SqlCommand command = new SqlCommand();
                DataSet ds = new DataSet();
                connection = new SqlConnection(constr);
                connection.Open();

                command = new SqlCommand();
                command.CommandType = CommandType.Text;
                command.Connection = connection;
                string strSQL = "", UserID = "";
                if (MobileNo.Length > 0)
                {
                    strSQL = "select UserId from [Identity].Users where ContactNumber=@MobileNo";
                    command.Parameters.AddWithValue("@MobileNo", MobileNo);
                    command.CommandText = strSQL;
                    UserID = command.ExecuteScalar().ToString();
                }
                command = new SqlCommand();
                command.CommandType = CommandType.Text;
                command.Connection = connection;
                strSQL = "select * from [App].[Notification] where (IsDeleted is null or IsDeleted = 0)";
                if (notiID > 0)
                {
                    command.Parameters.AddWithValue("@NotificationId", notiID);
                    strSQL += " and NotificationId = @NotificationId";
                }
                if (UserID.Length > 0)
                {
                    strSQL += " and (createdby = " + UserID + " or createdby = 1)";
                }
                strSQL += " order by NotificationId desc";

                command.CommandText = strSQL;
                adapter = new SqlDataAdapter(command);
                adapter.Fill(ds);
                connection.Close();

                var data = (ds.Tables[0].AsEnumerable().Select(dataRow => new Notification
                {
                    NotificationId = dataRow.Field<int>("NotificationId"),
                    Subject = dataRow.Field<string>("Subject"),
                    Message = dataRow.Field<string>("Message"),
                    NotificationTypeId = dataRow.Field<int>("NotificationTypeId"),
                    PropertyGroupId = dataRow.Field<int>("PropertyGroupId"),
                    CreatedOn = dataRow.Field<DateTime>("CreatedOn"),
                    CreatedBy = dataRow.Field<int>("CreatedBy"),
                    ExpirtyDate = dataRow.Field<DateTime>("ExpirtyDate"),
                    PropertyId = dataRow.Field<int>("PropertyId"),
                    IsActive = dataRow.Field<bool>("IsActive"),
                    IsDeleted = dataRow.Field<bool>("IsDeleted"),
                    CrOnStr = dataRow.Field<DateTime>("CreatedOn").ToString("dd, MMM"),
                    ExDtStr = dataRow.Field<DateTime>("ExpirtyDate").ToString("dd, MMM"),
                    IsRead = dataRow["IsRead"] == DBNull.Value ? false : dataRow["IsRead"] == null ? false : dataRow.Field<bool>("IsRead")

                })).ToList();

                return data;
            }
            catch (Exception ex)
            { string error = ex.Message; }

            return null;
        }

        public ActionResult UpdateNotification(int notiID = 0)
        {
            try
            {
                SqlConnection connection;
                SqlCommand command = new SqlCommand();
                connection = new SqlConnection(constr);
                connection.Open();
                command = new SqlCommand();
                command.CommandType = CommandType.Text;
                command.Connection = connection;
                string strSQL = "update [App].[Notification] set isread=1 where NotificationId=@NotificationId";
                command.Parameters.AddWithValue("@NotificationId", notiID);
                command.CommandText = strSQL;
                command.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception ex)
            { string error = ex.Message; }

            return null;
        }

        public ActionResult TaskTransaction(string MobileNo)
        {
            List<SelectListItem> Categoryname = new List<SelectListItem>();
            DataSet ds = CategoryList();
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                Categoryname.Add(new SelectListItem { Text = ds.Tables[0].Rows[i][0].ToString(), Value = ds.Tables[0].Rows[i][1].ToString() });
            }
            ViewBag.Categories = Categoryname.AsEnumerable();

            ViewData["Occurance"] = "";
            ViewData["TransDate"] = "";
            ViewData["MobileNo"] = MobileNo;

            ViewBag.Data = TaskTrans(Convert.ToInt32(ViewData["CategoryID"]), Convert.ToInt32(ViewData["SubCategoryID"]), ViewData["Occurance"].ToString(), ViewData["TransDate"].ToString(), "", "");

            ViewBag.DataQuest = taskWiseQuestionnaire(Convert.ToInt32(ViewData["TaskID"]), "");
            if (ViewBag.DataQuest == null)
            { ViewBag.DataQuest = ""; }
            return View(ViewBag.DataQuest);
        }

        public ActionResult TaskList(string MobileNo)
        {
            List<SelectListItem> Categoryname = new List<SelectListItem>();
            DataSet ds = CategoryList();
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                Categoryname.Add(new SelectListItem { Text = ds.Tables[0].Rows[i][0].ToString(), Value = ds.Tables[0].Rows[i][1].ToString() });
            }
            ViewBag.Categories = Categoryname.AsEnumerable();

            ViewData["Occurance"] = "";
            ViewData["TransDate"] = "";
            ViewData["MobileNo"] = MobileNo;

            ViewBag.Data = TaskTrans(Convert.ToInt32(ViewData["CategoryID"]), Convert.ToInt32(ViewData["SubCategoryID"]), ViewData["Occurance"].ToString(), ViewData["TransDate"].ToString(), "", "");

            ViewBag.DataQuest = taskWiseQuestionnaire(Convert.ToInt32(ViewData["TaskID"]), "");
            if (ViewBag.DataQuest == null)
            { ViewBag.DataQuest = ""; }
            return View(ViewBag.DataQuest);
        }

        public ActionResult DeleteTran(int transID = 0)
        {
            SqlConnection connection;
            SqlCommand command = new SqlCommand();
            DataSet ds = new DataSet();
            connection = new SqlConnection(constr);
            connection.Open();

            try
            {
                string strSQL = "delete from [dbo].[TaskWiseTransaction] where TransactionID = @TransactionID";

                command.Parameters.AddWithValue("@TransactionID", transID);
                command.Connection = connection;
                command.CommandType = CommandType.Text;
                command.CommandText = strSQL;

                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                connection.Close();
            }
            finally
            {
                connection.Close();
            }

            //ViewData["SelectedCatID"] = CategoryId;
            //ViewData["SelectedSubCatID"] = SubCategoryId;
            //ViewData["SelectedOccurance"] = Occurance;
            //ViewData["SelectedTaskID"] = taskID;

            return RedirectToAction("TaskTransaction"); ;
        }
        
        public ActionResult SaveTaskWiseTran(List<TaskWiseTransaction> taskWiseTransaction)
        {
            SqlConnection connection;
            SqlCommand command = new SqlCommand();
            DataSet ds = new DataSet();
            connection = new SqlConnection(constr);
            connection.Open();

            try
            {
                foreach (var item in taskWiseTransaction)
                {
                    string strSQL = "";
                    string transDate = item.TransactionDate.ToString();
                    string transactionID = item.TransactionID.ToString();
                    string catID = item.CategoryID.ToString();
                    string subCatID = item.SubCategoryID.ToString();
                    string remarks = "";// item.Remarks;

                    if (item.Remarks != null)
                    {
                        if (item.Remarks.Length > 0)
                        {
                            remarks = item.Remarks;
                        }
                    }

                    string taskName = item.TaskName;
                    string occurance = item.Occurance;
                    string action = item.Action;
                    string questID = item.QuestID.ToString();

                    strSQL = "select id from [dbo].[TaskMaster] where name= '" + taskName + "'";

                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = strSQL;

                    int taskID = 0;
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = connection;
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = "select id from [dbo].[TaskMaster] where name=@TaskName";
                        cmd.Parameters.AddWithValue("@TaskName", taskName);
                        taskID=Convert.ToInt32(command.ExecuteScalar());
                    }
                    TempData["SelectedTaskID"] = taskID;

                    string oldAction = "";
                    if (item.TransactionID > 0)
                    {
                        using (SqlCommand cmd = new SqlCommand())
                        {
                            cmd.Connection = connection;
                            cmd.CommandType = CommandType.Text;
                            cmd.CommandText = "select action from [dbo].[TaskWiseTransaction] where TransactionID = @TransactionID";
                            cmd.Parameters.AddWithValue("@TransactionID", item.TransactionID);
                            oldAction= cmd.ExecuteScalar().ToString();
                        }

                        strSQL = "update [dbo].[TaskWiseTransaction] set [Remarks] = '" + item.Remarks + "',[Action] = '" + item.Action + "', [UpdatedOn] = getdate() where TransactionID = " + item.TransactionID.ToString();

                        //if (item.TransactionID > 0)
                        //{
                        //    strSQL = "update [dbo].[TaskWiseTransaction] set [Remarks] = '" + item.Remarks + "',[Action] = '" + item.Action + "', [UpdatedOn] = getdate() where TransactionID = " + item.TransactionID.ToString();
                        //}
                        //else
                        //{
                        //    strSQL = "INSERT INTO[dbo].[TaskWiseTransaction] ([TransactionDate],[CategoryID],[SubCategoryID],[Remarks],[TaskName],QuestID, [Occurance],[Action], [createdon]) VALUES ('" + item.TransactionDate.ToString("MM/dd/yyyy") + "', '" + catID + "', '" + subCatID + "', '" + remarks + "', '" + taskName + "', '" + questID + "', '" + occurance + "', '" + action + "', getdate())";
                        //}
                    }
                    else
                    {
                        strSQL = "INSERT INTO[dbo].[TaskWiseTransaction] ([TransactionDate],[CategoryID],[SubCategoryID],[Remarks],[TaskName],QuestID, [Occurance],[Action], [createdon]) VALUES ('" + item.TransactionDate.ToString("MM/dd/yyyy") + "', '" + catID + "', '" + subCatID + "', '" + remarks + "', '" + taskName + "', '" + questID + "', '" + occurance + "', '" + action + "', getdate())";
                    }
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = strSQL;

                    command.ExecuteNonQuery();

                    if (action != "No")
                    {
                        if (item.TransactionID > 0)
                        {
                            if (oldAction == "No")
                            {
                                using (SqlCommand cmd = new SqlCommand())
                                {
                                    cmd.Connection = connection;
                                    cmd.CommandType = CommandType.Text;
                                    cmd.CommandText = "update [dbo].taskstatus set ComplQuests = ComplQuests + 1, remarks = '" + remarks + "', updatedon=getdate() where [dbo].taskstatus.taskid = @TaskID";
                                    cmd.Parameters.AddWithValue("@TaskID", taskID);
                                    cmd.ExecuteNonQuery();
                                }
                            }
                        }
                        else
                        {
                            using (SqlCommand cmd = new SqlCommand())
                            {
                                cmd.Connection = connection;
                                cmd.CommandType = CommandType.Text;
                                cmd.CommandText = "update [dbo].taskstatus set ComplQuests = ComplQuests + 1, remarks = '" + remarks + "', updatedon=getdate() where [dbo].taskstatus.taskid = @TaskID";
                                cmd.Parameters.AddWithValue("@TaskID", taskID);
                                cmd.ExecuteNonQuery();
                            }
                        }
                    }
                    else
                    {
                        if (item.TransactionID > 0)
                        {
                            if (oldAction != "No")
                            {
                                using (SqlCommand cmd = new SqlCommand())
                                {
                                    cmd.Connection = connection;
                                    cmd.CommandType = CommandType.Text;
                                    cmd.CommandText = "update [dbo].taskstatus set ComplQuests = ComplQuests - 1, remarks = '" + remarks + "', updatedon=getdate() where [dbo].taskstatus.taskid = @TaskID";
                                    cmd.Parameters.AddWithValue("@TaskID", taskID);
                                    cmd.ExecuteNonQuery();
                                }
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                connection.Close();
            }
            finally
            {
                connection.Close();
            }

            return RedirectToAction("Signin", "Account");
        }

        public ActionResult GetTransaction(DateTime transDate, string occurerance)
        {
            SqlConnection connection;
            SqlCommand command = new SqlCommand();
            DataSet ds = new DataSet();
            connection = new SqlConnection(constr);
            connection.Open();
            List<TaskWiseTransaction> taskWiseTransaction = new List<TaskWiseTransaction>();
            SqlDataAdapter adapter = new SqlDataAdapter();

            try
            {
                if (occurerance == "0")
                    occurerance = "W";
                else
                    occurerance = "D";

                string strSQL = "SELECT [TransactionID],[TransactionDate],[CategoryID],[SubCategoryID],[Remarks],[TaskName],[Occurance],[Action], c.CategoryName FROM[UfirmApp_Production].[dbo].[TaskWiseTransaction] t inner join [calendar].[Category] c on c.ScheduleCategoryId=t.CategoryID where TransactionDate = @transactionDate and SUBSTRING(Occurance,1,1) = @occurance order by TransactionID";
                command.Connection = connection;
                command.CommandType = CommandType.Text;
                command.CommandText = strSQL;
                command.Parameters.AddWithValue("@transactionDate", transDate.ToString("MM/dd/yyyy"));
                command.Parameters.AddWithValue("@occurance", occurerance);

                adapter = new SqlDataAdapter(command);
                adapter.Fill(ds);

                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    taskWiseTransaction.Add(new TaskWiseTransaction
                    {
                        CategoryID = Convert.ToInt32(ds.Tables[0].Rows[i][2].ToString()),
                        SubCategoryID = Convert.ToInt32(ds.Tables[0].Rows[i][3].ToString()),
                        CategoryName = ds.Tables[0].Rows[i][8].ToString(),
                        TaskName = ds.Tables[0].Rows[i][5].ToString(),
                        Action = ds.Tables[0].Rows[i][7].ToString(),
                        Occurance = occurerance,
                        Remarks = ds.Tables[0].Rows[i][4].ToString()
                    });
                }
            }
            catch (Exception ex)
            {
                adapter.Dispose();
                ds.Dispose();
                connection.Close();
            }
            finally
            {
                adapter.Dispose();
                ds.Dispose();
                connection.Close();
            }

            return Json(taskWiseTransaction, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DataQuestions(int taskID, string QRCode = "")
        {
            if (taskID == 0)
            {
                taskID = Convert.ToInt32(TempData["SelectedTaskID"]);
            }
            else
            {
                TempData["SelectedTaskID"] = taskID;
            }
            var taskTransactionList = taskWiseQuestionnaire(taskID, QRCode);

            return Json(taskTransactionList, JsonRequestBehavior.AllowGet);
        }

        IEnumerable<TaskWiseQuestionnaire> taskWiseQuestionnaire(int taskID = 0, string QRCode = "")
        {
            try
            {
                SqlConnection connection;
                SqlDataAdapter adapter;
                SqlCommand command = new SqlCommand();
                SqlParameter param;
                DataSet ds = new DataSet();
                DataSet ds0 = new DataSet();
                connection = new SqlConnection(constr);
                connection.Open();
                bool exits = false;
                if (taskID > 0 || QRCode.Length > 5)
                {
                    command = new SqlCommand();
                    command.CommandType = CommandType.Text;
                    command.Connection = connection;
                    string strSQL = "select cat.CategoryName, scat.SubCategoryName from TaskMaster tm inner join[calendar].[Category] cat on tm.CategoryId = cat.ScheduleCategoryId inner join[calendar].[SubCategory] scat on tm.SubCategoryId = scat.SubCategoryId where 1=1";
                    if (taskID > 0)
                    {
                        command.Parameters.AddWithValue("@taskID", taskID);
                        strSQL += " and tm.id = @taskID";
                    }
                    if (QRCode.Length > 5)
                    {
                        command.Parameters.AddWithValue("@QRCode", QRCode);
                        strSQL += " and tm.QRCode = @QRCode";
                    }
                    command.CommandText = strSQL;
                    adapter = new SqlDataAdapter(command);
                    adapter.Fill(ds0, "CatDetails");

                    command = new SqlCommand();
                    command.CommandType = CommandType.Text;
                    command.Connection = connection;
                    strSQL = "select t.TransactionID, t.QuestID QuestionID, q.QuestionName, t.TaskName, tm.Occurence, t.Action, t.Remarks, tm.id taskid, t.TransactionDate from TaskWiseTransaction t inner join TaskMaster tm on tm.Name=t.TaskName inner join TaskWiseQuestionnaire q on t.QuestID=q.QuestionID where 1=1";
                    if (taskID > 0)
                    {
                        command.Parameters.AddWithValue("@taskID", taskID);
                        strSQL += " and tm.id = @taskID";
                    }
                    if (QRCode.Length > 5)
                    {
                        command.Parameters.AddWithValue("@QRCode", QRCode);
                        strSQL += " and tm.QRCode = @QRCode";
                    }
                    strSQL += " order by TransactionID";

                    command.CommandText = strSQL;
                    adapter = new SqlDataAdapter(command);
                    adapter.Fill(ds);
                    if (ds.Tables.Count > 0)
                    {
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            exits = true;
                        }
                    }

                    if (!exits)
                    {
                        command = new SqlCommand();
                        command.CommandType = CommandType.Text;
                        command.Connection = connection;
                        strSQL = "select 0 TransactionID, q.QuestionID, q.QuestionName, t.Name taskName, t.Occurence, '' Action, '' Remarks, t.DateFrom TransactionDate, t.id TaskId from TaskWiseQuestionnaire q inner join TaskMaster t on q.taskid = t.id where 1=1";

                        if (taskID > 0)
                        {
                            command.Parameters.AddWithValue("@taskID", taskID);
                            strSQL += " and t.id = @taskID";
                        }
                        if (QRCode.Length > 5)
                        {
                            command.Parameters.AddWithValue("@QRCode", QRCode);
                            strSQL += " and t.QRCode = @QRCode";
                        }
                        strSQL += " order by id";

                        command.CommandText = strSQL;
                        adapter = new SqlDataAdapter(command);
                        adapter.Fill(ds);
                    }
                }
                connection.Close();

                var data = (ds.Tables[0].AsEnumerable().Select(dataRow => new TaskWiseQuestionnaire
                {
                    TransactionID = dataRow.Field<int>("TransactionID"),
                    TransDate = dataRow.Field<DateTime>("TransactionDate"),
                    TaskID = dataRow.Field<int>("taskID"),
                    TaskName = dataRow.Field<string>("taskName"),
                    Occurance = dataRow.Field<string>("Occurence"),
                    QuestionName = dataRow.Field<string>("QuestionName"),
                    QuestID = dataRow.Field<int>("QuestionID"),
                    Action = dataRow.Field<string>("Action"),
                    Remarks = dataRow.Field<string>("Remarks"),
                    CategoryName = ds0.Tables["CatDetails"].Rows[0]["CategoryName"].ToString(),
                    SubCategoryName = ds0.Tables["CatDetails"].Rows[0]["SubCategoryName"].ToString()
                })).ToList();

                return data;
            }
            catch (Exception ex)
            { string error = ex.Message; }

            return null;
        }

        [ChildActionOnly]
        public ActionResult _TaskWiseQuestionnaire()
        {
            return PartialView("_TaskWiseQuestionnaire");
        }

        public ActionResult ReadQR()
        {
            return View(ReadQRCode());
        }

        private QRCodeModel ReadQRCode()
        {
            QRCodeModel barcodeModel = new QRCodeModel();
            string barcodeText = "";
            string imagePath = "~/Images/QrCode.png";
            string barcodePath = Server.MapPath(imagePath);
            var barcodeReader = new BarcodeReader();

            var result = barcodeReader.Decode(new Bitmap(barcodePath));
            if (result != null)
            {
                barcodeText = result.Text;
            }
            return new QRCodeModel() { QRCodeText = barcodeText, QRCodeImagePath = imagePath };
        }

        public ActionResult TaskDetails(int Id)
        {
            //var data = DataQuestions(Id);
            var taskTransactionList = taskWiseQuestionnaire(Id, "");
            return PartialView("TaskDetails", taskTransactionList);
        }

        public ActionResult TaskDetailsQRScan()
        {
            //return PartialView("TaskDetailsQRScan");
            return PartialView("ScanQR");
        }

        public ActionResult TaskDetailsQRScanN()
        {
            string qrCodeR = Request.QueryString["QRCodeR"];
            string MobileNo = Request.QueryString["MobileNo"];

            ViewData["QRCode"] = qrCodeR;
            ViewData["MobileNo"] = MobileNo;

            return View("TaskDetailsQRScan");
        }

        public ActionResult TaskDetailsQRManual()
        {
            string qrCodeR = Request.QueryString["QRCodeR"];
            string mobileNo = Request.QueryString["MobileNo"];

            ViewData["QRCode"] = qrCodeR;
            ViewData["MobileNo"] = mobileNo;

            return View("TaskDetailsQRManual");
        }
        [HttpPost]
        public ActionResult TaskDetailsQRManual(string ScanResults, string QRCodeDB)
        {
            //TaskDetailsQRNewFormManual
            string mobileNo = Request.QueryString["MobileNo"];
            ViewData["MobileNo"] = mobileNo;

            if (ScanResults == QRCodeDB)
            {
                var taskTransactionList = taskWiseQuestionnaire(0, ScanResults);
                return View("TaskDetailsQRNewFormManual", taskTransactionList);
            }
            else
            {
                ViewData["Error"] = "Invalid QR Code !";
                ViewData["QRCode"] = QRCodeDB;
                return View();
            }
        }

        public ActionResult TaskDetailsQR(string QRCode)
        {
            var taskTransactionList = taskWiseQuestionnaire(0, QRCode);
            return PartialView("TaskDetails", taskTransactionList);
        }

        public ActionResult TaskDetailsQRNewForm(string QRCode)
        {
            var taskTransactionList = taskWiseQuestionnaire(0, QRCode);
            string MobileNo = Request.QueryString["MobileNo"];
            ViewData["MobileNo"] = MobileNo;

            return View("TaskDetailsScannedData", taskTransactionList);
        }

        public ActionResult TaskDetailsQRNewFormManual(string QRCode)
        {
            string mobileNo = Request.QueryString["MobileNo"];
            ViewData["MobileNo"] = mobileNo;

            var taskTransactionList = taskWiseQuestionnaire(0, QRCode);
            return View("TaskDetailsQRNewFormManual", taskTransactionList);
        }

        public ActionResult Task(string MobileNo)
        {
            List<SelectListItem> Categoryname = new List<SelectListItem>();
            DataSet ds = CategoryList();
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                Categoryname.Add(new SelectListItem { Text = ds.Tables[0].Rows[i][0].ToString(), Value = ds.Tables[0].Rows[i][1].ToString() });
            }
            ViewBag.Categories = Categoryname.AsEnumerable();

            ViewData["Occurance"] = "";
            ViewData["TransDate"] = "";
            ViewData["MobileNo"] = MobileNo;

            ViewBag.Data = TaskTrans(Convert.ToInt32(ViewData["CategoryID"]), Convert.ToInt32(ViewData["SubCategoryID"]), ViewData["Occurance"].ToString(), ViewData["TransDate"].ToString(), "", "");

            ViewBag.DataQuest = taskWiseQuestionnaire(Convert.ToInt32(ViewData["TaskID"]), "");
            if (ViewBag.DataQuest == null)
            { ViewBag.DataQuest = ""; }
            return View(ViewBag.DataQuest);
        }

        public ActionResult TaskListAOA(string MobileNo)
        {
            List<SelectListItem> Months = new List<SelectListItem>();
            for (int i = 1; i <= 12; i++)
            {
                string month = "";
                if (i == 1) { month = "Feb,2022"; }
                else if (i == 2) { month = "Mar,2022"; }
                else if (i == 3) { month = "Apr,2022"; }
                else if (i == 4) { month = "May,2022"; }
                else if (i == 5) { month = "Jun,2022"; }
                else if (i == 6) { month = "Jul,2022"; }
                else if (i == 7) { month = "Aug,2022"; }
                else if (i == 8) { month = "Sep,2022"; }
                else if (i == 9) { month = "Oct,2022"; }
                else if (i == 10) { month = "Nov,2022"; }
                else if (i == 11) { month = "Dec,2022"; }
                else if (i == 12) { month = "Jan,2023"; }
                Months.Add(new SelectListItem { Text = month, Value = i.ToString() });
            }
            ViewBag.Months = Months;
            ViewBag.Supervisors = SupervisorList();

            List<SelectListItem> Categoryname = new List<SelectListItem>();
            DataSet ds = CategoryList();
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                Categoryname.Add(new SelectListItem { Text = ds.Tables[0].Rows[i][0].ToString(), Value = ds.Tables[0].Rows[i][1].ToString() });
            }
            ViewBag.Categories = Categoryname.AsEnumerable();

            ViewData["Occurance"] = "";
            ViewData["TransDate"] = "";
            ViewData["MobileNo"] = MobileNo;

            //ViewBag.Data = TaskTrans(Convert.ToInt32(ViewData["CategoryID"]), Convert.ToInt32(ViewData["SubCategoryID"]), ViewData["Occurance"].ToString(), ViewData["TransDate"].ToString(), "", "");

            ViewBag.DataQuest = taskWiseQuestionnaire(Convert.ToInt32(ViewData["TaskID"]), "");
            if (ViewBag.DataQuest == null)
            { ViewBag.DataQuest = ""; }
            return View(ViewBag.DataQuest);
        }

        public ActionResult SaveLocation(decimal lat, decimal longi, string MobileNo)
        {
            using (SqlConnection con = new SqlConnection(constr))
            {
                con.Open();

                int userID = 0;

                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = con;
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "select top(1) id from GuardMaster where mobileno = @MobileNo";
                    cmd.Parameters.AddWithValue("@MobileNo", MobileNo);
                    userID = Convert.ToInt32(cmd.ExecuteScalar().ToString());
                }

                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = con;
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "insert into SpotVisitDetails(createdby, createdon, lat, longi) values(@CreatedBy, getdate(), @lat, @longi)";
                    cmd.Parameters.AddWithValue("@CreatedBy", userID);
                    cmd.Parameters.AddWithValue("@lat", lat);
                    cmd.Parameters.AddWithValue("@longi", longi);
                    cmd.ExecuteNonQuery();
                }
                
                con.Close();
            }

            return RedirectToAction("SpotVisit");
        }

        public ActionResult SpotVisitDetails(string MobileNo, DateTime? date=null)
        {
            using (SqlConnection con = new SqlConnection(constr))
            {
                con.Open();

                int userID = 0;

                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = con;
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "select top(1) id from GuardMaster where mobileno = @MobileNo";
                    cmd.Parameters.AddWithValue("@MobileNo", MobileNo);
                    userID = Convert.ToInt32(cmd.ExecuteScalar().ToString());
                }

                //using (SqlCommand cmd = new SqlCommand())
                //{
                //    cmd.Connection = con;
                //    cmd.CommandType = CommandType.Text;
                //    cmd.CommandText = "insert into SpotVisitDetails(createdby, createdon, lat, longi) values(@CreatedBy, getdate(), @lat, @longi)";
                //    cmd.Parameters.AddWithValue("@CreatedBy", userID);
                //    cmd.Parameters.AddWithValue("@lat", lat);
                //    cmd.Parameters.AddWithValue("@longi", longi);
                //    cmd.ExecuteNonQuery();
                //}

                con.Close();
            }

            return RedirectToAction("SpotVisit");
        }

    }
}
