//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace UrestComplaintManagement.Context
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;

    public partial class Ticket
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Ticket()
        {
            this.Ticket1 = new HashSet<Ticket>();
        }

        public List<TicketLogs> logs { get; set; }
        public List<TicketRating> rating { get; set; }
        public int TicketId { get; set; }
        public string TicketOrigin { get; set; }
        public Nullable<int> ParentTicketId { get; set; }
        public string TicketNumber { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Nullable<int> TicketPriorityId { get; set; }
        public string Label { get; set; }
        public Nullable<System.DateTime> StartDate { get; set; }
        public Nullable<System.DateTime> ActualCloseDate { get; set; }
        public Nullable<int> ReportedBy { get; set; }
        public int TicketTypeId { get; set; }
        public string TicketType { get; set; }

        public Nullable<int> PropertyDetaildId { get; set; }
        public int StatusTypeId { get; set; }
        public Nullable<System.DateTime> ExpectedCloseDate { get; set; }
        public Nullable<int> PropertyId { get; set; }
        public Nullable<System.DateTime> LastUpdatedOn { get; set; }
        public Nullable<int> LastupdatedBy { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public bool Status { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<bool> IsPaused { get; set; }
        public Nullable<int> PausedBy { get; set; }
        public Nullable<System.DateTime> PausedDate { get; set; }
        public Nullable<System.DateTime> PausedTilDate { get; set; }
        public string PausedComment { get; set; }
        public Nullable<bool> IsReopen { get; set; }
        public Nullable<System.DateTime> ReOpenOn { get; set; }
        public Nullable<int> ReopenBy { get; set; }
        public string ReopenReason { get; set; }
        public string TicketChannel { get; set; }
        public string Visibility { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Ticket> Ticket1 { get; set; }
        public virtual Ticket Ticket2 { get; set; }
    }
    public class TicketTypeModel
    {
        public int TicketTypeId { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public string cmdType { get; set; }

        // Added by RG 24/04/2021
        public int Status { get; set; }
        public string StaffMappingList { get; set; }


    }
    public class EmployeeMaster
    {
        public int Id { get; set; }
        public string EmployeeName { get; set; }
        public string FatherName { get; set; }
        public string Designation { get; set; }
        public string MobileNo { get; set; }
    }
    public class AmenitiesBooking
    {
        public int Id { get; set; }
        public int AmenityID { get; set; }
        public int UserID { get; set; }
        public int NoOfPersons { get; set; }
        public DateTime AmenityTime { get; set; }
        public DateTime AmenityTimeTo { get; set; }
        public string TimeFr { get; set; }
        public string TimeTo { get; set; }
        public string AminityName { get; set; }
        public DateTime BookingDate { get; set; }
        public int IsDeleted { get; set; }
        public string BkgDtStr { get; set; }
        public string FrStr { get; set; }
        public string ToStr { get; set; }
        public int Approved { get; set; }
    }
    public class KycDetails
    {
        public int Id { get; set; }
        public string EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string Gender { get; set; }
        public string JobProfile { set; get; }
        public string MobileNo { set; get; }
        public string Image { get; set; }
        public string IdDoc { get; set; }
        public string IdDocNo { get; set; }
        public int IsDeleted { get; set; }
        public int Approved { get; set; }
        public FileContentResult ProfileImage { get; set; }
        public ContentViewModel ImageDetails { get; set; }
    }
    public class KycImages
    {
        [Key]
        public int ID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Contents { get; set; }
        public byte[] Image { get; set; }
    }
    public class AttendanceLogs
    { 
        public int Id { get; set; }
        public string EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public DateTime PunchTime { get; set; }
        public string PunchType { get; set; }
        public string GateNo { get; set; }
        public int CreatedBy { get; set; }
    }
    public class ContentViewModel
    {
        /// <summary>
        /// Get and Set id
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        /// Get and Set id
        /// </summary>
        public int KycID { get; set; }
        /// <summary>
        /// Get and set title of content 
        /// </summary>
        [Required]
        public string Title { get; set; }
        /// <summary>
        /// Get and set Description for content
        /// </summary>
        [Required]
        public string Description { get; set; }
        /// <summary>
        /// Get and set Content for content
        /// </summary>
        [AllowHtml]
        [Required]
        public string Contents { get; set; }
        /// <summary>
        /// Images
        /// </summary>
        [Required]
        public byte[] Image { get; set; }
    }
    public class FormDataNoticeBoard
    {
        public string StatementType { get; set; }
        public int PropertyId { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public int NotificationTypeId { get; set; }
        public int PropertyGroupId { get; set; }
        // 1,2,3
        public int Notify { get; set; }
        public int AlertTypeId { get; set; }
        public string ExpirtyDate { get; set; }
        public string PropertyDetailsId { get; set; }
        public string PropertyTowerId { get; set; }
        public string PropertyRWAMemberId { get; set; }
        public List<NoticeBoardAttachment> NoticeBoardAttachment { get; set; }
    }
    public class NoticeBoardAttachment
    {
        public string filename { get; set; }
        public string filepath { get; set; }
    }
    public class NoticeBoardNoticeData
    {
        public int NoticeboardAlertMaster { get; set; }
        public string Subject { get; set; }
        public string NotificationType { get; set; }
        public string Message { get; set; }
        public string Name { get; set; }
        public string ContactNumber { get; set; }
        public string EmailAddress { get; set; }
    }
    public class NoticeBoardAttachments
    {
        public int NotifactionAttachmentsId { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
    }
    public class TicketLogs
    { 
        public string log { get; set; }
        public DateTime loggedon { get; set; }
    }
    public class TicketRating
    {
        public int TicketId { get; set; }
        public string Rating { get; set; }
        public string RatingDetails { get; set; }
        public int RatededBy { get; set; }
        public DateTime RatedOn { get; set; }

    }
    public class MobileDashBoard 
    {
        public bool IsAOA { get; set; }
        public bool IsResident { get; set; }
        public bool IsSupervisor { get; set; }
        public bool IsGuard { get; set; }
        public string FlatOwnerMobileNo { get; set; }
        public string FlatOwnerName { get; set; }
        public string TenantName { get; set; }
        public string TowerNumber { get; set; }
        public string FlatNumber { get; set; }
        public string TotalComplains { get; set; }
        public string OpenComplains { get; set; }
        public string ClosedComplains { get; set; }
    }
    public class EventList
    {
        public int EventId { get; set; }
        public int SubEventId { get; set; }
        public string EventName { get; set; }
        public string Repeat { get; set; }
        public string EventNumber { get; set; }
        public DateTime? Startdate { get; set; }
        public DateTime? EndDate { get; set; }
        public string StartdateS { get; set; }
        public string EndDateS { get; set; }
    }
    public class AssetsList
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string QRCode { get; set; }
        public int Isdeleted { get; set; }
    }
}
