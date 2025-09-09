using CAF.GstMatching.Data;
using CAF.GstMatching.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    public class SLEWayBillHelper
    {
        private readonly ApplicationDbContext _context;
        public SLEWayBillHelper(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task SaveEWayBillDataAsync(DataTable dataTable, string requestNo, string gstin)
        {
            // SL Invoice Headers
            var SLEWayBill = ConfigurationManager.AppSettings["SLEWayBill"];
            var SLEWayBillList = SLEWayBill.Split(',').ToList();

            //Console.WriteLine($"SLEWayBillList");
            //foreach (var item in SLEWayBillList)
            //{
            //    Console.WriteLine(item);
            //}

            // First, check if records with the given ticket exist
            var existingData = _context.Sales_Ledger_EWay_Bills.Where(x => x.Request_Number == requestNo).ToList();
            // If they exist, remove them
            if (existingData.Any())
            {
                _context.Sales_Ledger_EWay_Bills.RemoveRange(existingData);
                await _context.SaveChangesAsync(); // Save after deletion before inserting new records
            }
            foreach (DataRow row in dataTable.Rows)
            {
                DateTime? invoiceDate = null, EwbDate = null , ValidTillDate = null;

                string ewbDateString = row[SLEWayBillList[1]]?.ToString();            
                if(DateTime.TryParseExact(ewbDateString, "dd-MM-yyyy hh:mm:ss tt",
                                             CultureInfo.InvariantCulture,
                                             DateTimeStyles.None, out DateTime parsedEwbDate))
                {
                    EwbDate = parsedEwbDate;
                }
                string dateString = row[SLEWayBillList[4]]?.ToString();
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
                string validTillDateString = row[SLEWayBillList[17]]?.ToString();
                if (DateTime.TryParseExact(validTillDateString, "dd-MM-yyyy hh:mm:ss tt",
                                            CultureInfo.InvariantCulture,
                                            DateTimeStyles.None, out DateTime parsedValidTillDate))
                {
                    ValidTillDate = parsedValidTillDate;
                }
             

                var SLData = new Sales_Ledger_EWay_Bill
                {   //EWB No	EWB Date	Supply Type	Doc.No	Doc.Date	Doc.Type	TO GSTIN	status	
					//No of Items	Main HSN Code	Main HSN Desc	Assessable Value	
					//SGST Value	CGST Value	IGST Value CESS Value Total Invoice Value	Valid Till Date	Gen.Mode
					Unique_Value = "SLEWayINV-" + Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper(),
                  
                    EWB_Number = row[SLEWayBillList[0]]?.ToString(),
                    EWB_Date = EwbDate,
                    Supply_Type = row[SLEWayBillList[2]]?.ToString(),
                    Doc_Number = row[SLEWayBillList[3]]?.ToString(),
                    Doc_Date = invoiceDate,               
                    Doc_Type = row[SLEWayBillList[5]]?.ToString(),
                    TO_GSTIN = row[SLEWayBillList[6]]?.ToString(),
                    status = row[SLEWayBillList[7]]?.ToString(),
                    No_of_Items = row[SLEWayBillList[8]]?.ToString(),
                    Main_HSN_Code = row[SLEWayBillList[9]]?.ToString(),
                    Main_HSN_Desc = row[SLEWayBillList[10]]?.ToString(),
                    Assessable_Value = ParseDecimal(row[SLEWayBillList[11]]),
                    SGST = ParseDecimal(row[SLEWayBillList[12]]),
                    CGST = ParseDecimal(row[SLEWayBillList[13]]),
                    IGST = ParseDecimal(row[SLEWayBillList[14]]), // Assuming IGST is at the same index as Total Invoice Value
                    CESS = ParseDecimal(row[SLEWayBillList[15]]), // Assuming CESS is at the same index as Valid Till Date

                    Total_Invoice_Value = ParseDecimal(row[SLEWayBillList[16]]),
                    Valid_Till_Date = ValidTillDate,
                    Gen_Mode = row[SLEWayBillList[18]]?.ToString(),

                    Request_Number = requestNo,
                    Client_Gstin = gstin,
                    Date = day,
                    Month = month,
                    Year = year,
                };
                _context.Sales_Ledger_EWay_Bills.Add(SLData);
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
            catch (Exception ex)
            {
                //Console.WriteLine(ex.Message);
				throw new Exception("Failed to insert Data: " + ex.Message, ex);
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

        public async Task<List<SalesLedgerEWayBillsDataModel>> GetSLEWayBillData(string requestNo, string Clientgstin) // to download the data in excel
        {
            var SalesLedgerEWayBillData = await _context.Sales_Ledger_EWay_Bills
                .Where(x => x.Request_Number == requestNo && x.Client_Gstin == Clientgstin)
                .ToListAsync();
            var result = SalesLedgerEWayBillData.Select((item, index) => new SalesLedgerEWayBillsDataModel
            {
                EWBNumber = item.EWB_Number,
                EWBDate = item.EWB_Date,
                SupplyType = item.Supply_Type,
                DocNumber = item.Doc_Number,
                DocDate = item.Doc_Date,
                DocType = item.Doc_Type,
                TOGSTIN = item.TO_GSTIN,
                status = item.status,
                NoofItems = item.No_of_Items,
                MainHSNCode = item.Main_HSN_Code,
                MainHSNDesc = item.Main_HSN_Desc,
                AssessableValue = item.Assessable_Value,
                SGST = item.SGST,
                CGST = item.CGST,
                IGST = item.IGST,
                CESS = item.CESS,
                TotalInvoiceValue = item.Total_Invoice_Value,
                ValidTillDate = item.Valid_Till_Date,
                GenMode = item.Gen_Mode,
            }).ToList();

            return result;
        }
        public async Task<DataTable> GetUserDataBasedOnTicketAsync(string requestNo, string Clientgstin)  // For Comparision
        {
            var SalesLedgerEWayBillData = await _context.Sales_Ledger_EWay_Bills
                .Where(x => x.Request_Number == requestNo && x.Client_Gstin == Clientgstin)
                .ToListAsync();
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("Unique Value");
            dataTable.Columns.Add("Request Number");
            dataTable.Columns.Add("Client Gstin");
            dataTable.Columns.Add("EWB Number");
            dataTable.Columns.Add("EWB Date");
            dataTable.Columns.Add("Supply Type");
            dataTable.Columns.Add("Doc Number");
            dataTable.Columns.Add("Doc Date");
            dataTable.Columns.Add("Date");
            dataTable.Columns.Add("Month");
            dataTable.Columns.Add("Year");
            dataTable.Columns.Add("Doc Type");
            dataTable.Columns.Add("TO GSTIN");
            dataTable.Columns.Add("Status");
            dataTable.Columns.Add("No of Items");
            dataTable.Columns.Add("Main HSN Code");
            dataTable.Columns.Add("Main HSN Desc");
            dataTable.Columns.Add("Assessable Value");
            dataTable.Columns.Add("SGST");
            dataTable.Columns.Add("CGST");
            dataTable.Columns.Add("IGST");
            dataTable.Columns.Add("CESS");
            dataTable.Columns.Add("Total Invoice Value");
            dataTable.Columns.Add("Valid Till Date");
            dataTable.Columns.Add("Gen Mode");
            dataTable.Columns.Add("Client GSTIN");




            foreach (var item in SalesLedgerEWayBillData)
            {
                DataRow row = dataTable.NewRow();

                row["Unique Value"] = item.Unique_Value;
                row["Request Number"] = requestNo;
                row["Client Gstin"] = Clientgstin;
                row["EWB Number"] = item.EWB_Number;
                row["EWB Date"] = item.EWB_Date?.ToString("dd-MM-yyyy");
                row["Supply Type"] = item.Supply_Type;
                row["Doc Number"] = item.Doc_Number;
                row["Doc Date"] = item.Doc_Date?.ToString("dd-MM-yyyy");
                row["Date"] = item.Date;
                row["Month"] = item.Month;
                row["Year"] = item.Year;
                row["Doc Type"] = item.Doc_Type;
                row["TO GSTIN"] = item.TO_GSTIN;
                row["Status"] = item.status;
                row["No of Items"] = item.No_of_Items;
                row["Main HSN Code"] = item.Main_HSN_Code;
                row["Main HSN Desc"] = item.Main_HSN_Desc;
                row["Assessable Value"] = item.Assessable_Value?.ToString("0.00");
                row["SGST"] = item.SGST?.ToString("0.00");
                row["CGST"] = item.CGST?.ToString("0.00");
                row["IGST"] = item.IGST?.ToString("0.00");
                row["CESS"] = item.CESS?.ToString("0.00");
                row["Total Invoice Value"] = item.Total_Invoice_Value?.ToString("0.00");
                row["Valid Till Date"] = item.Valid_Till_Date?.ToString("dd-MM-yyyy");
                row["Gen Mode"] = item.Gen_Mode;
                row["Client GSTIN"] = item.Client_Gstin;


                dataTable.Rows.Add(row);
            }
            return dataTable;
        }
        public async Task saveEwayBillAPIsData(APIsDataModel data)
        {

			var Data = new Sales_Ledger_EWay_Bill_APIs_datum
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
			_context.Sales_Ledger_EWay_Bill_APIs_Data.Add(Data);
			await _context.SaveChangesAsync();
		}

     
    }
}
