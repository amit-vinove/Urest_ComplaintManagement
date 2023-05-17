using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UrestComplaintManagement.Context
{
    public partial class TaskTransactionModel
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TaskTransactionModel()
        {
            this.TaskTransactionModel1 = new HashSet<TaskTransactionModel>();
        }

        public virtual ICollection<TaskTransactionModel> TaskTransactionModel1 { get; set; }
        public virtual TaskTransactionModel TaskTransactionModel2 { get; set; }
        public int Id { get; set; }
        public int TaskId { get; set; }
        public int TaskCategoryId { get; set; }
        public int TaskSubCategoryId { get; set; }
        public string Name { get; set; }
        public string QRCode { get; set; }
        public string Description { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public DateTime? TimeFrom { get; set; }
        public DateTime? TimeTo { get; set; }
        public string Remarks { get; set; }
        public string Occurence { get; set; }
        public string CategoryName { get; set; }
        public string SubCategoryName { get; set; }
        public int EntryType { get; set; }

        public List<Category> categories { get; set; }
    }

    public class SpotPointMaster
    { 
        public int Id { get; set; }
        public string SpotName { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
    }

    public partial class Category
    {
        public int Id { get; set; }
        public string name { get; set; }
    }

    public partial class TaskWiseTransaction
    {
        public DateTime TransactionDate { get; set; }
        public int TransactionID { get; set; }
        public int CategoryID { get; set; }
        public int SubCategoryID { get; set; }
        public string CategoryName { get; set; }
        public string SubCategoryName { get; set; }
        public string TaskName { get; set; }
        public int QuestID { get; set; }
        public string Occurance { get; set; }
        public string Action { get; set; }
        public string Remarks { get; set; }

        public List<Category> categories { get; set; }
    }

    public partial class TaskWiseQuestionnaire
    {
        public TaskWiseQuestionnaire()
        {
        }
        public int TransactionID { get; set; }
        public int CategoryID { get; set; }
        public int SubCategoryID { get; set; }
        public string CategoryName { get; set; }
        public string SubCategoryName { get; set; }
        public int TaskID { get; set; }
        public DateTime TransDate { get; set; }
        public string TaskName { get; set; }
        public string Occurance { get; set; }
        public int QuestID { get; set; }
        public string QuestionName { get; set; }
        public string Action { get; set; }
        public string Remarks { get; set; }
    }

    public partial class Notification
    {
        public int NotificationId { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public int NotificationTypeId { get; set; }
        public int PropertyGroupId { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CrOnStr { get; set; }
        public int CreatedBy { get; set; }
        public DateTime ExpirtyDate { get; set; }
        public string ExDtStr { get; set; }
        public int PropertyId { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsRead { get; set; }
    }
}
