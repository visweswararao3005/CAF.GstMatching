using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAF.GstMatching.Models.CompareGst
{
    public class UnMatchedDataModel
    {
        public int S_No { get; set; }
        public string EXERTIDCOL { get; set; }
        public string Invoiceno { get; set; }
        public Nullable<System.DateTime> InvoiceDate { get; set; }
        public string SupplierGSTIN { get; set; }
        public Nullable<decimal> TaxAmount { get; set; }
        public string DataSource { get; set; }
        public Nullable<System.DateTime> ModifiedDateTime { get; set; }
        public string RequestNo { get; set; }
        public string ClientGSTIN { get; set; }


    }
}
