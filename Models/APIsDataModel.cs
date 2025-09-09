using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAF.GstMatching.Models
{
	public class APIsDataModel
	{
		public long UniqueID { get; set; }
		public string ClientGstin { get; set; }
		public string RequestNumber { get; set; }
		public string SessionID { get; set; }
		public string RequestURL { get; set; }
		public string RequestParameters { get; set; }
		public string RequestHeaders { get; set; }
		public string RequestBody { get; set; }
		public string Response { get; set; }
		public string ResponseCode { get; set; }
		public string Status { get; set; }
		public Nullable<System.DateTime> LoggedDateTime { get; set; }
	}
}
