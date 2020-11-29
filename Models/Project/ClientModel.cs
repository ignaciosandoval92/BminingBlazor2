using System.Diagnostics;

namespace Models.Project
{
    [DebuggerDisplay("{Id_Cliente}-{Nombre_Cliente}")]
    public class ClientModel
    {
        public int Id_Cliente { get; set; }
        public string Nombre_Cliente { get; set; }
    }
}
