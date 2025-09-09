using CAF.GstMatching.Data;
using Microsoft.Build.Framework.XamlTypes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAF.GstMatching.Helper
{
    public class ModifiedDataHelper
    {
        private readonly ApplicationDbContext _context;

        public ModifiedDataHelper(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task SaveModifiedData(DataTable Data, string RequestNo, string ClientGSTIN)
        {
            foreach (DataRow row in Data.Rows)
            {
                string dataSource = row["DataSource"].ToString();
                string uniqueValue = row["uniquevalue"].ToString();

                if (dataSource == "Invoice")
                {
                    var existing = await _context.Purchase_Data
                       .FirstOrDefaultAsync(x => x.EXERT_ID_COL == uniqueValue);
                    if (existing != null)
                    {
                        try
                        {
                            existing.Modified = "Yes";
                            await _context.SaveChangesAsync();
                        }
                       catch(DbEntityValidationException dbEx)
                        {
                            // Extract detailed validation errors
                            var errorMessages = new List<string>();

                            foreach (var validationErrors in dbEx.EntityValidationErrors)
                            {
                                foreach (var validationError in validationErrors.ValidationErrors)
                                {
                                    string error = $"Property: {validationError.PropertyName}, Error: {validationError.ErrorMessage}";
                                    errorMessages.Add(error);
                                    System.Diagnostics.Debug.WriteLine(error); // For debug output
                                }
                            }

                            string fullError = string.Join("; ", errorMessages);
                            throw new Exception("Validation failed while saving Purchase_Data: " + fullError);
                        }
                    }
                }
                else
                {
                    var existing = await _context.GSTR2_Data
                       .FirstOrDefaultAsync(x => x.EXERT_ID_COL == uniqueValue);
                    if (existing != null)
                    {
                        try
                        {
                               existing.Modified = "Yes";
                            await _context.SaveChangesAsync();
                        }
                        catch (DbEntityValidationException dbEx)
                        {
                            // Extract detailed validation errors
                            var errorMessages = new List<string>();

                            foreach (var validationErrors in dbEx.EntityValidationErrors)
                            {
                                foreach (var validationError in validationErrors.ValidationErrors)
                                {
                                    string error = $"Property: {validationError.PropertyName}, Error: {validationError.ErrorMessage}";
                                    errorMessages.Add(error);
                                    System.Diagnostics.Debug.WriteLine(error); // For debug output
                                }
                            }

                            string fullError = string.Join("; ", errorMessages);
                            throw new Exception("Validation failed while saving GSTR2_Data: " + fullError);
                        }
                       
                    }
                }

                try
                {
                    var modifiedData = new ModifiedData
                    {
                        EXERT_ID_COL = uniqueValue,
                        Invoiceno = row["InvoiceNumber"].ToString(),
                        Invoice_Date = string.IsNullOrWhiteSpace(row["InvoiceDate"].ToString())
                            ? (DateTime?)null
                            : Convert.ToDateTime(row["InvoiceDate"]),
                        Supplier_GSTIN = row["SupplierGSTIN"].ToString(),
                        Tax_Amount = string.IsNullOrWhiteSpace(row["TotalTax"].ToString())
                            ? 0
                            : Convert.ToDecimal(row["TotalTax"]),
                        Data_Source = dataSource,
                        Modified_DateTime = DateTime.Now,
                        Request_No = RequestNo,
                        Client_GSTIN = ClientGSTIN
                    };

                    _context.ModifiedDatas.Add(modifiedData);
                }
                catch (Exception innerEx)
                {
                    // This catches per-row issues like conversion errors
                    throw new Exception("Error processing row for uniqueValue: " + uniqueValue + ", " + innerEx.Message);
                }
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbEntityValidationException dbEx)
            {
                // Extract detailed validation errors
                var errorMessages = new List<string>();

                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        string error = $"Property: {validationError.PropertyName}, Error: {validationError.ErrorMessage}";
                        errorMessages.Add(error);
                        System.Diagnostics.Debug.WriteLine(error); // For debug output
                    }
                }

                string fullError = string.Join("; ", errorMessages);
                throw new Exception("Validation failed while saving ModifiedData: " + fullError);
            }
            catch (Exception ex)
            {
                throw new Exception("General error saving ModifiedData: " + ex.Message);
            }
        }

        //public async Task SaveModifiedData(DataTable Data, string RequestNo, string ClientGSTIN)
        //{
        //    foreach (DataRow row in Data.Rows)
        //    { 
        //        //Console.WriteLine("1");
        //        string dataSource = row["DataSource"].ToString();
        //        string exertIdCol = row["uniquevalue"].ToString();

        //        //Console.WriteLine("2");
        //        if (dataSource == "Invoice")
        //        {
        //            var existing = await _context.Purchase_Data
        //               .FirstOrDefaultAsync(x => x.EXERT_ID_COL == exertIdCol);
        //            if (existing != null)
        //            {
        //                existing.Modified = "Yes";
        //                await _context.SaveChangesAsync();
        //            }
        //        }
        //        else if (dataSource == "Portal")
        //        {
        //            var existing = await _context.GSTR2_Data
        //               .FirstOrDefaultAsync(x => x.EXERT_ID_COL == exertIdCol);
        //            if (existing != null)
        //            {
        //                existing.Modified = "Yes";
        //                await _context.SaveChangesAsync();
        //            }
        //        }
        //        //Console.WriteLine("3");
        //        var modifiedData = new ModifiedData
        //        {
        //            EXERT_ID_COL = exertIdCol,
        //            Invoiceno = row["InvoiceNumber"].ToString(),
        //            Invoice_Date = Convert.ToDateTime(row["InvoiceDate"]),
        //            Supplier_GSTIN = row["SupplierGSTIN"].ToString(),
        //            //Tax_Amount = Convert.ToDecimal(row["TotalTax"]),
        //            Data_Source = row["DataSource"].ToString(),
        //            Modified_DateTime = DateTime.Now,
        //            Request_No = RequestNo,
        //            Client_GSTIN = ClientGSTIN
        //        };

        //        _context.ModifiedDatas.Add(modifiedData);
        //    }

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbEntityValidationException ex)
        //    {
        //        var errorMessages = ex.EntityValidationErrors
        //            .SelectMany(e => e.ValidationErrors)
        //            .Select(e => $"Property: {e.PropertyName}, Error: {e.ErrorMessage}");

        //        var fullErrorMessage = string.Join("; ", errorMessages);
        //        var exceptionMessage = $"Validation failed: {fullErrorMessage}";

        //        throw new Exception(exceptionMessage, ex);
        //    }
        //}

        public async Task<DataTable> GetModifiedInvoiceDataBasedOnTicketAsync(string requestNo, string ClientGSTIN)
        {
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


            dataTable.Columns.Add("TaxAmount");
            dataTable.Columns.Add("Modified");

            var purchaseData = await _context.Purchase_Data
                .Where(x => x.Request_No == requestNo && x.Client_GSTIN == ClientGSTIN )
                .ToListAsync();


            foreach (var item in purchaseData)
            {
                DataRow row = dataTable.NewRow();
                row["EXERT_ID_COL"] = item.EXERT_ID_COL;
                row["Client_GSTIN"] = item.Client_GSTIN;
                row["Request_No"] = item.Request_No;
                row["Txn_Period"] = item.Txn_Period;
                
                row["Supplier_Name"] = item.Supplier_Name;
               
                row["Taxable_Value"] = item.Taxable_Value;
                row["CGST"] = item.CGST;
                row["SGST"] = item.SGST;
                row["Integrated_Tax"] = item.Integrated_Tax;
                row["CESS"] = item.CESS;
                row["Day"] = item.Day;
                row["Month"] = item.Month;
                row["Year"] = item.Year;
                row["Modified"] = item.Modified;
              
                if (!string.IsNullOrWhiteSpace(item.Modified) && item.Modified.Trim() == "Yes")
                {
                    var existing = await _context.ModifiedDatas
                           .Where(x => x.EXERT_ID_COL == item.EXERT_ID_COL.ToString())
                           .OrderByDescending(x => x.Modified_DateTime)
                           .FirstOrDefaultAsync();
                    row["Invoiceno"] = existing.Invoiceno;
                    row["Invoice_Date"] = existing.Invoice_Date;
                    row["Supplier_GSTIN"] = existing.Supplier_GSTIN;
                    row["TaxAmount"] = existing.Tax_Amount;
                    //Console.WriteLine($"Modified: {item.Modified}, EXERT_ID_COL: {item.EXERT_ID_COL} Invoiceno: {existing.Invoiceno}, Invoice_Date: {existing.Invoice_Date}, Supplier_GSTIN: {existing.Supplier_GSTIN} , Modified DateTime :{existing.Modified_DateTime}"); // Debug output

                }
                else
                {
                    row["Invoiceno"] = item.Invoiceno;
                    row["Invoice_Date"] = item.Invoice_Date;
                    row["Supplier_GSTIN"] = item.Supplier_GSTIN;
                    row["TaxAmount"] = item.CESS + item.CGST + item.SGST + item.Integrated_Tax;
                    //Console.WriteLine($"Modified: {item.Modified}, EXERT_ID_COL: {item.EXERT_ID_COL} Invoiceno: {item.Invoiceno}, Invoice_Date: {item.Invoice_Date}, Supplier_GSTIN: {item.Supplier_GSTIN}"); // Debug output


                }

                dataTable.Rows.Add(row);
            }

            return dataTable;

        }

        public async Task<DataTable> GetModifiedPortalDataBasedOnTicketAsync(string requestNo, string ClientGSTIN)
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

            dataTable.Columns.Add("TaxAmount");
            dataTable.Columns.Add("Modified");

            var gstr2Data = await _context.GSTR2_Data
               .Where(x => x.Request_No == requestNo && x.Client_GSTIN == ClientGSTIN)
               .ToListAsync();
            foreach (var item in gstr2Data)
            {
                var row = dataTable.NewRow();
                row["EXERT_ID_COL"] = item.EXERT_ID_COL;
                row["Client_GSTIN"] = item.Client_GSTIN;
                row["GST_Reg_Type"] = item.GST_Reg_Type;

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

                row["Modified"] = item.Modified;

                if (!string.IsNullOrWhiteSpace(item.Modified) && item.Modified.Trim() == "Yes")
                {
                    var existing = await _context.ModifiedDatas
                              .Where(x => x.EXERT_ID_COL == item.EXERT_ID_COL.ToString())
                              .OrderByDescending(x => x.Modified_DateTime)
                              .FirstOrDefaultAsync();

                    row["Supplier_Invoice_No"] = existing.Invoiceno;
                    row["Supplier_Invoice_Date"] = existing.Invoice_Date;
                    row["Supplier_GSTIN"] = existing.Supplier_GSTIN;
                    row["TaxAmount"] = existing.Tax_Amount;

                }
                else
                {
                    row["Supplier_Invoice_No"] = item.Supplier_Invoice_No;
                    row["Supplier_Invoice_Date"] = item.Supplier_Invoice_Date;
                    row["Supplier_GSTIN"] = item.Supplier_GSTIN;
                    row["TaxAmount"] = item.IGST_Value + item.CGST_Value + item.SGST_Value + item.CESS;
                }

                dataTable.Rows.Add(row);
            }
            return dataTable;


        }

    }
}
