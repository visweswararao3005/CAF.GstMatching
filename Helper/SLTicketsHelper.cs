using CAF.GstMatching.Data;
using CAF.GstMatching.Models;
using CAF.GstMatching.Models.PurchaseTicketModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAF.GstMatching.Helper
{
    public class SLTicketsHelper
    {
        private readonly ApplicationDbContext _context;
        public SLTicketsHelper(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task SaveSLTicketAsync(SalesLedgerTicketsModel data) //new Request
        {
            // First, check if records with the given ticket exist
            var existingData = _context.Sales_Ledger_Tickets.Where(x => x.Request_Number == data.RequestNumber).ToList();
            // If they exist, remove them
            if (existingData.Any())
            {
                _context.Sales_Ledger_Tickets.RemoveRange(existingData);
                await _context.SaveChangesAsync(); // Save after deletion before inserting new records
            }

            var ticket = new Sales_Ledger_Ticket
            {
                Unique_Value = "SLREQ-" + Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper(),
                Request_Number = data.RequestNumber,
                Client_Name = data.ClientName,
                CLient_Email = data.CLientEmail,
				Client_Gstin = data.ClientGstin,
                Request_Created_Date = data.RequestCreatedDate,
                Request_Updated_Date = data.RequestUpdatedDate,             
                File_Name = data.FileName,
                Edit = data.Edit,
                Financial_Year = data.FinancialYear,
                Period_Type = data.PeriodType,
                Period = data.Period,
                Ticket_Status = "Pending",   
            };
            _context.Sales_Ledger_Tickets.Add(ticket);
            await _context.SaveChangesAsync();
        }
        public async Task<SalesLedgerTicketsModel> GetUserDataBasedOnTicketAsync(string ticketNo, string ClientGSTIN) //Admin Used in compare page
        {
            return await _context.Sales_Ledger_Tickets
				.Where(p => p.Request_Number == ticketNo && p.Client_Gstin == ClientGSTIN)
                .Select(p => new SalesLedgerTicketsModel
				{
                   UniqueValue = p.Unique_Value,
                   RequestNumber = p.Request_Number,
                   ClientName = p.Client_Name,
                   CLientEmail = p.CLient_Email,
                   ClientGstin = p.Client_Gstin,
                   RequestCreatedDate = p.Request_Created_Date,
                   RequestUpdatedDate = p.Request_Updated_Date,
                   RequestCompleteDate = p.Request_Complete_Date,
                   FileName = p.File_Name,
                   EInvoiceFileName = p.EInvoice_File_Name,
                   EWayBillFileName = p.EWay_Bill_File_Name,
                   Edit = p.Edit,
                   FinancialYear = p.Financial_Year,
                   PeriodType = p.Period_Type,
                   Period = p.Period,
                   TicketStatus = p.Ticket_Status

                
                })
                .FirstOrDefaultAsync();
        }
        public async Task<List<SalesLedgerTicketsModel>> GetOpenTicketsStatusAsync(string ClientGSTIN , DateTime fromDate, DateTime toDate) // Open Request for client 
		{
			DateTime fromDate1 = fromDate.Date;
			DateTime toDate1 = toDate.Date.AddDays(1).AddTicks(-1);
			return await _context.Sales_Ledger_Tickets
				.Where(p => p.Client_Gstin == ClientGSTIN &&
							p.Request_Created_Date >= fromDate1 &&
							p.Request_Created_Date <= toDate1 &&
						   (p.Ticket_Status == "Pending" || p.Ticket_Status == "Analysed"))
				.Select(p => new SalesLedgerTicketsModel
                {
					UniqueValue = p.Unique_Value,
					RequestNumber = p.Request_Number,
					ClientName = p.Client_Name,
					CLientEmail = p.CLient_Email,
					ClientGstin = p.Client_Gstin,
					RequestCreatedDate = p.Request_Created_Date,
					RequestUpdatedDate = p.Request_Updated_Date,
					RequestCompleteDate = p.Request_Complete_Date,
					FileName = p.File_Name,
					EInvoiceFileName = p.EInvoice_File_Name,
					EWayBillFileName = p.EWay_Bill_File_Name,
					Edit = p.Edit,
					FinancialYear = p.Financial_Year,
					PeriodType = p.Period_Type,
					Period = p.Period,
					TicketStatus = p.Ticket_Status
				})
				.ToListAsync();
		}
        public async Task<List<SalesLedgerTicketsModel>> GetCloseTicketsStatusAsync(string ClientGSTIN, DateTime fromDate, DateTime toDate) // Closed Request for client
        {
			DateTime fromDate1 = fromDate.Date;
			DateTime toDate1 = toDate.Date.AddDays(1).AddTicks(-1);

            return await _context.Sales_Ledger_Tickets
                .Where(p => p.Client_Gstin == ClientGSTIN &&
                            p.Request_Complete_Date >= fromDate1 &&
                	        p.Request_Complete_Date <= toDate1 &&
                            p.Ticket_Status == "Completed")
                .Select(p => new SalesLedgerTicketsModel
                {
                    UniqueValue = p.Unique_Value,
                    RequestNumber = p.Request_Number,
                    ClientName = p.Client_Name,            
                    CLientEmail = p.CLient_Email,                       
                    ClientGstin = p.Client_Gstin,
                    RequestCreatedDate = p.Request_Created_Date,
                    RequestUpdatedDate = p.Request_Updated_Date,
                    RequestCompleteDate = p.Request_Complete_Date,
                    FileName = p.File_Name,
                    EInvoiceFileName = p.EInvoice_File_Name,
                    EWayBillFileName = p.EWay_Bill_File_Name,
                    Edit = p.Edit,
                    FinancialYear = p.Financial_Year,
                    PeriodType = p.Period_Type,
                    Period = p.Period,
                    TicketStatus = p.Ticket_Status
                }).ToListAsync();
		}
		
        public async Task<List<SalesLedgerTicketsModel>> GetClientsOpenTicketsStatusAsync(string[] clients, DateTime fromDate, DateTime toDate) // Open Request for Admin
		{
			DateTime fromDate1 = fromDate.Date;
			DateTime toDate1 = toDate.Date.AddDays(1).AddTicks(-1);

            var openTickets = _context.Sales_Ledger_Tickets.Where(p => p.Request_Created_Date >= fromDate1 &&
                                                                       p.Request_Created_Date <= toDate1 &&
                                                                      (p.Ticket_Status == "Pending" || p.Ticket_Status == "Analysed"));

            bool filterByClients = clients != null && clients.Any();

            if (filterByClients)
            {
                openTickets = openTickets.Where(p => clients.Contains(p.Client_Gstin));
            }

            return await openTickets
                .Select(p => new SalesLedgerTicketsModel
				{
					UniqueValue = p.Unique_Value,
					RequestNumber = p.Request_Number,
					ClientName = p.Client_Name,
					CLientEmail = p.CLient_Email,
					ClientGstin = p.Client_Gstin,
					RequestCreatedDate = p.Request_Created_Date,
					RequestUpdatedDate = p.Request_Updated_Date,
					RequestCompleteDate = p.Request_Complete_Date,
					FileName = p.File_Name,
					EInvoiceFileName = p.EInvoice_File_Name,
					EWayBillFileName = p.EWay_Bill_File_Name,
					Edit = p.Edit,
					FinancialYear = p.Financial_Year,
					PeriodType = p.Period_Type,
					Period = p.Period,
					TicketStatus = p.Ticket_Status
				})
				.ToListAsync();
		}
		public async Task<List<SalesLedgerTicketsModel>> GetClientsClosedTicketsStatusAsync(string[] clients, DateTime fromDate, DateTime toDate) // Closed Request for Admin
		{
			DateTime fromDate1 = fromDate.Date;
			DateTime toDate1 = toDate.Date.AddDays(1).AddTicks(-1);

            var closedTickets = _context.Sales_Ledger_Tickets.Where(p => p.Request_Complete_Date >= fromDate1 &&
                                                                         p.Request_Complete_Date <= toDate1 &&
                                                                         p.Ticket_Status == "Completed");
            bool filterByClients = clients != null && clients.Any();
            if (filterByClients)
            {
                closedTickets = closedTickets.Where(p => clients.Contains(p.Client_Gstin));
            }

			return await closedTickets
                .Select(p => new SalesLedgerTicketsModel
				{
					UniqueValue = p.Unique_Value,
					RequestNumber = p.Request_Number,
					ClientName = p.Client_Name,
					CLientEmail = p.CLient_Email,
					ClientGstin = p.Client_Gstin,
					RequestCreatedDate = p.Request_Created_Date,
					RequestUpdatedDate = p.Request_Updated_Date,
					RequestCompleteDate = p.Request_Complete_Date,
					FileName = p.File_Name,
					EInvoiceFileName = p.EInvoice_File_Name,
					EWayBillFileName = p.EWay_Bill_File_Name,
					Edit = p.Edit,
					FinancialYear = p.Financial_Year,
					PeriodType = p.Period_Type,
					Period = p.Period,
					TicketStatus = p.Ticket_Status
				}).ToListAsync();
	    }




        public async Task UpdateSLTicketAsync(string ticketNo, string ClientGSTIN, string EInvoice_AdminFileName,string EWayBill_AdminFileName) // update status in request for admin
        {
            var ticket = await _context.Sales_Ledger_Tickets
            .FirstOrDefaultAsync(p => p.Request_Number == ticketNo && p.Client_Gstin == ClientGSTIN);
            if (ticket != null)
            {
                ticket.Ticket_Status = "Analysed";  // Analysed
                ticket.EInvoice_File_Name = EInvoice_AdminFileName;
                ticket.EWay_Bill_File_Name = EWayBill_AdminFileName;

                await _context.SaveChangesAsync();
            }
        }
        public async Task CloseSLTicketAsync(string ticketNo, string ClientGSTIN) // Close request
        {
            var ticket = await _context.Sales_Ledger_Tickets
                .FirstOrDefaultAsync(p => p.Request_Number == ticketNo && p.Client_Gstin == ClientGSTIN);
            if (ticket != null)
            {
                ticket.Ticket_Status = "Completed";
                ticket.Request_Complete_Date = DateTime.Now;
                await _context.SaveChangesAsync();
            }
        }
       
      

	}
}
