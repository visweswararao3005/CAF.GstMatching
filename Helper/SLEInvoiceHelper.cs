using CAF.GstMatching.Data;
using CAF.GstMatching.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Validation;
using System.Data.Entity;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAF.GstMatching.Helper
{
    public class SLEInvoiceHelper
    {
        private readonly ApplicationDbContext _context;
        public SLEInvoiceHelper(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task SaveEInvoiceDataAsync(DataTable dataTable, string requestNo, string gstin)
        {
            // SL Invoice Headers
            var SLEInvoice = ConfigurationManager.AppSettings["SLEInvoice"];
            var SLEInvoiceList = SLEInvoice.Split(',').ToList();

            //Console.WriteLine("SLEInvoiceList");
            //foreach (var item in SLEInvoiceList)
            //{
            //    Console.WriteLine(item);
            //}

            // First, check if records with the given ticket exist
            var existingData = _context.Sales_Ledger_EInvoices.Where(x => x.Request_Number == requestNo).ToList();
            // If they exist, remove them
            if (existingData.Any())
            {
                _context.Sales_Ledger_EInvoices.RemoveRange(existingData);
                await _context.SaveChangesAsync(); // Save after deletion before inserting new records
            }
            foreach (DataRow row in dataTable.Rows)
            {
                DateTime? invoiceDate = null;
                string dateString = row[SLEInvoiceList[3]]?.ToString();
                string day = null, month = null, year = null;
                if (DateTime.TryParseExact(dateString, "dd-MM-yyyy hh:mm:ss tt",
                           CultureInfo.InvariantCulture,
                           DateTimeStyles.None, out DateTime parsedDate))
                {
                    invoiceDate = parsedDate;
                    day = parsedDate.ToString("dd");
                    month = parsedDate.ToString("MM");
                    year = parsedDate.ToString("yyyy");
                }
               
                DateTime? InrDateTime = null;
                string InrDateString = row[SLEInvoiceList[17]]?.ToString();
                if (DateTime.TryParseExact(InrDateString, "dd-MM-yyyy hh:mm:ss tt",
                           CultureInfo.InvariantCulture,
                           DateTimeStyles.None, out DateTime parsedInrDate))
                {
                    InrDateTime = parsedInrDate;
                }
                //Console.WriteLine("InrDateTime: " + InrDateTime);
                DateTime? Gstr1Date = null;
                string Gstr1DateString = row[SLEInvoiceList[19]]?.ToString();
                if (DateTime.TryParseExact(Gstr1DateString, "dd-MM-yyyy hh:mm:ss tt",
                           CultureInfo.InvariantCulture,
                           DateTimeStyles.None, out DateTime parsedGstr1Date))
                {
                    Gstr1Date = parsedGstr1Date;
                }
                //Console.WriteLine("Gstr1Date: " + Gstr1Date);

                var SLEInvoiceData = new Sales_Ledger_EInvoice
                {
                    //GSTIN/UIN of Recipient	Receiver Name	Invoice Number	Invoice date	Invoice Value	Place Of Supply	0-5
                    //Reverse Charge	Applicable % of Tax Rate	Invoice Type	E-Commerce GSTIN	Rate	Taxable Value	6-11
                    //Integrated Tax	Central Tax	State/UT Tax	Cess Amount	IRN	IRN date	E-invoice status	            12-18
                    //GSTR-1 auto-population/ deletion upon cancellation date	                              19
                    //GSTR-1 auto-population/ deletion status	,Error in auto-population/ deletion   20,21
                    Unique_Value = "SLEINV-" + Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper(),
                    
                    Recipient_GSTIN = row[SLEInvoiceList[0]]?.ToString(),
                    Receiver_Name = row[SLEInvoiceList[1]]?.ToString(),
                    Invoice_Number = row[SLEInvoiceList[2]]?.ToString(),
                    Invoice_date = invoiceDate,                 
                    Invoice_Value = ParseDecimal(row[SLEInvoiceList[4]]),
                    Place_Of_Supply = row[SLEInvoiceList[5]]?.ToString(),
                    Reverse_Charge = row[SLEInvoiceList[6]]?.ToString(),
                    Applicable_Tax_Rate = row[SLEInvoiceList[7]]?.ToString(),
                    Invoice_Type = row[SLEInvoiceList[8]]?.ToString(),
                    E_Commerce_GSTIN = row[SLEInvoiceList[9]]?.ToString(),
                    Tax_Rate = row[SLEInvoiceList[10]]?.ToString(),
                    Taxable_Value = ParseDecimal(row[SLEInvoiceList[11]]),
                    IGST = ParseDecimal(row[SLEInvoiceList[12]]),
                    CGST = ParseDecimal(row[SLEInvoiceList[13]]),
                    SGST = ParseDecimal(row[SLEInvoiceList[14]]),
                    CESS = ParseDecimal(row[SLEInvoiceList[15]]),
                    IRN = row[SLEInvoiceList[16]]?.ToString(),
                    IRN_Date = InrDateTime,
                    E_invoice_status = row[SLEInvoiceList[18]]?.ToString(),
                    GSTR1_Auto_Population_Deletion_Date = Gstr1Date, // This is not used in the model
                    GSTR1_Auto_Population_Deletion_Status = row[SLEInvoiceList[20]]?.ToString(), // This is not used in the model
                    Error_In_Auto_Population_Deletion = row[SLEInvoiceList[21]]?.ToString(), // This is not used in the model

                    Request_Number = requestNo,
                    Client_Gstin = gstin,
                    Date = day,
                    Month = month,
                    Year = year,
                };
             _context.Sales_Ledger_EInvoices.Add(SLEInvoiceData);
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
        public async Task<List<SalesLedgerEInvoicesDataModel>> GetSLEInvoiceData(string requestNo, string Clientgstin) // to download the data in excel
        {
            var SalesLedgerEInvoiceData = await _context.Sales_Ledger_EInvoices
                .Where(x => x.Request_Number == requestNo && x.Client_Gstin == Clientgstin)
                .ToListAsync();
            var result = SalesLedgerEInvoiceData.Select((item, index) => new SalesLedgerEInvoicesDataModel
            {
                //GSTIN/UIN of Recipient	Receiver Name	Invoice Number	Invoice date	Invoice Value	Place Of Supply	0-5
                //Reverse Charge	Applicable % of Tax Rate	Invoice Type	E-Commerce GSTIN	Rate	Taxable Value	6-11
                //Integrated Tax	Central Tax	State/UT Tax	Cess Amount	IRN	IRN date	E-invoice status	            12-18
                //GSTR-1 auto-population/ deletion upon cancellation date	                              19
                //GSTR-1 auto-population/ deletion status	,Error in auto-population/ deletion   20,21

                RecipientGSTIN = item.Recipient_GSTIN,
                ReceiverName = item.Receiver_Name,
                InvoiceNumber = item.Invoice_Number,
                Invoicedate = item.Invoice_date,
                InvoiceValue = item.Invoice_Value,
                PlaceOfSupply = item.Place_Of_Supply,
                ReverseCharge = item.Reverse_Charge,
                ApplicableTaxRate = item.Applicable_Tax_Rate, // This is not used in the model
                InvoiceType = item.Invoice_Type,
                ECommerceGSTIN = item.E_Commerce_GSTIN,
                TaxRate = item.Tax_Rate,
                TaxableValue = item.Taxable_Value,
                IGST = item.IGST,
                CGST = item.CGST,
                SGST = item.SGST,
                CESS = item.CESS,
                IRN = item.IRN,
                IRNDate = item.IRN_Date,
                Einvoicestatus = item.E_invoice_status,
                GSTR1AutoPopulationDate = item.GSTR1_Auto_Population_Deletion_Date,
                GSTR1AutoPopulationStatus = item.GSTR1_Auto_Population_Deletion_Status,
                ErrorInAutoPopulationDeletion = item.Error_In_Auto_Population_Deletion,

            }).ToList();

            return result;
        }
        public async Task<DataTable> GetUserDataBasedOnTicketAsync(string requestNo, string Clientgstin)  // For Comparision
        {
            var SalesLedgerEInvoiceData = await _context.Sales_Ledger_EInvoices
                .Where(x => x.Request_Number == requestNo && x.Client_Gstin == Clientgstin)
                .ToListAsync();
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("Unique Value");
            dataTable.Columns.Add("Request Number");
            dataTable.Columns.Add("Client Gstin");
            dataTable.Columns.Add("Recipient GSTIN");
            dataTable.Columns.Add("Receiver Name");
            dataTable.Columns.Add("Invoice Number");
            dataTable.Columns.Add("Invoice date");
            dataTable.Columns.Add("Date");
            dataTable.Columns.Add("Month");
            dataTable.Columns.Add("Year");
            dataTable.Columns.Add("Invoice Value");
            dataTable.Columns.Add("Place Of Supply");
            dataTable.Columns.Add("Reverse Charge");
       
            dataTable.Columns.Add("Invoice Type");
            dataTable.Columns.Add("E-Commerce GSTIN");
            dataTable.Columns.Add("Tax Rate");
            dataTable.Columns.Add("Taxable Value");
            dataTable.Columns.Add("Integrated Tax");
            dataTable.Columns.Add("Central Tax");
            dataTable.Columns.Add("State/UT Tax");
            dataTable.Columns.Add("Cess Amount");
            dataTable.Columns.Add("IRN");
            dataTable.Columns.Add("IRN date");
            dataTable.Columns.Add("E-invoice status");
           


            

            foreach (var item in SalesLedgerEInvoiceData)
            {
                DataRow row = dataTable.NewRow();

                row["Unique Value"] = item.Unique_Value;
                row["Request Number"] = requestNo;
                row["Client Gstin"] = Clientgstin;
                row["Recipient GSTIN"] = item.Recipient_GSTIN;
                row["Receiver Name"] = item.Receiver_Name;
                row["Invoice Number"] = item.Invoice_Number;
                row["Invoice date"] = item.Invoice_date;
                row["Date"] = item.Date;
                row["Month"] = item.Month;
                row["Year"] = item.Year;
                row["Invoice Value"] = item.Invoice_Value;
                row["Place Of Supply"] = item.Place_Of_Supply;
                
                row["Reverse Charge"] = item.Reverse_Charge;
                row["Invoice Type"] = item.Invoice_Type;
                row["E-Commerce GSTIN"] = item.E_Commerce_GSTIN;
                row["Tax Rate"] = item.Tax_Rate;
                row["Taxable Value"] = item.Taxable_Value;
                row["Integrated Tax"] = item.IGST;
                row["Central Tax"] = item.CGST;
                row["State/UT Tax"] = item.SGST;
                row["Cess Amount"] = item.CESS;
                row["IRN"] = item.IRN;
                row["IRN date"] = item.IRN_Date;
                row["E-invoice status"] = item.E_invoice_status;
               
                // Add more columns as needed

                dataTable.Rows.Add(row);
            }
            return dataTable;
        }

        public async Task saveEInvoiceAPIsData(APIsDataModel data)
        {

            var Data = new Sales_Ledger_EInvoice_APIs_datum
            {
                Client_Gstin = data.ClientGstin,
                Request_Number = data.RequestNumber,
                Session_ID = data.SessionID,
                Request_URL = data.RequestURL,
                Request_Parameters = data.RequestParameters,
                Request_Headers = data.RequestHeaders,
                Request_Body = data.RequestBody,
                Response = data.Response,
                Response_Code = data.ResponseCode,
                Status = data.Status,
                Logged_Date_Time = DateTime.Now, // Assuming you want to set the current time when saving	
            };
            _context.Sales_Ledger_EInvoice_APIs_Data.Add(Data);
            await _context.SaveChangesAsync();
        }

        public async Task saveEinvoiceTokenData(SalesLedgerTokenDataModel data)
        {

            // Check if a record already exists with same Request_Number and Client_Gstin
            var existingToken = await _context.Sales_Ledger_Token_Data
                .FirstOrDefaultAsync(x => x.RequestNumber == data.requestNumber && x.ClientGstin == data.clientGstin);

            if (existingToken != null)
            {
                _context.Sales_Ledger_Token_Data.Remove(existingToken);
            }

            // Create a new token data record
            var tokenData = new Sales_Ledger_Token_datum
            {
                ClientGstin = data.clientGstin,
                RequestNumber = data.requestNumber,
                EInvoiceAuthToken = data.eInvoiceAuthToken,
                EInvoiceAuthTokenCreatedDatetime = DateTime.Now,
                EInvoiceTokenExpiry = data.eInvoiceTokenExpiry,
                EInvoiceSek = data.eInvoiceSek,
            };
            _context.Sales_Ledger_Token_Data.Add(tokenData);
            await _context.SaveChangesAsync();
        }

        public async Task updateEinvoiceTokenData(SalesLedgerTokenDataModel data)
        {
            var tokenData = await _context.Sales_Ledger_Token_Data
                .FirstOrDefaultAsync(x => x.RequestNumber == data.requestNumber && x.ClientGstin == data.clientGstin);

            if (tokenData != null)
            {

                tokenData.EInvoiceAuthToken = data.eInvoiceAuthToken;
                tokenData.EInvoiceAuthTokenCreatedDatetime = DateTime.Now;
                tokenData.EInvoiceTokenExpiry = data.eInvoiceTokenExpiry;
                tokenData.EInvoiceSek = data.eInvoiceSek;

                await _context.SaveChangesAsync();
            }
        }

        public async Task<SalesLedgerTokenDataModel> GetEinvoiceTokenDataAsync(string requestNumber, string clientGstin)
        {

            var tokenData = await _context.Sales_Ledger_Token_Data
                .FirstOrDefaultAsync(x => x.RequestNumber.Trim() == requestNumber && x.ClientGstin == clientGstin);

            if (tokenData != null)
            {
                return new SalesLedgerTokenDataModel
                {
                    uniqueId = tokenData.uniqueId,
                    clientGstin = tokenData.ClientGstin,
                    requestNumber = tokenData.RequestNumber,
                    eInvoiceAuthToken = tokenData.EInvoiceAuthToken,
                    eInvoiceAuthTokenCreatedDatetime = tokenData.EInvoiceAuthTokenCreatedDatetime,
                    eInvoiceTokenExpiry = tokenData.EInvoiceTokenExpiry,
                    eInvoiceSek = tokenData.EInvoiceSek,
                };
            }
            return null;
        }


    }
}
