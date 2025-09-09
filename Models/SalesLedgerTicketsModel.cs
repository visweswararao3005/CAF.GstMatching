using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAF.GstMatching.Models
{
    public class SalesLedgerTicketsModel
    {
        public string UniqueValue { get; set; }
        public string RequestNumber { get; set; }
        public string ClientName { get; set; }
        public string CLientEmail { get; set; }
        public string ClientGstin { get; set; }
        public Nullable<System.DateTime> RequestCreatedDate { get; set; }
        public Nullable<System.DateTime> RequestUpdatedDate { get; set; }
        public Nullable<System.DateTime> RequestCompleteDate { get; set; }
        public string FileName { get; set; }
        public string EInvoiceFileName { get; set; }
        public string EWayBillFileName { get; set; }
        public string Edit { get; set; }
        public string FinancialYear { get; set; }
        public string PeriodType { get; set; }
        public string Period { get; set; }
        public string TicketStatus { get; set; }
    }
}
