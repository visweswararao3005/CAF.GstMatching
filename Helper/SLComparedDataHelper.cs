using CAF.GstMatching.Data;
using CAF.GstMatching.Models;
using CAF.GstMatching.Models.CompareGst;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Data.Entity.Validation;
using System.Globalization;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace CAF.GstMatching.Helper
{
    public class SLComparedDataHelper
    {
        private readonly ApplicationDbContext _context;
        public SLComparedDataHelper(ApplicationDbContext context)
        {
            _context = context;
        }

        private static class I //  1  Invoice (SLInvoice)
        {

            public const string UniqueValue = "Unique Value";
            public const string requestNo = "Request Number";
            public const string clientGSTIN = "Client GSTIN";
            public const string InvoiceNo = "INV NO";
            public const string InvoiceDate = "DATE";
            public const string Day = "Day";
            public const string Month = "Month";
            public const string Year = "Year";
            public const string CustomerName = "NAME OF CUSTOMERS";
            public const string CustomerGSTIN = "GST No";
            public const string TaxableAmount = "TAXABLE AMOUNT";
            public const string IGST = "IGST";
            public const string SGST = "SGST";
            public const string CGST = "CGST";
            public const string CESS = "CESS";
            public const string Total = "TOTAL";


            public const string SNo = "S.NO";          
            public const string POS = "State to Supply(POS)";           
            public const string TaxRate = "GST Rates";
            public const string HSNCode = "HSN/SAC Code";
            public const string Quantity = "Qty";
            public const string Units = "Units";
            public const string MaterialDescription = "Material Description";
            public const string InvoiceType = "Type of Invoice";
        }
        private static class EI // 2  E-Invoice (SLEInvoice)
        {
            public const string UniqueValue = "Unique Value";
            public const string requestNo = "Request Number";
            public const string clientGSTIN = "Client GSTIN";
            public const string InvoiceNo = "Invoice Number";
            public const string InvoiceDate = "Invoice date";
            public const string Day = "Date";
            public const string Month = "Month";
            public const string Year = "Year";
            public const string CustomerGSTIN = "Recipient GSTIN";
            public const string CustomerName = "Receiver Name";
            public const string TaxableAmount = "Taxable Value";
            public const string IGST = "Integrated Tax";
            public const string CGST = "Central Tax";
            public const string SGST = "State/UT Tax";
            public const string CESS = "Cess Amount";
            public const string Total = "Invoice Value";


            public const string POS = "Place Of Supply";
            public const string ReverseCharge = "Reverse Charge";
            public const string InvoiceType = "Invoice Type";
            public const string ECommerceGSTIN = "E-Commerce GSTIN";
            public const string TaxRate = "Tax Rate";            
            public const string IRN = "IRN";
            public const string IRNDate = "IRN date";
            public const string Status = "E-invoice status";
        }
        private static class EW // 3  E-Waybill (SLWayBill)
        {
            public const string UniqueValue = "Unique Value";
            public const string requestNo = "Request Number";
            public const string clientGSTIN = "Client GSTIN";
            public const string InvoiceNo = "Doc Number";
            public const string InvoiceDate = "Doc Date";
            public const string Day = "Date";
            public const string Month = "Month";
            public const string Year = "Year";
            public const string CustomerGSTIN = "TO GSTIN";
            public const string TaxableAmount = "Assessable Value";
            public const string SGST = "SGST";
            public const string CGST = "CGST";
            public const string IGST = "IGST";
            public const string CESS = "CESS";
            public const string Total = "Total Invoice Value";

          
            public const string EWBNumber = "EWB Number";
            public const string EWBDate = "EWB Date";
            public const string SupplyType = "Supply Type";
            public const string InvoiceType = "Doc Type";            
            public const string Status = "Status";
            public const string NoOfItems = "No of Items";
            public const string MainHSNCode = "Main HSN Code";
            public const string MainHSNDesc = "Main HSN Desc";
            public const string ValidTillDate = "Valid Till Date";
            public const string GenMode = "Gen Mode";         
        }

        public async Task CompareDataAsync(DataTable Invoice, DataTable EWayBill, DataTable EInvoice)
        {
          
            var matchTypes = ConfigurationManager.AppSettings["SLMatchTypes"];
            var matchTypeList = matchTypes.Split(',').Select(x => x.Trim()).ToList();
            //foreach (var Data in matchTypeList)
            //{
            //    Console.WriteLine(Data);
            //}

            var requestNo = Invoice.Rows[0][I.requestNo].ToString();
            var clientGSTIN = Invoice.Rows[0][I.clientGSTIN].ToString();

            // First, check if records with the given ticket exist
            var existingData = _context.SLComparedDatas.Where(x => x.Request_Number == requestNo && x.Client_Gstin == clientGSTIN).ToList();
            // If they exist, remove them
            if (existingData.Any())
            {
                _context.SLComparedDatas.RemoveRange(existingData);
                await _context.SaveChangesAsync(); // Save after deletion before inserting new records
            }

            var invoice = Invoice.AsEnumerable();         
            var eInvoice = EInvoice.AsEnumerable();
            var eWayBill = EWayBill.AsEnumerable();

            int eInvoiceRowNumber, eWayBillRowNumber;

            string TrimInvoiceNo(string invoiceNo)
            {
                string trimmedInvoiceNo = new string(invoiceNo.Where(char.IsDigit).ToArray());
                //Console.WriteLine($"InvoiceNo: {invoiceNo} - {trimmedInvoiceNo} "); // Debugging line
                return trimmedInvoiceNo;
            }
            string category;
            int sno;
            // DateSource :  Invoice ,EInvoice ,EWayBill

          
            // Compare between invoice and eInvoice  1VS2
            var category7ARows = new List<DataRow>();  // 7
            var category8ARows = new List<DataRow>();          
            eInvoiceRowNumber = 1;        
            // ===== STEP 1: Invoice -> eInvoice Matching =====
            foreach (var pdRow in invoice)
            {
                string pdInvoiceNo = GetString(pdRow, I.InvoiceNo)?.Trim().ToUpper();
                string pdGSTIN = GetString(pdRow, I.CustomerGSTIN)?.Trim().ToUpper();
                string pdDate = GetString(pdRow, I.InvoiceDate)?.Trim();

                var matchedGdRows = eInvoice.Where(g =>
                    GetString(g, EI.InvoiceNo)?.Trim().ToUpper() == pdInvoiceNo &&
                    GetString(g, EI.CustomerGSTIN)?.Trim().ToUpper() == pdGSTIN
                ).ToList();
               
                if (matchedGdRows.Any())
                {
                    var matched = false;
                    foreach (var gdRow in matchedGdRows)
                    {
                       
                        category = DetermineMatchCategory("1VS2", pdRow, gdRow);
                        sno = (int)GetDecimal(pdRow, I.SNo);
                        saveMatch(sno, pdRow, "Invoice", "1VS2", category);
					
						matched = true;
                        break;
                    }

                    if (!matched)
                    {
                        category7ARows.Add(pdRow);
                    }
                }
                else
                {
                    category7ARows.Add(pdRow);
                }
            }
            // ===== STEP 2: eInvoice -> Invoice Matching =====
            foreach (var gdRow in eInvoice)
            {
                string gdInvoiceNo = GetString(gdRow, EI.InvoiceNo)?.Trim().ToUpper();
                string gdGSTIN = GetString(gdRow, EI.CustomerGSTIN)?.Trim().ToUpper();
                string gdDate = GetString(gdRow, EI.InvoiceDate)?.Trim();

                var matchingPdRows = invoice.Where(p =>
                    GetString(p, I.InvoiceNo)?.Trim().ToUpper() == gdInvoiceNo &&
                    GetString(p, I.CustomerGSTIN)?.Trim().ToUpper() == gdGSTIN
                ).ToList();
                if (matchingPdRows.Any())
                {
                    var matched = false;
                    foreach (var pdRow in matchingPdRows)
                    {
                       
                        category = DetermineMatchCategory("1VS2", pdRow, gdRow);
						saveMatch(eInvoiceRowNumber++, gdRow, "EInvoice", "1VS2", category);

                        matched = true;
                        break;
                    }

                    if (!matched)
                    {
                        category8ARows.Add(gdRow);
                    }
                }
                else
                {
                    category8ARows.Add(gdRow);
                }
            }
            // ===== STEP 3: Rechecking Category 7A and 8A Using Trimmed Invoice Numbers =====          
            foreach (var pdRow in category7ARows.ToList())
            {
                string pdInvoiceTrim = TrimInvoiceNo(GetString(pdRow, I.InvoiceNo)?.Trim().ToUpper());
                string pdGSTIN = GetString(pdRow, I.CustomerGSTIN)?.Trim().ToUpper();

                var matchingGdRows = category8ARows
                    .Where(gd => TrimInvoiceNo(GetString(gd, EI.InvoiceNo)?.Trim().ToUpper()) == pdInvoiceTrim)
                    .ToList();

                if (matchingGdRows.Any())
                {
                    bool foundExactMatch = false;

                    foreach (var gdRow in matchingGdRows.ToList())
                    {
                        string gdGSTIN = GetString(gdRow, EI.CustomerGSTIN)?.Trim().ToUpper();

                        if (gdGSTIN == pdGSTIN)
                        {
                            category = DetermineMatchCategory("1VS2", pdRow, gdRow);

                            if (new[] {
                                            matchTypeList[0], // 1_Exactly_Matched_GST_INV_DT_TAXB_TAX
                                            matchTypeList[1], // 2_Matched_With_Tolerance_GST_INV_DT_TAXB_TAX
                                            matchTypeList[3], // 4_Partially_Matched_GST_DT
                                            matchTypeList[4], // 5_Probable_Matched_GST_TAXB
                                            matchTypeList[5]  // 6_UnMatched_Excess_Of_TaxAmount
                                        }.Contains(category))
                            {
                                sno = (int)GetDecimal(pdRow, I.SNo);
                                saveMatch(sno, pdRow, "Invoice", "1VS2", category);
                                saveMatch(eInvoiceRowNumber++, gdRow, "EInvoice", "1VS2", category);
                               
                                category7ARows.Remove(pdRow);
                                category8ARows.Remove(gdRow);

                                foundExactMatch = true;
                                break;
                            }
                        }
                    }

                    // Fallback to Category 2 if GSTIN matched but no exact match
                    if (!foundExactMatch)
                    {
                        var gdRow = matchingGdRows.FirstOrDefault(gd => GetString(gd, EI.CustomerGSTIN)?.Trim().ToUpper() == pdGSTIN);
                        if (gdRow != null)
                        {
                            sno = (int)GetDecimal(pdRow, I.SNo);
                            saveMatch(sno, pdRow, "Invoice", "1VS2", matchTypeList[2]);
                            saveMatch(eInvoiceRowNumber++, gdRow, "EInvoice", "1VS2", matchTypeList[2]);

                            category7ARows.Remove(pdRow);
                            category8ARows.Remove(gdRow);
                        }
                    }
                }
            }
            // ===== Final Unmatched Category 7 & 8 =====       
            foreach (var pdRow in category7ARows)
            {
                sno = (int)GetDecimal(pdRow, I.SNo);
                saveMatch(sno, pdRow, "Invoice", "1VS2", matchTypeList[6]);  // Category 7:  Available_In_Sales_Register_MISSING_In_E_Invoice
            }
            foreach (var gdRow in category8ARows)
            {
                saveMatch(eInvoiceRowNumber++, gdRow, "EInvoice", "1VS2", matchTypeList[8]); // Category 9: Available_In_E_Invoice_MISSING_In_Sales_Register
            }

           
          
            // Compare between eInvoice and eWayBill 2VS3
            var category8BRows = new List<DataRow>();
            var category9ARows = new List<DataRow>();
            eInvoiceRowNumber = 1;
            eWayBillRowNumber = 1;
            // ===== STEP 1: eInvoice -> eWayBill Matching =====
            foreach (var pdRow in eInvoice)
            {
                string pdInvoiceNo = GetString(pdRow, EI.InvoiceNo)?.Trim().ToUpper();
                string pdGSTIN = GetString(pdRow, EI.CustomerGSTIN)?.Trim().ToUpper();
              
                var matchedGdRows = eWayBill.Where(g =>
                    GetString(g, EW.InvoiceNo)?.Trim().ToUpper() == pdInvoiceNo &&
                    GetString(g, EW.CustomerGSTIN)?.Trim().ToUpper() == pdGSTIN
                ).ToList();
                if (matchedGdRows.Any())
                {
                    var matched = false;
                    foreach (var gdRow in matchedGdRows)
                    {
                        category = DetermineMatchCategory("2VS3",pdRow, gdRow);
                        saveMatch(eInvoiceRowNumber++, pdRow, "EInvoice", "2VS3", category);
                       
                        matched = true;
                        break;
                    }

                    if (!matched)
                    {
                        category8BRows.Add(pdRow);
                    }
                }
                else
                {
                    category8BRows.Add(pdRow);
                }
            }
            // ===== STEP 2: eWayBill -> eInvoice Matching =====
            foreach (var gdRow in eWayBill)
            {
                string gdInvoiceNo = GetString(gdRow, EW.InvoiceNo)?.Trim().ToUpper();
                string gdGSTIN = GetString(gdRow, EW.CustomerGSTIN)?.Trim().ToUpper();
                string gdDate = GetString(gdRow, EW.InvoiceDate)?.Trim();

                var matchingPdRows = eInvoice.Where(p =>
                    GetString(p, EI.InvoiceNo)?.Trim().ToUpper() == gdInvoiceNo &&
                    GetString(p, EI.CustomerGSTIN)?.Trim().ToUpper() == gdGSTIN
                ).ToList();

                if (matchingPdRows.Any())
                {

                    var matched = false;
                    foreach (var pdRow in matchingPdRows)
                    {
                        category = DetermineMatchCategory("2VS3", pdRow, gdRow);
                        saveMatch(eWayBillRowNumber++, gdRow, "EWayBill", "2VS3", category);
                        matched = true;
                        break;
                    }

                    if (!matched)
                    {
                        category9ARows.Add(gdRow);
                    }
                }
                else
                {
                    category9ARows.Add(gdRow);
                }
            }
            // ===== STEP 3: Rechecking Category 8 and 9 Using Trimmed Invoice Numbers =====
            foreach (var pdRow in category8BRows.ToList())
            {
                string pdInvoiceTrim = TrimInvoiceNo(GetString(pdRow, EI.InvoiceNo)?.Trim().ToUpper());
                string pdGSTIN = GetString(pdRow, EI.CustomerGSTIN)?.Trim().ToUpper();

                var matchingGdRows = category9ARows
                    .Where(gd => TrimInvoiceNo(GetString(gd, EW.InvoiceNo)?.Trim().ToUpper()) == pdInvoiceTrim)
                    .ToList();

                if (matchingGdRows.Any())
                {
                    bool foundExactMatch = false;

                    foreach (var gdRow in matchingGdRows.ToList())
                    {
                        string gdGSTIN = GetString(gdRow, EW.CustomerGSTIN)?.Trim().ToUpper();

                        if (gdGSTIN == pdGSTIN)
                        {
                            category = DetermineMatchCategory("2VS3", pdRow, gdRow);
                            if (new[] {
                                            matchTypeList[0], // 1_Exactly_Matched_GST_INV_DT_TAXB_TAX
                                            matchTypeList[1], // 2_Matched_With_Tolerance_GST_INV_DT_TAXB_TAX
                                            matchTypeList[3], // 4_Partially_Matched_GST_DT
                                            matchTypeList[4], // 5_Probable_Matched_GST_TAXB
                                            matchTypeList[5]  // 6_UnMatched_Excess_Of_TaxAmount
                                        }.Contains(category))
                            {

                                saveMatch(eInvoiceRowNumber++, pdRow, "EInvoice", "2VS3", category);
                                saveMatch(eWayBillRowNumber++, gdRow, "EWayBill", "2VS3", category);
                                category8BRows.Remove(pdRow);
                                category9ARows.Remove(gdRow);

                                foundExactMatch = true;
                                break;
                            }
                        }
                    }

                    // Fallback to Category 2 if GSTIN matched but no exact match
                    if (!foundExactMatch)
                    {
                        var gdRow = matchingGdRows.FirstOrDefault(gd => GetString(gd, EW.CustomerGSTIN)?.Trim().ToUpper() == pdGSTIN);
                        if (gdRow != null)
                        {
                            saveMatch(eInvoiceRowNumber++, pdRow, "EInvoice", "2VS3", matchTypeList[2]);
                            saveMatch(eWayBillRowNumber++, gdRow, "EWayBill", "2VS3", matchTypeList[2]);
                            category8BRows.Remove(pdRow);
                            category9ARows.Remove(gdRow);
                        }
                    }
                }
            }
            // ===== Final Unmatched Category 8 & 9 =====         
            foreach (var pdRow in category8BRows)
            {
                saveMatch(eInvoiceRowNumber++, pdRow, "EInvoice", "2VS3", matchTypeList[9]); // Category 10 : Available_In_E_Invoice_MISSING_In_E_WayBIll
            }        
            foreach (var gdRow in category9ARows)
            {
                saveMatch(eWayBillRowNumber++, gdRow, "EWayBill", "2VS3", matchTypeList[10]); // Category 11 : Available_In_E_WayBIll_MISSING_In_E_Invoice
            }
            //Console.WriteLine("1");
            // Compare between invoice and eWayBill  1VS3
            var category7BRows = new List<DataRow>();
            var category9BRows = new List<DataRow>();
            eWayBillRowNumber = 1;
            // ===== STEP 1: invoice -> eWayBill Matching =====
            foreach (var pdRow in invoice)
            {
                string pdInvoiceNo = GetString(pdRow, I.InvoiceNo)?.Trim().ToUpper();
                string pdGSTIN = GetString(pdRow, I.CustomerGSTIN)?.Trim().ToUpper();
                string pdDate = GetString(pdRow, I.InvoiceDate)?.Trim();

                var matchedGdRows = eWayBill.Where(g =>
                    GetString(g, EW.InvoiceNo)?.Trim().ToUpper() == pdInvoiceNo &&
                    GetString(g, EW.CustomerGSTIN)?.Trim().ToUpper() == pdGSTIN
                ).ToList();
               
                if (matchedGdRows.Any())
                {
                    var matched = false;
                    foreach (var gdRow in matchedGdRows)
                    {
                        category = DetermineMatchCategory("1VS3", pdRow, gdRow);
                        sno = (int)GetDecimal(pdRow, I.SNo);
                        saveMatch(sno, pdRow, "Invoice", "1VS3", category);
                        matched = true;
                        break;
                    }

                    if (!matched)
                    {
                        category7BRows.Add(pdRow);
                    }
                }
                else
                {
                    category7BRows.Add(pdRow);
                }
            }
            // ===== STEP 2: eWayBill -> invoice Matching =====
            foreach (var gdRow in eWayBill)
            {
                string gdInvoiceNo = GetString(gdRow, EW.InvoiceNo)?.Trim().ToUpper();
                string gdGSTIN = GetString(gdRow, EW.CustomerGSTIN)?.Trim().ToUpper();
                string gdDate = GetString(gdRow, EW.InvoiceDate)?.Trim();

                var matchingPdRows = invoice.Where(p =>
                    GetString(p, I.InvoiceNo)?.Trim().ToUpper() == gdInvoiceNo &&
                    GetString(p, I.CustomerGSTIN)?.Trim().ToUpper() == gdGSTIN
                ).ToList();

                if (matchingPdRows.Any())
                {
                    var matched = false;
                    foreach (var pdRow in matchingPdRows)
                    {
                        category = DetermineMatchCategory("1VS3", pdRow, gdRow);
                        saveMatch(eWayBillRowNumber++, gdRow, "EWayBill", "1VS3", category);
                        matched = true;
                        break;
                    }
                    if (!matched)
                    {
                        category9BRows.Add(gdRow);
                    }
                }
                else
                {
                    category9BRows.Add(gdRow);
                }
            }
            // ===== STEP 3: Rechecking Category 7 and 9 Using Trimmed Invoice Numbers =====
            foreach (var pdRow in category7BRows.ToList())
            {
                string pdInvoiceTrim = TrimInvoiceNo(GetString(pdRow, I.InvoiceNo)?.Trim().ToUpper());
                string pdGSTIN = GetString(pdRow, I.CustomerGSTIN)?.Trim().ToUpper();

                var matchingGdRows = category9BRows
                    .Where(gd => TrimInvoiceNo(GetString(gd, EW.InvoiceNo)?.Trim().ToUpper()) == pdInvoiceTrim)
                    .ToList();

                if (matchingGdRows.Any())
                {
                    bool foundExactMatch = false;

                    foreach (var gdRow in matchingGdRows.ToList())
                    {
                        string gdGSTIN = GetString(gdRow, EW.CustomerGSTIN)?.Trim().ToUpper();

                        if (gdGSTIN == pdGSTIN)
                        {
                            category = DetermineMatchCategory("1VS3", pdRow, gdRow);
                            if (new[] {
                                            matchTypeList[0], // 1_Exactly_Matched_GST_INV_DT_TAXB_TAX
                                            matchTypeList[1], // 2_Matched_With_Tolerance_GST_INV_DT_TAXB_TAX
                                            matchTypeList[3], // 4_Partially_Matched_GST_DT
                                            matchTypeList[4], // 5_Probable_Matched_GST_TAXB
                                            matchTypeList[5]  // 6_UnMatched_Excess_Of_TaxAmount
                                        }.Contains(category))
                            {
                                sno = (int)GetDecimal(pdRow, I.SNo);
                                saveMatch(sno, pdRow, "Invoice", "1VS3", category);
                                saveMatch(eWayBillRowNumber++, gdRow, "EWayBill", "1VS3", category);

                                category7BRows.Remove(pdRow);
                                category9BRows.Remove(gdRow);

                                foundExactMatch = true;
                                break;
                            }
                        }
                    }

                    // Fallback to Category 2 if GSTIN matched but no exact match
                    if (!foundExactMatch)
                    {
                        var gdRow = matchingGdRows.FirstOrDefault(gd => GetString(gd, EW.CustomerGSTIN)?.Trim().ToUpper() == pdGSTIN);
                        if (gdRow != null)
                        {
                            sno = (int)GetDecimal(pdRow, I.SNo);
                            saveMatch(sno, pdRow, "Invoice", "1VS3", matchTypeList[2]);
                            saveMatch(eWayBillRowNumber++, gdRow, "EWayBill", "1VS3", matchTypeList[2]);// Category 3 : 3_Partially_Matched_GST_INV

                            category7BRows.Remove(pdRow);
                            category9BRows.Remove(gdRow);
                        }
                    }
                }
            }
            // ===== Final Unmatched Category 7 & 9 =====
            foreach (var pdRow in category7BRows)
            {
                sno = (int)GetDecimal(pdRow, I.SNo);
                saveMatch(sno, pdRow, "Invoice", "1VS3", matchTypeList[7]); // Category 8 : Available_In_Sales_Register_MISSING_In_E_WayBIll
            }
            foreach (var gdRow in category9BRows)
            {
                saveMatch(eWayBillRowNumber++, gdRow, "EWayBill", "1VS3", matchTypeList[11]); // Category 12: Available_In_E_WayBIll_MISSING_In_Sales_Register
            }


            #region Compare invoice VS eInvoice and eWayBill
            //// Compare and save Invoice records
            //var UnmatchedRows = new List<DataRow>();// ===== STEP 1: Invoice -> EWayBill Matching =====
            //foreach (var Irow in invoice)
            //{
            //    string I_InvoiceNo = GetString(Irow, I.InvoiceNo)?.Trim().ToUpper();
            //    string I_GSTIN = GetString(Irow, I.CustomerGSTIN)?.Trim().ToUpper();
            //    string I_Day = GetString(Irow, I.Day)?.Trim();
            //    string I_Month = GetString(Irow , I.Month)?.Trim();
            //    string I_year = GetString(Irow,I.Year)?.Trim();

            //    string formatedIDate = $"{I_Day}-{I_Month}-{I_year}";
            //    decimal I_total = GetDecimal(Irow,I.Total);  

            //    var matchedEWayBill = eWayBill.Where(e =>
            //                                          GetString(e, EW.InvoiceNo)?.Trim().ToUpper() == I_InvoiceNo &&
            //                                          GetString(e, EW.CustomerGSTIN)?.Trim().ToUpper() == I_GSTIN
            //                                        );
            //    if (matchedEWayBill.Any())
            //    {
            //        var match = false;
            //        foreach (var EWrow in matchedEWayBill)
            //        {
            //            string EW_Day = GetString(EWrow, EW.Day)?.Trim();
            //            string EW_Month = GetString(EWrow,EW.Month)?.Trim();
            //            string EW_Year = GetString(EWrow , EW.Year)?.Trim();

            //            string formatedEWDate = $"{EW_Day}-{EW_Month}-{EW_Year}";
            //            decimal EW_total = GetDecimal(EWrow, EW.Total);

            //            if (formatedIDate == formatedEWDate  &&  I_total == EW_total) 
            //            {
            //                int sno = (int)GetDecimal(Irow, I.SNo);
            //                saveMatch(sno, Irow, "Invoice", matchTypeList[0]);                        
            //                match = true;
            //                break;
            //            }
            //        }
            //        if(!match)
            //        {
            //            UnmatchedRows.Add(Irow);
            //        }
            //    }
            //    else
            //    {
            //        UnmatchedRows.Add(Irow);
            //    }
            //}
            //var stillUnmatched = new List<DataRow>();// ===== STEP 2: Remaining Unmatched Invoice -> EInvoice Matching =====
            //foreach (var Irow in UnmatchedRows)
            //{
            //    string I_InvoiceNo = GetString(Irow, I.InvoiceNo)?.Trim().ToUpper();
            //    string I_GSTIN = GetString(Irow, I.CustomerGSTIN)?.Trim().ToUpper();
            //    string I_Day = GetString(Irow, I.Day)?.Trim();
            //    string I_Month = GetString(Irow, I.Month)?.Trim();
            //    string I_Year = GetString(Irow, I.Year)?.Trim();
            //    string formatedIDate = $"{I_Day}-{I_Month}-{I_Year}";
            //    decimal I_total = GetDecimal(Irow, I.Total);

            //    var matchedEInvoice = eInvoice.Where(e =>
            //        GetString(e, EI.InvoiceNo)?.Trim().ToUpper() == I_InvoiceNo &&
            //        GetString(e, EI.CustomerGSTIN)?.Trim().ToUpper() == I_GSTIN
            //    );

            //    if (matchedEInvoice.Any())
            //    {
            //        var match = false;
            //        foreach (var EIrow in matchedEInvoice)
            //        {
            //            string EI_Day = GetString(EIrow, EI.Day)?.Trim();
            //            string EI_Month = GetString(EIrow, EI.Month)?.Trim();
            //            string EI_Year = GetString(EIrow, EI.Year)?.Trim();
            //            string formatedEIDate = $"{EI_Day}-{EI_Month}-{EI_Year}";
            //            decimal EI_total = GetDecimal(EIrow, EI.Total);

            //            if (formatedIDate == formatedEIDate && I_total == EI_total)
            //            {
            //                int sno = (int)GetDecimal(Irow, I.SNo);
            //                saveMatch(sno, Irow, "Invoice", matchTypeList[1]); // Assuming second matchType is for EInvoice
            //                match = true;
            //                break;
            //            }
            //        }
            //        if (!match)
            //        {
            //            stillUnmatched.Add(Irow); // Not matched on details
            //        }
            //    }
            //    else
            //    {
            //        stillUnmatched.Add(Irow); // Not matched at all
            //    }
            //}
            //foreach (var Irow in stillUnmatched)
            //{
            //    int sno = (int)GetDecimal(Irow, I.SNo);
            //    saveMatch(sno, Irow, "Invoice", matchTypeList[2]);
            //}
            //// Compare and save EWayBill records
            //var EWUnMatchRows = new List<DataRow>();
            //int EWSno = 1;
            //foreach(var EWrow in eWayBill)
            //{
            //    string EW_InvoiceNo = GetString(EWrow, EW.InvoiceNo)?.Trim().ToUpper();
            //    string EW_GSTIN = GetString(EWrow, EW.CustomerGSTIN)?.Trim().ToUpper();
            //    string EW_Day = GetString(EWrow, EW.Day)?.Trim();
            //    string EW_Month = GetString(EWrow, EW.Month)?.Trim();
            //    string EW_year = GetString(EWrow, EW.Year)?.Trim();

            //    string formatedEWDate = $"{EW_Day}-{EW_Month}-{EW_year}";
            //    decimal EW_total = GetDecimal(EWrow, EW.Total);

            //    var matchedInvoice = invoice.Where(e =>
            //                                          GetString(e, I.InvoiceNo)?.Trim().ToUpper() == EW_InvoiceNo &&
            //                                          GetString(e, I.CustomerGSTIN)?.Trim().ToUpper() == EW_GSTIN
            //                                        );
            //    if (matchedInvoice.Any())
            //    {
            //        var match = false;
            //        foreach (var Irow in matchedInvoice)
            //        {
            //            string I_Day = GetString(Irow, I.Day)?.Trim();
            //            string I_Month = GetString(Irow, I.Month)?.Trim();
            //            string I_year = GetString(Irow, I.Year)?.Trim();

            //            string formatedIDate = $"{I_Day}-{I_Month}-{I_year}";
            //            decimal I_total = GetDecimal(Irow, I.Total);

            //            if (formatedIDate == formatedEWDate && I_total == EW_total)
            //            {
            //                saveMatch(EWSno++, EWrow, "EWayBill", matchTypeList[0]);
            //                match = true;
            //                break;
            //            }
            //        }
            //        if (!match)
            //        {
            //            EWUnMatchRows.Add(EWrow);
            //        }
            //    }
            //    else
            //    {
            //        EWUnMatchRows.Add(EWrow);
            //    }            
            //}
            //foreach(var EWrow in EWUnMatchRows)
            //{
            //    saveMatch(EWSno++, EWrow, "EWayBill", matchTypeList[2]);
            //}
            //// Compare and save EInvoice records
            //var EIUnMatchRows = new List<DataRow>();
            //int EISno = 1;
            //foreach(var EIrow in eInvoice)
            //{
            //    string EI_InvoiceNo = GetString(EIrow, EI.InvoiceNo)?.Trim().ToUpper();
            //    string EI_GSTIN = GetString(EIrow, EI.CustomerGSTIN)?.Trim().ToUpper();

            //    string EI_Day = GetString(EIrow, EI.Day)?.Trim();
            //    string EI_Month = GetString(EIrow, EI.Month)?.Trim();
            //    string EI_Year = GetString(EIrow, EI.Year)?.Trim();
            //    string formatedEIDate = $"{EI_Day}-{EI_Month}-{EI_Year}";
            //    decimal EI_total = GetDecimal(EIrow, EI.Total);

            //    var matchedInvoice = invoice.Where(e =>
            //        GetString(e, I.InvoiceNo)?.Trim().ToUpper() == EI_InvoiceNo &&
            //        GetString(e, I.CustomerGSTIN)?.Trim().ToUpper() == EI_GSTIN
            //    );

            //    if (matchedInvoice.Any())
            //    {
            //        var match = false;
            //        foreach (var Irow in matchedInvoice)
            //        {
            //            string I_Day = GetString(Irow, I.Day)?.Trim();
            //            string I_Month = GetString(Irow, I.Month)?.Trim();
            //            string I_Year = GetString(Irow, I.Year)?.Trim();
            //            string formatedIDate = $"{I_Day}-{I_Month}-{I_Year}";
            //            decimal I_total = GetDecimal(Irow, I.Total);

            //            if (formatedIDate == formatedEIDate && I_total == EI_total)
            //            {
            //                saveMatch(EISno++, EIrow, "EInvoice", matchTypeList[1]); // Assuming second matchType is for EInvoice
            //                match = true;
            //                break;
            //            }
            //        }
            //        if (!match)
            //        {
            //            EIUnMatchRows.Add(EIrow); // Not matched on details
            //        }
            //    }
            //    else
            //    {
            //        EIUnMatchRows.Add(EIrow); // Not matched at all
            //    }            
            //}
            //foreach(var EIrow in  EIUnMatchRows)
            //{
            //    saveMatch(EISno++, EIrow, "EInvoice", matchTypeList[2]);
            //}
            #endregion

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbEntityValidationException ex)
            {
                foreach (var eve in ex.EntityValidationErrors)
                {
                    Console.WriteLine($"Entity of type \"{eve.Entry.Entity.GetType().Name}\" has the following validation errors:");
                    foreach (var ve in eve.ValidationErrors)
                    {
                        var currentValue = eve.Entry.CurrentValues[ve.PropertyName];
                        Console.WriteLine($"- Property: \"{ve.PropertyName}\", Error: \"{ve.ErrorMessage}\"");
                        Console.WriteLine($"- Property: \"{ve.PropertyName}\", value: \"{currentValue}\"");
                        Console.WriteLine($"- Property: \"{ve.PropertyName}\", value type: \"{currentValue.GetType()}\"");
                    }
                }
                throw new Exception("Failed to insert data: " + ex.Message, ex);
            }


        }
        private string GetString(DataRow row, string columnName)
        {
            try
            {
                if (row == null)
                    throw new ArgumentNullException(nameof(row), "DataRow is null.");

                if (!row.Table.Columns.Contains(columnName))
                    throw new ArgumentException($"Column '{columnName}' does not exist in the DataRow.");

                var value = row[columnName];

                if (value == null || value == DBNull.Value)
                    throw new InvalidOperationException($"Value at column '{columnName}' is null or DBNull.");

                return value.ToString().Trim();
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }
        private decimal GetDecimal(DataRow row, string columnName)
        {
            try
            {
                if (row == null)
                    throw new ArgumentNullException(nameof(row), "DataRow is null.");

                if (!row.Table.Columns.Contains(columnName))
                    throw new ArgumentException($"Column '{columnName}' does not exist in the DataRow.");

                var value = row[columnName];

                if (value == null || value == DBNull.Value)
                    throw new InvalidOperationException($"Value at column '{columnName}' is null or DBNull.");

                if (decimal.TryParse(value.ToString(), out var parsedValue))
                {
                    return parsedValue;
                }
                else
                {
                    throw new FormatException($"Value '{value}' in column '{columnName}' is not a valid decimal.");
                }
            }
            catch (Exception ex)
            {
                // Optionally log the error or return a special value if needed
                Console.WriteLine($"Error: {ex.Message}");
                return 0;
            }
        }
        private string GetCategory(string Match_Type)
        {

            switch (Match_Type)
            {
                case "6_UnMatched_Excess_or_Short_1_Invoicewise":
                case "Available_In_Sales_Register_MISSING_In_E_WayBIll":
                case "Available_In_Sales_Register_MISSING_In_E_Invoice":
                case "Available_In_E_Invoice_MISSING_In_Sales_Register":
                case "Available_In_E_Invoice_MISSING_In_E_WayBIll":
                case "Available_In_E_WayBIll_MISSING_In_Sales_Register":
                case "Available_In_E_WayBIll_MISSING_In_E_Invoice":
                    return "UnMatched";

                case "5_Probable_Matched_GST_TAXB":
                case "4_Partially_Matched_GST_DT":
                case "3_Partially_Matched_GST_INV":

                    return "Partially_Matched";

                case "1_Exactly_Matched_GST_INV_DT_TAXB_TAX":
                case "2_Matched_With_Tolerance_GST_INV_DT_TAXB_TAX":
                    return "Completely_Matched";

                default:
                    return "Unknown";
            }
        }
        string DetermineMatchCategory(string compare, DataRow pdRow, DataRow gdRow)  // 0,1,3,4,5
        {
            var matchTypes = ConfigurationManager.AppSettings["SLMatchTypes"];
            var matchTypeList = matchTypes.Split(',').Select(x => x.Trim()).ToList();


            string pdDay, gdDay, pdMonth, gdMonth, pdYear, gdYear;
            decimal totalPDTax, totalGDTax;

            if (compare == "1VS2")
            {
                pdDay = GetString(pdRow, I.Day); gdDay = GetString(gdRow, EI.Day);
                pdMonth = GetString(pdRow, I.Month); gdMonth = GetString(gdRow, EI.Month);
                pdYear = GetString(pdRow, I.Year); gdYear = GetString(gdRow, EI.Year);
                totalPDTax = GetDecimal(pdRow, I.Total); totalGDTax = GetDecimal(gdRow, EI.Total);
            }
            else if (compare == "2VS3")
            {
                pdDay = GetString(pdRow, EI.Day); gdDay = GetString(gdRow, EW.Day);
                pdMonth = GetString(pdRow, EI.Month); gdMonth = GetString(gdRow, EW.Month);
                pdYear = GetString(pdRow, EI.Year); gdYear = GetString(gdRow, EW.Year);
                totalPDTax = GetDecimal(pdRow, EI.Total); totalGDTax = GetDecimal(gdRow, EW.Total);

            }
            else // compare == "1VS3"
            {
                pdDay = GetString(pdRow, I.Day); gdDay = GetString(gdRow, EW.Day);
                pdMonth = GetString(pdRow, I.Month); gdMonth = GetString(gdRow, EW.Month);
                pdYear = GetString(pdRow, I.Year); gdYear = GetString(gdRow, EW.Year);
                totalPDTax = GetDecimal(pdRow, I.Total); totalGDTax = GetDecimal(gdRow, EW.Total);
            }




            string formattedPDDate = $"{pdDay}-{pdMonth}-{pdYear}";
            string formattedGDDate = $"{gdDay}-{gdMonth}-{gdYear}";

            var isInvoiceDateMatch = formattedPDDate == formattedGDDate;


            if (!isInvoiceDateMatch)
                return matchTypeList[3]; // Invoice date mismatch   4_Partially_Matched_GST_DT,




            var taxDifference = Math.Abs(totalPDTax - totalGDTax);

            if (taxDifference == 0)
                return matchTypeList[0]; // Exact match  //1_Exactly_Matched_GST_INV_DT_TAXB_TAX,

            if (taxDifference < 10)
                return matchTypeList[1]; // Minor tax diff (< 10) // 2_Matched_With_Tolerance_GST_INV_DT_TAXB_TAX,

            if (totalPDTax > totalGDTax)  // pr is greater than portal 
                return matchTypeList[4];  // 5_Probable_Matched_GST_TAXB,

            //if (totalPDTax < totalGDTax) // portal is greater than pr               
            return matchTypeList[5];       //6_UnMatched_Excess_Of_TaxAmount,



        }
        public void saveMatch(int rowNumber, DataRow DR, string DS, string Comparer, string Match_Type)
        {
            //Console.WriteLine($"Compare :{Comparer} , DS-{DS} ,invoice number -{rowNumber}");   
         
            var CompareList = ConfigurationManager.AppSettings["Compare"].Split(',').Select(x => x.Trim()).ToList();

            string Compare;
            if (Comparer == "1VS2")
            {
                Compare = CompareList[0];
            }
            else if(Comparer =="2VS3")
            {
                Compare = CompareList[1];
            }
            else // Comparer == "1VS3"
            {
                Compare = CompareList[2];
            }
            string Catagory = GetCategory(Match_Type);
            // Declare first
            decimal Taxable_Value, CGST, SGST, IGST, CESS, Total_Tax ;
            string invoiceDateInput, period, invoiceno, supplierGstin, supplierName, uniquevalue ,POS ,GstRate , TOI , clientGstin , RequestNo;


            if (DS == "Invoice")
            {
                uniquevalue = GetString(DR, I.UniqueValue);
                clientGstin = GetString(DR, I.clientGSTIN);
                RequestNo = GetString(DR,I.requestNo);

                invoiceno = GetString(DR, I.InvoiceNo);
                invoiceDateInput = GetString(DR, I.InvoiceDate);
                supplierName = GetString(DR, I.CustomerName);
                supplierGstin = GetString(DR, I.CustomerGSTIN);
                POS = GetString(DR, I.POS);
                
                Taxable_Value = DR[I.TaxableAmount] == DBNull.Value ? 0 : Math.Round(Convert.ToDecimal(DR[I.TaxableAmount]), 2);
                CGST = DR[I.CGST] == DBNull.Value ? 0 : Math.Round(Convert.ToDecimal(DR[I.CGST]), 2);
                SGST = DR[I.SGST] == DBNull.Value ? 0 : Math.Round(Convert.ToDecimal(DR[I.SGST]), 2);
                IGST = DR[I.IGST] == DBNull.Value ? 0 : Math.Round(Convert.ToDecimal(DR[I.IGST]), 2);
                CESS = DR[I.CESS] == DBNull.Value ? 0 : Math.Round(Convert.ToDecimal(DR[I.CESS]), 2);
                Total_Tax = DR[I.Total] == DBNull.Value ? 0 : Math.Round(Convert.ToDecimal(DR[I.Total]), 2);
               
                TOI = GetString(DR,I.InvoiceType);
            }
            else if(DS == "EWayBill")
            {
                uniquevalue = GetString(DR, EW.UniqueValue);
                clientGstin = GetString(DR, EW.clientGSTIN);
                RequestNo = GetString(DR, EW.requestNo);

                invoiceno = GetString(DR, EW.InvoiceNo);
                invoiceDateInput = GetString(DR, EW.InvoiceDate);
                supplierName = "";// GetString(DR, EW.CustomerName);
                supplierGstin = GetString(DR, EW.CustomerGSTIN);
                POS = "";// GetString(DR, EW.POS);

                Taxable_Value = DR[EW.TaxableAmount] == DBNull.Value ? 0 : Math.Round(Convert.ToDecimal(DR[EW.TaxableAmount]), 2);
                CGST = DR[EW.CGST] == DBNull.Value ? 0 : Math.Round(Convert.ToDecimal(DR[EW.CGST]), 2);
                SGST = DR[EW.SGST] == DBNull.Value ? 0 : Math.Round(Convert.ToDecimal(DR[EW.SGST]), 2);
                IGST = DR[EW.IGST] == DBNull.Value ? 0 : Math.Round(Convert.ToDecimal(DR[EW.IGST]), 2);
                CESS = DR[EW.CESS] == DBNull.Value ? 0 : Math.Round(Convert.ToDecimal(DR[EW.CESS]), 2);
                Total_Tax = DR[EW.Total] == DBNull.Value ? 0 : Math.Round(Convert.ToDecimal(DR[EW.Total]), 2);

                TOI = GetString(DR, EW.InvoiceType);
            }
            else
            {
                uniquevalue = GetString(DR, EI.UniqueValue);
                clientGstin = GetString(DR, EI.clientGSTIN);
                RequestNo = GetString(DR, EI.requestNo);

                invoiceno = GetString(DR, EI.InvoiceNo);
                invoiceDateInput = GetString(DR, EI.InvoiceDate);
                supplierName = GetString(DR, EI.CustomerName);
                supplierGstin = GetString(DR, EI.CustomerGSTIN);
                POS = GetString(DR, EI.POS);

                Taxable_Value = DR[EI.TaxableAmount] == DBNull.Value ? 0 : Math.Round(Convert.ToDecimal(DR[EI.TaxableAmount]), 2);
                CGST = DR[EI.CGST] == DBNull.Value ? 0 : Math.Round(Convert.ToDecimal(DR[EI.CGST]), 2);
                SGST = DR[EI.SGST] == DBNull.Value ? 0 : Math.Round(Convert.ToDecimal(DR[EI.SGST]), 2);
                IGST = DR[EI.IGST] == DBNull.Value ? 0 : Math.Round(Convert.ToDecimal(DR[EI.IGST]), 2);
                CESS = DR[EI.CESS] == DBNull.Value ? 0 : Math.Round(Convert.ToDecimal(DR[EI.CESS]), 2);
                Total_Tax = DR[EI.Total] == DBNull.Value ? 0 : Math.Round(Convert.ToDecimal(DR[EI.Total]), 2);

                TOI = GetString(DR, EI.InvoiceType);
			}
			period = _context.Sales_Ledger_Tickets
	                    .Where(p => p.Request_Number == RequestNo && p.Client_Gstin == clientGstin)
	                    .Select(p => p.Period)
	                    .FirstOrDefaultAsync()
	                    .Result;



			//Console.WriteLine($"Compare :{Comparer} , DS-{DS} ,invoice number -{invoiceno} ,catagory - {Match_Type}");

			DateTime? parsedDate = DateTime.TryParse(invoiceDateInput, out DateTime dt) ? dt : (DateTime?)null;
            //string invoiceDate = parsedDate?.ToString("dd-MM-yyyy");
            //Console.WriteLine($"Row number : {rowNumber},Data Source : {DS},Invoice Date: {invoiceDate}, Period: {period}, FY: {FY} ,match type : {Match_Type}");
            var data = new SLComparedData
                {
                    Unique_Value = uniquevalue,
                    Client_Gstin = clientGstin,
                    Request_Number = RequestNo,

                    S_No = rowNumber,

                    Compare = Compare,
                    DataSource = DS,
                    Category = Catagory,
                    Match_Type = Match_Type,

                    Invoice_Number = invoiceno,
                    Invoice_Date = parsedDate ,
                    Customer_Name = supplierName,
                    Customer_Gstin = supplierGstin,
                   
                    Taxable_Amount = Taxable_Value,
                    CGST = CGST,
                    SGST = SGST,
                    IGST = IGST,
                    CESS = CESS,
                    Total_Amount = Total_Tax,

                    Place_of_Supply = POS,
                    Type_Of_Invoice = TOI,

                    Period = period,
                };

            _context.SLComparedDatas.Add(data);
        }

        public async Task<List<SLComparedDataModel>> GetComparedDataBasedOnTicketAsync(string requestno, string ClientGSTIN)
        {
            var data = await _context.SLComparedDatas
                .Where(x => x.Request_Number == requestno && x.Client_Gstin == ClientGSTIN)
                .ToListAsync();

            var result = new List<SLComparedDataModel>();

            foreach (var item in data)
            {
                // Map to result model
                var mappedItem = new SLComparedDataModel
                {
                    UniqueValue = item.Unique_Value,
                    RequestNumber = item.Request_Number,
                    ClientGstin = item.Client_Gstin,

                    SNo = item.S_No,
                    InvoiceNumber = item.Invoice_Number,
                    InvoiceDate = item.Invoice_Date,

                    CustomerName = item.Customer_Name,
                    CustomerGstin = item.Customer_Gstin,

                    PlaceofSupply = item.Place_of_Supply,
                    TypeOfInvoice = item.Type_Of_Invoice,

                    DataSource = item.DataSource,
                    Category = item.Category,
                    MatchType = item.Match_Type,
                    Compare = item.Compare,
                   
                    TaxableAmount = item.Taxable_Amount,
                    CGST = (decimal)item.CGST,
                    SGST = (decimal)item.SGST,
                    IGST = (decimal)item.IGST,
                    CESS = (decimal)item.CESS,
                    TotalAmount = (decimal)item.Total_Amount,


                };

                result.Add(mappedItem);
            }

            return result;
        }
        public async Task<List<SLComparedDataModel>> GetComparedData_API(string requestno,string ClientGSTIN,List<string> Category,List<string> otherdays,string formate)
        {
            // Step 1: Pull relevant records into memory first
            var allData = await _context.SLComparedDatas
                .Where(x => x.Request_Number == requestno && x.Client_Gstin == ClientGSTIN)
                .ToListAsync();

            // Step 2: Filter and remove unwanted records
            if (otherdays != null && otherdays.Any() && Category != null && Category.Any())
            {
                var toRemove = allData
                    .Where(x =>
                        Category.Contains(x.Match_Type) &&
                        x.Invoice_Date.HasValue &&
                        otherdays.Contains(x.Invoice_Date.Value.ToString(formate)))
                    .ToList();

                if (toRemove.Any())
                {
                    Console.WriteLine($"Removing {toRemove.Count} records based on Category and Date filter.");
                    _context.SLComparedDatas.RemoveRange(toRemove);
                    await _context.SaveChangesAsync(); // Important: commit the deletion
                    allData = allData.Except(toRemove).ToList(); // Refresh local list
                }
            }

            // Step 3: Map to result model
            var result = allData.Select(item => new SLComparedDataModel
            {
                UniqueValue = item.Unique_Value,
                RequestNumber = item.Request_Number,
                ClientGstin = item.Client_Gstin,

                SNo = item.S_No,
                InvoiceNumber = item.Invoice_Number,
                InvoiceDate = item.Invoice_Date,

                CustomerName = item.Customer_Name,
                CustomerGstin = item.Customer_Gstin,

                PlaceofSupply = item.Place_of_Supply,
                TypeOfInvoice = item.Type_Of_Invoice,

                DataSource = item.DataSource,
                Category = item.Category,
                MatchType = item.Match_Type,
                Compare = item.Compare,

                TaxableAmount = item.Taxable_Amount,
                CGST = item.CGST ?? 0,
                SGST = item.SGST ?? 0,
                IGST = item.IGST ?? 0,
                CESS = item.CESS ?? 0,
                TotalAmount = item.Total_Amount ?? 0

            }).ToList();

            return result;
        }



		public async Task<DashboardDataModel> GetDashboardDataAsync(string gstin, string period)
		{
			var requestNo = await _context.Sales_Ledger_Tickets
				.Where(x => x.Client_Gstin == gstin && x.Ticket_Status == "Completed")
				.Select(x => x.Request_Number)
				.ToListAsync();

			var data = await _context.SLComparedDatas
				.Where(x => x.Client_Gstin == gstin &&
							requestNo.Contains(x.Request_Number) && // Correct usage
							x.Period == period &&
							x.DataSource.Trim() == "Invoice")
				.GroupBy(x => x.Invoice_Number)
				.Select(g => g.FirstOrDefault()) // Gets one full record per unique Invoice_Number
				.ToListAsync();


			var dashboardData = new DashboardDataModel
			{
				TotalRecords = data.Count,

				MatchedRecordsCount = data.Count(x => x.Category == "Completely_Matched"),
				MatchedRecordsSum = (decimal)data.Where(x => x.Category == "Completely_Matched").Sum(x => x.Taxable_Amount),

				PartiallyMatchedRecordsCount = data.Count(x => x.Category == "Partially_Matched"),
				PartiallyMatchedRecordsSum = (decimal)data.Where(x => x.Category == "Partially_Matched").Sum(x => x.Taxable_Amount),

				UnmatchedRecordsCount = data.Count(x => x.Category == "UnMatched"),
				UnmatchedRecordsSum = (decimal)data.Where(x => x.Category == "UnMatched").Sum(x => x.Taxable_Amount)
			};

			return dashboardData;
		}
		public async Task<Dictionary<string, decimal>> GetDashboardBARDataAsync(string fromMonth, string toMonth, string gstin, string type)
		{
			var requestNo = await _context.Sales_Ledger_Tickets
				.Where(x => x.Client_Gstin == gstin && x.Ticket_Status == "Completed")
				.Select(x => x.Request_Number)
				.ToListAsync();

			var result = new Dictionary<string, decimal>();

			DateTime fromDate = DateTime.ParseExact(fromMonth, "MMM-yy", CultureInfo.InvariantCulture);
			DateTime toDate = DateTime.ParseExact(toMonth, "MMM-yy", CultureInfo.InvariantCulture);

			// Generate month list in lowercase for comparison
			var monthsList = new List<string>();
			var current = fromDate;
			while (current <= toDate)
			{
				monthsList.Add(current.ToString("MMM-yy", CultureInfo.InvariantCulture).ToLower()); // e.g., "apr-24"
				current = current.AddMonths(1);
			}

			// Fetch relevant data from DB and normalize period to lowercase

			var data = await _context.SLComparedDatas
				.Where(x => x.Client_Gstin == gstin &&
							requestNo.Contains(x.Request_Number) &&
							x.DataSource.Trim().ToLower() == "invoice" &&
							monthsList.Contains(x.Period.Trim().ToLower()))
				.ToListAsync();

			// Group and sum total tax for each month
			foreach (var month in monthsList)
			{
				var monthData = data.Where(x => x.Period.Trim().ToLower() == month);
				decimal totalTax = monthData.Sum(x => x.Total_Amount ?? 0);
				// Use null-coalescing operator to handle null values
				//(x.CGST ?? 0) + (x.SGST ?? 0) + (x.IGST ?? 0) + (x.CESS ?? 0));

				result[CultureInfo.InvariantCulture.TextInfo.ToTitleCase(month)] = totalTax;
				// Optional: convert back to "Apr-24" casing
			}

			return result;
		}

	}
}
