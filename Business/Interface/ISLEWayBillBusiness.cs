using CAF.GstMatching.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAF.GstMatching.Business.Interface
{
    public interface ISLEWayBillBusiness
    {
        Task SaveEWayBillDataAsync(DataTable dataTable, string requestNo, string gstin);
        Task<List<SalesLedgerEWayBillsDataModel>> GetSLEWayBillData(string requestNo, string Clientgstin);
        Task<DataTable> GetUserDataBasedOnTicketAsync(string requestNo, string Clientgstin);

        Task saveEwayBillAPIsData(APIsDataModel data);
      
	}
}
