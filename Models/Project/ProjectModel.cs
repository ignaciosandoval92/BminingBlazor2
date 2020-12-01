using System;

namespace Models.Project
{
    public class ProjectModel
    {
        public int ProjectId { get; set; }
        public string CodProject { get; set; }
        public string ProjectName { get; set; }
        public string EmailBmining { get; set; }
        public string ClientName { get; set; }
        public int CodProjectType { get; set; }
        public int Cod_TipoProyecto { get; set; }
        public DateTime Fecha_Inicio { get; set; }
        public DateTime Fecha_Fin { get; set; }
        public int Id_Creador { get; set; }
        public int ProjectManagerId { get; set; }        
        public int ClientId { get; set; }
        public int StatusId { get; set; }
    }
}
