using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INSS.ODS.WorldpayService.AcceptanceTest.Models
{
    public class ResponseModel
    {
        public string OrderCode { get; set; }
        public string RedirectUrl { get; set; }
        public string Error { get; set; }
    }
}
