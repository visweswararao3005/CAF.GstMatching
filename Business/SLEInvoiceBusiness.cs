using CAF.GstMatching.Business.Interface;
using CAF.GstMatching.Models;
using CAF.GstMatching.Helper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAF.GstMatching.Business
{
    public class SLEInvoiceBusiness : ISLEInvoiceBusiness
    {
        private readonly SLEInvoiceHelper _sLEInvoiceHelper;
        public SLEInvoiceBusiness(SLEInvoiceHelper sLEInvoiceHelper)
        {
            _sLEInvoiceHelper = sLEInvoiceHelper;
        }

        public async Task SaveEInvoiceDataAsync(DataTable dataTable, string requestNo, string gstin)
        {
            await _sLEInvoiceHelper.SaveEInvoiceDataAsync(dataTable, requestNo, gstin);
        }

        public async Task<List<SalesLedgerEInvoicesDataModel>> GetSLEInvoiceData(string requestNo, string Clientgstin)
        {
            return await _sLEInvoiceHelper.GetSLEInvoiceData(requestNo, Clientgstin);
        }
        public async Task<DataTable> GetUserDataBasedOnTicketAsync(string requestNo, string Clientgstin)
        {
            return await _sLEInvoiceHelper.GetUserDataBasedOnTicketAsync(requestNo, Clientgstin);
        }
        public async Task saveEInvoiceAPIsData(APIsDataModel data)
        {
            await _sLEInvoiceHelper.saveEInvoiceAPIsData(data);
        }
        public async Task saveEinvoiceTokenData(SalesLedgerTokenDataModel data)
        {
            await _sLEInvoiceHelper.saveEinvoiceTokenData(data);
        }
        public async Task updateEinvoiceTokenData(SalesLedgerTokenDataModel data)
        {
            await _sLEInvoiceHelper.updateEinvoiceTokenData(data);
        }
        public async Task<SalesLedgerTokenDataModel> GetEinvoiceTokenDataAsync(string requestNumber, string clientGstin)
        {
            return await _sLEInvoiceHelper.GetEinvoiceTokenDataAsync(requestNumber, clientGstin);
        }
    }
}
