using CAF.GstMatching.Business.Interface;
using CAF.GstMatching.Helper;
using CAF.GstMatching.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAF.GstMatching.Business
{
    public class SLDataBusiness : ISLDataBusiness
    {
        private readonly SLDataHelper _sLDataHelper;
        public SLDataBusiness(SLDataHelper sLDataHelper)
        {
            _sLDataHelper = sLDataHelper;
        }

        public async Task SaveSalesLedgerDataAsync(DataTable dataTable, string requestNo, string gstin)
        {
            await _sLDataHelper.SaveSalesLedgerDataAsync(dataTable, requestNo,gstin);
        }

        public async Task<List<SalesLedgerDataModel>> GetSLInvoiceData(string requestNo, string Clientgstin)
        {
            return await _sLDataHelper.GetSLInvoiceData(requestNo, Clientgstin);
        }
        public async Task<DataTable> GetUserDataBasedOnTicketAsync(string requestNo, string Clientgstin)
        {
            return await _sLDataHelper.GetUserDataBasedOnTicketAsync(requestNo, Clientgstin);
        }
        public async Task<List<string>> GetOtherDates(string ticketNo, string ClientGSTIN, string period, string formate)
        {
			return await _sLDataHelper.GetOtherDates(ticketNo, ClientGSTIN, period, formate);
        }
        public async Task<List<string>> GetIRNData(string requestNo, string Clientgstin)
        {
			return await _sLDataHelper.GetIRNData(requestNo, Clientgstin);
        }

    }
}
