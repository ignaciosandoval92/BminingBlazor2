using System;

namespace Models.Project
{
   public class PaymentModel
    {
        public int Cod_EstadoPago { get; set; }
        public string Estado_Pago { get; set; }
        public int Id_Proyecto { get; set; }
        public int Cod_TipoEstadoPago { get; set; }
        public DateTime IssueExpirationDate { get; set; }
        public DateTime InvoiceExpirationDate { get; set; }
    }
}
