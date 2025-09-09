using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAF.GstMatching.Models
{
    public class SalesLedgerEWayBillsDataModel
    {
        public string UniqueValue { get; set; }
        public string RequestNumber { get; set; }
        public string EWBNumber { get; set; }
        public Nullable<System.DateTime> EWBDate { get; set; }
        public string Date { get; set; }
        public string Month { get; set; }
        public string Year { get; set; }
        public string SupplyType { get; set; }
        public string DocNumber { get; set; }
        public Nullable<System.DateTime> DocDate { get; set; }
        public string DocType { get; set; }
        public string TOGSTIN { get; set; }
        public string status { get; set; }
        public string NoofItems { get; set; }
        public string MainHSNCode { get; set; }
        public string MainHSNDesc { get; set; }
        public Nullable<decimal> AssessableValue { get; set; }
        public Nullable<decimal> SGST { get; set; }
        public Nullable<decimal> CGST { get; set; }
        public Nullable<decimal> IGST { get; set; }
        public Nullable<decimal> CESS { get; set; }
        public Nullable<decimal> TotalInvoiceValue { get; set; }
        public Nullable<System.DateTime> ValidTillDate { get; set; }
        public string GenMode { get; set; }
        public string ClientGstin { get; set; }
    }
}
