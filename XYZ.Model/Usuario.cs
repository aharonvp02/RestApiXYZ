using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XYZ.Model
{
    public class Usuario
    {
        public int Codigo { get; set; }
        public string NombreUsuario { get; set; }
        public string Correo { get; set; }
        public string Telefono { get; set; }
        public string Puesto { get; set; }
        public int IdRol { get; set; }
        public string Rol { get; set; } 
        public string Clave { get; set; }
    }
}
