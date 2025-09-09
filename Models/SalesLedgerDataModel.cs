using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAF.GstMatching.Models
{
    public class SalesLedgerDataModel
    {
        public string UniqueValue { get; set; }
        public string RequestNumber { get; set; }
        public string Sno { get; set; }
        public string InvoiceNumber { get; set; }
        public Nullable<System.DateTime> InvoiceDate { get; set; }
        public string Date { get; set; }
        public string Month { get; set; }
        public string Year { get; set; }
        public string ClientGSTIN { get; set; }
        public string CustomerName { get; set; }
        public string CustomerGSTIN { get; set; }
        public string StateToSupply { get; set; }
        public Nullable<decimal> TaxableAmount { get; set; }
        public Nullable<decimal> CESS { get; set; }
        public Nullable<decimal> IGST { get; set; }
        public Nullable<decimal> SGST { get; set; }
        public Nullable<decimal> CGST { get; set; }
        public Nullable<decimal> TotalAmount { get; set; }
        public string GSTRate { get; set; }
        public string HSNSACCode { get; set; }
        public string Quantity { get; set; }
        public string Units { get; set; }
        public string MaterialDescription { get; set; }
        public string TypeOfInvoice { get; set; }
        public string Irn { get; set; }
    }
}
