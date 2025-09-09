using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAF.GstMatching.Models
{
	public class NoticeChatModel
	{
		public int UniqueId { get; set; }
		public string RequestNumber { get; set; }
		public string ClientGstin { get; set; }
		public string Source { get; set; }
		public string Name { get; set; }
		public string Message { get; set; }
		public Nullable<System.DateTime> MessageDatetime { get; set; }
        public Nullable<bool> IsRead { get; set; }

    }
}
