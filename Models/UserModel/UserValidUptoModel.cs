using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAF.GstMatching.Models.UserModel
{
    public class UserValidUptoModel
    {
        public string userEmail { get; set; }
        public string userGstin { get; set; }
        public string userName { get; set; }
        public Nullable<System.DateTime> accessFromDate { get; set; }
        public Nullable<System.DateTime> accessToDate { get; set; }

    }
}
