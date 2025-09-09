using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAF.GstMatching.Models
{
	public class NoticeDataModel
	{
		public int UniqueId { get; set; }
		public string RequestNumber { get; set; }
		public string ClientGstin { get; set; }
		public Nullable<System.DateTime> CreatedDatetime { get; set; }
		public string NoticeTitle { get; set; }
		public Nullable<System.DateTime> NoticeDatetime { get; set; }
		public string Priority { get; set; }
		public string NoticeDescription { get; set; }
		public string status { get; set; }
        public string ClientName { get; set; }
        public Nullable<System.DateTime> ClosedDatetime { get; set; }
        public Nullable<System.DateTime> UpdatedDateTime { get; set; }
        public string FileName { get; set; }
        public string ClosedBy { get; set; }


        public bool HasUnreadMessages { get; set; }  // 👈 Add this
        public bool IsAdmin { get; internal set; }

    }
}
