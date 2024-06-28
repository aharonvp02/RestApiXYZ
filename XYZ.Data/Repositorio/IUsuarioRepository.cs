using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XYZ.Model;

namespace XYZ.Data.Repositorio
{
    public interface IUsuarioRepository
    {
        //Uso de task para ejecutarlo asincronico y no bloquear el hilo principal
        Task<IEnumerable<Usuario>> GetAllUsuarios();
        Task<bool> ValidarCredenciales(int codigo, string contraseña);

    }
}
