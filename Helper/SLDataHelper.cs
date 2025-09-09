using CAF.GstMatching.Data;
using CAF.GstMatching.Models;
using CAF.GstMatching.Models.PurchaseDataModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAF.GstMatching.Helper
{
    public class SLDataHelper
    {
        private readonly ApplicationDbContext _context;
        public SLDataHelper(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task SaveSalesLedgerDataAsync(DataTable dataTable, string requestNo,string gstin)
        {
            // SL Invoice Headers
            var _SLinvoice = ConfigurationManager.AppSettings["SLInvoice"];
            var _SLinvoiceList = _SLinvoice.Split(',').ToList();
            //Console.WriteLine("SL Invoice Headers:");
            //foreach (var item in _SLinvoiceList)
            //{
            //    Console.WriteLine(item);
            //}


            // First, check if records with the given ticket exist
            var existingData = _context.Sales_Ledger_Data.Where(x => x.Request_Number == requestNo).ToList();
            // If they exist, remove them
            if (existingData.Any())
            {
                _context.Sales_Ledger_Data.RemoveRange(existingData);
                await _context.SaveChangesAsync(); // Save after deletion before inserting new records
            }
            //Console.WriteLine("1");
            foreach (DataRow row in dataTable.Rows)
            {
                DateTime? invoiceDate = null;
                string dateString = row[_SLinvoiceList[2]]?.ToString();
                string day = null, month = null, year = null;
                //Console.WriteLine("1");
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
                //Console.WriteLine("1");

                var SLData = new Sales_Ledger_datum
                {
                    //S.NO	INV NO	DATE	NAME OF CUSTOMERS	GST No	State to Supply(POS)	
                    //TAXABLE AMOUNT	IGST	SGST	CGST	TOTAL	
                    // GST Rates	HSN/SAC Code	Qty	 Units	Material Description		 
                    // Type of Invoice User Gstin	IRN

                    //S.NO,INV NO,DATE,NAME OF CUSTOMERS,GST No,State to Supply(POS)
                    //,TAXABLE AMOUNT,IGST,SGST,CGST,TOTAL,
                    //GST Rates,HSN/SAC Code,Qty,Units,Material Description,
                    //Type of Invoice,User Gstin,IRN

                    Unique_Value = "SLINV-" + Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper(),
                  
                    Sno = row[_SLinvoiceList[0]]?.ToString(),
                    Invoice_Number = row[_SLinvoiceList[1]]?.ToString(),
                    Invoice_Date = invoiceDate,                 
                    Customer_Name = row[_SLinvoiceList[3]]?.ToString(),
                    Customer_GSTIN = row[_SLinvoiceList[4]]?.ToString(),
                    State_To_Supply = row[_SLinvoiceList[5]]?.ToString(),
                    Taxable_Amount = ParseDecimal(row[_SLinvoiceList[6]]),
                    IGST = ParseDecimal(row[_SLinvoiceList[7]]),
                    SGST = ParseDecimal(row[_SLinvoiceList[8]]),
                    CGST = ParseDecimal(row[_SLinvoiceList[9]]),
                    //CESS = ParseDecimal(row[_SLinvoiceList[10]]), // Assuming CESS is at index 10, adjust if needed
                    Total_Amount = ParseDecimal(row[_SLinvoiceList[10]]),
                    GST_Rate = row[_SLinvoiceList[11]]?.ToString(),
                    HSN_SAC_Code = row[_SLinvoiceList[12]]?.ToString(),
                    Quantity = row[_SLinvoiceList[13]]?.ToString(),
                    Units = row[_SLinvoiceList[14]]?.ToString(),
                    Material_Description = row[_SLinvoiceList[15]]?.ToString(),
                    Type_Of_Invoice = row[_SLinvoiceList[16]]?.ToString(),
                    IRN = row[_SLinvoiceList[18]]?.ToString(), // Assuming IRN is at index 17, adjust if needed
                   
                    Request_Number = requestNo,
                    Client_GSTIN = gstin,
                    Date = day,
                    Month = month,
                    Year = year,
                };
                //Console.WriteLine($"Invoice number - {row[_SLinvoiceList[1]]?.ToString()}");
               _context.Sales_Ledger_Data.Add(SLData);
            }
            try
            {
                //Console.WriteLine("Saving Sales Ledger Data to the database...");
                await _context.SaveChangesAsync();
            }
            catch (DbEntityValidationException ex)
            {
                //Console.WriteLine("Validation errors occurred while saving Sales Ledger Data:");
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
                throw new Exception("Failed to insert data: \n" + message);
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
        public async Task<List<SalesLedgerDataModel>> GetSLInvoiceData(string requestNo , string Clientgstin) // to download the data in excel
        {
            var SalesLedgerData = await _context.Sales_Ledger_Data
                .Where(x => x.Request_Number == requestNo && x.Client_GSTIN == Clientgstin)
                .ToListAsync();
            var result = SalesLedgerData.Select((item, index) => new SalesLedgerDataModel
            {
                Sno = item.Sno,
                InvoiceNumber = item.Invoice_Number,
                InvoiceDate = item.Invoice_Date,
                CustomerName = item.Customer_Name,
                CustomerGSTIN = item.Customer_GSTIN,
                StateToSupply = item.State_To_Supply,               
                TaxableAmount = item.Taxable_Amount,
                IGST = item.IGST,
                SGST = item.SGST,
                CGST = item.CGST,
                CESS = 0, // item.CESS, // Uncomment if CESS is needed
                TotalAmount = item.Total_Amount,
                GSTRate = item.GST_Rate,
                HSNSACCode = item.HSN_SAC_Code,
                Quantity = item.Quantity,
                Units = item.Units,
                MaterialDescription = item.Material_Description,
                TypeOfInvoice = item.Type_Of_Invoice,
                Irn = item.IRN,

            }).ToList();

            return result;
        }
        public async Task<DataTable> GetUserDataBasedOnTicketAsync(string requestNo , string Clientgstin)  // For Comparision
        {
            var SalesLedgerData = await _context.Sales_Ledger_Data
                .Where(x => x.Request_Number == requestNo && x.Client_GSTIN == Clientgstin)
                .ToListAsync();
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("Unique Value");
            dataTable.Columns.Add("Request Number");
            dataTable.Columns.Add("Client GSTIN");
            dataTable.Columns.Add("S.NO");
            dataTable.Columns.Add("INV NO");
            dataTable.Columns.Add("DATE");
            dataTable.Columns.Add("Day");
            dataTable.Columns.Add("Month");
            dataTable.Columns.Add("Year");
            dataTable.Columns.Add("NAME OF CUSTOMERS");
            dataTable.Columns.Add("GST No");
            dataTable.Columns.Add("State to Supply(POS)");
            dataTable.Columns.Add("TAXABLE AMOUNT");
            dataTable.Columns.Add("IGST");
            dataTable.Columns.Add("SGST");
            dataTable.Columns.Add("CGST");    
            dataTable.Columns.Add("CESS"); // Uncomment if CESS is needed
            dataTable.Columns.Add("TOTAL");
            dataTable.Columns.Add("GST Rates");
            dataTable.Columns.Add("HSN/SAC Code");
            dataTable.Columns.Add("Qty");
            dataTable.Columns.Add("Units");
            dataTable.Columns.Add("Material Description");
            dataTable.Columns.Add("Type of Invoice");
           
            foreach (var item in SalesLedgerData)
            {
                DataRow row = dataTable.NewRow();
                row["Unique Value"] = item.Unique_Value;
                row["Request Number"] = requestNo;
                row["Client GSTIN"] = Clientgstin;
                row["S.NO"] = item.Sno;
                row["INV NO"] = item.Invoice_Number;
                row["DATE"] = item.Invoice_Date;
                row["Day"] = item.Date;
                row["Month"] = item.Month;
                row["Year"] = item.Year;
                row["NAME OF CUSTOMERS"] = item.Customer_Name;
                row["GST No"] = item.Customer_GSTIN;
                row["State to Supply(POS)"] = item.State_To_Supply;
                row["TAXABLE AMOUNT"] = item.Taxable_Amount;
                row["IGST"] = item.IGST;
                row["SGST"] = item.SGST;
                row["CGST"] = item.CGST;
                row["CESS"] = 0; // item.CESS; // Uncomment if CESS is needed
                row["TOTAL"] = item.Total_Amount;
                row["GST Rates"] = item.GST_Rate;
                row["HSN/SAC Code"] = item.HSN_SAC_Code;
                row["Qty"] = item.Quantity;
                row["Units"] = item.Units;
                row["Material Description"] = item.Material_Description;
                row["Type of Invoice"] = item.Type_Of_Invoice;

                dataTable.Rows.Add(row);
            }
            return dataTable;
        }

		public async Task<List<string>> GetOtherDates(string ticketNo, string ClientGSTIN, string period , string formate)  // For fetching other dates outside the selected period
		{
			// Step 1: Parse the input period (e.g., "Mar-25") into a DateTime
			DateTime periodStart = DateTime.ParseExact("01-" + period, "dd-MMM-yy", null);
			DateTime periodEnd = periodStart.AddMonths(1).AddDays(-1); // End of the month

			// Step 2: Fetch raw invoice dates from DB
			var dates = await _context.Sales_Ledger_Data
				.Where(p => p.Request_Number == ticketNo && p.Client_GSTIN == ClientGSTIN)
				.Select(p => p.Invoice_Date)
				.ToListAsync();

			// Step 3: Filter out dates in the period and format the rest as yyyy-MM-dd
			var result = dates
				.Where(d => d < periodStart || d > periodEnd) // Exclude dates in selected period
				.Select(d => d.Value.ToString(formate))        // Format to string
				.Distinct()                                   // Ensure uniqueness
				.ToList();

			return result;
		}

        public async Task<List<string>> GetIRNData(string requestNo, string Clientgstin)
        {
			var SalesLedgerData = await _context.Sales_Ledger_Data
				.Where(x => x.Request_Number == requestNo && x.Client_GSTIN == Clientgstin)
				.ToListAsync();

			var result = SalesLedgerData.Select(item => item.IRN).Distinct().ToList();
			return result;
        }
	}
}
