using CAF.GstMatching.Models.PurchaseTicketModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAF.GstMatching.Business.Interface
{
    public interface IPurchaseTicketBusiness
    {
        //Define the methods for purchase ticket business logic
        Task SavePurchaseTicketAsync(TicketsStatusModel data);

        Task UpdatePurchaseTicketAsync(string requestNo , string ClientGSTIN); //update status in request
        Task UpdatePurchaseTicketAsync(string requestNo, string ClientGSTIN, string AdminFileName); //update status in request for admin


        Task<TicketsStatusModel> GetUserDataBasedOnTicketAsync(string requestNo , string ClientGSTIN); //Admin

        //SavePurchaseTicketAsync
        Task<List<TicketsStatusModel>> GetOpenTicketsStatusAsync(string clientGSTIN, DateTime fromDate, DateTime toDate); //for vendor
        Task<List<TicketsStatusModel>> GetCloseTicketsStatusAsync(string clientGSTIN, DateTime fromDate, DateTime toDate); //for vendor
        

        Task<List<TicketsStatusModel>> GetAllOpenTicketsStatusAsync(DateTime fromDate, DateTime toDate); //Admin
        Task<List<TicketsStatusModel>> GetAllCloseTicketsStatusAsync(DateTime fromDate, DateTime toDate); //Admin

        Task<List<TicketsStatusModel>> GetClientsOpenTicketsStatusAsync(string[] clients, DateTime fromDate, DateTime toDate);// Only for his clients
		Task<List<TicketsStatusModel>> GetClientsClosedTicketsStatusAsync(string[] clients, DateTime fromDate, DateTime toDate); // Only for his clients


		

    }


}
