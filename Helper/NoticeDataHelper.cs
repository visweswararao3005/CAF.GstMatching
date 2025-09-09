using CAF.GstMatching.Data;
using CAF.GstMatching.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAF.GstMatching.Helper
{
	public class NoticeDataHelper
	{
		private readonly ApplicationDbContext _context;
		public NoticeDataHelper(ApplicationDbContext context)
		{
			_context = context;
		}
		
		public async Task saveNotices(NoticeDataModel Data)
		{
            // First, check if records with the given ticket exist
            var existingData = _context.NoticeDatas.Where(x => x.Request_Number == Data.RequestNumber).ToList();

            // If they exist, remove them
            if (existingData.Any())
            {
                _context.NoticeDatas.RemoveRange(existingData);
                await _context.SaveChangesAsync(); // Save after deletion before inserting new records
            }

            var data = new NoticeData
			{
				//UniqueId = Data.UniqueId,
				Request_Number = Data.RequestNumber,
				Client_Gstin = Data.ClientGstin,
				Client_Name = Data.ClientName,
				Created_Datetime = Data.CreatedDatetime,
				UpdatedDateTime = Data.UpdatedDateTime,
				Notice_Title = Data.NoticeTitle,
				Notice_Datetime = Data.NoticeDatetime,
				Priority = Data.Priority,
				Notice_Description = Data.NoticeDescription,
				status = Data.status,
				FileName = Data.FileName
			};
			_context.NoticeDatas.Add(data);
			await _context.SaveChangesAsync();
		}
		public async Task UpdateNoticeStatus(string requestNo,string gstin)
		{
			var notice = await _context.NoticeDatas
				.FirstOrDefaultAsync(n => n.Request_Number == requestNo && n.Client_Gstin == gstin);
			if (notice != null)
			{
				notice.status = "In Progress";
				await _context.SaveChangesAsync();
			}

		}
		public async Task CloseNotice(string requestNo, string gstin, string ClosedBy)
		{
			var notice = await _context.NoticeDatas
				.FirstOrDefaultAsync(n => n.Request_Number == requestNo && n.Client_Gstin == gstin);
			if (notice != null)
			{
				notice.status = "Closed";
				notice.Closed_Datetime = DateTime.Now; // Set the closed datetime
				notice.ClosedBy = ClosedBy; // Set the closed by field if needed
				await _context.SaveChangesAsync();
			}

		}
		public async Task<NoticeDataModel> GetNoticeDetails(string requestNo, string gstin)
		{
			return await _context.NoticeDatas
				.Where(n => n.Request_Number == requestNo && n.Client_Gstin == gstin)
				.Select(n => new NoticeDataModel
				{
					RequestNumber = n.Request_Number,
					ClientName = n.Client_Name,
					ClientGstin = n.Client_Gstin,
					CreatedDatetime = n.Created_Datetime,
					UpdatedDateTime = n.UpdatedDateTime, 
					NoticeTitle = n.Notice_Title,
					NoticeDatetime = n.Notice_Datetime,
					Priority = n.Priority,
					NoticeDescription = n.Notice_Description,
					status = n.status,
					ClosedDatetime = n.Closed_Datetime,
					ClosedBy = n.ClosedBy,
					FileName = n.FileName
				})
				.FirstOrDefaultAsync();
		}
		public async Task DeleteFileNameInNotice(string requestNo, string gstin)
		{
            var notice = await _context.NoticeDatas
                .FirstOrDefaultAsync(n => n.Request_Number == requestNo && n.Client_Gstin == gstin);
            if (notice != null)
            {
                notice.FileName = null;
                await _context.SaveChangesAsync();
            }
        }


		public async Task<List<NoticeDataModel>> GetActiveNotices(string gstin, DateTime fromDateTime, DateTime toDateTime, string statusValue, string priorityValue)
		{
			DateTime fromDate1 = fromDateTime.Date;
			DateTime toDate1 = toDateTime.Date.AddDays(1).AddTicks(-1);

			var notices =  await _context.NoticeDatas
				.Where(n => n.Client_Gstin == gstin &&
		                    n.Created_Datetime >= fromDate1 &&
					        n.Created_Datetime <= toDate1 &&
						   (statusValue == "All"
												? (n.status == "Open" || n.status == "In Progress")
												: n.status == statusValue) &&
						   (priorityValue == "All" || n.Priority == priorityValue))
				.Select(n => new NoticeDataModel
				 {
					RequestNumber = n.Request_Number,
					ClientGstin = n.Client_Gstin,
					ClientName = n.Client_Name,
					CreatedDatetime = n.Created_Datetime,
					UpdatedDateTime = n.UpdatedDateTime,
					NoticeTitle = n.Notice_Title,
					NoticeDatetime = n.Notice_Datetime,
					Priority = n.Priority,
					NoticeDescription = n.Notice_Description,
					status = n.status,
                    ClosedBy = n.ClosedBy,
                    FileName = n.FileName 
				 })
				.ToListAsync();

            // Fetch unread messages for the gstin
            var unreadMessages = await _context.NoticeChats
                .Where(m => m.Client_Gstin == gstin && m.IsRead == false)
                .ToListAsync();

            foreach (var notice in notices)
            {
                // Check if any unread chat exists for this request
                notice.HasUnreadMessages = unreadMessages.Any(msg => msg.Request_Number == notice.RequestNumber);

                var thisNoticeMessages = unreadMessages
					.Where(m => m.Request_Number == notice.RequestNumber)
					.OrderByDescending(m => m.Message_Datetime)
					.ToList();

                if (thisNoticeMessages.Any())
                {
                    var latestMsg = thisNoticeMessages.First();
                    if (latestMsg.Source == "Admin") // 🧠 For vendor view
                    {
                        notice.IsAdmin = true;
                    }
                }
            }
            return notices;
        }
		public async Task<List<NoticeDataModel>> GetClosedNotices(string gstin, DateTime fromDateTime, DateTime toDateTime, string priorityValue)
		{
			DateTime fromDate1 = fromDateTime.Date;
			DateTime toDate1 = toDateTime.Date.AddDays(1).AddTicks(-1);

			return await _context.NoticeDatas
				.Where(n => n.Client_Gstin == gstin &&
							n.Closed_Datetime >= fromDate1 &&
							n.Closed_Datetime <= toDate1 &&
						    n.status == "Closed" &&
						   (priorityValue == "All" || n.Priority == priorityValue))
				.Select(n => new NoticeDataModel
				{
					RequestNumber = n.Request_Number,
					ClientGstin = n.Client_Gstin,
					ClientName = n.Client_Name,
					CreatedDatetime = n.Created_Datetime,
					UpdatedDateTime = n.UpdatedDateTime, 
					FileName = n.FileName,
					NoticeTitle = n.Notice_Title,
					NoticeDatetime = n.Notice_Datetime,
					Priority = n.Priority,
					NoticeDescription = n.Notice_Description,
					status = n.status,
                    ClosedBy = n.ClosedBy,
                    ClosedDatetime = n.Closed_Datetime 
				})
				.ToListAsync();
		}
        public async Task<List<NoticeDataModel>> GetUnreadNotices(string gstin, string source)
        {
            // Step 1: Get request numbers that have unread messages
            var unreadRequestNumbers = await _context.NoticeChats
                .Where(m => m.Client_Gstin == gstin && m.Source.Trim() == source && m.IsRead == false)
                .Select(m => m.Request_Number)
                .Distinct()
                .ToListAsync();

            if (unreadRequestNumbers == null || unreadRequestNumbers.Count == 0)
                return new List<NoticeDataModel>();

            // Step 2: Get notice data for those request numbers
            var notices = await _context.NoticeDatas
                .Where(n => n.Client_Gstin == gstin && unreadRequestNumbers.Contains(n.Request_Number))
                .Select(n => new NoticeDataModel
                {
                    RequestNumber = n.Request_Number,
					ClientName = n.Client_Name,
                    ClientGstin = n.Client_Gstin,
                    CreatedDatetime = n.Created_Datetime,
					UpdatedDateTime = n.UpdatedDateTime,
					FileName = n.FileName,
                    NoticeTitle = n.Notice_Title,
                    NoticeDatetime = n.Notice_Datetime,
                    Priority = n.Priority,
                    NoticeDescription = n.Notice_Description,
                    status = n.status,
                    ClosedBy = n.ClosedBy,
                    HasUnreadMessages = true // ✅ We already know they are unread
                })
                .ToListAsync();

            // Fetch unread messages for the gstin
            var unreadMessages = await _context.NoticeChats
                .Where(m => m.Client_Gstin == gstin && m.IsRead == false)
                .ToListAsync();

            foreach (var notice in notices)
            {
                // Check if any unread chat exists for this request
                notice.HasUnreadMessages = unreadMessages.Any(msg => msg.Request_Number == notice.RequestNumber);

                var thisNoticeMessages = unreadMessages
                    .Where(m => m.Request_Number == notice.RequestNumber)
                    .OrderByDescending(m => m.Message_Datetime)
                    .ToList();

                if (thisNoticeMessages.Any())
                {
                    var latestMsg = thisNoticeMessages.First();
                    if (latestMsg.Source == "MainAdmin") // 🧠 For vendor view
                    {
                        notice.IsAdmin = true;
                    }
                }
            }

            return notices;
        }


  //      public async Task<List<NoticeDataModel>> GetClientsActiveNotices1(string[] gstin, DateTime fromDateTime, DateTime toDateTime, string statusValue, string priorityValue)
		//{
		//	DateTime fromDate1 = fromDateTime.Date;
		//	DateTime toDate1 = toDateTime.Date.AddDays(1).AddTicks(-1);

		//	var notices = await _context.NoticeDatas
		//		.Where(n => gstin.Contains(n.Client_Gstin) &&    
		//					n.Created_Datetime >= fromDate1 &&
		//					n.Created_Datetime <= toDate1 &&
		//					(statusValue == "All"   ? (n.status == "Open" || n.status == "In Progress")
		//											: n.status == statusValue) &&
		//				   (priorityValue == "All" || n.Priority == priorityValue))
		//		.Select(n => new NoticeDataModel
		//		{
		//			RequestNumber = n.Request_Number,
		//			ClientGstin = n.Client_Gstin,
		//			ClientName = n.Client_Name,
		//			CreatedDatetime = n.Created_Datetime,
		//			UpdatedDateTime = n.UpdatedDateTime,
		//			FileName = n.FileName,
		//			NoticeTitle = n.Notice_Title,
		//			NoticeDatetime = n.Notice_Datetime,
		//			Priority = n.Priority,
		//			NoticeDescription = n.Notice_Description,
  //                  ClosedBy = n.ClosedBy,
  //                  status = n.status
		//		})
		//		.ToListAsync();

  //          // Fetch unread messages for the gstin
  //          var unreadMessages = await _context.NoticeChats
  //              .Where(m => gstin.Contains(m.Client_Gstin) && m.IsRead == false)
  //              .ToListAsync();

  //          foreach (var notice in notices)
  //          {
  //              // Check if any unread chat exists for this request
  //              notice.HasUnreadMessages = unreadMessages.Any(msg => msg.Request_Number == notice.RequestNumber);
		//		//notice.IsAdmin = unreadMessages.Request_Number == notice.RequestNumber && unreadMessages.Source == "Admin" ? true : false;

  //              var thisNoticeMessages = unreadMessages
  //                 .Where(m => m.Request_Number == notice.RequestNumber)
  //                 .OrderByDescending(m => m.Message_Datetime)
  //                 .ToList();

  //              if (thisNoticeMessages.Any())
  //              { 
  //                  var latestMsg = thisNoticeMessages.First();
  //                  if (latestMsg.Source == "Admin") // 🧠 For vendor view
  //                  {
  //                      notice.IsAdmin = true;
  //                  }
  //              }
  //          }

  //          return notices;
  //      }
        
        public async Task<List<NoticeDataModel>> GetClientsActiveNotices(string[] gstin, DateTime fromDateTime, DateTime toDateTime, string statusValue, string priorityValue)
        {
            DateTime fromDate1 = fromDateTime.Date;
            DateTime toDate1 = toDateTime.Date.AddDays(1).AddTicks(-1);

            bool filterByGstin = gstin != null && gstin.Any();

            // Base query for notices
            var noticesQuery = _context.NoticeDatas
                .Where(n =>
                    n.Created_Datetime >= fromDate1 &&
                    n.Created_Datetime <= toDate1 &&
                    (statusValue == "All"
                        ? (n.status == "Open" || n.status == "In Progress")
                        : n.status == statusValue) &&
                    (priorityValue == "All" || n.Priority == priorityValue));

            if (filterByGstin)
            {
                noticesQuery = noticesQuery.Where(n => gstin.Contains(n.Client_Gstin));
            }

            var notices = await noticesQuery
                .Select(n => new NoticeDataModel
                {
                    RequestNumber = n.Request_Number,
                    ClientGstin = n.Client_Gstin,
                    ClientName = n.Client_Name,
                    CreatedDatetime = n.Created_Datetime,
                    UpdatedDateTime = n.UpdatedDateTime,
                    FileName = n.FileName,
                    NoticeTitle = n.Notice_Title,
                    NoticeDatetime = n.Notice_Datetime,
                    Priority = n.Priority,
                    NoticeDescription = n.Notice_Description,
                    ClosedBy = n.ClosedBy,
                    status = n.status
                })
                .ToListAsync();

            // Base query for unread messages
            var unreadMessagesQuery = _context.NoticeChats
                .Where(m => m.IsRead == false); 

            if (filterByGstin)
            {
                unreadMessagesQuery = unreadMessagesQuery.Where(m => gstin.Contains(m.Client_Gstin));
            }

            var unreadMessages = await unreadMessagesQuery.ToListAsync();

            foreach (var notice in notices)
            {
                notice.HasUnreadMessages = unreadMessages.Any(msg => msg.Request_Number == notice.RequestNumber);

                var thisNoticeMessages = unreadMessages
                    .Where(m => m.Request_Number == notice.RequestNumber)
                    .OrderByDescending(m => m.Message_Datetime)
                    .ToList();

                if (thisNoticeMessages.Any() && thisNoticeMessages.First().Source == "Admin")
                {
                    notice.IsAdmin = true;
                }
            }

            return notices;
        }

  //      public async Task<List<NoticeDataModel>> GetClientsClosedNotices1(string[] gstin, DateTime fromDateTime, DateTime toDateTime, string priorityValue)
		//{
		//	DateTime fromDate1 = fromDateTime.Date;
		//	DateTime toDate1 = toDateTime.Date.AddDays(1).AddTicks(-1);

		//	return await _context.NoticeDatas
		//		.Where(n => gstin.Contains(n.Client_Gstin) &&
		//					n.Closed_Datetime >= fromDate1 &&
		//					n.Closed_Datetime <= toDate1 &&
		//					n.status == "Closed" &&
		//				   (priorityValue == "All" || n.Priority == priorityValue))
		//		.Select(n => new NoticeDataModel
		//		{
		//			RequestNumber = n.Request_Number,
		//			ClientName = n.Client_Name,
		//			ClientGstin = n.Client_Gstin,
		//			CreatedDatetime = n.Created_Datetime,
		//			UpdatedDateTime = n.UpdatedDateTime,
		//			FileName = n.FileName,
		//			NoticeTitle = n.Notice_Title,
		//			NoticeDatetime = n.Notice_Datetime,
		//			Priority = n.Priority,
		//			NoticeDescription = n.Notice_Description,
		//			status = n.status,
		//			ClosedBy = n.ClosedBy,
		//			ClosedDatetime = n.Closed_Datetime // Include ClosedDatetime if needed
		//		})
		//		.ToListAsync();
		//}
        public async Task<List<NoticeDataModel>> GetClientsClosedNotices(string[] gstin, DateTime fromDateTime, DateTime toDateTime, string priorityValue)
        {
            DateTime fromDate1 = fromDateTime.Date;
            DateTime toDate1 = toDateTime.Date.AddDays(1).AddTicks(-1);

            bool filterByGstin = gstin != null && gstin.Any();

            // Base query for closed notices
            var closedNoticesQuery = _context.NoticeDatas
                .Where(n =>
                    n.Closed_Datetime >= fromDate1 &&
                    n.Closed_Datetime <= toDate1 &&
                    n.status == "Closed" &&
                    (priorityValue == "All" || n.Priority == priorityValue));

            if (filterByGstin)
            {
                closedNoticesQuery = closedNoticesQuery.Where(n => gstin.Contains(n.Client_Gstin));
            }

            return await closedNoticesQuery
                .Select(n => new NoticeDataModel
                {
                    RequestNumber = n.Request_Number,
                    ClientName = n.Client_Name,
                    ClientGstin = n.Client_Gstin,
                    CreatedDatetime = n.Created_Datetime,
                    UpdatedDateTime = n.UpdatedDateTime,
                    FileName = n.FileName,
                    NoticeTitle = n.Notice_Title,
                    NoticeDatetime = n.Notice_Datetime,
                    Priority = n.Priority,
                    NoticeDescription = n.Notice_Description,
                    status = n.status,
                    ClosedBy = n.ClosedBy,
                    ClosedDatetime = n.Closed_Datetime
                })
                .ToListAsync();
        }

     //   public async Task<List<NoticeDataModel>> GetClientsUnreadNotices1(string[] gstin, string source )
     //   {
     //       // Step 1: Get request numbers that have unread messages
     //       var unreadRequestNumbers = await _context.NoticeChats
     //         .Where(m => gstin.Contains(m.Client_Gstin) && m.Source.Trim() == source && m.IsRead == false)

     //           .Select(m => m.Request_Number)
     //           .Distinct()
     //           .ToListAsync();

     //       if (unreadRequestNumbers == null || unreadRequestNumbers.Count == 0)
     //           return new List<NoticeDataModel>();

     //       // Step 2: Get notice data for those request numbers
     //       var notices = await _context.NoticeDatas
     //           .Where(n => gstin.Contains(n.Client_Gstin) && unreadRequestNumbers.Contains(n.Request_Number))
     //           .Select(n => new NoticeDataModel
     //           {
     //               RequestNumber = n.Request_Number,
     //               ClientGstin = n.Client_Gstin,
					//ClientName = n.Client_Name,
     //               CreatedDatetime = n.Created_Datetime,
					//UpdatedDateTime = n.UpdatedDateTime,
					//FileName = n.FileName,
     //               NoticeTitle = n.Notice_Title,
     //               NoticeDatetime = n.Notice_Datetime,
     //               Priority = n.Priority,
     //               NoticeDescription = n.Notice_Description,
     //               status = n.status,
					//ClosedBy = n.ClosedBy,
     //               HasUnreadMessages = true // ✅ We already know they are unread
     //           })
     //           .ToListAsync();

     //       // Fetch unread messages for the gstin
     //       var unreadMessages = await _context.NoticeChats
     //          .Where(m => gstin.Contains(m.Client_Gstin) && m.IsRead == false)
     //           .ToListAsync();

     //       foreach (var notice in notices)
     //       {
     //           // Check if any unread chat exists for this request
     //           notice.HasUnreadMessages = unreadMessages.Any(msg => msg.Request_Number == notice.RequestNumber);

     //           var thisNoticeMessages = unreadMessages
     //               .Where(m => m.Request_Number == notice.RequestNumber)
     //               .OrderByDescending(m => m.Message_Datetime)
     //               .ToList();

     //           if (thisNoticeMessages.Any())
     //           {
     //               var latestMsg = thisNoticeMessages.First();
     //               if (latestMsg.Source == "Admin") // 🧠 For vendor view
     //               {
     //                   notice.IsAdmin = true;
     //               }
     //           }
     //       }

     //       return notices;
     //   }
        public async Task<List<NoticeDataModel>> GetClientsUnreadNotices(string[] gstin, string source)
        {
            bool filterByGstin = gstin != null && gstin.Any();

            // Step 1: Get request numbers that have unread messages
            var unreadRequestNumbersQuery = _context.NoticeChats
                .Where(m => m.Source.Trim() == source && m.IsRead == false);

            if (filterByGstin)
            {
                unreadRequestNumbersQuery = unreadRequestNumbersQuery
                    .Where(m => gstin.Contains(m.Client_Gstin));
            }

            var unreadRequestNumbers = await unreadRequestNumbersQuery
                .Select(m => m.Request_Number)
                .Distinct()
                .ToListAsync();

            if (unreadRequestNumbers == null || unreadRequestNumbers.Count == 0)
                return new List<NoticeDataModel>();

            // Step 2: Get notice data for those request numbers
            var noticesQuery = _context.NoticeDatas
                .Where(n => unreadRequestNumbers.Contains(n.Request_Number));

            if (filterByGstin)
            {
                noticesQuery = noticesQuery.Where(n => gstin.Contains(n.Client_Gstin));
            }

            var notices = await noticesQuery
                .Select(n => new NoticeDataModel
                {
                    RequestNumber = n.Request_Number,
                    ClientGstin = n.Client_Gstin,
                    ClientName = n.Client_Name,
                    CreatedDatetime = n.Created_Datetime,
                    UpdatedDateTime = n.UpdatedDateTime,
                    FileName = n.FileName,
                    NoticeTitle = n.Notice_Title,
                    NoticeDatetime = n.Notice_Datetime,
                    Priority = n.Priority,
                    NoticeDescription = n.Notice_Description,
                    status = n.status,
                    ClosedBy = n.ClosedBy,
                    HasUnreadMessages = true // ✅ We already know they are unread
                })
                .ToListAsync();

            // Step 3: Fetch unread messages for admin/vendor check
            var unreadMessagesQuery = _context.NoticeChats
                .Where(m => m.IsRead == false);

            if (filterByGstin)
            {
                unreadMessagesQuery = unreadMessagesQuery
                    .Where(m => gstin.Contains(m.Client_Gstin));
            }

            var unreadMessages = await unreadMessagesQuery.ToListAsync();

            foreach (var notice in notices)
            {
                // Check if any unread chat exists for this request
                notice.HasUnreadMessages = unreadMessages.Any(msg => msg.Request_Number == notice.RequestNumber);

                var thisNoticeMessages = unreadMessages
                    .Where(m => m.Request_Number == notice.RequestNumber)
                    .OrderByDescending(m => m.Message_Datetime)
                    .ToList();

                if (thisNoticeMessages.Any() && thisNoticeMessages.First().Source == "MainAdmin")
                {
                    notice.IsAdmin = true;
                }
            }

            return notices;
        }






        public async Task savaChatData(NoticeChatModel Data)
		{
			var data = new NoticeChat
			{
				//UniqueId = Data.UniqueId,
				Request_Number = Data.RequestNumber,
				Client_Gstin = Data.ClientGstin,
				Source = Data.Source,
				Name = Data.Name,
				Message = Data.Message,
				Message_Datetime = DateTime.Now,
				IsRead = false // Default to false if IsRead is null
			};
			_context.NoticeChats.Add(data);
			await _context.SaveChangesAsync();
		}
		public async Task<List<NoticeChatModel>> GetChatData(string requestNo , string gstin)
		{
			return await _context.NoticeChats
				.Where(n => n.Request_Number == requestNo &&
				            n.Client_Gstin == gstin)
				.Select(n => new NoticeChatModel
				{
					RequestNumber = n.Request_Number,
					ClientGstin = n.Client_Gstin,
					Source = n.Source,
					Name = n.Name,
					Message = n.Message,
					MessageDatetime = n.Message_Datetime,
					IsRead = n.IsRead				
				})
				.ToListAsync();
		}
       
		
		public async Task MarkAdminMessagesAsRead(string requestNo, string gstin) // client/MainAdmin controler use this helper to unread admin msgs
        {
            var unreadAdminMessages = await _context.NoticeChats
                .Where(m => m.Request_Number == requestNo &&
                            m.Client_Gstin == gstin &&
                            m.IsRead == false &&
                            m.Source == "Admin") // ✅ Only mark Admin's unread messages
                .ToListAsync();

            if (unreadAdminMessages.Any())
            {
                foreach (var message in unreadAdminMessages)
                {
                    message.IsRead = true;
                }

                await _context.SaveChangesAsync();
            }
        }
	    public async Task MarkClientMessagesAsRead(string requestNo, string gstin) // Admin controler use this helper to unread client msgs
        {
            var unreadVendorMessages = await _context.NoticeChats
                .Where(m => m.Request_Number == requestNo &&
                            m.Client_Gstin == gstin &&
                            m.IsRead == false &&
                            m.Source == "Client") // ✅ Only mark Vendor's unread messages
                .ToListAsync();

            if (unreadVendorMessages.Any())
            {
                foreach (var message in unreadVendorMessages)
                {
                    message.IsRead = true;
                }

                await _context.SaveChangesAsync();
            }
        }

        public async Task MarkMainAdminMessagesAsRead(string requestNo, string gstin) // Admin controler use this helper to unread client msgs
        {
            var unreadVendorMessages = await _context.NoticeChats
                .Where(m => m.Request_Number == requestNo &&
                            m.Client_Gstin == gstin &&
                            m.IsRead == false &&
                            m.Source == "MainAdmin") // ✅ Only mark MainAdmin's unread messages
                .ToListAsync();

            if (unreadVendorMessages.Any())
            {
                foreach (var message in unreadVendorMessages)
                {
                    message.IsRead = true;
                }

                await _context.SaveChangesAsync();
            }
        }




    }
}
