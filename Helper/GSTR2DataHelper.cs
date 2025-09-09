using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using CAF.GstMatching.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using CAF.GstMatching.Models.GSTR2DataModel;
using System.Globalization;
using System.Configuration;
using CAF.GstMatching.Models;

namespace CAF.GstMatching.Helper
{
   
    public class GSTR2DataHelper
    {
        private readonly ApplicationDbContext _context;
        public GSTR2DataHelper(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task SaveGSTR2DataAsync(DataTable dataTable, string Request, string clientgstin)
        {
            var portalHeaders = ConfigurationManager.AppSettings["PortalHeaders"];
            var portalHeadersList = portalHeaders.Split(',').ToList();

            // First, check if records with the given ticket exist
            var existingData = _context.GSTR2_Data.Where(x => x.Request_No == Request && x.Client_GSTIN == clientgstin).ToList();

            // If they exist, remove them
            if (existingData.Any())
            {
                _context.GSTR2_Data.RemoveRange(existingData);
                await _context.SaveChangesAsync(); // Save after deletion before inserting new records
            }

            foreach (DataRow row in dataTable.Rows)
            {
                string dateString = row[portalHeadersList[3]]?.ToString();
                //Console.WriteLine(dateString);
                //string dateString = row["Doc Date"]?.ToString();
                DateTime? invoiceDate = null;
                string day = null, month = null, year = null;

                string[] formates = { "dd-MM-yyyy hh:mm:ss tt", "dd-MM-yyyy" };

                if (DateTime.TryParseExact(dateString, formates,
                           CultureInfo.InvariantCulture,
                           DateTimeStyles.None, out DateTime parsedDate))
                {
                    invoiceDate = parsedDate;
                    day = parsedDate.ToString("dd");
                    month = parsedDate.ToString("MM");
                    year = parsedDate.ToString("yyyy");
                }


                string dateString1 = row[portalHeadersList[15]]?.ToString();
                //string dateString1 = row["Tax Period"]?.ToString();
                string txnPeriod = dateString1;
                if (DateTime.TryParse(dateString1, out DateTime parsedDate1))
                {
                    txnPeriod = parsedDate1.ToString("MMM-yy"); // e.g., "022025"
                }

               

                var GSTR2Data = new GSTR2_Data
                {//"User GSTIN,GstRegType,invoiceno,invoice_date,SupplierGSTIN,SupplierName,
                 //IsRcmApplied,InvoiceValue,ItemTaxableValue,GstRate,IGSTAmount,
                 //CGSTAmount,SGSTAmount,CESS,IsReturnFiled,ReturnPeriod"
                    EXERT_ID_COL = "GSTR2-" + Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper(),
                    Client_GSTIN = row[portalHeadersList[0]]?.ToString(),
                    GST_Reg_Type = row[portalHeadersList[1]]?.ToString(),
                    Supplier_Invoice_No = row[portalHeadersList[2]]?.ToString(),
                    Supplier_Invoice_Date = invoiceDate,
                    Supplier_GSTIN = row[portalHeadersList[4]]?.ToString(),
                    Supplier_Name = row[portalHeadersList[5]]?.ToString(),
                    IsRCMApplied = row[portalHeadersList[6]]?.ToString(),
                    Invoice_Value = ParseDecimal(row[portalHeadersList[7]]),
                    Item_Taxable_Value = ParseDecimal(row[portalHeadersList[8]]),
                    GST_Rate = ParseDecimal(row[portalHeadersList[9]]),
                    IGST_Value = ParseDecimal(row[portalHeadersList[10]]),
                    CGST_Value = ParseDecimal(row[portalHeadersList[11]]),
                    SGST_Value = ParseDecimal(row[portalHeadersList[12]]),
                    CESS = ParseDecimal(row[portalHeadersList[13]]),
                    IsReturnFiled = row[portalHeadersList[14]]?.ToString(),
                    Txn_Period = txnPeriod,

                    Request_No = Request,
                    Day = day,
                    Month = month,
                    Year = year,


                };
                _context.GSTR2_Data.Add(GSTR2Data);

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

        public async Task<DataTable> GetPortalDataBasedOnTicketAsync(string ticket , string gstin )
        {
            var dataTable = new DataTable();
            dataTable.Columns.Add("EXERT_ID_COL");
            dataTable.Columns.Add("Client_GSTIN");
            dataTable.Columns.Add("GST_Reg_Type");
            dataTable.Columns.Add("Supplier_Invoice_No");
            dataTable.Columns.Add("Supplier_Invoice_Date");
            dataTable.Columns.Add("Day");
            dataTable.Columns.Add("Month");
            dataTable.Columns.Add("Year");
            dataTable.Columns.Add("Supplier_GSTIN");
            dataTable.Columns.Add("Supplier_Name");
            dataTable.Columns.Add("IsRCMApplied");
            dataTable.Columns.Add("Invoice_Value");
            dataTable.Columns.Add("Item_Taxable_Value");
            dataTable.Columns.Add("GST_Rate");
            dataTable.Columns.Add("IGST_Value");
            dataTable.Columns.Add("CGST_Value");
            dataTable.Columns.Add("SGST_Value");
            dataTable.Columns.Add("CESS");
            dataTable.Columns.Add("IsReturnFiled");
            dataTable.Columns.Add("Txn_Period");
            dataTable.Columns.Add("Request_No");

            var gstr2Data = await _context.GSTR2_Data
                .Where(x => x.Request_No == ticket && x.Client_GSTIN == gstin)
                .ToListAsync();

            foreach (var item in gstr2Data)
            {
                var row = dataTable.NewRow();
                row["EXERT_ID_COL"] = item.EXERT_ID_COL;
                row["Client_GSTIN"] = item.Client_GSTIN;
                row["GST_Reg_Type"] = item.GST_Reg_Type;
                row["Supplier_Invoice_No"] = item.Supplier_Invoice_No;
                row["Supplier_Invoice_Date"] = item.Supplier_Invoice_Date;
                row["Supplier_GSTIN"] = item.Supplier_GSTIN;
                row["Supplier_Name"] = item.Supplier_Name;
                row["IsRCMApplied"] = item.IsRCMApplied;
                row["Invoice_Value"] = item.Invoice_Value;
                row["Item_Taxable_Value"] = item.Item_Taxable_Value;
                row["GST_Rate"] = item.GST_Rate;
                row["IGST_Value"] = item.IGST_Value;
                row["CGST_Value"] = item.CGST_Value;
                row["SGST_Value"] = item.SGST_Value;
                row["CESS"] = item.CESS;
                row["IsReturnFiled"] = item.IsReturnFiled;
                row["Txn_Period"] = item.Txn_Period;
                row["Request_No"] = item.Request_No;
                row["Day"] = item.Day;
                row["Month"] = item.Month;
                row["Year"] = item.Year;

                dataTable.Rows.Add(row);
            }               
            return dataTable;
        }

        public async Task<List<GetPortalModel>> GetPortalData(string ticket , string gstin)
        {
           var portalData = await _context.GSTR2_Data
                .Where(x => x.Request_No == ticket && x.Client_GSTIN == gstin)
                .Select(x => new GetPortalModel
                {
                   
                    ClientGSTIN = x.Client_GSTIN,
                    GSTRegType = x.GST_Reg_Type,
                    SupplierInvoiceNo = x.Supplier_Invoice_No,
                    SupplierInvoiceDate = x.Supplier_Invoice_Date,
                    SupplierGSTIN = x.Supplier_GSTIN,
                    SupplierName = x.Supplier_Name,
                    IsRCMApplied = x.IsRCMApplied,
                    InvoiceValue = x.Invoice_Value,
                    ItemTaxableValue = x.Item_Taxable_Value,
                    GSTRate = x.GST_Rate,
                    IGSTValue = x.IGST_Value,
                    CGSTValue = x.CGST_Value,
                    SGSTValue = x.SGST_Value,
                    CESS = x.CESS,
                    IsReturnFiled = x.IsReturnFiled,
                    TxnPeriod = x.Txn_Period
                }).ToListAsync();

            return portalData;
        }

        public async Task savePortalAPIsData(APIsDataModel data)
        {

            var Data = new GSTR2_APIs_Data
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
            _context.GSTR2_APIs_Data.Add(Data);
            await _context.SaveChangesAsync();
        }

        public async Task saveTokenData(GSTR2TokenDataModel data)
        {

            // Check if a record already exists with same Request_Number and Client_Gstin
            var existingToken = await _context.GSTR2_Token_Data
                .FirstOrDefaultAsync(x => x.Request_Number == data.RequestNumber && x.Client_Gstin == data.ClientGstin);

            if (existingToken != null)
            {
                _context.GSTR2_Token_Data.Remove(existingToken);
            }

            // Create a new token data record
            var tokenData = new GSTR2_Token_Data
            {
                Client_Gstin = data.ClientGstin,
                Request_Number = data.RequestNumber,
                User_Name = data.UserName,
                X_App_Key = data.XAppKey,
                OTP = data.OTP,
                Auth_Token = data.AuthToken,
                Auth_Token_Created_Datetime = DateTime.Now ,
                Expiry = data.Expiry,
                SEK = data.SEK
            };
            _context.GSTR2_Token_Data.Add(tokenData);
            await _context.SaveChangesAsync();
        }

        public async Task updateTokenData(GSTR2TokenDataModel data)
        {
            var tokenData = await _context.GSTR2_Token_Data
                .FirstOrDefaultAsync(x => x.Request_Number == data.RequestNumber && x.Client_Gstin == data.ClientGstin);

            if (tokenData != null)
            {

                tokenData.Auth_Token = data.AuthToken;
                tokenData.Auth_Token_Created_Datetime = DateTime.Now;
                tokenData.Expiry = data.Expiry;
                tokenData.SEK = data.SEK;

                await _context.SaveChangesAsync();
            }
        }

        public async Task<GSTR2TokenDataModel> GetTokenDataAsync(string clientGstin)
        {

            var tokenData = await _context.GSTR2_Token_Data
                 .Where(x => x.Client_Gstin == clientGstin)
                 .OrderByDescending(x => x.uniqueId)
                 .FirstOrDefaultAsync();

            if (tokenData != null)
            {
                return new GSTR2TokenDataModel
                {
                    uniqueId = tokenData.uniqueId,
                    ClientGstin = tokenData.Client_Gstin,
                    RequestNumber = tokenData.Request_Number,
                    UserName = tokenData.User_Name,
                    XAppKey = tokenData.X_App_Key,
                    OTP = tokenData.OTP,
                    AuthToken = tokenData.Auth_Token,
                    AuthTokenCreatedDatetime = tokenData.Auth_Token_Created_Datetime,
                    Expiry = tokenData.Expiry,
                    SEK = tokenData.SEK
                };
            }
            return null;
        }

    }
}
