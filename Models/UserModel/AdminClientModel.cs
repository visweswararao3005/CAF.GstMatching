using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAF.GstMatching.Models.UserModel
{
	public class AdminClientModel
	{
		public int Unique_ID { get; set; }
		public string Email { get; set; }
		public string ClientGSTIN { get; set; }
        public string ClientName { get; set; }
        public Nullable<System.DateTime> FromDate { get; set; }
        public Nullable<System.DateTime> ToDate { get; set; }




        public override bool Equals(object obj)
        {
            return obj is AdminClientModel other &&
                   ClientGSTIN == other.ClientGSTIN &&
                   ClientName == other.ClientName;
        }
        public override int GetHashCode()
        {
            unchecked // allow overflow
            {
                int hash = 17;
                hash = hash * 23 + (ClientGSTIN?.GetHashCode() ?? 0);
                hash = hash * 23 + (ClientName?.GetHashCode() ?? 0);
                return hash;
            }
        }

    }
}
