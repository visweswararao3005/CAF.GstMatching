using CAF.GstMatching.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAF.GstMatching.Business.Interface
{
    public interface ISLEInvoiceBusiness
    {
        Task SaveEInvoiceDataAsync(DataTable dataTable, string requestNo, string gstin);
        Task<List<SalesLedgerEInvoicesDataModel>> GetSLEInvoiceData(string requestNo, string Clientgstin);
        Task<DataTable> GetUserDataBasedOnTicketAsync(string requestNo, string Clientgstin);
        Task saveEInvoiceAPIsData(APIsDataModel data);
        Task saveEinvoiceTokenData(SalesLedgerTokenDataModel data);
        Task updateEinvoiceTokenData(SalesLedgerTokenDataModel data);
        Task<SalesLedgerTokenDataModel> GetEinvoiceTokenDataAsync(string requestNumber, string clientGstin);
    }
}
