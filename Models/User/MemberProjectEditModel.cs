﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    public class MemberProjectEditModel
    {
        public int Id { get; set; }
        public string Email_Bmining { get; set; }
        public string Nombre { get; set; }
        public string Apellido_Paterno { get; set; }
        public string Apellido_Materno { get; set; }
        public string Rut { get; set; }
        public string Cargo { get; set; }
        public string Telefono { get; set; }
        public string Direccion { get; set; }
        public int Cod_TipoContrato { get; set; }
        public int Cod_Integrantes { get; set; }
    }
}
