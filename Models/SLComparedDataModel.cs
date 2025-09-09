using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAF.GstMatching.Models
{
    public class SLComparedDataModel
    {
        public string UniqueValue { get; set; }
        public string ClientGstin { get; set; }
        public string RequestNumber { get; set; }
        public Nullable<int> SNo { get; set; }
        public string InvoiceNumber { get; set; }
        public Nullable<System.DateTime> InvoiceDate { get; set; }
        public string CustomerName { get; set; }
        public string CustomerGstin { get; set; }
        public string PlaceofSupply { get; set; }
        public Nullable<decimal> TaxableAmount { get; set; }
        public Nullable<decimal> CGST { get; set; }
        public Nullable<decimal> SGST { get; set; }
        public Nullable<decimal> IGST { get; set; }
        public Nullable<decimal> CESS { get; set; }
        public Nullable<decimal> TotalAmount { get; set; }
        public string GSTRate { get; set; }
        public string TypeOfInvoice { get; set; }
        public string Category { get; set; }
        public string DataSource { get; set; }
        public string MatchType { get; set; }
        public string Compare { get; set; }
		public string period { get; set; }


		public Nullable<decimal> catagory1InvoiceNumber { get; set; }
        public Nullable<decimal> catagory1InvoiceSum { get; set; }
        public Nullable<decimal> catagory1EWayWillNumber { get; set; }
        public Nullable<decimal> catagory1EWayWillSum { get; set; }
        public Nullable<decimal> catagory1EInvoiceNumber { get; set; }
        public Nullable<decimal> catagory1EInvoiceSum { get; set; }

        public Nullable<decimal> catagory2InvoiceNumber { get; set; }
        public Nullable<decimal> catagory2InvoiceSum { get; set; }
        public Nullable<decimal> catagory2EWayWillNumber { get; set; }
        public Nullable<decimal> catagory2EWayWillSum { get; set; }
        public Nullable<decimal> catagory2EInvoiceNumber { get; set; }
        public Nullable<decimal> catagory2EInvoiceSum { get; set; }

        public Nullable<decimal> catagory3InvoiceNumber { get; set; }
        public Nullable<decimal> catagory3InvoiceSum { get; set; }
        public Nullable<decimal> catagory3EWayWillNumber { get; set; }
        public Nullable<decimal> catagory3EWayWillSum { get; set; }
        public Nullable<decimal> catagory3EInvoiceNumber { get; set; }
        public Nullable<decimal> catagory3EInvoiceSum { get; set; }


        public Nullable<decimal> catagory4InvoiceNumber { get; set; }
        public Nullable<decimal> catagory4InvoiceSum { get; set; }
        public Nullable<decimal> catagory4EWayWillNumber { get; set; }
        public Nullable<decimal> catagory4EWayWillSum { get; set; }
        public Nullable<decimal> catagory4EInvoiceNumber { get; set; }
        public Nullable<decimal> catagory4EInvoiceSum { get; set; }

        public Nullable<decimal> catagory5InvoiceNumber { get; set; }
        public Nullable<decimal> catagory5InvoiceSum { get; set; }
        public Nullable<decimal> catagory5EWayWillNumber { get; set; }
        public Nullable<decimal> catagory5EWayWillSum { get; set; }
        public Nullable<decimal> catagory5EInvoiceNumber { get; set; }
        public Nullable<decimal> catagory5EInvoiceSum { get; set; }

        public Nullable<decimal> catagory6InvoiceNumber { get; set; }
        public Nullable<decimal> catagory6InvoiceSum { get; set; }
        public Nullable<decimal> catagory6EWayWillNumber { get; set; }
        public Nullable<decimal> catagory6EWayWillSum { get; set; }
        public Nullable<decimal> catagory6EInvoiceNumber { get; set; }
        public Nullable<decimal> catagory6EInvoiceSum { get; set; }

        public Nullable<decimal> catagory7InvoiceNumber { get; set; }
        public Nullable<decimal> catagory7InvoiceSum { get; set; }
        public Nullable<decimal> catagory7EWayWillNumber { get; set; }
        public Nullable<decimal> catagory7EWayWillSum { get; set; }
        public Nullable<decimal> catagory7EInvoiceNumber { get; set; }
        public Nullable<decimal> catagory7EInvoiceSum { get; set; }

        public Nullable<decimal> catagory8InvoiceNumber { get; set; }
        public Nullable<decimal> catagory8InvoiceSum { get; set; }
        public Nullable<decimal> catagory8EWayWillNumber { get; set; }
        public Nullable<decimal> catagory8EWayWillSum { get; set; }
        public Nullable<decimal> catagory8EInvoiceNumber { get; set; }
        public Nullable<decimal> catagory8EInvoiceSum { get; set; }

        public Nullable<decimal> catagory9InvoiceNumber { get; set; }
        public Nullable<decimal> catagory9InvoiceSum { get; set; }
        public Nullable<decimal> catagory9EWayWillNumber { get; set; }
        public Nullable<decimal> catagory9EWayWillSum { get; set; }
        public Nullable<decimal> catagory9EInvoiceNumber { get; set; }
        public Nullable<decimal> catagory9EInvoiceSum { get; set; }

        public Nullable<decimal> catagory10InvoiceNumber { get; set; }
        public Nullable<decimal> catagory10InvoiceSum { get; set; }
        public Nullable<decimal> catagory10EWayWillNumber { get; set; }
        public Nullable<decimal> catagory10EWayWillSum { get; set; }
        public Nullable<decimal> catagory10EInvoiceNumber { get; set; }
        public Nullable<decimal> catagory10EInvoiceSum { get; set; }

        public Nullable<decimal> catagory11InvoiceNumber { get; set; }
        public Nullable<decimal> catagory11InvoiceSum { get; set; }
        public Nullable<decimal> catagory11EWayWillNumber { get; set; }
        public Nullable<decimal> catagory11EWayWillSum { get; set; }
        public Nullable<decimal> catagory11EInvoiceNumber { get; set; }
        public Nullable<decimal> catagory11EInvoiceSum { get; set; }

        public Nullable<decimal> catagory12InvoiceNumber { get; set; }
        public Nullable<decimal> catagory12InvoiceSum { get; set; }
        public Nullable<decimal> catagory12EWayWillNumber { get; set; }
        public Nullable<decimal> catagory12EWayWillSum { get; set; }
        public Nullable<decimal> catagory12EInvoiceNumber { get; set; }
        public Nullable<decimal> catagory12EInvoiceSum { get; set; }

    }
}
