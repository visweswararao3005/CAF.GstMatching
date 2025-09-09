using CAF.GstMatching.Models.PurchaseTicketModel;
using CAF.GstMatching.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.Entity;
using Microsoft.Build.Framework;
using System.Globalization;

namespace CAF.GstMatching.Helper
{
    
    public class PurchaseTicketHelper 
    {
        private readonly ApplicationDbContext _context;
     
        public PurchaseTicketHelper(ApplicationDbContext context)
        {
            _context = context;           
        }
        public async Task SavePurchaseTicketAsync(TicketsStatusModel data) //new Request
        {
            // First, check if records with the given ticket exist
            var existingData = _context.Purchase_Ticket.Where(x => x.Request_No == data.RequestNo).ToList();

            // If they exist, remove them
            if (existingData.Any())
            {
                _context.Purchase_Ticket.RemoveRange(existingData);
                await _context.SaveChangesAsync(); // Save after deletion before inserting new records
            }

            var ticket = new Purchase_Ticket
            {
                EXERT_ID_COL = "REQ-" + Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper(),
                EXERT_USER_NAME = data.EXERTUSERNAME,
                Client_GSTIN = data.ClientGSTIN,
                Request_No = data.RequestNo,
                Request_Created_Date = data.RequestCreatedDate,
                Request_Updated_Date = data.RequestUpdatedDate,
                Financial_Year = data.FinancialYear,
                Period_Type = data.PeriodType,
                Txn_Period = data.TxnPeriod,
                Ticket_Status = "Pending",
                File_Name = data.FileName,
                Email = data.Email,
            };
            _context.Purchase_Ticket.Add(ticket);
            await _context.SaveChangesAsync();
        }

