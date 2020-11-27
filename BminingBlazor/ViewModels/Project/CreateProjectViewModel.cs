using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BminingBlazor.ViewModels.Project
{
    public class CreateProjectViewModel
    {
        public string Cod_Proyecto { get; set; }
        public string Nombre_Proyecto { get; set; }
        public int Cod_TipoProyecto { get; set; }
        public DateTime Fecha_Inicio { get; set; }
        public DateTime Fecha_Fin { get; set; }
        public int Id_Creador { get; set; }
        public int Id_JefeProyecto { get; set; }
        public int Id_Proyecto { get; set; }
        public int Id_Cliente { get; set; }
        public int Id_Status { get; set; }
        public int Id_Usuario { get; set; }
        public double HoursProject { get; set; }
        public string Name { get; set; }
        public DateTime IssueExpirationDate { get; set; }
        public DateTime InvoiceExpirationDate { get; set; }
    }
}
