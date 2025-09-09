using CAF.GstMatching.Data;
using System;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CAF.GstMatching
{
    public class ApplicationDbContext : DbContext
    {
        // This constructor will use the connection string from Web.config
        public ApplicationDbContext() :  base("name=tjoeGSTEntities")
        {
        }


        public DbSet<TBL_PRD_USERS> TBL_PRD_USERS { get; set; } // Ensure this matches your table
        public DbSet<GST_State_Code> GST_State_Codes { get; set; }
        public DbSet<Client_Api_datum> Client_Api_Data { get; set; }
        public DbSet<AdminUser> AdminUsers { get; set; }
        public DbSet<Admin_Clients> Admin_Clients { get; set; }
        public DbSet<UserValidUpto> UserValidUptoes { get; set; }
        public DbSet<PaymentDetail> PaymentDetails { get; set; }



        public DbSet<Purchase_datum> Purchase_Data { get; set; } // Ensure this matches your table
        public DbSet<Purchase_Ticket> Purchase_Ticket { get; set; } // Ensure this matches your table
        public DbSet<GSTR2_Token_Data> GSTR2_Token_Data { get; set; }
        public DbSet<GSTR2_APIs_Data> GSTR2_APIs_Data { get; set; }
        public DbSet<GSTR2_Data> GSTR2_Data { get; set; }
        public DbSet<GSTCompareData> GSTCompareDatas { get; set; }
        public DbSet<ModifiedData> ModifiedDatas { get; set; }

        public DbSet<GSTR2_Header> GSTR2_Header { get; set; } // Ensure this matches your table



        public DbSet<Sales_Ledger_datum> Sales_Ledger_Data { get; set; }
        public DbSet<Sales_Ledger_Ticket> Sales_Ledger_Tickets { get; set; }
        public DbSet<Sales_Ledger_Token_datum> Sales_Ledger_Token_Data { get; set; }
        public DbSet<Sales_Ledger_EInvoice> Sales_Ledger_EInvoices { get; set; }
        public DbSet<Sales_Ledger_EInvoice_APIs_datum> Sales_Ledger_EInvoice_APIs_Data { get; set; }
        public DbSet<Sales_Ledger_EWay_Bill> Sales_Ledger_EWay_Bills { get; set; }
        public DbSet<Sales_Ledger_EWay_Bill_APIs_datum> Sales_Ledger_EWay_Bill_APIs_Data { get; set; }
        public DbSet<SLComparedData> SLComparedDatas { get; set; }

		


		public DbSet<NoticeData> NoticeDatas { get; set; }
		public DbSet<NoticeChat> NoticeChats { get; set; }
	}
}
