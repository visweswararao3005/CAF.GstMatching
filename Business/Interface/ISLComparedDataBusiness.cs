using CAF.GstMatching.Models;
using CAF.GstMatching.Models.CompareGst;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAF.GstMatching.Business.Interface
{
    public interface ISLComparedDataBusiness
    {
        Task CompareDataAsync(DataTable Invoice, DataTable EWayBill, DataTable EInvoice);

        Task<List<SLComparedDataModel>> GetComparedDataBasedOnTicketAsync(string requestno, string ClientGSTIN);
        Task<List<SLComparedDataModel>> GetComparedData_API(string requestno, string ClientGSTIN, List<string> Category, List<string> otherdays, string formate);

		Task<DashboardDataModel> GetDashboardDataAsync(string gstin, string period);
		Task<Dictionary<string, decimal>> GetDashboardBARDataAsync(string fromMonth, string toMonth, string gstin, string type);
	}
}
