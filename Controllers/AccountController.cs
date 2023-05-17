using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Windows.Controls;
using UrestComplaintManagement.Context;
using Xceed.Wpf.Toolkit;
using ZXing;

namespace UrestComplaintManagement.Controllers
{
    public class AccountController : Controller
    {
        string constr = string.Empty;
        UfirmApp_ProductionEntities6 dbObj = new UfirmApp_ProductionEntities6();

        public AccountController()
        {
            constr = ConfigurationManager.ConnectionStrings["adoConnectionstring"].ConnectionString;
        }

        public ActionResult Complaint(string Mobile, int month = 0, int year = 0)
        {
            ViewData["MobileNo"] = Mobile;
            var eList = ComplaintData(Mobile, month, year);
            return View(eList);
        }

        public List<Ticket> ComplaintData(string Mobile, int month = 0, int year = 0)
        {
            ViewData["MobileNo"] = Mobile;

            SqlConnection connection;
            SqlDataAdapter adapter;
            SqlCommand command = new SqlCommand();
            SqlParameter param;
            DataSet ds = new DataSet();
            List<Ticket> eList = new List<Ticket>();
            connection = new SqlConnection(constr);
            connection.Open();
            command.Connection = connection;

            try
            {
                command.CommandType = CommandType.Text;
                param = new SqlParameter("@MobileNo", Mobile);
                param.Direction = ParameterDirection.Input;
                param.DbType = DbType.String;
                command.Parameters.Add(param);
                command.CommandText = "select top 1 pmem.name FirstName, '' LastName, ptwr.Towername, pdet.FlatDetailNumber, pmem.PropertyMemberId UserId, pdet.PropertyId from [App].[PropertyMember] pmem inner join [App].[MemberPropertyMapping] pmap on pmap.PropertyMemberId=pmem.PropertyMemberId inner join [App].[PropertyDetails] pdet on pdet.PropertyDetailsId=pmap.PropertyDetailId inner join [App].[PropertyTowers] ptwr on ptwr.PropertyTowerId=pdet.PropertyTowerId where pmem.IsActive = 1 and pmap.IsActive = 1 and (pdet.IsDeleted is null or pdet.IsDeleted=0) and  (ptwr.IsDeleted is null or ptwr.IsDeleted=0) and pmem.ContactNumber=@MobileNo";
                adapter = new SqlDataAdapter(command);
                adapter.Fill(ds, "FlatDetails");
                bool exists = false;
                bool flatExist = false;
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        ViewBag.Name = ds.Tables["FlatDetails"].Rows[0]["FirstName"].ToString() + ' ' + ds.Tables["FlatDetails"].Rows[0]["LastName"].ToString();
                        ViewBag.FlatDetail = ds.Tables["FlatDetails"].Rows[0]["FlatDetailNumber"].ToString();
                        ViewBag.Flat = ds.Tables["FlatDetails"].Rows[0]["FlatDetailNumber"].ToString();

                        flatExist = true;
                    }
                }
                if (!flatExist)
                {
                    command.CommandType = CommandType.Text;
                    command.CommandText = "select top 1 usrd.FirstName, usrd.LastName, proptwr.Towername, propdet.FlatDetailNumber, usrd.UserId, prop.PropertyId from[Identity].[Users] usrd inner join[App].[UserPropertyAssignment] prop on prop.userid = usrd.userid inner join[App].[PropertyTowers] proptwr on   proptwr.PropertyId = prop.PropertyId inner join[App].[PropertyDetails] propdet on propdet.PropertyTowerId = proptwr.PropertyTowerId where usrd.IsActive=1 and (prop.isdeleted is null) and (proptwr.IsDeleted is null) and (propdet.isdeleted is null) and usrd.ContactNumber = @MobileNo order by proptwr.PropertyTowerId desc, propdet.PropertyDetailsId desc, usrd.UserId desc";
                    adapter = new SqlDataAdapter(command);
                    adapter.Fill(ds, "FlatDetails");
                }
                if (ds.Tables["FlatDetails"].Rows.Count > 0)
                {
                    ViewBag.Name = ds.Tables["FlatDetails"].Rows[0]["FirstName"].ToString() + ' ' + ds.Tables[0].Rows[0]["LastName"].ToString();
                    ViewBag.FlatDetail = ds.Tables["FlatDetails"].Rows[0]["FlatDetailNumber"].ToString();
                    ViewBag.Flat = ds.Tables["FlatDetails"].Rows[0]["FlatDetailNumber"].ToString();
                }
                command.CommandType = CommandType.Text;

                if (month > 0)
                {
                    command.CommandText = "Select * From [App].[Ticket] tkt where (tkt.IsDeleted is null) and month(createdon) = " + month + " and year(createdon) = " + year + " order by ticketid desc";
                }
                else
                {
                    command.CommandText = "Select * From [App].[Ticket] tkt inner join [Identity].[Users] usr on usr.UserId = tkt.CreatedBy where (tkt.IsDeleted is null) and usr.ContactNumber=@MobileNo order by ticketid desc";
                }
                adapter = new SqlDataAdapter(command);
                adapter.Fill(ds, "Tickets");
                if (ds.Tables["Tickets"].Rows.Count > 0)
                {
                    exists = true;
                }
                if (!exists)
                {
                    return eList;
                }

                eList = (ds.Tables[1].AsEnumerable().Select(dataRow => new Ticket
                {
                    TicketNumber = dataRow["TicketNumber"] == DBNull.Value ? null : dataRow.Field<string>("TicketNumber"),
                    Description = dataRow["Description"] == DBNull.Value ? null : dataRow.Field<string>("Description"),
                    TicketOrigin = dataRow["TicketOrigin"] == DBNull.Value ? null : dataRow.Field<string>("TicketOrigin"),
                    Title = dataRow["Title"] == DBNull.Value ? null : dataRow.Field<string>("Title"),
                    Visibility = dataRow["Visibility"] == DBNull.Value ? null : dataRow.Field<string>("Visibility"),
                    TicketId = dataRow["TicketId"] == DBNull.Value ? 0 : dataRow.Field<int>("TicketId"),
                    PropertyDetaildId = dataRow["PropertyDetaildId"] == DBNull.Value ? 0 : dataRow.Field<int>("PropertyDetaildId"),
                    StatusTypeId = dataRow["StatusTypeId"] == DBNull.Value ? 0 : dataRow.Field<int>("StatusTypeId"),
                    PausedComment = dataRow["PausedComment"] == DBNull.Value ? null : dataRow.Field<string>("PausedComment"),
                    TicketChannel = dataRow["TicketChannel"] == DBNull.Value ? null : dataRow.Field<string>("TicketChannel")

                })).ToList();

                connection.Close();