        public async Task UpdatePurchaseTicketAsync(string ticketNo, string ClientGSTIN) // update status in request
        {
            var ticket = await _context.Purchase_Ticket
                .FirstOrDefaultAsync(p => p.Request_No == ticketNo && p.Client_GSTIN == ClientGSTIN);
            if (ticket != null)
            {
                ticket.Ticket_Status = "Completed";  
                ticket.Completed_Date = DateTime.Now;
                await _context.SaveChangesAsync();
            }
        }
        public async Task UpdatePurchaseTicketAsync(string ticketNo,string ClientGSTIN, string AdminFileName) // update status in request for admin
        {
            var ticket = await _context.Purchase_Ticket
            .FirstOrDefaultAsync(p => p.Request_No == ticketNo && p.Client_GSTIN == ClientGSTIN);
            if (ticket != null)
            {
                ticket.Ticket_Status = "Analysed";  // Analysed
                //ticket.Completed_Date = DateTime.Now;
                ticket.Admin_File_Name = AdminFileName;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<TicketsStatusModel> GetUserDataBasedOnTicketAsync(string ticketNo, string ClientGSTIN) //Admin Used in compare page
        {
            return await _context.Purchase_Ticket
                .Where(p => p.Request_No == ticketNo && p.Client_GSTIN == ClientGSTIN)
                .Select(p => new TicketsStatusModel
                {
                    EXERTIDCOL = p.EXERT_ID_COL,
                    EXERTUSERNAME = p.EXERT_USER_NAME,
                    ClientGSTIN = p.Client_GSTIN,
                    RequestNo = p.Request_No,
                    RequestCreatedDate = p.Request_Created_Date,
                    RequestUpdatedDate = p.Request_Updated_Date,
                    RequestCompletedDateTime= p.Completed_Date,
                    FinancialYear = p.Financial_Year,
                    PeriodType = p.Period_Type,
                    TxnPeriod = p.Txn_Period,
                    TicketStatus = p.Ticket_Status,
                    FileName = p.File_Name,
                    AdminFileName = p.Admin_File_Name,
                    NoEdit = p.No_Edit,
                    Email = p.Email
                })
                .FirstOrDefaultAsync();
        }

        public async Task<List<TicketsStatusModel>> GetOpenTicketsStatusAsync(string clientGSTIN, DateTime fromDate, DateTime toDate) // open request for client
        {
			DateTime fromDate1 = fromDate.Date;
			DateTime toDate1 = toDate.Date.AddDays(1).AddTicks(-1);
            //Console.WriteLine($"to search in DB  :: From Date: {fromDate1}, To Date : {toDate1}" );
			return await _context.Purchase_Ticket
                .Where(p => p.Client_GSTIN == clientGSTIN &&
				            p.Request_Created_Date >= fromDate1 &&
                            p.Request_Created_Date <= toDate1 &&
                             (p.Ticket_Status == "Pending" || p.Ticket_Status == "Analysed"))
				.Select(p => new TicketsStatusModel
                {
                    EXERTIDCOL = p.EXERT_ID_COL,
                    RequestNo = p.Request_No,
                    FileName = p.File_Name,
                    ClientGSTIN = p.Client_GSTIN,
                    AdminFileName = p.Admin_File_Name,
                    RequestCreatedDate = p.Request_Created_Date,
                    RequestUpdatedDate = p.Request_Updated_Date,
                    FinancialYear = p.Financial_Year,
                    PeriodType = p.Period_Type,
                    TxnPeriod = p.Txn_Period,
                    TicketStatus = p.Ticket_Status,
                    NoEdit = p.No_Edit
                })
                .ToListAsync();
        }
		public async Task<List<TicketsStatusModel>> GetCloseTicketsStatusAsync(string clientGSTIN, DateTime fromDate, DateTime toDate) // Closed Request for client
		{
			DateTime fromDate1 = fromDate.Date;
			DateTime toDate1 = toDate.Date.AddDays(1).AddTicks(-1);

			return await _context.Purchase_Ticket
				.Where(p => p.Client_GSTIN == clientGSTIN &&
							p.Completed_Date >= fromDate1 &&
							p.Completed_Date <= toDate1 &&
                            p.Ticket_Status == "Completed")
				.Select(p => new TicketsStatusModel
				{
					EXERTIDCOL = p.EXERT_ID_COL,
					RequestNo = p.Request_No,
                    FileName = p.File_Name,
                    ClientGSTIN = p.Client_GSTIN,
                    AdminFileName = p.Admin_File_Name,
                    RequestCreatedDate = p.Request_Created_Date,
                    RequestUpdatedDate = p.Request_Updated_Date,
                    CompletedDate = p.Completed_Date,
					FinancialYear = p.Financial_Year,
					PeriodType = p.Period_Type,
					TxnPeriod = p.Txn_Period,
					TicketStatus = p.Ticket_Status,
				})
				.ToListAsync();
		}
       
        public async Task<List<TicketsStatusModel>> GetAllOpenTicketsStatusAsync(DateTime fromDate, DateTime toDate)//admin
        {
			DateTime fromDate1 = fromDate.Date;
			DateTime toDate1 = toDate.Date.AddDays(1).AddTicks(-1);

			return await _context.Purchase_Ticket
                .Where(p => p.Request_Created_Date >= fromDate1 &&
                            p.Request_Created_Date <= toDate1 &&
                             (p.Ticket_Status == "Pending" || p.Ticket_Status == "Analysed"))
                .Select(p => new TicketsStatusModel
                {
                    EXERTIDCOL = p.EXERT_ID_COL,
                    EXERTUSERNAME = p.EXERT_USER_NAME,
                    RequestNo = p.Request_No,
                    FileName = p.File_Name,
                    AdminFileName = p.Admin_File_Name,
                    RequestCreatedDate = p.Request_Created_Date,
                    RequestUpdatedDate = p.Request_Updated_Date,
                    FinancialYear = p.Financial_Year,
                    PeriodType = p.Period_Type,
                    CompletedDate = p.Completed_Date,
                    TxnPeriod = p.Txn_Period,
                    TicketStatus = p.Ticket_Status,
                    NoEdit = p.No_Edit
                })
                .ToListAsync();
        }
        public async Task<List<TicketsStatusModel>> GetAllCloseTicketsStatusAsync(DateTime fromDate, DateTime toDate) //admin
        {
			DateTime fromDate1 = fromDate.Date;
			DateTime toDate1 = toDate.Date.AddDays(1).AddTicks(-1);

			return await _context.Purchase_Ticket
				.Where(p => p.Completed_Date >= fromDate1 &&
                		    p.Completed_Date <= toDate1 &&
                            p.Ticket_Status == "Completed")
				.Select(p => new TicketsStatusModel
                {
					EXERTIDCOL = p.EXERT_ID_COL,
					EXERTUSERNAME = p.EXERT_USER_NAME,
					RequestNo = p.Request_No,
                    FileName = p.File_Name,
                    AdminFileName = p.Admin_File_Name,
                    RequestCreatedDate = p.Request_Created_Date,
                    RequestUpdatedDate = p.Request_Updated_Date,
                    CompletedDate = p.Completed_Date,
					FinancialYear = p.Financial_Year,
					PeriodType = p.Period_Type,
					TxnPeriod = p.Txn_Period,
					TicketStatus = p.Ticket_Status,
					NoEdit = p.No_Edit
				})
				.ToListAsync();
		}     
       

        public async Task<List<TicketsStatusModel>> GetClientsOpenTicketsStatusAsync(string[] clients , DateTime fromDate, DateTime toDate)
        {
			DateTime fromDate1 = fromDate.Date;
			DateTime toDate1 = toDate.Date.AddDays(1).AddTicks(-1);

            bool filterByGstin = clients != null && clients.Any();

            var openTickets = _context.Purchase_Ticket.Where(p => p.Request_Created_Date >= fromDate1 &&
                                                                  p.Request_Created_Date <= toDate1 &&
                                                                 (p.Ticket_Status == "Pending" || p.Ticket_Status == "Analysed"));

            if (filterByGstin)
            {
                openTickets = openTickets.Where(p => clients.Contains(p.Client_GSTIN));
            }

            return await openTickets
                .Select(p => new TicketsStatusModel
                {
					EXERTIDCOL = p.EXERT_ID_COL,
					EXERTUSERNAME = p.EXERT_USER_NAME,
					RequestNo = p.Request_No,
                    ClientGSTIN = p.Client_GSTIN,
					FileName = p.File_Name,
					AdminFileName = p.Admin_File_Name,
					RequestCreatedDate = p.Request_Created_Date,
					RequestUpdatedDate = p.Request_Updated_Date,
					FinancialYear = p.Financial_Year,
					PeriodType = p.Period_Type,
					CompletedDate = p.Completed_Date,
					TxnPeriod = p.Txn_Period,
					TicketStatus = p.Ticket_Status,
					NoEdit = p.No_Edit
				})
                .ToListAsync();
        }
		public async Task<List<TicketsStatusModel>> GetClientsClosedTicketsStatusAsync(string[] clients, DateTime fromDate, DateTime toDate)
		{
			DateTime fromDate1 = fromDate.Date;
			DateTime toDate1 = toDate.Date.AddDays(1).AddTicks(-1);

            bool filterByGstin = clients != null && clients.Any();

            var closedTickets = _context.Purchase_Ticket.Where(p => p.Completed_Date >= fromDate1 &&
                                                                    p.Completed_Date <= toDate1 &&
                                                                    p.Ticket_Status == "Completed");

            if (filterByGstin)
            {
                closedTickets = closedTickets.Where(p => clients.Contains(p.Client_GSTIN));
            }

			return await closedTickets
                .Select(p => new TicketsStatusModel
				{
					EXERTIDCOL = p.EXERT_ID_COL,
					EXERTUSERNAME = p.EXERT_USER_NAME,
					RequestNo = p.Request_No,
                    ClientGSTIN = p.Client_GSTIN,
                    FileName = p.File_Name,
					AdminFileName = p.Admin_File_Name,
					RequestCreatedDate = p.Request_Created_Date,
					RequestUpdatedDate = p.Request_Updated_Date,
					FinancialYear = p.Financial_Year,
					PeriodType = p.Period_Type,
					CompletedDate = p.Completed_Date,
					TxnPeriod = p.Txn_Period,
					TicketStatus = p.Ticket_Status,
					NoEdit = p.No_Edit
				})
				.ToListAsync();
		}


	}
}
