using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAF.GstMatching.Models
{
	public class ClientAPIDataModel
	{
		public string ClientGstin { get; set; }
		public string APIIRISUsername { get; set; }
		public string APIIRISPassword { get; set; }
        public string GstPortalUsername { get; set; }
    }
}
