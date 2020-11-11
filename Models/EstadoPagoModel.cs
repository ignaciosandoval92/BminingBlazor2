using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
   public class EstadoPagoModel
    {
        public int Cod_EstadoPago { get; set; }
        public string Estado_Pago { get; set; }
        public int Id_Proyecto { get; set; }
        public int Cod_TipoEstadoPago { get; set; }
    }
}
