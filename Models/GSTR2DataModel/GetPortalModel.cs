using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAF.GstMatching.Models.GSTR2DataModel
{
    public class GetPortalModel
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
        public string day { get; set; }
        public string month { get; set; }
        public string year { get; set; }


        public string ClientGSTIN { get; set; }
        public string GSTRegType { get; set; }
        public string SupplierInvoiceNo { get; set; }
        public Nullable<System.DateTime> SupplierInvoiceDate { get; set; }
        public string SupplierGSTIN { get; set; }
        public string SupplierName { get; set; }
        public string IsRCMApplied { get; set; }
        public Nullable<decimal> InvoiceValue { get; set; }
        public Nullable<decimal> ItemTaxableValue { get; set; }
        public Nullable<decimal> GSTRate { get; set; }
        public Nullable<decimal> IGSTValue { get; set; }
        public Nullable<decimal> CGSTValue { get; set; }
        public Nullable<decimal> SGSTValue { get; set; }
        public Nullable<decimal> CESS { get; set; }
        public string IsReturnFiled { get; set; }
        public string TxnPeriod { get; set; }
        public string Modified { get; set; }

    }
}
