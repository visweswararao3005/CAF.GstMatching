using CAF.GstMatching.Models.CompareGst;
using CAF.GstMatching.Models.PurchaseTicketModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAF.GstMatching.Business.Interface
{
    public interface ICompareGstBusiness
    {
        Task SaveCompareDataAsync(DataTable purchaseData, DataTable gstr2Data);
        //Task<DataTable> GetComparedDataBasedONTicketAsync(string ticket);
        Task<List<CompareGstResultModel>> GetComparedDataBasedOnTicketAsync(string Request , string ClientGSTIN);
        Task<DataTable> GetUnMatchedData(string requestno, string ClientGSTIN);
        Task<DataTable> GetPortalUnMatchedData(string requestno, string ClientGSTIN);

        Task<DashboardDataModel> GetDashboardDataAsync(string gstin, string period);

        Task<Dictionary<string, decimal>> GetDashboardBARDataAsync(string fromMonth, string toMonth, string gstin, string type);
    }
}
