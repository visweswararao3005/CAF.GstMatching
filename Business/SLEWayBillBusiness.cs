using CAF.GstMatching.Business.Interface;
using CAF.GstMatching.Helper;
using CAF.GstMatching.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAF.GstMatching.Business
{
    public class SLEWayBillBusiness : ISLEWayBillBusiness
    {
        private readonly SLEWayBillHelper _sLEWayBillHelper;
        public SLEWayBillBusiness(SLEWayBillHelper sLEWayBillHelper)
        {
            _sLEWayBillHelper = sLEWayBillHelper;
        }

        public async Task SaveEWayBillDataAsync(DataTable dataTable, string requestNo, string gstin)
        {
            await _sLEWayBillHelper.SaveEWayBillDataAsync(dataTable, requestNo, gstin);
        }
        public async Task<List<SalesLedgerEWayBillsDataModel>> GetSLEWayBillData(string requestNo, string Clientgstin)
        {
            return await _sLEWayBillHelper.GetSLEWayBillData(requestNo, Clientgstin);
        }
        public async Task<DataTable> GetUserDataBasedOnTicketAsync(string requestNo, string Clientgstin)
        {
            return await _sLEWayBillHelper.GetUserDataBasedOnTicketAsync(requestNo, Clientgstin);
        }
        public async Task saveEwayBillAPIsData(APIsDataModel data)
        {
			await _sLEWayBillHelper.saveEwayBillAPIsData(data);
        }
     
       
    }
}
