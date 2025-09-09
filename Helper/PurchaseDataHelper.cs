using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using CAF.GstMatching.Business.Interface;
using CAF.GstMatching.Data;
using CAF.GstMatching.Models.PurchaseDataModel;



namespace CAF.GstMatching.Helper
{
    
    public class PurchaseDataHelper  
    {
        private readonly ApplicationDbContext _context;
        public PurchaseDataHelper(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task SavePurchaseDataAsync(DataTable dataTable, string requestNo, string clientgstin)
        {
            //var InvoiceHeaders = "InvoiceHeaders".AppSetting();
            
            var invoiceHeaders = ConfigurationManager.AppSettings["InvoiceHeaders"];
            var invoiceHeadersList = invoiceHeaders.Split(',').ToList();

            //Console.WriteLine("InvoiceHeaders: " + invoiceHeaders);
              

            // First, check if records with the given ticket exist
            var existingData = _context.Purchase_Data.Where(x => x.Request_No == requestNo).ToList();

            // If they exist, remove them
            if (existingData.Any())
            {
                _context.Purchase_Data.RemoveRange(existingData);
                await _context.SaveChangesAsync(); // Save after deletion before inserting new records
            }
            foreach (DataRow row in dataTable.Rows)
            {
                DateTime? invoiceDate =null ;
                string dateString = row[invoiceHeadersList[5]]?.ToString();
                string day = null, month = null, year = null;
               
                //Console.WriteLine(dateString);
                //string dateString = row["Document Date"]?.ToString();

                if (DateTime.TryParseExact(dateString, "dd-MM-yyyy hh:mm:ss tt",
                           CultureInfo.InvariantCulture,
                           DateTimeStyles.None, out DateTime parsedDate))
                {
                    invoiceDate = parsedDate;
                    day = parsedDate.ToString("dd");
                    month = parsedDate.ToString("MM");
                    year = parsedDate.ToString("yyyy");
                }

                string dateString1 = row[invoiceHeadersList[1]]?.ToString();
                string txnPeriod = dateString1;

                if (DateTime.TryParse(dateString1, out DateTime parsedDate1))
                {
                    txnPeriod = parsedDate1.ToString("MMM-yy", CultureInfo.InvariantCulture); // Use InvariantCulture to avoid culture differences
                }

                var purchaseData = new Purchase_datum
                { 
                    //User GSTIN period Supplier GSTIN Supplier Name invoiceno invoice_date Taxable Value  cgst sgst Integrated Tax  Cess
                    EXERT_ID_COL = "REQ-" + Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper(),
                    Request_No = requestNo,

                    Client_GSTIN = row[invoiceHeadersList[0]]?.ToString(),
                    Txn_Period = txnPeriod,
                    Supplier_GSTIN = row[invoiceHeadersList[2]]?.ToString(),
                    Supplier_Name = row[invoiceHeadersList[3]]?.ToString(),
                    Invoiceno = row[invoiceHeadersList[4]]?.ToString(),
                    Invoice_Date = invoiceDate,
                    Taxable_Value = ParseDecimal(row[invoiceHeadersList[6]]),
                    CGST = ParseDecimal(row[invoiceHeadersList[7]]),
                    SGST = ParseDecimal(row[invoiceHeadersList[8]]),
                    Integrated_Tax = ParseDecimal(row[invoiceHeadersList[9]]),
                    CESS = ParseDecimal(row[invoiceHeadersList[10]]),
                    Day = day,
                    Month = month,
                    Year = year,

                };
               
                _context.Purchase_Data.Add(purchaseData);
            }
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbEntityValidationException ex)
            {
                var message = "";
                foreach (var eve in ex.EntityValidationErrors)
                {                  
                    //Console.WriteLine($"Entity of type \"{eve.Entry.Entity.GetType().Name}\" has the following validation errors:");
                    foreach (var ve in eve.ValidationErrors)
                    {
                        var currentValue = eve.Entry.CurrentValues[ve.PropertyName];
                        message += $"- Column : \"{ve.PropertyName}\", Error: \"{ve.ErrorMessage}\"\n";                   
                        message += $"- Column : \"{ve.PropertyName}\", value: \"{currentValue}\"\n";                   
                        message += $"\n";                    
                    }
                }
                throw new Exception("Failed to insert data: \n" + message );
            }
        }

        private decimal ParseDecimal(object val)
        {
            if (val == null || string.IsNullOrWhiteSpace(val.ToString()))
                return 0;

            if (decimal.TryParse(val.ToString(), out decimal result))
                return result;

            return 0; // fallback if parsing fails
        }

        public async Task<DataTable> GetUserDataBasedOnTicketAsync(string requestNo , string gstin)  // For Comparision
        {
            var purchaseData = await _context.Purchase_Data
                .Where(x => x.Request_No == requestNo && x.Client_GSTIN == gstin)
                .ToListAsync();

            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("EXERT_ID_COL");
            dataTable.Columns.Add("Client_GSTIN");
            dataTable.Columns.Add("Request_No");
            dataTable.Columns.Add("Txn_Period");
            dataTable.Columns.Add("Supplier_GSTIN");
            dataTable.Columns.Add("Supplier_Name");
            dataTable.Columns.Add("Invoiceno");
            dataTable.Columns.Add("Invoice_Date");
            dataTable.Columns.Add("Taxable_Value");
            dataTable.Columns.Add("CGST");
            dataTable.Columns.Add("SGST");
            dataTable.Columns.Add("Integrated_Tax");
            dataTable.Columns.Add("CESS");
            dataTable.Columns.Add("Day");
            dataTable.Columns.Add("Month");
            dataTable.Columns.Add("Year");

            foreach (var item in purchaseData)
            {
                DataRow row = dataTable.NewRow();
                row["EXERT_ID_COL"] = item.EXERT_ID_COL;
                row["Client_GSTIN"] = item.Client_GSTIN;
                row["Request_No"] = item.Request_No;
                row["Txn_Period"] = item.Txn_Period;
                row["Supplier_GSTIN"] = item.Supplier_GSTIN;
                row["Supplier_Name"] = item.Supplier_Name;
                row["Invoiceno"] = item.Invoiceno;
                row["Invoice_Date"] = item.Invoice_Date;
                row["Taxable_Value"] = item.Taxable_Value;
                row["CGST"] = item.CGST;
                row["SGST"] = item.SGST;
                row["Integrated_Tax"] = item.Integrated_Tax;
                row["CESS"] = item.CESS;
                row["Day"] = item.Day;
                row["Month"] = item.Month;
                row["Year"] = item.Year;

                dataTable.Rows.Add(row);
            }

            return dataTable;
        }

        public async Task<List<GetInvoiceModel>> GetInvoiceData(string requestNo, string gstin)
        {
            
            var purchaseData = await _context.Purchase_Data
                .Where(x => x.Request_No == requestNo && x.Client_GSTIN == gstin)
                .ToListAsync();
            var result = purchaseData.Select((item, index) => new GetInvoiceModel
            {
                ClientGSTIN = item.Client_GSTIN,
                TxnPeriod = item.Txn_Period,
                SupplierGSTIN = item.Supplier_GSTIN,
                SupplierName = item.Supplier_Name,
                Invoiceno = item.Invoiceno,
                InvoiceDate = item.Invoice_Date,
                TaxableValue = item.Taxable_Value,
                CGST = item.CGST,
                SGST = item.SGST,
                IntegratedTax = item.Integrated_Tax,
                CESS = item.CESS
            }).ToList();

            return result;
        }
    }
}
