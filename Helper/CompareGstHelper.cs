using CAF.GstMatching.Data;
using CAF.GstMatching.Models.CompareGst;
using Microsoft.Build.Framework.XamlTypes;
using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Data.Entity.Validation;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CAF.GstMatching.Helper
{
    public class CompareGstHelper
    {
        private readonly ApplicationDbContext _context;
       
        public CompareGstHelper(ApplicationDbContext context)
        {
            _context = context;
        }
        // Column name constants for consistency
        private static class PD  //colunm names for Purchase Data file   based on DB
        {
            public const string EXERT_ID = "EXERT_ID_COL";
            public const string ClientGSTIN = "Client_GSTIN";
            public const string RequestNo = "Request_No";
            public const string TxnPeriod = "Txn_Period";
            public const string SupplierGSTIN = "Supplier_GSTIN";
            public const string SupplierName = "Supplier_Name";
            public const string InvoiceNo = "Invoiceno";
            public const string InvoiceDate = "Invoice_Date";
            public const string Day = "Day";
            public const string Month = "Month";
            public const string Year = "Year";
            public const string TaxableValue = "Taxable_Value";
            public const string CGST = "CGST";
            public const string SGST = "SGST";
            public const string IGST = "Integrated_Tax";
            public const string CESS = "CESS";

            public const string Modified = "Modified";
            public const string TaxAmount = "TaxAmount";
        }
        private static class GD  //colunm names for GSTR2 Data file    based on DB
        {
            public const string EXERT_ID = "EXERT_ID_COL";
            public const string ClientGSTIN = "Client_GSTIN";
            public const string GSTRegType = "GST_Reg_Type";
            public const string InvoiceNo = "Supplier_Invoice_No";
            public const string InvoiceDate = "Supplier_Invoice_Date";
            public const string Day = "Day";
            public const string Month = "Month";
            public const string Year = "Year";
            public const string SupplierGSTIN = "Supplier_GSTIN";
            public const string SupplierName = "Supplier_Name";
            public const string IsRCMApplied = "IsRCMApplied";
            public const string InvoiceValue = "Invoice_Value";
            public const string TaxableValue = "Item_Taxable_Value";
            public const string GSTRate = "GST_Rate";
            public const string IGST = "IGST_Value";
            public const string CGST = "CGST_Value";
            public const string SGST = "SGST_Value";
            public const string CESS = "CESS";
            public const string IsReturnFiled = "IsReturnFiled";
            public const string TxnPeriod = "Txn_Period";
            public const string RequestNo = "Request_No";

            public const string Modified = "Modified";
        }

        #region first 2 methods 

        //string DS = "Invoice";
        //rowNumber = 1;
        //foreach (var row in PurchaseData)
        //         {
        //             //Console.WriteLine($"Row number : {rowNumber} , DS : {DS}"); // Debugging line
        //             // In the first loop

        //	var PDInvoice = GetString(row, PD.InvoiceNo)?.Trim().ToUpper();
        //	var matchingGDs = Gstr2Data
        //		.Where(b => GetString(b, GD.InvoiceNo)?.Trim().ToUpper() == PDInvoice)
        //		.ToList();

        //	if (matchingGDs.Any())
        //	{
        //		bool matchFound = false;
        //		foreach (var matchGD in matchingGDs)
        //		{
        //			string Day = GetString(row, PD.Day), GDay = GetString(matchGD, GD.Day);
        //			string Month = GetString(row, PD.Month), GMonth = GetString(matchGD, GD.Month);
        //			string Year = GetString(row, PD.Year), GYear = GetString(matchGD, GD.Year);

        //			string formattedPDDate = $"{Day}-{Month}-{Year}";
        //			string formattedGDDate = $"{GDay}-{GMonth}-{GYear}";

        //			var isInvoiceDateMatch = formattedPDDate == formattedGDDate;
        //			var isTaxableValueMatch = GetDecimal(row, PD.TaxableValue) == GetDecimal(matchGD, GD.TaxableValue);
        //			var isSupplierGstinMatch = GetString(row, PD.SupplierGSTIN) == GetString(matchGD, GD.SupplierGSTIN);

        //			if (!isInvoiceDateMatch)
        //			{
        //				CreateMismatch(rowNumber++, row, DS, matchTypeList[3]);
        //				continue; // Skip to the next iteration 
        //			}
        //			if (!isTaxableValueMatch)
        //			{
        //				CreateMismatch(rowNumber++, row, DS, matchTypeList[4]);
        //				continue; // Skip to the next iteration 
        //			}
        //			if (!isSupplierGstinMatch)
        //			{
        //				CreateMismatch(rowNumber++, row, DS, matchTypeList[5]);
        //				continue; // Skip to the next iteration 
        //			}

        //			// If all match, now check tax values
        //			decimal totalPDTax = GetDecimal(row, PD.CGST) + GetDecimal(row, PD.SGST) + GetDecimal(row, PD.IGST) + GetDecimal(row, PD.CESS);
        //			decimal totalGDTax = GetDecimal(matchGD, GD.CGST) + GetDecimal(matchGD, GD.SGST) + GetDecimal(matchGD, GD.IGST) + GetDecimal(matchGD, GD.CESS);

        //			var TaxAmount = Math.Abs(totalPDTax - totalGDTax);

        //			if (TaxAmount < 10)
        //			{
        //				if (TaxAmount != 0)
        //					CreateMismatch(rowNumber++, row, DS, matchTypeList[1]);
        //				else
        //					CreateMismatch(rowNumber++, row, DS, matchTypeList[0]);
        //			}
        //			else
        //			{
        //				CreateMismatch(rowNumber++, row, DS, matchTypeList[6]);
        //			}

        //			matchFound = true;
        //			break; // Stop after first best match
        //		}

        //		//if (!matchFound)
        //		//{
        //		//	CreateMismatch(rowNumber++, row, DS, matchTypeList[7]); // No exact match among duplicates
        //		//}
        //	}
        //	else
        //	{
        //		CreateMismatch(rowNumber++, row, DS, matchTypeList[8]); // No matching invoice at all
        //	}

        //	//var PDInvoice = GetString(row, PD.InvoiceNo)?.Trim().ToUpper();
        //	//var matchGD = Gstr2Data.FirstOrDefault(b => GetString(b, GD.InvoiceNo)?.Trim().ToUpper() == PDInvoice);
        //	//if (matchGD != null)
        // //            {

        // //                string Day = GetString(row, PD.Day), GDay = GetString(matchGD, GD.Day);
        // //                string Month = GetString(row, PD.Month), GMonth = GetString(matchGD, GD.Month);
        // //                string Year = GetString(row, PD.Year), GYear = GetString(matchGD, GD.Year);

        // //                string formattedPDDate = $"{Day}-{Month}-{Year}" , formattedGDDate = $"{GDay}-{GMonth}-{GYear}";

        // //                var isInvoiceDateMatch = formattedPDDate == formattedGDDate;                
        // //                var isTaxableValueMatch = GetDecimal(row, PD.TaxableValue) == GetDecimal(matchGD, GD.TaxableValue);
        // //                var isSupplierGstinMatch = GetString(row, PD.SupplierGSTIN) == GetString(matchGD, GD.SupplierGSTIN);

        // //                if (!isInvoiceDateMatch)
        // //                {
        // //                    CreateMismatch(rowNumber++, row, DS, matchTypeList[3]);
        // //                    continue; // Skip to the next iteration 
        // //                }
        // //                if (!isTaxableValueMatch)
        // //                {
        // //                    CreateMismatch(rowNumber++, row, DS, matchTypeList[4]);
        // //                    continue; // Skip to the next iteration 
        // //                }
        // //                if (!isSupplierGstinMatch)
        // //                {
        // //                    CreateMismatch(rowNumber++, row, DS, matchTypeList[5]);
        // //                    continue; // Skip to the next iteration 
        // //                }

        // //                if (isSupplierGstinMatch && isInvoiceDateMatch && isTaxableValueMatch)
        // //                {
        // //                    decimal totalPDTax = GetDecimal(row, PD.CGST) + GetDecimal(row, PD.SGST) + GetDecimal(row, PD.IGST) + GetDecimal(row, PD.CESS);
        // //                    decimal totalGDTax = GetDecimal(matchGD, GD.CGST) + GetDecimal(matchGD, GD.SGST) + GetDecimal(matchGD, GD.IGST) + GetDecimal(matchGD, GD.CESS);

        // //                    var TaxAmount = Math.Abs(totalPDTax - totalGDTax);

        // //                    if (TaxAmount < 10)
        // //                    {
        // //                        if (TaxAmount != 0)
        // //                        {
        // //                            CreateMismatch(rowNumber++, row, DS, matchTypeList[1]);                              
        // //                        }
        // //                        else
        // //                        {
        // //                            CreateMismatch(rowNumber++, row, DS, matchTypeList[0]);
        // //                        }
        // //                    }
        // //                    else
        // //                    {
        // //                        CreateMismatch(rowNumber++, row, DS, matchTypeList[6]);
        // //                    }
        // //                    continue; // Skip to the next iteration
        // //                }
        // //                CreateMismatch(rowNumber++, row, DS, matchTypeList[7]);
        // //            }
        // //            else
        // //            {
        // //                CreateMismatch(rowNumber++, row, DS, matchTypeList[8]); // 3_Partially_Matched_GST_INV                
        // //            }
        //         }

        //         DS = "Portal";
        //         rowNumber = 1;
        //foreach (var row in Gstr2Data)
        //{
        //	var GDInvoice = GetString(row, GD.InvoiceNo)?.Trim().ToUpper();
        //	var matchingPDs = PurchaseData
        //		.Where(p => GetString(p, PD.InvoiceNo)?.Trim().ToUpper() == GDInvoice)
        //		.ToList();

        //	if (matchingPDs.Any())
        //	{
        //		bool exactMatchFound = false;

        //		foreach (var matchPD in matchingPDs)
        //		{
        //			string Day = GetString(matchPD, PD.Day), GDay = GetString(row, GD.Day);
        //			string Month = GetString(matchPD, PD.Month), GMonth = GetString(row, GD.Month);
        //			string Year = GetString(matchPD, PD.Year), GYear = GetString(row, GD.Year);

        //			string formattedPDDate = $"{Day}-{Month}-{Year}", formattedGDDate = $"{GDay}-{GMonth}-{GYear}";

        //			var isSupplierGstinMatch = GetString(row, GD.SupplierGSTIN) == GetString(matchPD, PD.SupplierGSTIN);
        //			var isInvoiceDateMatch = formattedGDDate == formattedPDDate;
        //			var isTaxableValueMatch = GetDecimal(row, GD.TaxableValue) == GetDecimal(matchPD, PD.TaxableValue);

        //			if (!isInvoiceDateMatch)
        //			{
        //				CreateMismatch(rowNumber++, row, DS, matchTypeList[3]);
        //				continue;
        //			}
        //			if (!isTaxableValueMatch)
        //			{
        //				CreateMismatch(rowNumber++, row, DS, matchTypeList[4]);
        //				continue;
        //			}
        //			if (!isSupplierGstinMatch)
        //			{
        //				CreateMismatch(rowNumber++, row, DS, matchTypeList[5]);
        //				continue;
        //			}

        //			if (isSupplierGstinMatch && isInvoiceDateMatch && isTaxableValueMatch)
        //			{
        //				decimal totalPDTax = GetDecimal(matchPD, PD.CGST) + GetDecimal(matchPD, PD.SGST) + GetDecimal(matchPD, PD.IGST) + GetDecimal(matchPD, PD.CESS);
        //				decimal totalGDTax = GetDecimal(row, GD.CGST) + GetDecimal(row, GD.SGST) + GetDecimal(row, GD.IGST) + GetDecimal(row, GD.CESS);

        //				var TaxAmount = Math.Abs(totalPDTax - totalGDTax);

        //				if (TaxAmount < 10)
        //				{
        //					if (TaxAmount != 0)
        //						CreateMismatch(rowNumber++, row, DS, matchTypeList[1]);
        //					else
        //						CreateMismatch(rowNumber++, row, DS, matchTypeList[0]);
        //				}
        //				else
        //				{
        //					CreateMismatch(rowNumber++, row, DS, matchTypeList[6]);
        //				}

        //				exactMatchFound = true;
        //				break; // Exit inner loop after match
        //			}
        //		}

        //		//if (!exactMatchFound)
        //		//{
        //		//	CreateMismatch(rowNumber++, row, DS, matchTypeList[7]); // Partial match
        //		//}
        //	}
        //	else
        //	{
        //		CreateMismatch(rowNumber++, row, DS, matchTypeList[9]); // No invoice found in PurchaseData
        //	}
        //}


        //foreach (var row in Gstr2Data)
        //{

        //    // In the second loop
        //    var GDInvoice = GetString(row, GD.InvoiceNo)?.Trim().ToUpper();         
        //    var matchPD = PurchaseData.FirstOrDefault(p => GetString(p, PD.InvoiceNo)?.Trim().ToUpper() == GDInvoice);

        //    if (matchPD != null)
        //    {
        //        string Day = GetString(matchPD, PD.Day), GDay = GetString(row, GD.Day);
        //        string Month = GetString(matchPD, PD.Month), GMonth = GetString(row, GD.Month);
        //        string Year = GetString(matchPD, PD.Year), GYear = GetString(row, GD.Year);

        //        string formattedPDDate = $"{Day}-{Month}-{Year}", formattedGDDate = $"{GDay}-{GMonth}-{GYear}";

        //        var isSupplierGstinMatch = GetString(row, GD.SupplierGSTIN) == GetString(matchPD, PD.SupplierGSTIN);
        //        var isInvoiceDateMatch = formattedGDDate == formattedPDDate;
        //        var isTaxableValueMatch = GetDecimal(row, GD.TaxableValue) == GetDecimal(matchPD, PD.TaxableValue);

        //        if (!isInvoiceDateMatch)
        //        {
        //            CreateMismatch(rowNumber++, row, DS, matchTypeList[3]);
        //            continue; // Skip to the next iteration 
        //        }
        //        if (!isTaxableValueMatch)
        //        {
        //            CreateMismatch(rowNumber++, row, DS, matchTypeList[4]);
        //            continue; // Skip to the next iteration 
        //        }
        //        if (!isSupplierGstinMatch)
        //        {
        //            CreateMismatch(rowNumber++, row, DS, matchTypeList[5]);
        //            continue; // Skip to the next iteration 
        //        }

        //        if (isSupplierGstinMatch && isInvoiceDateMatch && isTaxableValueMatch)
        //        {
        //            decimal totalGDTax = GetDecimal(row, GD.CGST) + GetDecimal(row, GD.SGST) + GetDecimal(row, GD.IGST) + GetDecimal(row, GD.CESS);
        //            decimal totalPDTax = GetDecimal(matchPD, PD.CGST) + GetDecimal(matchPD, PD.SGST) + GetDecimal(matchPD, PD.IGST) + GetDecimal(matchPD, PD.CESS);

        //            var TaxAmount = Math.Abs(totalPDTax - totalGDTax);

        //            if (TaxAmount < 10)
        //            {
        //                if (TaxAmount != 0)
        //                {
        //                    CreateMismatch(rowNumber++, row, DS, matchTypeList[1]);
        //                }
        //                else
        //                {
        //                    CreateMismatch(rowNumber++, row, DS, matchTypeList[0]);
        //                }
        //            }
        //            else
        //            {
        //                CreateMismatch(rowNumber++, row, DS, matchTypeList[6]);
        //            }
        //            continue; // Skip to the next iteration
        //        }
        //        CreateMismatch(rowNumber++, row, DS, matchTypeList[7]);
        //    }
        //    else
        //    {
        //        CreateMismatch(rowNumber++, row, DS, matchTypeList[9]);  // 3_Partially_Matched_GST_INV
        //    }
        //}

        #endregion

        #region 2nd tryle Method
        //int rowNumber = 1;
        //// Step 1: Process PurchaseData
        //foreach (var pdRow in PurchaseData)
        //{
        //	string pdInvoiceNo = GetString(pdRow, PD.InvoiceNo)?.Trim().ToUpper();
        //	string pdGSTIN = GetString(pdRow, PD.SupplierGSTIN)?.Trim().ToUpper();
        //	string pdDate = GetString(pdRow, PD.InvoiceDate)?.Trim();

        //	// Get all GSTR2 rows with same InvoiceNo
        //	var matchingGdRows = Gstr2Data.Where(g =>
        //		GetString(g, GD.InvoiceNo)?.Trim().ToUpper() == pdInvoiceNo &&
        //		!matchedGDKeys.Contains(GetMatchKey(g, GD.InvoiceNo, GD.SupplierGSTIN, GD.TaxableValue))
        //	).ToList();

        //	bool foundMatch = false;

        //	foreach (var gdRow in matchingGdRows)
        //	{
        //		string gdGSTIN = GetString(gdRow, GD.SupplierGSTIN)?.Trim().ToUpper();
        //		string gdDate = GetString(gdRow, GD.InvoiceDate)?.Trim();

        //		if(pdGSTIN != gdGSTIN)
        //		{
        //			string category = DetermineMatchCategory(pdRow, gdRow);
        //			CreateMismatch(rowNumber++, pdRow, "Invoice", category);
        //			//CreateMismatch(rowNumber++, gdRow, "Portal", category);
        //			matchedPDKeys.Add(GetMatchKey(pdRow, PD.InvoiceNo, PD.SupplierGSTIN, PD.TaxableValue));
        //			//matchedGDKeys.Add(GetMatchKey(gdRow, GD.InvoiceNo, GD.SupplierGSTIN, GD.TaxableValue));

        //			//foundMatch = true;
        //			break;

        //		}
        //		if(pdDate != gdDate)
        //		{
        //			string category = DetermineMatchCategory(pdRow, gdRow);
        //			CreateMismatch(rowNumber++, pdRow, "Invoice", category);
        //			CreateMismatch(rowNumber++, gdRow, "Portal", category);
        //			matchedPDKeys.Add(GetMatchKey(pdRow, PD.InvoiceNo, PD.SupplierGSTIN, PD.TaxableValue));
        //			matchedGDKeys.Add(GetMatchKey(gdRow, GD.InvoiceNo, GD.SupplierGSTIN, GD.TaxableValue));

        //			foundMatch = true;
        //			break;
        //		}


        //		// Check SupplierGSTIN and InvoiceDate match
        //		if (pdGSTIN == gdGSTIN && pdDate == gdDate)
        //		{
        //			string category = DetermineMatchCategory(pdRow, gdRow);
        //			CreateMismatch(rowNumber++, pdRow, "Invoice", category);
        //			CreateMismatch(rowNumber++, gdRow, "Portal", category);

        //			matchedPDKeys.Add(GetMatchKey(pdRow, PD.InvoiceNo, PD.SupplierGSTIN, PD.TaxableValue));
        //			matchedGDKeys.Add(GetMatchKey(gdRow, GD.InvoiceNo, GD.SupplierGSTIN, GD.TaxableValue));

        //			foundMatch = true;
        //			break;
        //		}
        //	}

        //	if (!foundMatch)
        //	{
        //		// Case 8: InvoiceNo exists, but GSTIN/Date mismatched in all possible rows
        //		CreateMismatch(rowNumber++, pdRow, "Invoice", matchTypeList[8]);
        //	}
        //}


        //// Step 1: Process PurchaseData
        //foreach (var pdRow in PurchaseData)
        //{
        //	string key = GetMatchKey(pdRow, PD.InvoiceNo, PD.SupplierGSTIN, PD.TaxableValue);
        //	var gdRow = Gstr2Data.FirstOrDefault(g =>
        //		!matchedGDKeys.Contains(GetMatchKey(g, GD.InvoiceNo, GD.SupplierGSTIN, GD.TaxableValue)) &&
        //		GetString(g, GD.InvoiceNo)?.Trim().ToUpper() == GetString(pdRow, PD.InvoiceNo)?.Trim().ToUpper());

        //	if (gdRow != null)
        //	{
        //		string category = DetermineMatchCategory(pdRow, gdRow);
        //		CreateMismatch(rowNumber++, pdRow, "Invoice", category);
        //		CreateMismatch(rowNumber++, gdRow, "Portal", category);

        //		matchedPDKeys.Add(key);
        //		matchedGDKeys.Add(GetMatchKey(gdRow, GD.InvoiceNo, GD.SupplierGSTIN, GD.TaxableValue));
        //	}
        //	else
        //	{
        //		CreateMismatch(rowNumber++, pdRow, "Invoice", matchTypeList[8]); // No match in GSTR2
        //	}
        //}

        //// Step 2: Process GSTR2Data (to catch unprocessed rows)
        //foreach (var gdRow in Gstr2Data)
        //{
        //	string key = GetMatchKey(gdRow, GD.InvoiceNo, GD.SupplierGSTIN, GD.TaxableValue);
        //	if (!matchedGDKeys.Contains(key))
        //	{
        //		CreateMismatch(rowNumber++, gdRow, "Portal", matchTypeList[9]); // No match in Purchase
        //	}
        //}


        //var matchedPDKeys = new HashSet<string>();
        //var matchedGDKeys = new HashSet<string>();

        //int rowNumber = 1;
        //// Step 1: Process PurchaseData
        //foreach (var pdRow in PurchaseData)
        //{
        //	string key = GetMatchKey(pdRow, PD.InvoiceNo, PD.SupplierGSTIN, PD.TaxableValue);
        //	var gdRow = Gstr2Data.FirstOrDefault(g =>
        //		GetString(g, GD.InvoiceNo)?.Trim().ToUpper() == GetString(pdRow, PD.InvoiceNo)?.Trim().ToUpper());

        //	if (gdRow != null)
        //	{
        //		// Match found — determine category
        //		string category = DetermineMatchCategory(pdRow, gdRow);
        //		CreateMismatch(rowNumber++, pdRow, "Invoice", category);

        //		matchedPDKeys.Add(key);
        //		matchedGDKeys.Add(GetMatchKey(gdRow, GD.InvoiceNo, GD.SupplierGSTIN, GD.TaxableValue));
        //	}
        //	else
        //	{
        //		CreateMismatch(rowNumber++, pdRow, "Invoice", matchTypeList[8]); // No match in GSTR2
        //	}
        //}

        //// Step 2: Process GSTR2Data (to catch unprocessed rows)

        //foreach (var gdRow in Gstr2Data)
        //{
        //	string key = GetMatchKey(gdRow, GD.InvoiceNo, GD.SupplierGSTIN, GD.TaxableValue);
        //	if (matchedGDKeys.Contains(key))
        //		continue; // Already processed with Purchase

        //	var pdRow = PurchaseData.FirstOrDefault(p =>
        //		GetString(p, PD.InvoiceNo)?.Trim().ToUpper() == GetString(gdRow, GD.InvoiceNo)?.Trim().ToUpper());

        //	if (pdRow != null)
        //	{
        //		string category = DetermineMatchCategory(pdRow, gdRow);
        //		CreateMismatch(rowNumber++, gdRow, "Portal", category);
        //	}
        //	else
        //	{
        //		CreateMismatch(rowNumber++, gdRow, "Portal", matchTypeList[9]); // No match in Purchase
        //	}
        //}
        #endregion

        public async Task SaveCompareDataAsync(DataTable purchaseData, DataTable gstr2Data)
        {
			var matchTypes = ConfigurationManager.AppSettings["MatchTypes"];
			var matchTypeList = matchTypes.Split(',').Select(x => x.Trim()).ToList();

			var requestNo = purchaseData.Rows[0][PD.RequestNo].ToString();

            // First, check if records with the given ticket exist
            var existingData = _context.GSTCompareDatas.Where(x => x.Request_No == requestNo).ToList();
            // If they exist, remove them
            if (existingData.Any())
            {
                _context.GSTCompareDatas.RemoveRange(existingData);
                await _context.SaveChangesAsync(); // Save after deletion before inserting new records
            }

            var PurchaseData = purchaseData.AsEnumerable(); 
            var Gstr2Data = gstr2Data.AsEnumerable();
			var matchedPDKeys = new HashSet<string>();
			var matchedGDKeys = new HashSet<string>();

            #region  3rd Trile


            var category7Rows = new List<DataRow>(); // Category 7: Available_In_PR_Not_In_Portal
            var category8Rows = new List<DataRow>(); // Category 8: Available_In_Portal_Not_In_PR
           
            int invoiceRowNumber = 1;
            int portalRowNumber = 1;


            // ===== STEP 1: Invoice -> Portal Matching =====
            foreach (var pdRow in PurchaseData)
            {
                string pdInvoiceNo = GetString(pdRow, PD.InvoiceNo)?.Trim().ToUpper();
                string pdGSTIN = GetString(pdRow, PD.SupplierGSTIN)?.Trim().ToUpper();
                string pdDate = GetString(pdRow, PD.InvoiceDate)?.Trim();

                var matchedGdRows = Gstr2Data.Where(g =>
                    GetString(g, GD.InvoiceNo)?.Trim().ToUpper() == pdInvoiceNo &&
                    GetString(g, GD.SupplierGSTIN)?.Trim().ToUpper() == pdGSTIN 
                ).ToList();
                string category;

                if (matchedGdRows.Any())
                {
                    var matched = false;
                    foreach (var gdRow in matchedGdRows)
                    {
                        string gdDate = GetString(gdRow, GD.InvoiceDate)?.Trim();
                        if (pdDate != gdDate)
                        {
                            category = DetermineMatchCategory(pdRow, gdRow);
                            CreateMismatch(invoiceRowNumber++, pdRow, "Invoice", category);
                            //CreateMismatch(rowNumber++, gdRow, "Portal", category);
                            matched = true;
                            break;
                        }

                        category = DetermineMatchCategory(pdRow, gdRow);
                        CreateMismatch(invoiceRowNumber++, pdRow, "Invoice", category);
                        //CreateMismatch(rowNumber++, gdRow, "Portal", category);

                        matchedPDKeys.Add(GetMatchKey(pdRow, PD.InvoiceNo, PD.SupplierGSTIN, PD.TaxableValue));
                        matchedGDKeys.Add(GetMatchKey(gdRow, GD.InvoiceNo, GD.SupplierGSTIN, GD.TaxableValue));

                        matched = true;
                        break;
                    }

                    if (!matched)
                    {
                        category7Rows.Add(pdRow);
                    }
                }
                else
                {
                    category7Rows.Add(pdRow);
                }
            }
           
            // ===== STEP 2: Portal -> Invoice Matching =====
            foreach (var gdRow in Gstr2Data)
            {
                string gdInvoiceNo = GetString(gdRow, GD.InvoiceNo)?.Trim().ToUpper();
                string gdGSTIN = GetString(gdRow, GD.SupplierGSTIN)?.Trim().ToUpper();
                string gdDate = GetString(gdRow, GD.InvoiceDate)?.Trim();

                var matchingPdRows = PurchaseData.Where(p =>
                    GetString(p, PD.InvoiceNo)?.Trim().ToUpper() == gdInvoiceNo &&
                    GetString(p, PD.SupplierGSTIN)?.Trim().ToUpper() == gdGSTIN
                ).ToList();

                string category;

                if (matchingPdRows.Any())
                {
                    var matched = false;
                    foreach (var pdRow in matchingPdRows)
                    {
                        string pdDate = GetString(pdRow, PD.InvoiceDate)?.Trim();
                        if (pdDate != gdDate)
                        {
                            category = DetermineMatchCategory(pdRow, gdRow);
                            //CreateMismatch(rowNumber++, pdRow, "Invoice", category);
                            CreateMismatch(portalRowNumber++, gdRow, "Portal", category);
                            matched = true;
                            break;
                        }

                        category = DetermineMatchCategory(pdRow, gdRow);
                        //CreateMismatch(rowNumber++, pdRow, "Invoice", category);
                        CreateMismatch(portalRowNumber++, gdRow, "Portal", category);

                        matchedPDKeys.Add(GetMatchKey(pdRow, PD.InvoiceNo, PD.SupplierGSTIN, PD.TaxableValue));
                        matchedGDKeys.Add(GetMatchKey(gdRow, GD.InvoiceNo, GD.SupplierGSTIN, GD.TaxableValue));

                        matched = true;
                        break;
                    }

                    if (!matched)
                    {
                        category8Rows.Add(gdRow);
                    }
                }
                else
                {
                    category8Rows.Add(gdRow);
                }


            }

            //Console.WriteLine($"Category 7 Rows Count: {category7Rows.Count}"); // Debugging line
            //Console.WriteLine($"Category 8 Rows Count: {category8Rows.Count}"); // Debugging line

            // ===== STEP 3: Rechecking Category 7 and 8 Using Trimmed Invoice Numbers =====
            string TrimInvoiceNo(string invoiceNo)
            {               
                string trimmedInvoiceNo =new string(invoiceNo.Where(char.IsDigit).ToArray());
                //Console.WriteLine($"InvoiceNo: {invoiceNo} - {trimmedInvoiceNo} "); // Debugging line
                return trimmedInvoiceNo;            
            }

            foreach (var pdRow in category7Rows.ToList())
            {
                string pdInvoiceTrim = TrimInvoiceNo(GetString(pdRow, PD.InvoiceNo)?.Trim().ToUpper());
                string pdGSTIN = GetString(pdRow, PD.SupplierGSTIN)?.Trim().ToUpper();

                var matchingGdRows = category8Rows
                    .Where(gd => TrimInvoiceNo(GetString(gd, GD.InvoiceNo)?.Trim().ToUpper()) == pdInvoiceTrim)
                    .ToList();

                if (matchingGdRows.Any())
                {
                    bool foundExactMatch = false;

                    foreach (var gdRow in matchingGdRows.ToList())
                    {
                        string gdGSTIN = GetString(gdRow, GD.SupplierGSTIN)?.Trim().ToUpper();

                        if (gdGSTIN == pdGSTIN)
                        {
                            string category = DetermineMatchCategory(pdRow, gdRow);

                            if (new[] {
                                            matchTypeList[0], // 1_Exactly_Matched_GST_INV_DT_TAXB_TAX
                                            matchTypeList[1], // 2_Matched_With_Tolerance_GST_INV_DT_TAXB_TAX
                                            matchTypeList[3], // 4_Partially_Matched_GST_DT
                                            matchTypeList[4], // 5_Probable_Matched_GST_TAXB
                                            matchTypeList[5]  // 6_UnMatched_Excess_Of_TaxAmount
                                        }.Contains(category))
                            {
                               CreateMismatch(invoiceRowNumber++, pdRow, "Invoice", category); 
                                CreateMismatch(portalRowNumber++, gdRow, "Portal", category);

                                category7Rows.Remove(pdRow);
                                category8Rows.Remove(gdRow);

                                foundExactMatch = true;
                                break;
                            }
                        }
                    }

                    // Fallback to Category 2 if GSTIN matched but no exact match
                    if (!foundExactMatch)
                    {
                        var gdRow = matchingGdRows.FirstOrDefault(gd => GetString(gd, GD.SupplierGSTIN)?.Trim().ToUpper() == pdGSTIN);
                        if (gdRow != null)
                        {
                            CreateMismatch(invoiceRowNumber++, pdRow, "Invoice", matchTypeList[2]); // Category 3 : 3_Partially_Matched_GST_INV
                            CreateMismatch(portalRowNumber++, gdRow, "Portal", matchTypeList[2]);  // Category 3 : 3_Partially_Matched_GST_INV

                            category7Rows.Remove(pdRow);
                            category8Rows.Remove(gdRow);
                        }
                    }
                }
            }

            // ===== Final Unmatched Category 8 & 9 =====
            //Console.WriteLine($"Final Category 7 Rows Count: {category7Rows.Count}"); // Debugging line
            //Console.WriteLine($"Final Category 8 Rows Count: {category8Rows.Count}"); // Debugging line
            foreach (var pdRow in category7Rows)
            {
                CreateMismatch(invoiceRowNumber++, pdRow, "Invoice", matchTypeList[6]); // Category 7: Available_In_PR_Not_In_Portal
            }
            foreach (var gdRow in category8Rows)
            {
                CreateMismatch(portalRowNumber++, gdRow, "Portal", matchTypeList[7]); // Category 8: Available_In_Portal_Not_In_PR
            }

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
		string DetermineMatchCategory(DataRow pdRow, DataRow gdRow)  // 0,1,3,4,5
		{
			var matchTypes = ConfigurationManager.AppSettings["MatchTypes"];
			var matchTypeList = matchTypes.Split(',').Select(x => x.Trim()).ToList();

			string pdDay = GetString(pdRow, PD.Day), gdDay = GetString(gdRow, GD.Day);
			string pdMonth = GetString(pdRow, PD.Month), gdMonth = GetString(gdRow, GD.Month);
			string pdYear = GetString(pdRow, PD.Year), gdYear = GetString(gdRow, GD.Year);

			string formattedPDDate = $"{pdDay}-{pdMonth}-{pdYear}";
			string formattedGDDate = $"{gdDay}-{gdMonth}-{gdYear}";

			var isInvoiceDateMatch = formattedPDDate == formattedGDDate;
			
			
			if (!isInvoiceDateMatch)
				return matchTypeList[3]; // Invoice date mismatch   4_Partially_Matched_GST_DT,


           

            // All 3 above match – now check tax difference
            decimal totalPDTax = GetDecimal(pdRow, PD.CGST) + GetDecimal(pdRow, PD.SGST) + GetDecimal(pdRow, PD.IGST) + GetDecimal(pdRow, PD.CESS);
			if(GetString(pdRow, PD.Modified) == "Yes")
            {
                //Console.WriteLine($"Modified {GetString(pdRow, PD.Modified)} , MTaxAmount : {GetDecimal(pdRow, PD.TaxAmount)} , TaxAmount : {totalPDTax}"); // Debugging line
                totalPDTax = GetDecimal(pdRow, PD.TaxAmount);
            }
            
            decimal totalGDTax = GetDecimal(gdRow, GD.CGST) + GetDecimal(gdRow, GD.SGST) + GetDecimal(gdRow, GD.IGST) + GetDecimal(gdRow, GD.CESS);

			var taxDifference = Math.Abs(totalPDTax - totalGDTax);

			if (taxDifference == 0)
				return matchTypeList[0]; // Exact match  //1_Exactly_Matched_GST_INV_DT_TAXB_TAX,

            if (taxDifference < 10)
				return matchTypeList[1]; // Minor tax diff (< 10) // 2_Matched_With_Tolerance_GST_INV_DT_TAXB_TAX,

            if (totalPDTax > totalGDTax)  // pr is greater than portal 
                return matchTypeList[4]; // 5_Probable_Matched_GST_TAXB,

            //if (totalPDTax < totalGDTax) // portal is greater than pr               
            return matchTypeList[5]; //6_UnMatched_Excess_Of_TaxAmount,



		}

		string GetMatchKey(DataRow row, string invoiceNoCol, string gstinCol, string taxableValueCol)
		{
			return $"{GetString(row, invoiceNoCol)?.Trim().ToUpper()}_" +
				   $"{GetString(row, gstinCol)?.Trim().ToUpper()}_" +
				   $"{GetDecimal(row, taxableValueCol)}";
		}

		public void CreateMismatch(int rowNumber, DataRow DR, string DS, string Match_Type)
        {
            //Console.WriteLine($"Row number : {rowNumber},Data Source : {DS},Match Type : {Match_Type}"); // Debugging line
            var category = GetCategory(Match_Type);
            var matchingResults = GetMatching_Results(category);

            
            // Declare first
            decimal Taxable_Value, CGST, SGST, IGST, CESS, Total_Tax;
            string invoiceDateInput,period ,invoiceno, supplierGstin ,supplierName, uniquevalue;

           
            if (DS == "Invoice")
            {
                uniquevalue = GetString(DR, PD.EXERT_ID);

                invoiceDateInput = GetString(DR, PD.InvoiceDate);
                invoiceno = GetString(DR, PD.InvoiceNo);
                supplierGstin = GetString(DR, PD.SupplierGSTIN);
                supplierName = GetString(DR, PD.SupplierName);
                period = GetString(DR, PD.TxnPeriod);

                Taxable_Value = DR[PD.TaxableValue] == DBNull.Value ? 0 : Math.Round(Convert.ToDecimal(DR[PD.TaxableValue]), 2);
                CGST = DR[PD.CGST] == DBNull.Value ? 0 : Math.Round(Convert.ToDecimal(DR[PD.CGST]), 2);
                SGST = DR[PD.SGST] == DBNull.Value ? 0 : Math.Round(Convert.ToDecimal(DR[PD.SGST]), 2);
                IGST = DR[PD.IGST] == DBNull.Value ? 0 : Math.Round(Convert.ToDecimal(DR[PD.IGST]), 2);
                CESS = DR[PD.CESS] == DBNull.Value ? 0 : Math.Round(Convert.ToDecimal(DR[PD.CESS]), 2);

                Total_Tax = CGST + SGST + IGST + CESS;

            }

            else
            {
                uniquevalue = GetString(DR, GD.EXERT_ID);
                invoiceDateInput = GetString(DR, GD.InvoiceDate);
                period = GetString(DR, GD.TxnPeriod);
                invoiceno = GetString(DR, GD.InvoiceNo);
                supplierGstin = GetString(DR, GD.SupplierGSTIN);
                supplierName = GetString(DR, GD.SupplierName);

                Taxable_Value = DR[GD.TaxableValue] == DBNull.Value ? 0 : Math.Round(Convert.ToDecimal(DR[GD.TaxableValue]), 2);
                CGST = DR[GD.CGST] == DBNull.Value ? 0 : Math.Round(Convert.ToDecimal(DR[GD.CGST]), 2);
                SGST = DR[GD.SGST] == DBNull.Value ? 0 : Math.Round(Convert.ToDecimal(DR[GD.SGST]), 2);
                IGST = DR[GD.IGST] == DBNull.Value ? 0 : Math.Round(Convert.ToDecimal(DR[GD.IGST]), 2);
                CESS = DR[GD.CESS] == DBNull.Value ? 0 : Math.Round(Convert.ToDecimal(DR[GD.CESS]), 2);
                Total_Tax = CGST + SGST + IGST + CESS;
            }
            DateTime? parsedDate = DateTime.TryParse(invoiceDateInput, out DateTime dt) ? dt : (DateTime?)null;
            string invoiceDate = parsedDate?.ToString("dd-MM-yyyy");
            string YearMonth = parsedDate?.ToString("yyyyMM");
           
            string FY = GetFinancialYear(invoiceDate);
            //Console.WriteLine($"Row number : {rowNumber},Data Source : {DS},Invoice Date: {invoiceDate}, Period: {period}, FY: {FY} ,match type : {Match_Type}");
            var data = new GSTCompareData
            {
                unique_value= uniquevalue, 
                Row_Number = rowNumber,
                Request_No = GetString(DR, PD.RequestNo),
                Client_GSTIN = GetString(DR, PD.ClientGSTIN),
                Data_Source = DS,
                Financial_Year = FY,
                Year_Month = YearMonth,
                Period = period,
                Matching_Results = matchingResults,
                Category = category,
                Match_Type = Match_Type,
                Invoice_Number = invoiceno,
                Invoice_Date = invoiceDate,
                Supplier_GSTIN = supplierGstin ,
                Supplier_Name = supplierName,
                Taxable_Value = Taxable_Value,
                Total_Tax = Total_Tax,
                CGST = CGST,
                SGST = SGST,
                IGST = IGST,
                CESS = CESS,

                //Modified = MinvoiceDateInput,
                //InvoiceNo = Minvoiceno,
                //SupplierGSTIN = MsupplierGstin,
                //InvoiceDate = invoiceDateInput,


            };

        _context.GSTCompareDatas.Add(data);
        }

        private string GetCategory(string Match_Type)
        {
            switch (Match_Type)
            {
                case "6_UnMatched_Excess_or_Short_1_Invoicewise":               
                case "Available_In_PR_Not_In_Portal":
                case "Available_In_Portal_Not_In_PR":           
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
        private string GetMatching_Results(string Category)
        {
            switch (Category)
            {
                case "UnMatched":
                    return "IMS Pending";

                case "Partially_Matched":
                case "Completely_Matched":
                    return "IMS Accept";
                default:
                    return "Unknown";
            }

        }
        public string GetFinancialYear(string date)
        {
            if (DateTime.TryParseExact(date, "dd-MM-yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime parsedDate))
            {
                int year = parsedDate.Year;
                int month = parsedDate.Month;

                if (month >= 4) // April or later → current year - next year
                {
                    return $"{year}-{(year + 1).ToString().Substring(2)}";
                }
                else // Jan, Feb, March → previous year - current year
                {
                    return $"{year - 1}-{year.ToString().Substring(2)}";
                }
            }

            return "Invalid FY";
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

        public async Task<List<CompareGstResultModel>> GetComparedDataBasedOnTicketAsync(string requestno, string ClientGSTIN)
        {
            var data = await _context.GSTCompareDatas
                .Where(x => x.Request_No == requestno && x.Client_GSTIN == ClientGSTIN)
                .ToListAsync();

            var result = new List<CompareGstResultModel>();

            

            foreach (var item in data)
            {
                string MinvoiceDateInput = null, Minvoiceno = null, MsupplierGstin = null;
                string OinvoiceDateInput = null, Oinvoiceno = null, OsupplierGstin = null;
                decimal MtotalTax = 0 , OtotalTax = 0;

                if(item.Data_Source.Trim() == "Invoice")
                {
                    var purchaseData = await _context.Purchase_Data
                   .Where(x => x.EXERT_ID_COL.Trim() == item.unique_value)
                   .Select(x => new
                   {
                       x.Supplier_GSTIN,
                       x.Modified,
                       x.Invoiceno,
                       x.Invoice_Date,
                       totalTax = x.CGST + x.SGST + x.Integrated_Tax + x.CESS
                   })
                   .FirstOrDefaultAsync();
                    //Console.WriteLine($"Exert id :{item.unique_value},Supplier Gstin :{purchaseData.Supplier_GSTIN} ,Modified : {purchaseData.Modified} , invoice no : {purchaseData.Invoiceno} , invoice date  {purchaseData.Invoice_Date} , total tax : {purchaseData.totalTax}"); // Debugging line

                    OinvoiceDateInput = purchaseData.Invoice_Date?.ToString("dd-MM-yyyy");
                    Oinvoiceno = purchaseData.Invoiceno;
                    OsupplierGstin = purchaseData.Supplier_GSTIN;
                    OtotalTax = (decimal)purchaseData.totalTax;


                    if (purchaseData != null && !string.IsNullOrWhiteSpace(purchaseData.Modified) && purchaseData.Modified.Trim() == "Yes")
                    {
                        MinvoiceDateInput = item.Invoice_Date;
                        Minvoiceno = item.Invoice_Number;
                        MsupplierGstin = item.Supplier_GSTIN;
                        MtotalTax = (decimal)item.Total_Tax;

                        if (OinvoiceDateInput == MinvoiceDateInput) MinvoiceDateInput = null;
                        if (Oinvoiceno == Minvoiceno) Minvoiceno = null;
                        if (OsupplierGstin == MsupplierGstin) MsupplierGstin = null;
                        if (OtotalTax == MtotalTax) MtotalTax = 0;
                    }

                }
                else
                {
                    OinvoiceDateInput = item.Invoice_Date;
                    Oinvoiceno = item.Invoice_Number;
                    OsupplierGstin = item.Supplier_GSTIN;
                    OtotalTax = (decimal)item.Total_Tax;
                  
                }
               

                // Map to result model
                var mappedItem = new CompareGstResultModel
                {
                    uniquevalue = item.unique_value,
                    RowNumber = item.Row_Number,
                    RequestNo = item.Request_No,
                    YearMonth = item.Year_Month,
                    ClientGSTIN = item.Client_GSTIN,
                    DataSource = item.Data_Source,
                    FinancialYear = item.Financial_Year,
                    Period = item.Period,
                    MatchingResults = item.Matching_Results,
                    Category = item.Category,
                    MatchType = item.Match_Type,

                    InvoiceNumber = Oinvoiceno ,
                    InvoiceDate = OinvoiceDateInput,
                    SupplierGSTIN = OsupplierGstin,
                    TotalTax = OtotalTax,


                    SupplierName = item.Supplier_Name,
                    TaxableValue = (long)item.Taxable_Value,
                   
                    CGST = (decimal)item.CGST,
                    SGST = (decimal)item.SGST,
                    IGST = (decimal)item.IGST,
                    CESS = (decimal)item.CESS,

                    ModifiedInvoiceDate = MinvoiceDateInput,
                    ModifiedInvoiceNumber = Minvoiceno,
                    ModifiedSupplierGSTIN = MsupplierGstin,
                    ModifiedTotalTax = MtotalTax
                };

                result.Add(mappedItem);
            }

            return result;
        }

        //public async Task<List<CompareGstResultModel>> GetComparedDataBasedOnTicketAsync(string requestno , string ClientGSTIN)
        //{
        //    string Modified, MinvoiceDateInput, Minvoiceno, MsupplierGstin;

        //    // Fetch the data based on Request_No
        //    var data = await _context.GSTCompareDatas
        //        .Where(x => x.Request_No == requestno &&
        //                    x.Client_GSTIN == ClientGSTIN)
        //        .ToListAsync();


        //    // Map the data to CompareGstResultModel
        //    var result = data.Select((item, index) => new CompareGstResultModel
        //    {
        //        uniquevalue = item.unique_value,
        //        RowNumber = item.Row_Number,
        //        RequestNo = item.Request_No,
        //        YearMonth = item.Year_Month,
        //        ClientGSTIN = item.Client_GSTIN,
        //        DataSource = item.Data_Source,
        //        FinancialYear = item.Financial_Year,
        //        Period = item.Period,
        //        MatchingResults = item.Matching_Results,
        //        Category = item.Category,
        //        MatchType = item.Match_Type,
        //        InvoiceNumber = item.Invoice_Number,
        //        InvoiceDate = item.Invoice_Date,
        //        SupplierGSTIN = item.Supplier_GSTIN,
        //        SupplierName = item.Supplier_Name,
        //        TaxableValue = (long)item.Taxable_Value,
        //        TotalTax = (decimal)item.Total_Tax,
        //        CGST = (decimal)item.CGST,
        //        SGST = (decimal)item.SGST,
        //        IGST = (decimal)item.IGST,
        //        CESS = (decimal)item.CESS 
        //    }).ToList();
        //    return result;
        //}

        public async Task<DataTable> GetUnMatchedData(string requestno, string ClientGSTIN)
        {
            var UnMatched = ConfigurationManager.AppSettings["UnMatched"];
            var UnMatchedList = UnMatched.Split(',').Select(x => x.Trim()).ToList();

            var data = await _context.GSTCompareDatas
                      .Where(x => x.Request_No == requestno &&
                                  x.Client_GSTIN == ClientGSTIN &&
                                  UnMatchedList.Contains(x.Match_Type.Trim()))
                      .ToListAsync();

            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("uniquevalue");
            dataTable.Columns.Add("RowNumber");
            dataTable.Columns.Add("InvoiceNumber");
            dataTable.Columns.Add("InvoiceDate");
            dataTable.Columns.Add("SupplierGSTIN");
            dataTable.Columns.Add("TotalTax");
            dataTable.Columns.Add("DataSource");
            dataTable.Columns.Add("MatchType");

            //dataTable.Columns.Add("UserGstin");
            //dataTable.Columns.Add("DataSource");
            //dataTable.Columns.Add("FinancialYear");
            //dataTable.Columns.Add("Period");
            //dataTable.Columns.Add("MatchingResults");
            //dataTable.Columns.Add("Category");
            //dataTable.Columns.Add("SupplierName");
            //dataTable.Columns.Add("TaxableValue");
            //dataTable.Columns.Add("CGST");
            //dataTable.Columns.Add("SGST");
            //dataTable.Columns.Add("IGST");    
            //dataTable.Columns.Add("CESS");
            foreach (var item in data)
            {
                DataRow row = dataTable.NewRow();
                row["uniquevalue"] = item.unique_value;
                row["RowNumber"] = item.Row_Number;
                row["DataSource"] = item.Data_Source;
                row["MatchType"] = item.Match_Type;

                if (item.Data_Source.Trim() == "Invoice")
                {
                    var purchaseData = await _context.Purchase_Data
                   .Where(x => x.EXERT_ID_COL.Trim() == item.unique_value)
                   .Select(x => new
                   {
                       x.Modified 
                   })
                   .FirstOrDefaultAsync();

                    if (!string.IsNullOrWhiteSpace(purchaseData.Modified) && purchaseData.Modified.Trim() == "Yes")
                    {
                        var existing = await _context.ModifiedDatas
                                   .Where(x => x.EXERT_ID_COL == item.unique_value.ToString())
                                   .OrderByDescending(x => x.Modified_DateTime)
                                   .FirstOrDefaultAsync();

                        row["InvoiceNumber"] = existing.Invoiceno;
                        row["InvoiceDate"] = existing.Invoice_Date?.ToString("dd-MM-yyyy");
                        row["SupplierGSTIN"] = existing.Supplier_GSTIN;
                        row["TotalTax"] = (decimal)existing.Tax_Amount;
                    }
                    else
                    {
                        row["InvoiceNumber"] = item.Invoice_Number;
                        row["InvoiceDate"] = item.Invoice_Date;
                        row["SupplierGSTIN"] = item.Supplier_GSTIN;
                        row["TotalTax"] = (decimal)item.Total_Tax;
                    }

                }
                else
                {
                    row["InvoiceNumber"] = item.Invoice_Number;
                    row["InvoiceDate"] = item.Invoice_Date;
                    row["SupplierGSTIN"] = item.Supplier_GSTIN;
                    row["TotalTax"] = (decimal)item.Total_Tax;
                }

               
                    

                //row["UserGstin"] = item.Client_GSTIN;
                //
                //row["FinancialYear"] = item.Financial_Year;
                //row["Period"] = item.Period;
                //row["MatchingResults"] = item.Matching_Results;
                //row["Category"] = item.Category;
                //         
                //row["SupplierName"] = item.Supplier_Name;
                //row["TaxableValue"] = (long)item.Taxable_Value;             
                //row["CGST"] = (decimal)item.CGST;
                //row["SGST"] = (decimal)item.SGST;
                //row["IGST"] = (decimal)item.IGST;
                //row["CESS"] = (decimal)item.CESS;

                dataTable.Rows.Add(row);
            }

            return dataTable;
        }
        public async Task<DataTable> GetPortalUnMatchedData(string requestno, string ClientGSTIN)
        {
            var data = await _context.GSTCompareDatas
                .Where(x => x.Request_No == requestno &&
                            x.Client_GSTIN == ClientGSTIN &&
                            x.Data_Source.Trim() == "Portal" &&
                            x.Match_Type.Trim() == "Available_In_Portal_Not_In_PR")
                .ToListAsync();

            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("uniquevalue");
            dataTable.Columns.Add("RowNumber");
            dataTable.Columns.Add("InvoiceNumber");
            dataTable.Columns.Add("InvoiceDate");
            dataTable.Columns.Add("SupplierGSTIN");
            dataTable.Columns.Add("TotalTax");
            dataTable.Columns.Add("DataSource");
            foreach (var item in data)
            {
                DataRow row = dataTable.NewRow();
                row["uniquevalue"] = item.unique_value;
                row["RowNumber"] = item.Row_Number;
                row["InvoiceNumber"] = item.Invoice_Number;
                row["InvoiceDate"] = item.Invoice_Date;
                row["SupplierGSTIN"] = item.Supplier_GSTIN;
                row["TotalTax"] = (decimal)item.Total_Tax;
                row["DataSource"] = item.Data_Source;

                dataTable.Rows.Add(row);
            }

            return dataTable;
        }
         
        public async Task<DashboardDataModel> GetDashboardDataAsync(string gstin , string period)
        {
            var requestNo = await _context.Purchase_Ticket
                .Where(x => x.Client_GSTIN == gstin && x.Ticket_Status == "Completed")
                .Select(x => x.Request_No)
                .ToListAsync();
           
            var data = await _context.GSTCompareDatas
                .Where(x => x.Client_GSTIN == gstin &&
                            requestNo.Contains(x.Request_No) && // Correct usage
                            x.Period == period &&
                            x.Data_Source.Trim() == "Invoice")
                .GroupBy(x => x.Invoice_Number)
                .Select(g => g.FirstOrDefault()) // Gets one full record per unique Invoice_Number
                .ToListAsync();


            var dashboardData = new DashboardDataModel
            {
                TotalRecords = data.Count,

                MatchedRecordsCount = data.Count(x => x.Category == "Completely_Matched"),
                MatchedRecordsSum = (decimal)data.Where(x => x.Category == "Completely_Matched").Sum(x => x.Taxable_Value),

                PartiallyMatchedRecordsCount = data.Count(x => x.Category == "Partially_Matched"),
                PartiallyMatchedRecordsSum = (decimal)data.Where(x => x.Category == "Partially_Matched").Sum(x => x.Taxable_Value),

                UnmatchedRecordsCount = data.Count(x => x.Category == "UnMatched"),
                UnmatchedRecordsSum = (decimal)data.Where(x => x.Category == "UnMatched").Sum(x => x.Taxable_Value)
            };

            return dashboardData;
        }
		public async Task<Dictionary<string, decimal>> GetDashboardBARDataAsync(string fromMonth, string toMonth, string gstin, string type)
		{
            var requestNo = await _context.Purchase_Ticket
                .Where(x => x.Client_GSTIN == gstin && x.Ticket_Status == "Completed")
                .Select(x => x.Request_No)
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

			var data = await _context.GSTCompareDatas
				.Where(x => x.Client_GSTIN == gstin &&
                            requestNo.Contains(x.Request_No) &&
                            x.Data_Source.Trim().ToLower() == "invoice" &&
							monthsList.Contains(x.Period.Trim().ToLower()))
				.ToListAsync();

			// Group and sum total tax for each month
			foreach (var month in monthsList)
			{
				var monthData = data.Where(x => x.Period.Trim().ToLower() == month);
				decimal totalTax = monthData.Sum(x => x.Total_Tax ?? 0); 
                // Use null-coalescing operator to handle null values
					//(x.CGST ?? 0) + (x.SGST ?? 0) + (x.IGST ?? 0) + (x.CESS ?? 0));

				result[CultureInfo.InvariantCulture.TextInfo.ToTitleCase(month)] = totalTax;
				// Optional: convert back to "Apr-24" casing
			}

			return result;
		}


	}
}


//public async Task<DataTable> GetComparedDataBasedONTicketAsync(string requestno)
// {
//    var Data = await _context.GSTCompareDatas
//         .Where(x => x.Request_No == requestno)
//         .ToListAsync();


//     DataTable dataTable = new DataTable();
//     dataTable.Columns.Add("Sno");
//     dataTable.Columns.Add("UserGstin");
//     dataTable.Columns.Add("DataSource");
//     dataTable.Columns.Add("FinancialYear");
//     dataTable.Columns.Add("Period");
//     dataTable.Columns.Add("MatchingResults");
//     dataTable.Columns.Add("Category");
//     dataTable.Columns.Add("MatchType");
//     dataTable.Columns.Add("InvoiceNumber");
//     dataTable.Columns.Add("InvoiceDate");
//     dataTable.Columns.Add("SupplierGSTIN");
//     dataTable.Columns.Add("SupplierName");
//     dataTable.Columns.Add("TaxableValue");
//     dataTable.Columns.Add("TotalTax");
//     dataTable.Columns.Add("CGST");
//     dataTable.Columns.Add("SGST");
//     dataTable.Columns.Add("IGST");    
//     dataTable.Columns.Add("CESS");

//     foreach (var item in Data)
//     {
//         DataRow row = dataTable.NewRow();
//         row["Sno"] = item.Row_Number;
//         row["UserGstin"] = item.Client_GSTIN;
//         row["DataSource"] = item.Data_Source;
//         row["FinancialYear"] = item.Financial_Year;
//         row["Period"] = item.Period;
//         row["MatchingResults"] = item.Matching_Results;
//         row["Category"] = item.Category;
//         row["MatchType"] = item.Match_Type;
//         row["InvoiceNumber"] = item.Invoice_Number;
//         row["InvoiceDate"] = item.Invoice_Date;
//         row["SupplierGSTIN"] = item.Supplier_GSTIN;
//         row["SupplierName"] = item.Supplier_Name;
//         row["TaxableValue"] = item.Taxable_Value;
//         row["TotalTax"] = item.Total_Tax;
//         row["CGST"] = item.CGST;
//         row["SGST"] = item.SGST;
//         row["IGST"] = item.IGST;
//         row["CESS"] = item.CESS;

//         dataTable.Rows.Add(row);
//     }
//     return dataTable;
// }
