using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    public class ProjectModel
    {
        public string Cod_Proyecto { get; set; }
        public string Nombre_Proyecto { get; set; }
        public int Cod_TipoProyecto { get; set; }
        public DateTime Fecha_Inicio { get; set; }
        public DateTime Fecha_Fin { get; set; }
        public int Id_Creador { get; set; }
        public int Id_JefeProyecto { get; set; }
        public int Id_proyecto { get; set; }
        public int Id_Cliente { get; set; }
        public int Id_Status { get; set; }
    }
}
