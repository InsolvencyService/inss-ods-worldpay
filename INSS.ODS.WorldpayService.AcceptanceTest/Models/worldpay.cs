using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INSS.ODS.WorldpayService.AcceptanceTest.Models
{
    public class worldpay
    {
        public string OrderCode { get; set; }
        public string Description { get; set; }
        public string OrderValue { get; set; }
        public string RefundValue { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string IncludeAddress { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string PostCode { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string CountryCode { get; set; }
        public string TelephoneNumber { get; set; }

        public string CurrencyCode { get; set; }

    }

}

