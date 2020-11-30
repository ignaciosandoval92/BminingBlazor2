using System.Diagnostics;

namespace Models.Project
{
    [DebuggerDisplay("{Id_Cliente}-{Nombre_Cliente}")]
    public class ClientModel
    {
        public int ClientId { get; set; }
        public string ClientName { get; set; }
    }
}
