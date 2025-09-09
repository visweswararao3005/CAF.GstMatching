using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAF.GstMatching.Models
{
    public class SalesLedgerEInvoicesDataModel
    {
        public string UniqueValue { get; set; }
        public string RecipientGSTIN { get; set; }
        public string ReceiverName { get; set; }
        public string InvoiceNumber { get; set; }
        public Nullable<System.DateTime> Invoicedate { get; set; }
        public string Date { get; set; }
        public string Month { get; set; }
        public string Year { get; set; }
        public Nullable<decimal> InvoiceValue { get; set; }
        public string PlaceOfSupply { get; set; }
        public string ReverseCharge { get; set; }
        public string ApplicableTaxRate { get; set; }
        public string InvoiceType { get; set; }
        public string ECommerceGSTIN { get; set; }
        public string TaxRate { get; set; }
        public Nullable<decimal> TaxableValue { get; set; }
        public Nullable<decimal> IGST { get; set; }
        public Nullable<decimal> CGST { get; set; }
        public Nullable<decimal> SGST { get; set; }
        public Nullable<decimal> CESS { get; set; }
        public string IRN { get; set; }
        public Nullable<System.DateTime> IRNDate { get; set; }
        public string Einvoicestatus { get; set; }
        public string RequestNumber { get; set; }
        public string ClientGstin { get; set; }
        public Nullable<System.DateTime> GSTR1AutoPopulationDate { get; set; }   
        public string GSTR1AutoPopulationStatus { get; set; }
        public string ErrorInAutoPopulationDeletion { get; set; }
    }
}
