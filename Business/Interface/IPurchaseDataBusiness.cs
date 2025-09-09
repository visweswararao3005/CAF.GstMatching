using CAF.GstMatching.Models.PurchaseDataModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAF.GstMatching.Business.Interface
{
    public interface IPurchaseDataBusiness
    {
        //Define the methods for purchase data business logic
        Task SavePurchaseDataAsync(DataTable dataTable, string RequestNo, string clientgstin);
 
        Task<DataTable> GetUserDataBasedOnTicketAsync(string RequestNo , string gstin) ;

        Task<List<GetInvoiceModel>> GetInvoiceData(string requestNo, string gstin);
    }
}
