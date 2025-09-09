using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CAF.GstMatching.Business.Interface;
using CAF.GstMatching.Helper;
using CAF.GstMatching.Models.PurchaseDataModel;

namespace CAF.GstMatching.Business
{
    public class PurchaseDataBusiness : IPurchaseDataBusiness
    {

        private readonly PurchaseDataHelper _purchaseDataHelper;
        public PurchaseDataBusiness(PurchaseDataHelper purchaseDataHelper)
        {
            // Constructor logic here
            _purchaseDataHelper = purchaseDataHelper;
        }
    

        // Define the methods for purchase data business logic
        public async Task SavePurchaseDataAsync(DataTable dataTable, string requestNo , string clientgstin)
        {
            await _purchaseDataHelper.SavePurchaseDataAsync(dataTable, requestNo, clientgstin);
        }
     
        public async Task<DataTable> GetUserDataBasedOnTicketAsync(string requestNo, string gstin)
        {
            return await _purchaseDataHelper.GetUserDataBasedOnTicketAsync(requestNo, gstin);
        }
        public async Task<List<GetInvoiceModel>> GetInvoiceData(string requestNo , string gstin)
        {
            return await _purchaseDataHelper.GetInvoiceData(requestNo, gstin);
        }
    }
}
