using CAF.GstMatching.Business.Interface;
using CAF.GstMatching.Helper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAF.GstMatching.Business
{
    public class ModifiedDataBusiness : IModifiedDataBusiness
    {
        private readonly ModifiedDataHelper _modifiedDataHelper;

        public ModifiedDataBusiness(ModifiedDataHelper modifiedDataHelper)
        {
            _modifiedDataHelper = modifiedDataHelper;
        }

        public async Task SaveModifiedData(DataTable Data, string RequestNo, string ClientGSTIN)
        {
            await _modifiedDataHelper.SaveModifiedData(Data, RequestNo, ClientGSTIN);
        }

        public async Task<DataTable> GetModifiedPortalDataBasedOnTicketAsync(string requestNo, string ClientGSTIN)
        {
            return await _modifiedDataHelper.GetModifiedPortalDataBasedOnTicketAsync(requestNo, ClientGSTIN);

        }
        public async Task<DataTable> GetModifiedInvoiceDataBasedOnTicketAsync(string requestNo, string ClientGSTIN)
        {
            return await _modifiedDataHelper.GetModifiedInvoiceDataBasedOnTicketAsync(requestNo, ClientGSTIN);
        }
    }
}
