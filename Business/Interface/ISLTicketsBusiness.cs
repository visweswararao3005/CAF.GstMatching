using CAF.GstMatching.Models;
using CAF.GstMatching.Models.PurchaseTicketModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAF.GstMatching.Business.Interface
{
    public interface ISLTicketsBusiness
    {
        Task SaveSLTicketAsync(SalesLedgerTicketsModel data);

        Task<SalesLedgerTicketsModel> GetUserDataBasedOnTicketAsync(string requestNo, string ClientGSTIN);

		Task<List<SalesLedgerTicketsModel>> GetOpenTicketsStatusAsync(string ClientGSTIN, DateTime fromDate, DateTime toDate);
        Task<List<SalesLedgerTicketsModel>> GetCloseTicketsStatusAsync(string ClientGSTIN, DateTime fromDate, DateTime toDate);

        Task<List<SalesLedgerTicketsModel>> GetClientsOpenTicketsStatusAsync(string[] clients, DateTime fromDate, DateTime toDate);
		Task<List<SalesLedgerTicketsModel>> GetClientsClosedTicketsStatusAsync(string[] clients, DateTime fromDate, DateTime toDate);


        Task UpdateSLTicketAsync(string ticketNo, string ClientGSTIN, string EInvoice_AdminFileName, string EWayBill_AdminFileName);
        Task CloseSLTicketAsync(string ticketNo, string ClientGSTIN);
    }
}
