using CAF.GstMatching.Business.Interface;
using CAF.GstMatching.Helper;
using CAF.GstMatching.Models;
using CAF.GstMatching.Models.CompareGst;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAF.GstMatching.Business
{
    public class SLComparedDataBusiness : ISLComparedDataBusiness
    {
        private readonly SLComparedDataHelper _sLComparedDataHelper;
        public SLComparedDataBusiness(SLComparedDataHelper sLComparedDataHelper)
        {
            _sLComparedDataHelper = sLComparedDataHelper;
        }

        public async Task CompareDataAsync(DataTable Invoice, DataTable EWayBill, DataTable EInvoice)
        {
           await _sLComparedDataHelper.CompareDataAsync(Invoice, EWayBill, EInvoice);
        }

        public async Task<List<SLComparedDataModel>> GetComparedDataBasedOnTicketAsync(string requestno, string ClientGSTIN)
        {
            return await _sLComparedDataHelper.GetComparedDataBasedOnTicketAsync(requestno, ClientGSTIN);
        }

        public async Task<List<SLComparedDataModel>> GetComparedData_API(string requestno, string ClientGSTIN, List<string> Category, List<string> otherdays, string formate)
        {
            return await _sLComparedDataHelper.GetComparedData_API(requestno, ClientGSTIN, Category, otherdays, formate);
        }

		public async Task<DashboardDataModel> GetDashboardDataAsync(string gstin, string period)
		{
			return await _sLComparedDataHelper.GetDashboardDataAsync(gstin, period);
		}
		public async Task<Dictionary<string, decimal>> GetDashboardBARDataAsync(string fromMonth, string toMonth, string gstin, string type)
		{
			return await _sLComparedDataHelper.GetDashboardBARDataAsync(fromMonth, toMonth, gstin, type);
		}
	}
}
