using CAF.GstMatching.Data;
using CAF.GstMatching.Models;
using CAF.GstMatching.Models.GSTR2DataModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAF.GstMatching.Business.Interface
{
    public interface IGSTR2DataBusiness
    {
        // Define methods for GSTR2DataBusiness here
        Task SaveGSTR2DataAsync(DataTable dataTable, string ticket, string clientgstin);
        Task<DataTable> GetPortalDataBasedOnTicketAsync(string ticket , string gstin);
        Task<List<GetPortalModel>> GetPortalData(string ticket, string gstin);
        Task savePortalAPIsData(APIsDataModel data);
        Task saveTokenData(GSTR2TokenDataModel data);
        Task updateTokenData(GSTR2TokenDataModel data);
        Task<GSTR2TokenDataModel> GetTokenDataAsync(string clientGstin);

    }
}
