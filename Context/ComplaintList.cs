using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UrestComplaintManagement.Context
{
    public class ComplaintList
    {
        [key]
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
    }
}