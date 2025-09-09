using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAF.GstMatching.Models.CompareGst
{
    public class DashboardDataModel
    {
        public int TotalRecords { get; set; }

        public int MatchedRecordsCount { get; set; }
        public decimal MatchedRecordsSum { get; set; }
        public int PartiallyMatchedRecordsCount { get; set; }
        public decimal PartiallyMatchedRecordsSum { get; set; }
        public int UnmatchedRecordsCount { get; set; }
        public decimal UnmatchedRecordsSum { get; set; }


    }
}
