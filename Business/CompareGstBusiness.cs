using CAF.GstMatching.Business.Interface;
using CAF.GstMatching.Helper;
using CAF.GstMatching.Models.CompareGst;
using CAF.GstMatching.Models.PurchaseTicketModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAF.GstMatching.Business
{
    public class CompareGstBusiness : ICompareGstBusiness
    {
        private readonly CompareGstHelper _compareGstHelper;
        public CompareGstBusiness(CompareGstHelper compareGstHelper)
        {
            _compareGstHelper = compareGstHelper;
        }
        public async Task SaveCompareDataAsync(DataTable purchaseData, DataTable gstr2Data)
        {
            await _compareGstHelper.SaveCompareDataAsync(purchaseData, gstr2Data);
        }

        //public async Task<DataTable> GetComparedDataBasedONTicketAsync(string ticket)
        //{
        //    return await _compareGstHelper.GetComparedDataBasedONTicketAsync(ticket);          
        //}
       
        public async Task<List<CompareGstResultModel>> GetComparedDataBasedOnTicketAsync(string Request , string ClientGSTIN)
        {
            return await _compareGstHelper.GetComparedDataBasedOnTicketAsync(Request , ClientGSTIN);
        }
        public async Task<DataTable> GetUnMatchedData(string requestno, string ClientGSTIN)
        {
            return await _compareGstHelper.GetUnMatchedData(requestno, ClientGSTIN);
        }
        public async Task<DataTable> GetPortalUnMatchedData(string requestno, string ClientGSTIN)
        {
            return await _compareGstHelper.GetPortalUnMatchedData(requestno, ClientGSTIN);
        }


        public async Task<DashboardDataModel> GetDashboardDataAsync(string gstin, string period)
        {
            return await _compareGstHelper.GetDashboardDataAsync(gstin, period);
        }
        public async Task<Dictionary<string, decimal>> GetDashboardBARDataAsync(string fromMonth, string toMonth, string gstin, string type)
        {
            return await _compareGstHelper.GetDashboardBARDataAsync(fromMonth, toMonth, gstin, type);
        }
    }
}
