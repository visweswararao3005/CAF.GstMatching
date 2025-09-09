using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAF.GstMatching.Models
{
    public class SalesLedgerTokenDataModel
    {
        public long uniqueId { get; set; }
        public string clientGstin { get; set; }
        public string requestNumber { get; set; }
        public string eInvoiceAuthToken { get; set; }
        public Nullable<System.DateTime> eInvoiceAuthTokenCreatedDatetime { get; set; }
        public string eInvoiceSek { get; set; }
        public string eInvoiceTokenExpiry { get; set; }
    }
}
