using System;

namespace Models.Project
{
   public class PaymentModel
    {
        public int PaymentId { get; set; }
        public int CodPaymentStatusType { get; set; }
        public string PaymentName { get; set; }
        public int ProjectId { get; set; }
        public int Cod_TipoEstadoPago { get; set; }
        public DateTime IssueExpirationDate { get; set; }
        public DateTime InvoiceExpirationDate { get; set; }
    }
}
