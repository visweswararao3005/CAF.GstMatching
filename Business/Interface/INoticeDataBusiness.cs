using CAF.GstMatching.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAF.GstMatching.Business.Interface
{
	public interface INoticeDataBusiness
	{
		Task saveNotices(NoticeDataModel Data);
		Task UpdateNoticeStatus(string requestNo, string gstin);
		Task CloseNotice(string requestNo, string gstin, string ClosedBy);
		Task<NoticeDataModel> GetNoticeDetails(string requestNo, string gstin);
		Task DeleteFileNameInNotice(string requestNo, string gstin);


        Task<List<NoticeDataModel>> GetActiveNotices(string gstin, DateTime fromDateTime, DateTime toDateTime, string statusValue, string priorityValue);
		Task<List<NoticeDataModel>> GetClosedNotices(string gstin, DateTime fromDateTime, DateTime toDateTime, string priorityValue);
		Task<List<NoticeDataModel>> GetUnreadNotices(string gstin, string source);


        Task<List<NoticeDataModel>> GetClientsActiveNotices(string[] gstin, DateTime fromDateTime, DateTime toDateTime, string statusValue, string priorityValue);
		Task<List<NoticeDataModel>> GetClientsClosedNotices(string[] gstin, DateTime fromDateTime, DateTime toDateTime, string priorityValue);
	    Task<List<NoticeDataModel>> GetClientsUnreadNotices(string[] gstin, string source);



        Task savaChatData(NoticeChatModel Data);
		Task<List<NoticeChatModel>> GetChatData(string requestNo, string gstin);
		Task MarkAdminMessagesAsRead(string requestNo, string gstin);
		Task MarkClientMessagesAsRead(string requestNo, string gstin);
        Task MarkMainAdminMessagesAsRead(string requestNo, string gstin);

    }
}
