using CAF.GstMatching.Models;
using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAF.GstMatching.Business.Interface
{
    public interface ISLDataBusiness
    {
        Task SaveSalesLedgerDataAsync(DataTable data, string requestNo ,string gstin);
        Task<List<SalesLedgerDataModel>> GetSLInvoiceData(string requestNo, string Clientgstin);
        Task<DataTable> GetUserDataBasedOnTicketAsync(string requestNo, string Clientgstin);
        Task<List<string>> GetOtherDates(string ticketNo, string ClientGSTIN, string period, string formate);
        Task<List<string>> GetIRNData(string requestNo, string Clientgstin);
	}
}
