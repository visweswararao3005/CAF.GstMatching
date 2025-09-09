using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAF.GstMatching.Models.PurchaseDataModel
{
    public class GetInvoiceModel
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
        public string RequestNo { get; set; }


        public string ClientGSTIN { get; set; }     
        public string TxnPeriod { get; set; }
        public string SupplierGSTIN { get; set; }
        public string SupplierName { get; set; }
        public string Invoiceno { get; set; }
        public Nullable<System.DateTime> InvoiceDate { get; set; }
        public Nullable<decimal> TaxableValue { get; set; }
        public Nullable<decimal> CGST { get; set; }
        public Nullable<decimal> SGST { get; set; }
        public Nullable<decimal> IntegratedTax { get; set; }
        public Nullable<decimal> CESS { get; set; }
        public string Modified { get; set; }
    }
}
