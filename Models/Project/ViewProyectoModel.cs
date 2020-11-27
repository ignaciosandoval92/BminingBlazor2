using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    // TODO: Utilizar un ViewModel
    public class ViewProyectoModel
    {
        public int Id_Proyecto { get; set; }
        public string Cod_Proyecto { get; set; }
        public string Nombre_Proyecto { get; set; }
        public string Tipo_Pago { get; set; }
        public string TipoEstadoPago { get; set; }
        public string Tipo_Proyecto { get; set; }
        public int Cod_EstadoPago { get; set; }
        public string EmailProjectManager { get; set; }
        public string Nombre_Cliente { get; set; }
        public string Status { get; set; }
    }
}
