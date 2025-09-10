using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAF.GstMatching.Models.UserModel
{
    public class UserLoginHistoryModel
    {
        public long id { get; set; }
        public string email { get; set; }
        public string gstin { get; set; }
        public string userName { get; set; }
        public string sessionId { get; set; }
        public Nullable<System.DateTime> loginDateTime { get; set; }
        public Nullable<System.DateTime> logoutDateTime { get; set; }
    }
}
