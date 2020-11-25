using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BminingBlazor.ViewModels.Project
{
    public class PaymentStatusItemViewModel
    {
        public string Name { get; set; }
        public DateTime IssueExpirationDate { get; set; }
        public DateTime InvoiceExpirationDate { get; set; }

    }
}
