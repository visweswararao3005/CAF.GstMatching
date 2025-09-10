using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAF.GstMatching.Models.UserModel
{
    public class MenuItemsModel
    {
        public long id { get; set; }
        public string email { get; set; }
        public string userGstin { get; set; }
        public string userName { get; set; }
        public string allowMasterMenuItems { get; set; }
        public string allowIrisMenuItems { get; set; }
    }
}
