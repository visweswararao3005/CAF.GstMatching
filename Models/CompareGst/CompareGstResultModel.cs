using System;

namespace CAF.GstMatching.Models.CompareGst
{
    public class CompareGstResultModel
    {
        public string uniquevalue { get; set; }
        public Nullable<int> RowNumber { get; set; }
        public string RequestNo { get; set; }
        public string ClientGSTIN { get; set; }
        public string DataSource { get; set; }
        public string YearMonth { get; set; }
        public string FinancialYear { get; set; }
        public string Period { get; set; }
        public string MatchingResults { get; set; }
        public string Category { get; set; }
        public string MatchType { get; set; }
        public string InvoiceNumber { get; set; }
        public string InvoiceDate { get; set; }
        public string SupplierGSTIN { get; set; }
        public string SupplierName { get; set; }
        public Nullable<decimal> TaxableValue { get; set; }
        public Nullable<decimal> TotalTax { get; set; }
        public Nullable<decimal> CGST { get; set; }
        public Nullable<decimal> SGST { get; set; }
        public Nullable<decimal> IGST { get; set; }
        public Nullable<decimal> CESS { get; set; }



       
        public string ModifiedInvoiceDate { get; set; }
        public string ModifiedInvoiceNumber { get; set; }
        public string ModifiedSupplierGSTIN { get; set; }
        public Nullable<decimal> ModifiedTotalTax { get; set; }

    }
}
