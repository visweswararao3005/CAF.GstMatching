using CAF.GstMatching.Business.Interface;
using CAF.GstMatching.Helper;
using CAF.GstMatching.Models;
using CAF.GstMatching.Models.PurchaseTicketModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAF.GstMatching.Business
{
    public class SLTicketsBusiness : ISLTicketsBusiness
    {
        private readonly SLTicketsHelper _sLTicketsHelper;
        public SLTicketsBusiness(SLTicketsHelper sLTicketsHelper)
        {
            _sLTicketsHelper = sLTicketsHelper;
        }

        public async Task SaveSLTicketAsync(SalesLedgerTicketsModel data)
        {
            await _sLTicketsHelper.SaveSLTicketAsync(data);
        }

        public async Task<SalesLedgerTicketsModel> GetUserDataBasedOnTicketAsync(string requestNo, string ClientGSTIN)
        {
            return await _sLTicketsHelper.GetUserDataBasedOnTicketAsync(requestNo, ClientGSTIN);
        }

        public async Task<List<SalesLedgerTicketsModel>> GetOpenTicketsStatusAsync(string ClientGSTIN, DateTime fromDate, DateTime toDate)
        {
			return await _sLTicketsHelper.GetOpenTicketsStatusAsync(ClientGSTIN, fromDate, toDate);
		}
        public async Task<List<SalesLedgerTicketsModel>> GetCloseTicketsStatusAsync(string ClientGSTIN, DateTime fromDate, DateTime toDate)
        {
			return await _sLTicketsHelper.GetCloseTicketsStatusAsync(ClientGSTIN, fromDate, toDate);
		}

		public async Task<List<SalesLedgerTicketsModel>> GetClientsOpenTicketsStatusAsync(string[] clients, DateTime fromDate, DateTime toDate)
        {
			return await _sLTicketsHelper.GetClientsOpenTicketsStatusAsync(clients, fromDate, toDate);
		}

		public async Task<List<SalesLedgerTicketsModel>> GetClientsClosedTicketsStatusAsync(string[] clients, DateTime fromDate, DateTime toDate)
        {
			return await _sLTicketsHelper.GetClientsClosedTicketsStatusAsync(clients, fromDate, toDate);
		}


        public async Task UpdateSLTicketAsync(string ticketNo, string ClientGSTIN, string EInvoice_AdminFileName, string EWayBill_AdminFileName)
        {
            await _sLTicketsHelper.UpdateSLTicketAsync(ticketNo, ClientGSTIN, EInvoice_AdminFileName, EWayBill_AdminFileName);
        }

        public async Task CloseSLTicketAsync(string ticketNo, string ClientGSTIN)
        {
            await _sLTicketsHelper.CloseSLTicketAsync(ticketNo, ClientGSTIN);
        }


    }
}
