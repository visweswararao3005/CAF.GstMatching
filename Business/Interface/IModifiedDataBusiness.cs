using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAF.GstMatching.Business.Interface
{
    public interface IModifiedDataBusiness
    {
        Task SaveModifiedData(DataTable Data, string RequestNo, string ClientGSTIN);
        Task<DataTable> GetModifiedPortalDataBasedOnTicketAsync(string requestNo, string ClientGSTIN);
        Task<DataTable> GetModifiedInvoiceDataBasedOnTicketAsync(string requestNo, string ClientGSTIN);
    }
}
