using System;

namespace BminingBlazor.ViewModels.Projects
{
    public class PaymentViewModel
    {
        public int Id { get; set; }
        public int MyProjectId { get; set; }
        public string MyName { get; set; }
        public PaymentStatusTypeEnum PaymentStatusType { get; set; }
        public DateTime IssueExpirationDate { get; set; }
        public DateTime InvoiceExpirationDate { get; set; }
    }
}