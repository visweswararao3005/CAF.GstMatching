using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAF.GstMatching.Models.UserModel
{
    public class PaymentDetailModel
    {
        public long Id { get; set; }
        public string ClientName { get; set; }
        public string ClientGstin { get; set; }
        public string ClientEmail { get; set; }
        public string ClientPhone { get; set; }
        public string ClientAddress { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public Nullable<int> Days { get; set; }
        public string OrderId { get; set; }
        public string PaymentId { get; set; }
        public string PaymentStatus { get; set; }
        public Nullable<System.DateTime> PaymentDate { get; set; }
        public string TransactionId { get; set; }
    }
}

