using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using CAF.GstMatching.Business.Interface;
using CAF.GstMatching.Helper;
using CAF.GstMatching.Models;
using CAF.GstMatching.Models.GSTR2DataModel;

namespace CAF.GstMatching.Business
{
    public class GSTR2DataBusiness : IGSTR2DataBusiness
    {
        private readonly GSTR2DataHelper _gstr2DataHelper;
        public GSTR2DataBusiness(GSTR2DataHelper gstr2DataHelper)
        {
            // Constructor logic here
            _gstr2DataHelper = gstr2DataHelper;
        }

        // Define methods for GSTR2DataBusiness here
        public async Task SaveGSTR2DataAsync(DataTable dataTable, string requestNo , string clientgstin)
        {
            await _gstr2DataHelper.SaveGSTR2DataAsync(dataTable, requestNo , clientgstin);
        }

        public async Task<DataTable> GetPortalDataBasedOnTicketAsync(string ticket , string gstin)
        {
            return await _gstr2DataHelper.GetPortalDataBasedOnTicketAsync(ticket , gstin);          
        }
        public async Task<List<GetPortalModel>> GetPortalData(string ticket, string gstin)
        {
           return await _gstr2DataHelper.GetPortalData(ticket , gstin);         
        }
        public async Task savePortalAPIsData(APIsDataModel data)
        {
            await _gstr2DataHelper.savePortalAPIsData(data);
        }
        public async Task saveTokenData(GSTR2TokenDataModel data)
        {
            await _gstr2DataHelper.saveTokenData(data);
        }
        public async Task updateTokenData(GSTR2TokenDataModel data)
        {
            await _gstr2DataHelper.updateTokenData(data);
        }
        public async Task<GSTR2TokenDataModel> GetTokenDataAsync(string clientGstin)
        {
            return await _gstr2DataHelper.GetTokenDataAsync(clientGstin);
        }
    }
}

            
