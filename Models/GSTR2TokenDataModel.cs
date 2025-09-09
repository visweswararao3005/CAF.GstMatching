using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAF.GstMatching.Models
{
    public class GSTR2TokenDataModel
    {

        public long uniqueId { get; set; }
        public string ClientGstin { get; set; }
        public string RequestNumber { get; set; }
        public string UserName { get; set; }
        public string XAppKey { get; set; }
        public string OTP { get; set; }
        public string AuthToken { get; set; }
        public Nullable<System.DateTime> AuthTokenCreatedDatetime { get; set; }
        public string Expiry { get; set; }
        public string SEK { get; set; }
    }
}
