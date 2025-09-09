using CAF.GstMatching.Business.Interface;
using CAF.GstMatching.Helper;
using CAF.GstMatching.Models.PurchaseTicketModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAF.GstMatching.Business
{
    public class PurchaseTicketBusiness : IPurchaseTicketBusiness
    {
        private readonly PurchaseTicketHelper _purchaseTicketHelper;
        
        public PurchaseTicketBusiness(PurchaseTicketHelper purchaseTicketHelper)
        {
            _purchaseTicketHelper = purchaseTicketHelper;
            
        }
        public async Task SavePurchaseTicketAsync(TicketsStatusModel data)
        {
            await _purchaseTicketHelper.SavePurchaseTicketAsync(data);
        }

        public async Task UpdatePurchaseTicketAsync(string requestNo , string ClientGSTIN) //update status in request
        {
            await _purchaseTicketHelper.UpdatePurchaseTicketAsync(requestNo, ClientGSTIN);
        }
        public async Task UpdatePurchaseTicketAsync(string requestNo, string ClientGSTIN, string AdminFileName) //update status in request for admin
        {
            await _purchaseTicketHelper.UpdatePurchaseTicketAsync(requestNo, ClientGSTIN, AdminFileName);
        }


        public async Task<TicketsStatusModel> GetUserDataBasedOnTicketAsync(string requestNo, string ClientGSTIN)// For Admin
        {
            return await _purchaseTicketHelper.GetUserDataBasedOnTicketAsync(requestNo, ClientGSTIN);
        }


        public async Task<List<TicketsStatusModel>> GetOpenTicketsStatusAsync(string clientGSTIN, DateTime fromDate, DateTime toDate) //for vendor
        {
            return await _purchaseTicketHelper.GetOpenTicketsStatusAsync(clientGSTIN,fromDate,toDate);
        }
        public async Task<List<TicketsStatusModel>> GetCloseTicketsStatusAsync(string clientGSTIN, DateTime fromDate, DateTime toDate) //for vendor
        {
            return await _purchaseTicketHelper.GetCloseTicketsStatusAsync(clientGSTIN, fromDate, toDate);
        }

        public async Task<List<TicketsStatusModel>> GetAllOpenTicketsStatusAsync(DateTime fromDate, DateTime toDate) //for Admin
        {
            return await _purchaseTicketHelper.GetAllOpenTicketsStatusAsync(fromDate, toDate);
        }
        public async Task<List<TicketsStatusModel>> GetAllCloseTicketsStatusAsync(DateTime fromDate, DateTime toDate) //for Admin
        {
            return await _purchaseTicketHelper.GetAllCloseTicketsStatusAsync(fromDate, toDate);
        }


        public async Task<List<TicketsStatusModel>> GetClientsOpenTicketsStatusAsync(string[] clients, DateTime fromDate, DateTime toDate) // Only for his clients
        {
			return await _purchaseTicketHelper.GetClientsOpenTicketsStatusAsync(clients, fromDate, toDate);
		}
        public async Task<List<TicketsStatusModel>> GetClientsClosedTicketsStatusAsync(string[] clients, DateTime fromDate, DateTime toDate) // Only for his clients
        {
			return await _purchaseTicketHelper.GetClientsClosedTicketsStatusAsync(clients, fromDate, toDate);
		}



    }
    
}