                return eList;
            }
            catch (Exception ex)
            {
                string error = ex.Message;
                connection.Close();

                return eList;
            }
        }

        public ActionResult ComplaintsRecords(string Mobile, int month = 0, int year = 0)
        {
            return Json(ComplaintData(Mobile, month, year), JsonRequestBehavior.AllowGet);
        }

        public ActionResult ComplaintDetail(string id, string MobileNo)
        {
            string connetionString = null;
            SqlConnection connection;
            SqlDataAdapter adapter;
            SqlCommand command = new SqlCommand();
            SqlParameter param;
            DataSet ds = new DataSet();
            DataSet dss = new DataSet();
            DataSet dsss = new DataSet();

            int i = 0;

            //connetionString = "Data Source=WIN-HBSI1RRBVE0;Initial Catalog=UfirmApp_Production;integrated security=true";
            connection = new SqlConnection(constr);

            connection.Open();
            command.Connection = connection;
            command.CommandType = CommandType.Text;
            command.CommandText = "Select * From [App].[Ticket] where TicketId =" + id;
            adapter = new SqlDataAdapter(command);
            adapter.Fill(ds, "ticket");

            command.CommandType = CommandType.Text;
            command.CommandText = "select top(1) * from [App].[TicketRating] where TicketId = " + id + " order by ratingid desc";
            adapter = new SqlDataAdapter(command);
            adapter.Fill(ds, "ticketrating");

            command.CommandType = CommandType.Text;
            command.CommandText = "select * from [App].[Ticketlog] where TicketId = " + id + " order by loggedon desc";
            adapter = new SqlDataAdapter(command);
            adapter.Fill(ds, "ticketlogs");

            DataRow dataRow = ds.Tables[0].Rows[0];
            Ticket ticket = new Ticket()
            {
                TicketNumber = dataRow.Field<string>("TicketNumber"),
                Description = dataRow.Field<string>("Description"),
                TicketOrigin = dataRow.Field<string>("TicketOrigin"),
                Title = dataRow.Field<string>("Title"),
                Visibility = dataRow.Field<string>("Visibility"),
                TicketId = dataRow.Field<int>("TicketId"),
                PropertyDetaildId = dataRow.Field<int>("PropertyDetaildId"),
                StatusTypeId = dataRow.Field<int>("StatusTypeId"),
                PausedComment = dataRow.Field<string>("PausedComment"),
                TicketChannel = dataRow.Field<string>("TicketChannel"),

                rating = (ds.Tables[1].AsEnumerable().Select(dataRow0 => new TicketRating
                {
                    Rating = dataRow0.Field<string>("Rating"),
                })).ToList(),

                logs = (ds.Tables[2].AsEnumerable().Select(dataRow0 => new TicketLogs
                {
                    log = dataRow0.Field<string>("Log"),
                    loggedon = dataRow0.Field<DateTime>("LoggedOn")
                })).ToList()

            };

            connection.Close();
            ViewData["Mobile"] = MobileNo;

            return View(ticket);
        }

        public ActionResult AmenityBookingList(string MobileNo, DateTime BookingDate)
        {
            SqlConnection connection;
            connection = new SqlConnection(constr);
            connection.Open();

            try
            {
                SqlDataAdapter adapter;
                SqlCommand command = new SqlCommand();
                DataSet ds = new DataSet();

                string sql = "select max(userid) from [Identity].[Users] where IsActive=1 and ((IsDeleted is null) or (IsDeleted = 0)) and ContactNumber=@MobileNo";
                command = new SqlCommand();
                command.Connection = connection;
                command.CommandType = CommandType.Text;
                command.CommandText = sql;
                command.Parameters.AddWithValue("@MobileNo", MobileNo);
                string userId = command.ExecuteScalar().ToString();

                command = new SqlCommand();
                command.Connection = connection;
                command.CommandType = CommandType.Text;
                command.CommandText = "select abkg.id, abkg.BookingDate, abkg.TimeSlotFr, abkg.TimeSlotTo, mast.AmenitiesName, abkg.NosOfPersons, abkg.IsDeleted, abkg.Approved from [dbo].[AmenitiesBooking] abkg inner join [Master].[Amenities] mast on abkg.AmenitiesId=mast.AmenitiesId where cast(abkg.CreatedOn as date) <=@BookingDate and abkg.UserId=@UserId and (abkg.isdeleted is null or abkg.isdeleted=0) " +
                    "order by abkg.id desc";
                command.Parameters.AddWithValue("@UserId", userId);
                command.Parameters.AddWithValue("@BookingDate", BookingDate);

                adapter = new SqlDataAdapter(command);
                adapter.Fill(ds, "Booking");

                var elist = (ds.Tables["Booking"].AsEnumerable().Select(dataRow => new AmenitiesBooking
                {
                    Id = dataRow.Field<int>("Id"),

                    BookingDate = dataRow.Field<DateTime>("BookingDate"),
                    AminityName = dataRow.Field<string>("AmenitiesName"),
                    TimeFr = dataRow["TimeSlotFr"].ToString(),
                    TimeTo = dataRow["TimeSlotTo"].ToString(),
                    NoOfPersons = dataRow.Field<int>("NosOfPersons"),
                    IsDeleted = dataRow["IsDeleted"] == DBNull.Value ? 0 : dataRow["IsDeleted"] == null ? 0 : dataRow.Field<int>("IsDeleted"),

                    BkgDtStr = dataRow.Field<DateTime>("BookingDate").ToString("dd, MMM"),
                    FrStr = dataRow.Field<DateTime>("TimeSlotFr").ToString("hh:mm"),
                    ToStr = dataRow.Field<DateTime>("TimeSlotTo").ToString("hh:mm"),
                    Approved = dataRow["Approved"] == DBNull.Value ? 0 : dataRow["Approved"] == null ? 0 : dataRow.Field<int>("Approved")

                })).ToList();

                connection.Close();

                return Json(elist, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                connection.Close();
                string message = ex.Message;
                return null;
            }
        }

        public List<AmenitiesBooking> AmenityBookingDataList(string MobileNo)
        {
            SqlConnection connection;
            connection = new SqlConnection(constr);
            connection.Open();

            try
            {
                SqlDataAdapter adapter;
                SqlCommand command = new SqlCommand();
                DataSet ds = new DataSet();

                string sql = "select max(userid) from [Identity].[Users] where IsActive=1 and ((IsDeleted is null) or (IsDeleted = 0)) and ContactNumber=@MobileNo";
                command = new SqlCommand();
                command.Connection = connection;
                command.CommandType = CommandType.Text;
                command.CommandText = sql;
                command.Parameters.AddWithValue("@MobileNo", MobileNo);
                string userId = command.ExecuteScalar().ToString();

                command = new SqlCommand();
                command.Connection = connection;
                command.CommandType = CommandType.Text;
                command.CommandText = "select abkg.id, abkg.BookingDate, abkg.TimeSlotFr, abkg.TimeSlotTo, mast.AmenitiesName, abkg.NosOfPersons, abkg.IsDeleted, abkg.Approved from [dbo].[AmenitiesBooking] abkg inner join [Master].[Amenities] mast on abkg.AmenitiesId=mast.AmenitiesId where abkg.UserId=@UserId and (abkg.isdeleted is null or abkg.isdeleted=0) " +
                    "order by abkg.id desc";
                command.Parameters.AddWithValue("@UserId", userId);

                adapter = new SqlDataAdapter(command);
                adapter.Fill(ds, "Booking");

                var elist = (ds.Tables["Booking"].AsEnumerable().Select(dataRow => new AmenitiesBooking
                {
                    Id = dataRow.Field<int>("Id"),

                    BookingDate = dataRow.Field<DateTime>("BookingDate"),
                    AminityName = dataRow.Field<string>("AmenitiesName"),
                    TimeFr = dataRow["TimeSlotFr"].ToString(),
                    TimeTo = dataRow["TimeSlotTo"].ToString(),
                    NoOfPersons = dataRow.Field<int>("NosOfPersons"),
                    IsDeleted = dataRow["IsDeleted"] == DBNull.Value ? 0 : dataRow["IsDeleted"] == null ? 0 : dataRow.Field<int>("IsDeleted"),

                    BkgDtStr = dataRow.Field<DateTime>("BookingDate").ToString("dd, MMM"),
                    FrStr = dataRow.Field<DateTime>("TimeSlotFr").ToString("hh:mm"),
                    ToStr = dataRow.Field<DateTime>("TimeSlotTo").ToString("hh:mm"),
                    Approved = dataRow["Approved"] == DBNull.Value ? 0 : dataRow["Approved"] == null ? 0 : dataRow.Field<int>("Approved")

                })).ToList();

                connection.Close();

                return elist;
            }
            catch (Exception ex)
            {
                connection.Close();
                string message = ex.Message;
                return null;
            }
        }

        public List<KycDetails> KycDetailsList(string MobileNo)
        {
            SqlConnection connection = new SqlConnection(constr);
            SqlDataAdapter adapter;
            SqlCommand command = new SqlCommand();
            DataSet ds = new DataSet();

            try
            {
                connection.Open();
                command = new SqlCommand();
                command.Connection = connection;
                command.CommandType = CommandType.Text;

                command.CommandText = "select Id,MobileNo,EmployeeId,EmployeeName,Gender,JobProfile,Image,IsDeleted,Approved, IdDoc from KYCdetails where (IsDeleted is null or IsDeleted = 0) and MobileNo = '" + MobileNo + "' order by id";

                adapter = new SqlDataAdapter(command);
                adapter.Fill(ds);

                if (ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
                {
                    command = new SqlCommand();
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;

                    command.CommandText = "select 0 Id, '' MobileNo, '' EmployeeId, EmployeeName, '' Gender, Designation JobProfile, '' Image, 0 IsDeleted, 0 Approved, '' IdDoc from EmployeeList where (IsDeleted is null or IsDeleted = 0) and MobileNo = '" + MobileNo + "' order by id";

                    adapter = new SqlDataAdapter(command);
                    adapter.Fill(ds);
                }

                var eList = (ds.Tables[0].AsEnumerable().Select(dataRow => new KycDetails
                {
                    Id = dataRow.Field<int>("Id"),
                    EmployeeId = dataRow.Field<string>("EmployeeId"),
                    EmployeeName = dataRow.Field<string>("EmployeeName"),
                    Gender = dataRow.Field<string>("Gender"),
                    JobProfile = dataRow.Field<string>("JobProfile"),
                    MobileNo = dataRow.Field<string>("MobileNo"),
                    Image = dataRow.Field<string>("Image"),
                    //ProfileImage = (FileContentResult)RetrieveImage(dataRow.Field<int>("Id")),
                    IsDeleted = dataRow["IsDeleted"] == DBNull.Value ? 0 : dataRow.Field<int>("IsDeleted"),
                    Approved = dataRow["Approved"] == DBNull.Value ? 0 : dataRow.Field<int>("Approved"),
                    IdDoc = dataRow["IdDoc"] == DBNull.Value ? "" : dataRow.Field<string>("IdDoc")
                })).ToList();

                if (ds.Tables[0].Rows.Count > 0)
                {
                    ViewData["KycID"] = ds.Tables[0].Rows[0].Field<int>("Id");
                }
                ds.Dispose();
                connection.Close();
                return eList;
            }
            catch (Exception ex)
            {
                string error = ex.Message;
                ds.Dispose();
                connection.Close();
                return null;
            }
        }

        public ActionResult RetrieveImage(int kycId)
        {
            byte[] cover = GetImageFromDataBase(kycId);
            if (cover != null)
            {
                return File(cover, "image/png");
            }
            else
            {
                return null;
            }
        }
        public byte[] GetImageFromDataBase(int KycId)
        {
            SqlConnection sqlConnection = new SqlConnection(constr);
            sqlConnection.Open();
            SqlCommand command = new SqlCommand();
            command.Connection = sqlConnection;
            command.CommandType = CommandType.Text;

            command.CommandText = "select ProfileImage from [dbo].[KycImages] where kycid=" + KycId;// + " and description = 'ProfileImage'";

            DataSet ds = new DataSet();
            SqlDataAdapter adapter = new SqlDataAdapter(command);
            adapter.Fill(ds);

            byte[] cover = null;
            if (ds.Tables[0].Rows.Count > 0)
            { cover = (byte[])ds.Tables[0].Rows[0][0]; }

            sqlConnection.Close();

            return cover;
        }

        public ActionResult ServiceDirecory(string MobileNo)
        {
            ViewData["MobileNo"] = MobileNo;
            ViewBag.ServiceDirecortyData = EmployeeListData("0");
            return View();
        }

        public ActionResult EmployeeList(string PropertyID)
        {
            var elist = EmployeeListData(PropertyID);
            return Json(elist, JsonRequestBehavior.AllowGet);

            //SqlConnection connection;
            //connection = new SqlConnection(constr);
            //connection.Open();

            //try
            //{
            //    SqlDataAdapter adapter;
            //    SqlCommand command = new SqlCommand();
            //    DataSet ds = new DataSet();

            //    command = new SqlCommand();
            //    command.Connection = connection;
            //    command.CommandType = CommandType.Text;
            //    command.CommandText = "select Id, EmployeeName, FatherName, Designation, MobileNo from [dbo].[employeelist] order by employeename";
            //    command.Parameters.AddWithValue("@PropertyId", PropertyID);

            //    adapter = new SqlDataAdapter(command);
            //    adapter.Fill(ds, "Employees");

            //    var elist = (ds.Tables["Employees"].AsEnumerable().Select(dataRow => new EmployeeMaster
            //    {
            //        Id = dataRow.Field<int>("Id"),
            //        EmployeeName = dataRow.Field<string>("EmployeeName"),
            //        FatherName = dataRow.Field<string>("FatherName"),
            //        Designation = dataRow.Field<string>("Designation"),
            //        MobileNo = dataRow.Field<string>("MobileNo"),

            //    })).ToList();

            //    connection.Close();

            //    return Json(elist, JsonRequestBehavior.AllowGet);
            //}
            //catch (Exception ex)
            //{
            //    connection.Close();
            //    string message = ex.Message;
            //    return null;
            //}
        }

        public List<EmployeeMaster> EmployeeListData(string PropertyID)
        {
            SqlConnection connection;
            connection = new SqlConnection(constr);
            connection.Open();

            try
            {
                SqlDataAdapter adapter;
                SqlCommand command = new SqlCommand();
                DataSet ds = new DataSet();

                command = new SqlCommand();
                command.Connection = connection;
                command.CommandType = CommandType.Text;
                command.CommandText = "select Id, EmployeeName, FatherName, Designation, MobileNo from [dbo].[employeelist] order by employeename";
                command.Parameters.AddWithValue("@PropertyId", PropertyID);

                adapter = new SqlDataAdapter(command);
                adapter.Fill(ds, "Employees");

                var elist = (ds.Tables["Employees"].AsEnumerable().Select(dataRow => new EmployeeMaster
                {
                    Id = dataRow.Field<int>("Id"),
                    EmployeeName = dataRow.Field<string>("EmployeeName"),
                    FatherName = dataRow.Field<string>("FatherName"),
                    Designation = dataRow.Field<string>("Designation"),
                    MobileNo = dataRow.Field<string>("MobileNo"),

                })).ToList();

                connection.Close();

                return elist;
            }
            catch (Exception ex)
            {
                connection.Close();
                string message = ex.Message;
                return null;
            }
        }

        public static DataTable ToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);
            //Get all the properties by using reflection   
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Setting column names as Property names  
                dataTable.Columns.Add(prop.Name);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {

                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            return dataTable;
        }

        public ActionResult SendNotification(FormDataNoticeBoard model, string mobileNo)
        {
            string userID = "0"; var resp = "1";
            try
            {
                SqlConnection connection;
                SqlCommand command = new SqlCommand();
                connection = new SqlConnection(constr);
                connection.Open();

                string sql = "select max(userid) from [Identity].[Users] where IsActive=1 and ((IsDeleted is null) or (IsDeleted = 0)) and ContactNumber='" + mobileNo + "'";
                command = new SqlCommand();
                command.Connection = connection;
                command.CommandType = CommandType.Text;
                command.CommandText = sql;
                userID = command.ExecuteScalar().ToString();

                connection.Close();

                if (model.PropertyId != 0 && model.NotificationTypeId != 0 && model.PropertyGroupId != 0 && model.Notify != 0 && model.AlertTypeId != 0 && model.Subject != "")
                {
                    List<NoticeBoardAttachment> finalAttachmentData = new List<NoticeBoardAttachment>();

                    string sp = "INSERT INTO app.Notification (PropertyId,Subject,Message,NotificationTypeId,PropertyGroupId,IsActive,IsDeleted,CreatedOn,CreatedBy,ExpirtyDate) VALUES (@PropertyId,@Subject,@Message,@NotificationTypeId,@PropertyGroupId,@IsActive,0,GETDATE(),@CurrentUserId,@ExpirtyDate)";//"App.ManageNoticeBoard_Amen";

                    SqlConnection con = new SqlConnection(constr);
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = con;
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = sp;
                    cmd.Parameters.AddWithValue("@CmdType", model.StatementType);
                    cmd.Parameters.AddWithValue("@NotificationId", userID);
                    cmd.Parameters.AddWithValue("@PropertyId", Convert.ToString(model.PropertyId));
                    cmd.Parameters.AddWithValue("@Subject", Convert.ToString(model.Subject));
                    cmd.Parameters.AddWithValue("@Message", Convert.ToString(model.Message));
                    cmd.Parameters.AddWithValue("@NotificationTypeId", Convert.ToString(model.NotificationTypeId));
                    cmd.Parameters.AddWithValue("@PropertyGroupId", Convert.ToString(model.PropertyGroupId));
                    cmd.Parameters.AddWithValue("@ExpirtyDate", model.ExpirtyDate);
                    cmd.Parameters.AddWithValue("@ExecutionDate", model.ExpirtyDate);
                    cmd.Parameters.AddWithValue("@IsActive", 1);
                    cmd.Parameters.AddWithValue("@PropertyDetailsId", model.PropertyDetailsId);
                    cmd.Parameters.AddWithValue("@PropertyTowerId", model.PropertyTowerId);
                    cmd.Parameters.AddWithValue("@Notify", Convert.ToString(model.Notify));
                    cmd.Parameters.AddWithValue("@AlertTypeId", Convert.ToString(model.AlertTypeId));
                    cmd.Parameters.AddWithValue("@IsSent", 0);
                    cmd.Parameters.AddWithValue("@CurrentUserId", userID);

                    con.Open();
                    cmd.ExecuteNonQuery();

                    con.Close();

                }

            }
            catch (Exception ex)
            {
                resp = ex.Message;

            }
            return Json(resp, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ComplaintCloseAndRating(string id, string MobileNo)
        {
            string connetionString = null;
            SqlConnection connection;
            SqlDataAdapter adapter;
            SqlCommand command = new SqlCommand();
            SqlParameter param;
            DataSet ds = new DataSet();
            DataSet dss = new DataSet();
            DataSet dsss = new DataSet();

            int i = 0;

            //connetionString = "Data Source=WIN-HBSI1RRBVE0;Initial Catalog=UfirmApp_Production;integrated security=true";
            connection = new SqlConnection(constr);

            connection.Open();
            command.Connection = connection;
            command.CommandType = CommandType.Text;
            command.CommandText = "Select * From [App].[Ticket] where TicketId =" + id;

            adapter = new SqlDataAdapter(command);
            adapter.Fill(ds);

            DataRow dataRow = ds.Tables[0].Rows[0];
            Ticket ticket = new Ticket()
            {
                TicketNumber = dataRow.Field<string>("TicketNumber"),
                Description = dataRow.Field<string>("Description"),
                TicketOrigin = dataRow.Field<string>("TicketOrigin"),
                Title = dataRow.Field<string>("Title"),
                Visibility = dataRow.Field<string>("Visibility"),
                TicketId = dataRow.Field<int>("TicketId"),
                PropertyDetaildId = dataRow.Field<int>("PropertyDetaildId"),
                StatusTypeId = dataRow.Field<int>("StatusTypeId"),
                PausedComment = dataRow.Field<string>("PausedComment"),
                TicketChannel = dataRow.Field<string>("TicketChannel")
            };

            connection.Close();
            ViewData["MobileNo"] = MobileNo;

            return View(ticket);
        }
        [HttpPost]
        public ActionResult ComplaintCloseAndRating(IEnumerable<Ticket> ticket, string TicketId = "0", string MobileNo = "", string rate = "0")
        {
            SqlConnection connection;
            SqlCommand command = new SqlCommand();
            connection = new SqlConnection(constr);
            connection.Open();

            string sql = "select max(userid) from [Identity].[Users] where IsActive=1 and (IsDeleted != 1) and ContactNumber='" + MobileNo + "'";
            command.Connection = connection;
            command.CommandType = CommandType.Text;
            command.CommandText = sql;
            string userID = command.ExecuteScalar().ToString();

            sql = "update [app].Ticket set StatusTypeId = 4, updatedby = " + userID + " where TicketId=" + TicketId;

            command.Connection = connection;
            command.CommandType = CommandType.Text;
            command.CommandText = sql;

            command.ExecuteNonQuery();

            //logs  
            sql = "INSERT INTO[App].[TicketLog] ([Log],[LoggedBy],[LoggedOn],[TicketId],[LoggedByType]) VALUES('Ticket status changed  RESOLVED To CLOSED', " + userID + ", getdate()," + TicketId + ", 'User')";

            command.Connection = connection;
            command.CommandType = CommandType.Text;
            command.CommandText = sql;

            command.ExecuteNonQuery();

            connection.Close();

            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand("SP_WebCreateNewTicketRating", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TicketId", TicketId);
                    cmd.Parameters.AddWithValue("@Rating", rate);
                    cmd.Parameters.AddWithValue("@RatingDetails", "");
                    cmd.Parameters.AddWithValue("@RatededBy", userID);
                    //cmd.Parameters.AddWithValue("@RatedOn", DateTime.Now.Date.ToString());

                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
            }

            return RedirectToAction("Complaint", new { Mobile = MobileNo });
        }

        public ActionResult ComplaintReOpen(string id, string MobileNo)
        {
            string connetionString = null;
            SqlConnection connection;
            SqlDataAdapter adapter;
            SqlCommand command = new SqlCommand();
            SqlParameter param;
            DataSet ds = new DataSet();
            DataSet dss = new DataSet();
            DataSet dsss = new DataSet();

            int i = 0;

            //connetionString = "Data Source=WIN-HBSI1RRBVE0;Initial Catalog=UfirmApp_Production;integrated security=true";
            connection = new SqlConnection(constr);

            connection.Open();
            command.Connection = connection;
            command.CommandType = CommandType.Text;
            command.CommandText = "Select * From [App].[Ticket] where TicketId =" + id;

            adapter = new SqlDataAdapter(command);
            adapter.Fill(ds);

            DataRow dataRow = ds.Tables[0].Rows[0];
            Ticket ticket = new Ticket()
            {
                TicketNumber = dataRow.Field<string>("TicketNumber"),
                Description = dataRow.Field<string>("Description"),
                TicketOrigin = dataRow.Field<string>("TicketOrigin"),
                Title = dataRow.Field<string>("Title"),
                Visibility = dataRow.Field<string>("Visibility"),
                TicketId = dataRow.Field<int>("TicketId"),
                PropertyDetaildId = dataRow.Field<int>("PropertyDetaildId"),
                StatusTypeId = dataRow.Field<int>("StatusTypeId"),
                PausedComment = dataRow.Field<string>("PausedComment"),
                TicketChannel = dataRow.Field<string>("TicketChannel")
            };

            connection.Close();
            ViewData["MobileNo"] = MobileNo;

            return View(ticket);// eList);
        }
        [HttpPost]
        public ActionResult ComplaintReOpen(string ReopenReason, string TicketId = "0", string MobileNo = "")
        {
            SqlConnection connection;
            SqlCommand command = new SqlCommand();

            connection = new SqlConnection(constr);

            connection.Open();

            string sql = "select max(userid) from [Identity].[Users] where IsActive=1 and (IsDeleted != 1) and ContactNumber='" + MobileNo + "'";
            command.Connection = connection;
            command.CommandType = CommandType.Text;
            command.CommandText = sql;
            string userID = command.ExecuteScalar().ToString();

            sql = "update [app].Ticket set StatusTypeId=1, IsReopen=1, updatedby = " + userID + ", ReOpenBy = " + userID + ", ReOpenOn = getdate(), ReopenReason = '" + ReopenReason + "' where TicketId=" + TicketId;

            command.Connection = connection;
            command.CommandType = CommandType.Text;
            command.CommandText = sql;

            command.ExecuteNonQuery();

            //logs  
            sql = "INSERT INTO[App].[TicketLog] ([Log],[LoggedBy],[LoggedOn],[TicketId],[LoggedByType]) VALUES('Ticket status changed Re-Open : " + ReopenReason + "', " + userID + ", getdate()," + TicketId + ", 'User')";

            command.Connection = connection;
            command.CommandType = CommandType.Text;
            command.CommandText = sql;

            command.ExecuteNonQuery();

            connection.Close();

            return RedirectToAction("Complaint", new { Mobile = MobileNo });
        }

        public DataSet AmenitiesList()
        {
            SqlConnection connection;
            SqlDataAdapter adapter;
            SqlCommand command = new SqlCommand();
            DataSet ds = new DataSet();
            connection = new SqlConnection(constr);
            connection.Open();
            command.Connection = connection;
            command.CommandType = CommandType.Text;
            command.CommandText = "select AmenitiesName, AmenitiesId from [Master].[Amenities] order by AmenitiesName";

            adapter = new SqlDataAdapter(command);
            adapter.Fill(ds);

            connection.Close();

            return ds;
        }

        public ActionResult ValidateAminityBooking(string mobileNo, DateTime bkgDate, string timeFr, string timeTo, int ameID, int nosP)
        {
            try
            {
                bool ressonse = ValidateAminityBookingData(mobileNo, bkgDate, timeFr, timeTo, ameID, nosP);
                if (ressonse)
                {
                    return Json("0", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("1", JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            { string error = ex.Message; return null; }
        }

        public bool ValidateAminityBookingData(string mobileNo, DateTime bkgDate, string timeFr, string timeTo, int ameID, int nosP)
        {
            try
            {
                SqlConnection connection;
                SqlCommand command = new SqlCommand();
                connection = new SqlConnection(constr);
                connection.Open();

                string sql = "select max(userid) from [Identity].[Users] where IsActive=1 and ((IsDeleted is null) or (IsDeleted = 0)) and ContactNumber=@MobileNo";
                command = new SqlCommand();
                command.Connection = connection;
                command.CommandType = CommandType.Text;
                command.CommandText = sql;
                command.Parameters.AddWithValue("@MobileNo", mobileNo);
                string userId = command.ExecuteScalar().ToString();

                command = new SqlCommand();
                command.CommandType = CommandType.Text;
                command.Connection = connection;
                string strSQL = "select count(id) from [dbo].[AmenitiesBooking] where UserId = @UserId and BookingDate = @BookingDate and TimeSlotFr = @TimeSlotFr and TimeSlotTo = @TimeSlotTo and AmenitiesId = @AmenitiesId";
                command.Parameters.AddWithValue("@UserId", userId);
                command.Parameters.AddWithValue("@BookingDate", bkgDate.ToString("MM/dd/yyyy"));
                command.Parameters.AddWithValue("@TimeSlotFr", timeFr);
                command.Parameters.AddWithValue("@TimeSlotTo", timeTo);
                command.Parameters.AddWithValue("@AmenitiesId", ameID);
                command.CommandText = strSQL;
                var resp = command.ExecuteScalar();
                connection.Close();

                if (resp.ToString() == "1")
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception ex)
            { string error = ex.Message; return false; }
        }

        public bool IsKycExists(string mobileNo, out int id)
        {
            id = 0;
            try
            {
                SqlConnection connection;
                SqlCommand command = new SqlCommand();
                connection = new SqlConnection(constr);
                connection.Open();

                string sql = "select id from [dbo].kycdetails where mobileno=@MobileNo";
                command = new SqlCommand();
                command.Connection = connection;
                command.CommandType = CommandType.Text;
                command.CommandText = sql;
                command.Parameters.AddWithValue("@MobileNo", mobileNo);
                var Id = command.ExecuteScalar();
                connection.Close();
                if (Id == null || Id == DBNull.Value || Id.ToString() == "")
                {
                    return false;
                }
                else
                {
                    if (Convert.ToInt32(Id) == 0)
                    {
                        return false;
                    }
                    else
                    {
                        id = Convert.ToInt32(Id);
                        return true;
                    }

                }
            }
            catch (Exception ex)
            { string error = ex.Message; return false; }
        }

        public ActionResult DeleteAmenitesBooking(int Id, string MobileNo)
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
                string strSQL = "update [dbo].[AmenitiesBooking] set isdeleted = 1 where (Approved is null or Approved = 0) and Id=@Id";
                command.Parameters.AddWithValue("@Id", Id);
                command.CommandText = strSQL;
                command.ExecuteNonQuery();
                connection.Close();
                var resp = "1";

                //return Json(resp, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            { string error = ex.Message; return null; }

            SetAmenetiesList(MobileNo);
            ViewBag.Response = "Booking Delete !";
            return RedirectToAction("CreateAmeniBooking", new { MobileNo = MobileNo });
        }

        public ActionResult DeleteKyc(int Id, string MobileNo)
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
                string strSQL = "update [dbo].[[CreateKycDetails]] set isdeleted = 1 where (Approved is null or Approved = 0) and Id=@Id";
                command.Parameters.AddWithValue("@Id", Id);
                command.CommandText = strSQL;
                command.ExecuteNonQuery();
                connection.Close();
                var resp = "1";

                //return Json(resp, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            { string error = ex.Message; return null; }

            SetAmenetiesList(MobileNo);
            ViewBag.Response = "KYC Delete !";
            return RedirectToAction("CreateKYC", new { MobileNo = MobileNo });
        }

        public ActionResult CreateAmeniBooking(string MobileNo)
        {
            SetAmenetiesList(MobileNo);
            return View();
        }

        public ActionResult AttendancePunch(string MobileNo)
        {
            ViewData["MobileNo"] = MobileNo;
            ViewData["KycID"] = "0";
            ViewData["LastAccessedGateNo"] = LastAccessedGateNo(MobileNo);
            return View();
        }

        public ActionResult Attendance(string MobileNoGaurd, string EmployeeIdAttendee)
        {
            ViewData["MobileNo"] = MobileNoGaurd;
            ViewData["KycID"] = "0";
            ViewData["LastAccessedGateNo"] = LastAccessedGateNo(MobileNoGaurd);
            ViewBag.Attendee = EmpData(EmployeeIdAttendee);
            return View();
        }

        public string LastAccessedGateNo(string MobileNo)
        {
            SqlConnection conn = new SqlConnection(constr);
            conn.Open();
            string gateNo = "";

            try
            {
                SqlCommand comm = new SqlCommand();
                comm.CommandType = CommandType.Text;
                comm.Connection = conn;
                comm.Parameters.AddWithValue("@MobileNo", MobileNo);
                comm.CommandText = "select top(1) att.GateNo from attendancelogs att inner join GuardMaster emp on att.createdby=emp.Id where emp.MobileNo= @MobileNo order by att.id desc";
                gateNo = comm.ExecuteScalar() == DBNull.Value ? "" : comm.ExecuteScalar() == null ? "" : comm.ExecuteScalar().ToString();

            }
            catch (Exception ex)
            {
                string error = ex.Message;
            }
            conn.Close();

            return gateNo;
        }

        [HttpPost]
        public ActionResult Attendance(AttendanceLogs attendanceLogs, string MobileNo)
        {
            ViewData["MobileNo"] = MobileNo;
            SqlConnection sqlConnection = new SqlConnection(constr);
            sqlConnection.Open();
            string resp = "1";

            if (attendanceLogs.EmployeeId.Length == 0 || attendanceLogs.EmployeeName == null || attendanceLogs.EmployeeName.Length == 0)
            { resp = "0"; }

            try
            {
                if (resp == "1")
                {
                    SqlCommand comm = new SqlCommand();
                    comm.CommandType = CommandType.Text;
                    comm.Connection = sqlConnection;
                    comm.Parameters.AddWithValue("@MobileNo", MobileNo);
                    comm.CommandText = "select id from GuardMaster where MobileNo= @MobileNo";
                    int createdBy = comm.ExecuteScalar() == DBNull.Value ? 0 : comm.ExecuteScalar() == null ? 0 : Convert.ToInt32(comm.ExecuteScalar().ToString());

                    comm = new SqlCommand("insert into [dbo].[attendancelogs](EmployeeId,PunchTime,PunchType,GateNo,CreatedBy,CreatedOn) values(@EmployeeId,getdate(),@PunchType,@GateNo,@CreatedBy,getdate())");
                    comm.CommandType = CommandType.Text;
                    comm.Connection = sqlConnection;
                    comm.Parameters.AddWithValue("@EmployeeId", attendanceLogs.EmployeeId);
                    comm.Parameters.AddWithValue("@PunchType", attendanceLogs.PunchType);
                    comm.Parameters.AddWithValue("@GateNo", attendanceLogs.GateNo);
                    comm.Parameters.AddWithValue("@CreatedBy", createdBy);

                    comm.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                resp = ex.Message;
            }

            sqlConnection.Close();

            if (resp == "1")
            {
                return RedirectToAction("GoHomePage", new { MobileNo = MobileNo });
            }
            else
            {
                ViewBag.Saved = "0";
                ViewBag.Response = "Invalid Employee Name, etc.";
                return View();
            }
        }

        public ActionResult GetEmpData(string empId)
        {
            return Json(EmpData(empId), JsonRequestBehavior.AllowGet);
        }
        IEnumerable<KycDetails> EmpData(string empId)
        {
            try
            {
                SqlConnection connection;
                SqlCommand command = new SqlCommand();
                connection = new SqlConnection(constr);
                connection.Open();

                string MobileNo = "";
                command = new SqlCommand();
                command.Connection = connection;
                command.CommandType = CommandType.Text;
                command.CommandText = "select MobileNo from [dbo].[KycDetails] where employeeid = '" + empId + "'";
                MobileNo = command.ExecuteScalar() == DBNull.Value ? "" : command.ExecuteScalar() == null ? "" : command.ExecuteScalar().ToString();

                connection.Close();

                return KycDetailsList(MobileNo);
            }
            catch (Exception ex)
            { string error = ex.Message; }

            return null;
        }

        public ActionResult CreateKYC(string MobileNo)
        {
            ViewData["MobileNo"] = MobileNo;
            ViewBag.KycDetailsList = KycDetailsList(MobileNo);
            return View();
        }

        [HttpPost]
        public ActionResult CreateKYC(KycDetails kycDetails, string MobileNo)
        {
            HttpPostedFileBase file = Request.Files["ImageData"];
            string resp = "1";

            try
            {
                MobileNo = kycDetails.MobileNo;
                ViewBag.KycDetailsList = KycDetailsList(MobileNo);
                string attachementJson = JsonConvert.SerializeObject("");
                if (string.IsNullOrEmpty(kycDetails.EmployeeId))
                {
                    ViewBag.Response = "Invalid employee id !";
                    return View();
                }
                if (kycDetails.EmployeeName.Length == 0)
                {
                    ViewBag.Response = "Invalid employee name !";
                    return View();
                }
                if (kycDetails.Gender.Length == 0)
                {
                    ViewBag.Response = "Invalid gender value !";
                    return View();
                }
                if (kycDetails.JobProfile.Length == 0)
                {
                    ViewBag.Response = "Invalid job profile !";
                    return View();
                }
                int id = 0;
                IsKycExists(MobileNo, out id);

                Ticket er = new Ticket();
                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand("[App].[CreateKycDetails]", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@Id", kycDetails.Id);
                        cmd.Parameters.AddWithValue("@EmployeeId", kycDetails.EmployeeId);
                        cmd.Parameters.AddWithValue("@EmployeeName", kycDetails.EmployeeName);
                        cmd.Parameters.AddWithValue("@Gender", kycDetails.Gender);
                        cmd.Parameters.AddWithValue("@JobProfile", kycDetails.JobProfile);
                        cmd.Parameters.AddWithValue("@MobileNo", kycDetails.MobileNo);
                        cmd.Parameters.AddWithValue("@Image", kycDetails.Image);
                        cmd.Parameters.AddWithValue("@IdDoc", kycDetails.IdDoc);

                        cmd.Parameters.Add("@KycId", SqlDbType.Int);
                        cmd.Parameters["@KycId"].Direction = ParameterDirection.Output;

                        con.Open();
                        cmd.ExecuteNonQuery();

                        con.Close();

                        if (file.InputStream.Length > 0)
                        {
                            int kycID = Convert.ToInt32(cmd.Parameters["@KycId"].Value);
                            ContentViewModel model = new ContentViewModel()
                            {
                                KycID = kycID,
                                Title = kycDetails.EmployeeId,
                                Description = "ProfileImage"
                            };
                            ContentRepository service = new ContentRepository();
                            int i = service.UploadImageInDataBase(file, model, constr);
                        }
                    }
                }

                ViewBag.KycDetailsList = KycDetailsList(kycDetails.MobileNo);
                ViewBag.Response = "Kyc Details Saved !";
                ViewBag.Saved = "1";
            }
            catch (Exception ex)
            {
                ViewBag.Response = "Data Saving Fail !";
                ViewBag.Saved = "0";
                resp = "0";
            }

            if (resp == "1")
            {
                return RedirectToAction("GoHomePage", new { MobileNo = MobileNo });
            }
            else
            {
                ViewBag.Saved = "0";
                ViewBag.Response = "Invalid Employee Name, etc.";
                return View();
            }
            //ViewData["MobileNo"] = MobileNo;
            //ViewBag.KycDetailsList = KycDetailsList(MobileNo);
            //return View();
        }

        void SetAmenetiesList(string MobileNo)
        {
            ViewData["MobileNo"] = MobileNo;
            ViewBag.AmenitiesBookings = AmenityBookingDataList(MobileNo);

            List<SelectListItem> Amenities = new List<SelectListItem>();
            DataSet ds = AmenitiesList();
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                Amenities.Add(new SelectListItem { Text = ds.Tables[0].Rows[i][0].ToString(), Value = ds.Tables[0].Rows[i][1].ToString() });
            }
            ViewBag.Amenities = Amenities.AsEnumerable();
        }

        [HttpPost]
        public ActionResult CreateAmeniBooking(AmenitiesBooking amenitiesBooking, string MobileNo)
        {
            string attachementJson = JsonConvert.SerializeObject("");
            SetAmenetiesList(MobileNo);
            if (amenitiesBooking.NoOfPersons == 0)
            {
                ViewBag.Response = "Invalid Nos of persons !";
                return View();
            }
            if (Convert.ToDateTime(amenitiesBooking.TimeTo) <= Convert.ToDateTime(amenitiesBooking.TimeFr))
            {
                ViewBag.Response = "From time can not greater then to time !";
                return View();
            }
            if (ValidateAminityBookingData(MobileNo, amenitiesBooking.AmenityTime, amenitiesBooking.TimeFr, amenitiesBooking.TimeTo, amenitiesBooking.AmenityID, amenitiesBooking.NoOfPersons)) { }
            else
            {
                ViewBag.Response = "This booking areadly exists !";
                return View();
            }

            SqlConnection connection;
            SqlCommand command = new SqlCommand();
            connection = new SqlConnection(constr);
            connection.Open();

            string sql = "select max(userid) from [Identity].[Users] where IsActive=1 and ((IsDeleted is null) or (IsDeleted = 0)) and ContactNumber='" + MobileNo + "'";
            command = new SqlCommand();
            command.Connection = connection;
            command.CommandType = CommandType.Text;
            command.CommandText = sql;
            string userID = command.ExecuteScalar().ToString();

            sql = "select AmenitiesName from [Master].[Amenities] where AmenitiesId=" + amenitiesBooking.AmenityID;
            command = new SqlCommand();
            command.Connection = connection;
            command.CommandType = CommandType.Text;
            command.CommandText = sql;
            string amenityName = command.ExecuteScalar().ToString();

            connection.Close();

            Ticket er = new Ticket();
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand("[App].[CreateAmenitiesBooking]", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@UserId", userID);
                    cmd.Parameters.AddWithValue("@AmenityId", amenitiesBooking.AmenityID);
                    cmd.Parameters.AddWithValue("@NosOfPersons", amenitiesBooking.NoOfPersons);
                    cmd.Parameters.AddWithValue("@BookingDate", amenitiesBooking.AmenityTime);
                    cmd.Parameters.AddWithValue("@SlotTimeFr", amenitiesBooking.TimeFr);
                    cmd.Parameters.AddWithValue("@SlotTimeTo", amenitiesBooking.TimeTo);

                    cmd.Parameters.Add("@BookingId", SqlDbType.Int);
                    cmd.Parameters["@BookingId"].Direction = ParameterDirection.Output;

                    con.Open();
                    cmd.ExecuteNonQuery();

                    con.Close();

                    var propertyID = "4";

                    FormDataNoticeBoard formDataNoticeBoard = new FormDataNoticeBoard()
                    {
                        StatementType = "C",
                        PropertyId = Convert.ToInt32(propertyID),
                        Subject = "Amenities Booking",
                        Message = amenityName + " Booked for " + amenitiesBooking.NoOfPersons + " for time slot: " + amenitiesBooking.TimeFr + " - " + amenitiesBooking.TimeTo,
                        NotificationTypeId = 1,
                        PropertyGroupId = 3,
                        // 1,2,3
                        Notify = 1,
                        AlertTypeId = 1,
                        ExpirtyDate = DateTime.Now.Date.AddDays(3).ToString("yyyy-MM-dd"),
                        PropertyDetailsId = "",
                        PropertyTowerId = "",
                        PropertyRWAMemberId = ""
                    };
                    SendNotification(formDataNoticeBoard, MobileNo);
                }
            }

            SetAmenetiesList(MobileNo);
            ViewBag.Response = "Amenities Booking Saved !";
            ViewBag.Saved = "Saved";
            return View();// RedirectToAction("CreateAmeniBooking", new { Mobile = MobileNo }); // RedirectToAction("Intermidiate", new { Mobile = MobileNo });
        }

        public IEnumerable<TicketTypeModel> GetAllTicketType()
        {
            List<TicketTypeModel> oticket = new List<TicketTypeModel>();
            SqlConnection connection;
            SqlCommand command = new SqlCommand();
            connection = new SqlConnection(constr);
            connection.Open();
            command.Connection = connection;
            command.CommandType = CommandType.Text;

            command.CommandText = "Select TicketTypeId,Type,Status From [App].TicketType order by TicketTypeId";
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                oticket.Add(new TicketTypeModel
                {
                    TicketTypeId = Convert.ToInt32(reader["TicketTypeId"]),
                    Type = Convert.ToString(reader["Type"]),
                    Status = Convert.ToInt32(reader["Status"])
                });
            }
            connection.Close();

            return oticket.OrderBy(x => x.Type).ToList();
        }

        public ActionResult CreateNewTicket(string MobileNo)
        {
            ViewBag.TicketType = GetAllTicketType();
            ViewData["MobileNo"] = MobileNo;
            return View();
        }
        [HttpPost]
        public ActionResult CreateNewTicket(Ticket tcs, string MobileNo)
        {

            string attachementJson = JsonConvert.SerializeObject("");

            tcs.TicketTypeId =  Convert.ToInt32(tcs.TicketType);

            SqlConnection connection;
            SqlCommand command = new SqlCommand();
            connection = new SqlConnection(constr);
            connection.Open();

            string sql = "select max(userid) from [Identity].[Users] where IsActive=1 and ((IsDeleted is null) or (IsDeleted = 0)) and ContactNumber='" + MobileNo + "'";
            command = new SqlCommand();
            command.Connection = connection;
            command.CommandType = CommandType.Text;
            command.CommandText = sql;
            string userID = command.ExecuteScalar().ToString();
            string propertyID = "";
            string propertyDetailID = "";
            if (userID.Length > 0)
            {
                sql = "select top(1) PropertyId from [App].[UserPropertyAssignment] where userid=" + userID + " order by UserPropertyAssignmentId desc";
                command = new SqlCommand();
                command.Connection = connection;
                command.CommandType = CommandType.Text;
                command.CommandText = sql;
                propertyID = command.ExecuteScalar() == null ? "1" : command.ExecuteScalar().ToString();
                if (command.ExecuteScalar() == null || command.ExecuteScalar() == DBNull.Value)
                {
                    sql = "select top 1 PropertyId from [App].[PropertyDetails] where ContactNumber = '" + MobileNo + "'";
                    command = new SqlCommand();
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = sql;
                    propertyID = command.ExecuteScalar() == null ? "1" : command.ExecuteScalar().ToString();
                }
            }
            else
            {
                userID = "1";// MobileNo;
                sql = "select top 1 PropertyId from [App].[PropertyDetails] where ContactNumber = '" + MobileNo + "'";
                command = new SqlCommand();
                command.Connection = connection;
                command.CommandType = CommandType.Text;
                command.CommandText = sql;
                propertyID = command.ExecuteScalar() == null ? "1" : command.ExecuteScalar().ToString();
            }

            sql = "select top 1 PropertyDetailsId from [App].[PropertyDetails] where ContactNumber = '" + MobileNo + "' and (IsDeleted is null or IsDeleted = 0)";
            command = new SqlCommand();
            command.Connection = connection;
            command.CommandType = CommandType.Text;
            command.CommandText = sql;
            propertyDetailID = command.ExecuteScalar() == null ? "1" : command.ExecuteScalar().ToString();

            string ticketTypeID = tcs.TicketTypeId.ToString();// command.ExecuteScalar().ToString();
            string teamMemberID = "58";// command.ExecuteScalar().ToString();

            connection.Close();

            Ticket er = new Ticket();
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand("App.CreateNewTicket", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@PropertyId", propertyID);
                    cmd.Parameters.AddWithValue("@TicketOrigion", tcs.TicketOrigin);
                    cmd.Parameters.AddWithValue("@TicketTitle", tcs.Title);
                    cmd.Parameters.AddWithValue("@Desctiption", tcs.Description);
                    cmd.Parameters.AddWithValue("@TicketPriorityId", tcs.TicketPriorityId);
                    cmd.Parameters.AddWithValue("@ReportedBy", Convert.ToInt64(userID));
                    cmd.Parameters.AddWithValue("@TicketTypeId", ticketTypeID);
                    cmd.Parameters.AddWithValue("@PropertyDetaildId", propertyDetailID);
                    cmd.Parameters.AddWithValue("@CurrentUserId", Convert.ToInt64(userID));
                    cmd.Parameters.AddWithValue("@Visibility", tcs.Visibility);
                    cmd.Parameters.AddWithValue("@TeamMemberId", teamMemberID);
                    cmd.Parameters.AddWithValue("@AttachmentJson", "{}");
                    cmd.Parameters.AddWithValue("@Ticketchanel", "App");

                    cmd.Parameters.Add("@TicketId", SqlDbType.Int);
                    cmd.Parameters["@TicketId"].Direction = ParameterDirection.Output;

                    con.Open();
                    cmd.ExecuteNonQuery();

                    con.Close();
                }
            }

            return RedirectToAction("Complaint", new { Mobile = MobileNo });
        }

        public ActionResult GetAllTowers(string MobileNo)
        {
            string connetionString = null;
            SqlConnection connection;
            SqlDataAdapter adapter;
            SqlCommand command = new SqlCommand();
            SqlParameter param;
            DataSet ds = new DataSet();
            DataSet dss = new DataSet();
            DataSet dsss = new DataSet();


            int i = 0;

            //connetionString = "Data Source=WIN-HBSI1RRBVE0;Initial Catalog=UfirmApp_Production;integrated security=true";
            connection = new SqlConnection(constr);

            connection.Open();
            command.Connection = connection;
            //command.CommandType = CommandType.StoredProcedure;
            //command.CommandText = "[App].[GetComplaintss]";

            //// param = new SqlParameter("@Mobile", Mobile);
            //param.Direction = ParameterDirection.Input;
            //param.DbType = DbType.String;
            //command.Parameters.Add(param);




            //selecting Name

            command.CommandType = CommandType.Text;
            command.CommandText = "select Towername from [App].[PropertyTowers]";
            adapter = new SqlDataAdapter(command);
            adapter.Fill(ds);

            var eList = (ds.Tables[0].AsEnumerable().Select(dataRow => new PropertyTower
            {
                Towername = dataRow.Field<string>("Towername"),

            })).ToList();

            connection.Close();


            return View(eList);

        }

        public ActionResult TowerDetail(string id)
        {
            SqlConnection connection;
            SqlDataAdapter adapter;
            SqlCommand command = new SqlCommand();
            DataSet ds = new DataSet();
            DataSet dss = new DataSet();
            DataSet dsss = new DataSet();

            int i = 0;

            //connetionString = "Data Source=WIN-HBSI1RRBVE0;Initial Catalog=UfirmApp_Production;integrated security=true";
            connection = new SqlConnection(constr);

            connection.Open();
            command.Connection = connection;

            string query = "select max(PropertyTowerId) towerid from [App].[PropertyTowers] where Towername='" + id + "'";
            command.CommandType = CommandType.Text;
            command.CommandText = query;
            adapter = new SqlDataAdapter(command);
            adapter.Fill(ds);

            string towerID = ds.Tables[0].Rows[0]["towerid"].ToString();

            query = "Select a.name, b.Resident from [App].PropertyMember a,[App].[ResidentType] b where a.ResidentTypeId = b.ResidentTypeId and a.PropertyMemberId in(select PropertyMemberId from[App].[MemberPropertyMapping] where PropertyDetailId in(select PropertyDetailsId from[App].[PropertyDetails] where PropertyTowerId = " + towerID + "))";

            command.CommandType = CommandType.Text;
            command.CommandText = query;
            adapter = new SqlDataAdapter(command);
            adapter.Fill(ds);

            var eList = (ds.Tables[0].AsEnumerable().Select(dataRow => new PropertyOwnerDetails
            {
                name = dataRow.Field<string>("name"),
                Resident = dataRow.Field<string>("Resident"),

            })).ToList();

            connection.Close();

            return View(eList);

        }

        public ActionResult TicketSuccess()
        {
            return View();
        }

        public ActionResult ResidentDirectory()
        {
            return View();
        }

        public ActionResult EmergencyDirectory(string MobileNo)
        {
            ViewData["MobileNo"] = MobileNo;
            return View();
        }

        public ActionResult Intermidiate(string Mobile)
        {
            SqlConnection connection;
            SqlDataAdapter adapter;
            SqlCommand command = new SqlCommand();
            DataSet ds = new DataSet();

            connection = new SqlConnection(constr);

            connection.Open();

            MobileDashBoard mobileDashBoard = new MobileDashBoard();
            mobileDashBoard.FlatOwnerMobileNo = Mobile;
            bool isResident = false, isSupervior = false, isGuard = false, isAOA = false;
            int ttlTickets = 0, closedTickets = 0, openTickets = 0;

            try
            {
                if (Mobile.ToUpper() != "SUPER")
                {
                    command.Connection = connection;
                    string query = "select top 1 designation from [dbo].EmployeeList where MobileNo = @MobileNo";
                    command.Parameters.AddWithValue("@MobileNo", Mobile);
                    command.CommandType = CommandType.Text;
                    command.CommandText = query;
                    adapter = new SqlDataAdapter(command);
                    adapter.Fill(ds, "desig");
                    string desig = "";
                    if (ds.Tables["desig"].Rows.Count > 0)
                    {
                        desig = ds.Tables["desig"].Rows[0][0] == DBNull.Value ? "" : ds.Tables["desig"].Rows[0][0] == null ? "" : ds.Tables["desig"].Rows[0][0].ToString();
                    }
                    else
                    {
                        command = new SqlCommand();
                        command.Connection = connection;
                        query = "select top 1 designation from [dbo].GuardMaster where MobileNo = @MobileNo";
                        command.Parameters.AddWithValue("@MobileNo", Mobile);
                        command.CommandType = CommandType.Text;
                        command.CommandText = query;
                        adapter = new SqlDataAdapter(command);
                        adapter.Fill(ds, "desig");
                        desig = "";
                        if (ds.Tables["desig"].Rows.Count > 0)
                        {
                            desig = ds.Tables["desig"].Rows[0][0] == DBNull.Value ? "" : ds.Tables["desig"].Rows[0][0] == null ? "" : ds.Tables["desig"].Rows[0][0].ToString();
                        }
                        else
                        {
                            command = new SqlCommand();
                            command.Connection = connection;
                            query = "select top 1 userid from [Identity].Users where ContactNumber = @MobileNo";
                            command.Parameters.AddWithValue("@MobileNo", Mobile);
                            command.CommandType = CommandType.Text;
                            command.CommandText = query;
                            adapter = new SqlDataAdapter(command);
                            adapter.Fill(ds, "desig");
                            desig = "";
                            if (ds.Tables["desig"].Rows.Count > 0)
                            {
                                desig = ds.Tables["desig"].Rows[0][1] == DBNull.Value ? "" : ds.Tables["desig"].Rows[0][1] == null ? "" : "RESIDENT";
                            }
                            else
                            {
                                command = new SqlCommand();
                                command.Connection = connection;
                                query = "SELECT top 1 PropertyId from [App].[PropertyMaster] where ContactNumber = @MobileNo";
                                command.Parameters.AddWithValue("@MobileNo", Mobile);
                                command.CommandType = CommandType.Text;
                                command.CommandText = query;
                                adapter = new SqlDataAdapter(command);
                                adapter.Fill(ds, "desig");
                                desig = "";
                                if (ds.Tables["desig"].Rows.Count > 0)
                                {
                                    desig = ds.Tables["desig"].Rows[0][0] == DBNull.Value ? "" : ds.Tables["desig"].Rows[0][0] == null ? "" : "RESIDENT";
                                }
                            }
                        }

                    }

                    isResident = false; isSupervior = false; isGuard = false; isAOA = false;
                    if (desig.IndexOf("SUPERVISOR") > -1)
                    { isSupervior = true; }
                    else if (desig.IndexOf("GUARD") > -1)
                    { isGuard = true; }
                    else if (desig.IndexOf("RESIDENT") > -1)
                    { isResident = true; }
                    if (Mobile == "9910856284")
                    {
                        isResident = false; isSupervior = false; isGuard = false; isAOA = false;
                        isAOA = true;

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
                    }

                    query = "select top 1 pmem.name FirstName, '' LastName, ptwr.Towername, pdet.FlatDetailNumber Flat, pmem.PropertyMemberId UserId, pdet.PropertyId from [App].[PropertyMember] pmem inner join [App].[MemberPropertyMapping] pmap on pmap.PropertyMemberId=pmem.PropertyMemberId inner join [App].[PropertyDetails] pdet on pdet.PropertyDetailsId=pmap.PropertyDetailId inner join [App].[PropertyTowers] ptwr on ptwr.PropertyTowerId=pdet.PropertyTowerId where pmem.IsActive = 1 and pmap.IsActive = 1 and (pdet.IsDeleted is null or pdet.IsDeleted=0) and  (ptwr.IsDeleted is null or ptwr.IsDeleted=0) and pmem.ContactNumber=@MobileNo";
                    command.CommandType = CommandType.Text;
                    command.CommandText = query;
                    adapter = new SqlDataAdapter(command);
                    adapter.Fill(ds, "PrpertyDetails");
                    if (ds.Tables["PrpertyDetails"].Rows.Count < 1)
                    {
                        query = "select top 1 usrd.FirstName, usrd.LastName, proptwr.Towername, propdet.FlatDetailNumber Flat, usrd.UserId, prop.PropertyId from[Identity].[Users] usrd inner join[App].[UserPropertyAssignment] prop on prop.userid = usrd.userid inner join[App].[PropertyTowers] proptwr on   proptwr.PropertyId = prop.PropertyId inner join[App].[PropertyDetails] propdet on propdet.PropertyTowerId = proptwr.PropertyTowerId where usrd.IsActive=1 and (prop.isdeleted is null) and (proptwr.IsDeleted is null) and (propdet.isdeleted is null) and usrd.ContactNumber = @MobileNo order by proptwr.PropertyTowerId desc, propdet.PropertyDetailsId desc, usrd.UserId desc";
                        command.CommandType = CommandType.Text;
                        command.CommandText = query;
                        adapter = new SqlDataAdapter(command);
                        adapter.Fill(ds, "PrpertyDetails");
                    }

                    query = "select count(tkt.ticketid) TicketCount from [App].[Ticket] tkt inner join [Identity].[Users] usr on usr.UserId = tkt.CreatedBy where (tkt.IsDeleted is null) and usr.ContactNumber=@MobileNo";

                    string propID = "";
                    if (ds.Tables["PrpertyDetails"].Rows.Count > 0)
                    {
                        propID = ds.Tables["PrpertyDetails"].Rows[0]["PropertyId"] == DBNull.Value ? "" : ds.Tables["PrpertyDetails"].Rows[0]["PropertyId"] == null ? "" : ds.Tables["PrpertyDetails"].Rows[0]["PropertyId"].ToString();
                    }

                    command.Parameters.AddWithValue("@PropertyId", propID);
                    command.CommandType = CommandType.Text;
                    command.CommandText = query;
                    adapter = new SqlDataAdapter(command);
                    adapter.Fill(ds, "TotalTickets");
                    if (ds.Tables.Count > 2 && ds.Tables["TotalTickets"].Rows.Count > 0)
                    {
                        ttlTickets = Convert.ToInt32(ds.Tables["TotalTickets"].Rows[0]["TicketCount"]);

                        query = "select count(tkt.ticketid) TicketCount from [App].[Ticket] tkt inner join [Identity].[Users] usr on usr.UserId = tkt.CreatedBy where (tkt.IsDeleted is null) and usr.ContactNumber=@MobileNo and StatusTypeId = 4";
                        command.CommandType = CommandType.Text;
                        command.CommandText = query;
                        adapter = new SqlDataAdapter(command);
                        adapter.Fill(ds, "TicketDetails");
                        if (ds.Tables.Count > 3 && ds.Tables["TicketDetails"].Rows.Count > 0)
                        {
                            closedTickets = Convert.ToInt32(ds.Tables["TicketDetails"].Rows[0][0].ToString());
                            openTickets = ttlTickets - closedTickets;
                        }
                    }
                }
                else
                { isSupervior = true; }
                mobileDashBoard.FlatOwnerName = "";
                mobileDashBoard.TenantName = "";
                mobileDashBoard.TowerNumber = "";
                mobileDashBoard.FlatNumber = "";
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables["PrpertyDetails"].Rows.Count > 0)
                    {
                        mobileDashBoard.FlatOwnerName = ds.Tables["PrpertyDetails"].Rows[0]["FirstName"].ToString() + " " + ds.Tables["PrpertyDetails"].Rows[0]["LastName"].ToString();

                        mobileDashBoard.TenantName = "Ram Lal";
                        mobileDashBoard.TowerNumber = ds.Tables["PrpertyDetails"].Rows[0]["Towername"].ToString();
                        mobileDashBoard.FlatNumber = ds.Tables["PrpertyDetails"].Rows[0]["Flat"].ToString();
                    }
                }
                mobileDashBoard.TotalComplains = ttlTickets.ToString();
                mobileDashBoard.OpenComplains = openTickets.ToString();
                mobileDashBoard.ClosedComplains = closedTickets.ToString();
                mobileDashBoard.IsResident = isResident;
                mobileDashBoard.IsSupervisor = isSupervior;
                mobileDashBoard.IsGuard = isGuard;
                mobileDashBoard.IsAOA = isAOA;

                connection.Close();
            }
            catch (Exception ex)
            {
                string error = ex.Message;
                command.Connection.Close();
            }

            ViewData["mobileDashBoard"] = mobileDashBoard;
            return View(ViewData["mobileDashBoard"]);
        }
        [HttpPost]
        public ActionResult Intermidiate(MemberComplaintRegistration e)
        {
            HttpResponseMessage response = GlobaVariables.WebApiClient.PostAsJsonAsync("MemberComplaintRegistrations", e).Result;
            var mob = e.Mobile;
            return RedirectToAction("Complaint", new { Mobile = mob });
        }

        public ActionResult SocietyDirectory(string MobileNo)
        {
            ViewData["MobileNo"] = MobileNo;
            return View();
        }

        public ActionResult SignupSuccess()
        {
            return View();
        }

        public ActionResult TicketDetails(int Id)
        {
            //SqlConnection connection;
            //connection = new SqlConnection(constr);

            //string Query = "select * from [App].[Ticket] where TicketId=@Tic";
            //SqlCommand cmd = new SqlCommand(Query, connection);
            //connection.Open();
            //cmd.Parameters.AddWithValue("@Tic", Id);
            //SqlDataReader sdr = cmd.ExecuteReader();
            //sdr.Read();
            //var data = sdr.GetValue(1).ToString();
            //return View(data);
            //connection.Close();
            var data = dbObj.Tickets.Where(model => model.TicketId == Id).FirstOrDefault();
            return View(data);

        }
        public ActionResult Signup()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Signup(MemberComplaintRegistration e)
        {
            HttpResponseMessage response = GlobaVariables.WebApiClient.PostAsJsonAsync("MemberComplaintRegistrations", e).Result;
            return RedirectToAction("Signin");
        }

        public ActionResult Signin()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Signin(MemberComplaintRegistration e)
        {
            HttpResponseMessage response = GlobaVariables.WebApiClient.PostAsJsonAsync("MemberComplaintRegistrations", e).Result;
            var mob = e.Mobile;
            ViewData["MobileNo"] = mob;

            int desig = 0;
            string data = "";
            if (mob.ToUpper() != "SUPER")
            {
                SqlConnection connection;
                connection = new SqlConnection(constr);
                string Query = "select UserTypeId from [Identity].[Users] where ContactNumber=@ContactNumber order by userid desc";
                SqlCommand cmd = new SqlCommand(Query, connection);
                connection.Open();
                cmd.Parameters.AddWithValue("@ContactNumber", mob);

                DataSet ds = new DataSet();
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(ds);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    data = ds.Tables[0].Rows[0][0].ToString();
                }
                Query = "select count(id) from [dbo].EmployeeList where Designation like '%SUPERVISOR%' and  MobileNo = @ContactNumber";
                cmd = new SqlCommand(Query, connection);
                cmd.Parameters.AddWithValue("@ContactNumber", mob);
                ds = new DataSet();
                adapter = new SqlDataAdapter(cmd);
                adapter.Fill(ds, "desig");
                desig = ds.Tables["desig"].Rows[0][0] == DBNull.Value ? 0 : ds.Tables["desig"].Rows[0][0] == null ? 0 : Convert.ToInt32(ds.Tables["desig"].Rows[0][0].ToString());
                connection.Close();
            }
            //if (desig==1 || mob.ToUpper() == "SUPER")// || data == "3" || data == "4" || data == "5" || data == "6")
            //{
            //    return RedirectToAction("TaskTransaction", "Tasks");
            //}
            //else
            //{
            return RedirectToAction("Intermidiate", new { Mobile = mob });
            //}
        }

        public ActionResult GoHomePage(string MobileNo)
        {
            return RedirectToAction("Intermidiate", new { Mobile = MobileNo });
        }

        public ActionResult GoFirst(string MobileNo)
        {
            ViewData["MobileNo"] = MobileNo;
            return View();
        }
        public ActionResult GoThird(string MobileNo)
        {
            ViewData["MobileNo"] = MobileNo;
            return View();
        }
    }
}



    
