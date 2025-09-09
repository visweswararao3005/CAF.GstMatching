using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CAF.GstMatching.Business.Interface;
using CAF.GstMatching.Helper;
using CAF.GstMatching.Models;

namespace CAF.GstMatching.Business
{
	public class NoticeDataBusiness : INoticeDataBusiness
	{
		private readonly NoticeDataHelper _noticeDataHelper;
		public NoticeDataBusiness(NoticeDataHelper noticeDataHelper)
		{
			_noticeDataHelper = noticeDataHelper ;
		}

		public async Task saveNotices(NoticeDataModel Data)
		{
			await _noticeDataHelper.saveNotices(Data);
		}
		public async Task UpdateNoticeStatus(string requestNo, string gstin)
		{
			await _noticeDataHelper.UpdateNoticeStatus(requestNo, gstin);
		}
		public async Task CloseNotice(string requestNo, string gstin, string ClosedBy)
		{
			await _noticeDataHelper.CloseNotice(requestNo, gstin, ClosedBy);
		}
		public async Task<NoticeDataModel> GetNoticeDetails(string requestNo, string gstin)
		{
			return await _noticeDataHelper.GetNoticeDetails(requestNo, gstin);
		}
		public async Task DeleteFileNameInNotice(string requestNo, string gstin)
		{
		    await _noticeDataHelper.DeleteFileNameInNotice(requestNo, gstin);
		}



        public async Task<List<NoticeDataModel>> GetActiveNotices(string gstin, DateTime fromDateTime, DateTime toDateTime, string statusValue, string priorityValue)
		{
			return await _noticeDataHelper.GetActiveNotices(gstin, fromDateTime, toDateTime, statusValue, priorityValue);
		}
		public async Task<List<NoticeDataModel>> GetClosedNotices(string gstin, DateTime fromDateTime, DateTime toDateTime, string priorityValue)
		{
			return await _noticeDataHelper.GetClosedNotices(gstin, fromDateTime, toDateTime, priorityValue);
		}
		public async Task<List<NoticeDataModel>> GetUnreadNotices(string gstin, string source)
		{
            return await _noticeDataHelper.GetUnreadNotices(gstin, source);
        }


		public async Task<List<NoticeDataModel>> GetClientsActiveNotices(string[] gstin, DateTime fromDateTime, DateTime toDateTime, string statusValue, string priorityValue)
		{
			return await _noticeDataHelper.GetClientsActiveNotices(gstin, fromDateTime, toDateTime, statusValue, priorityValue);
		}
		public async Task<List<NoticeDataModel>> GetClientsClosedNotices(string[] gstin, DateTime fromDateTime, DateTime toDateTime, string priorityValue)
		{
			return await _noticeDataHelper.GetClientsClosedNotices(gstin, fromDateTime, toDateTime, priorityValue);
		}
		public async Task<List<NoticeDataModel>> GetClientsUnreadNotices(string[] gstin , string source)
		{
            return await _noticeDataHelper.GetClientsUnreadNotices(gstin, source);
        }


		public async Task savaChatData(NoticeChatModel Data)
		{
			await _noticeDataHelper.savaChatData(Data);
		}
		public async Task<List<NoticeChatModel>> GetChatData(string requestNo, string gstin)
		{
			return await _noticeDataHelper.GetChatData(requestNo, gstin);
		}
		public async Task MarkAdminMessagesAsRead(string requestNo, string gstin)
		{
            await _noticeDataHelper.MarkAdminMessagesAsRead(requestNo, gstin);
        }
		public async Task MarkClientMessagesAsRead(string requestNo, string gstin)
		{
            await _noticeDataHelper.MarkClientMessagesAsRead(requestNo, gstin);
		}
        public async Task MarkMainAdminMessagesAsRead(string requestNo, string gstin)
        {
            await _noticeDataHelper.MarkMainAdminMessagesAsRead(requestNo, gstin);
        }

    }
}
