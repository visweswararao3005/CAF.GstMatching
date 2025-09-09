using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAF.GstMatching.Models.PurchaseTicketModel
{
    public class TicketsStatusModel
    {
        public string EXERTIDCOL { get; set; }
        public string REFEXERTIDCOL { get; set; }
        public Nullable<bool> EXERTSTATUS { get; set; }
        public string EXERTROLLCODE { get; set; }
        public string EXERTROLLNAME { get; set; }
        public string EXERTUSERCODE { get; set; }
        public string EXERTUSERNAME { get; set; }
        public string EXERTENTITYCODE { get; set; }
        public string EXERTENTITYNAME { get; set; }
        public Nullable<System.DateTime> EXERTENTRYDATE { get; set; }
        public string ClientGSTIN { get; set; }
        public string RequestNo { get; set; }
        public Nullable<System.DateTime> RequestCreatedDate { get; set; }
        public Nullable<System.DateTime> RequestUpdatedDate { get; set; }
        public Nullable<System.DateTime> RequestCompletedDateTime { get; set; }
        public string TxnPeriod { get; set; }
        public string TicketStatus { get; set; }
        public string NoEdit { get; set; }
        public Nullable<System.DateTime> CompletedDate { get; set; }
        public string FinancialYear { get; set; }
        public string PeriodType { get; set; }
		public string FileName { get; set; }
        public string AdminFileName { get; set; }
        public string Email { get; set; }

	}
}
