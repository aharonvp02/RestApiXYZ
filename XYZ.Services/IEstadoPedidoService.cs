using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XYZ.Model;

namespace XYZ.Services
{
    public interface IEstadoPedidoService
    {
        bool PuedeCambiarEstado(int estadoActual, int nuevoEstado);
        void RegistrarFechaGestion(Pedido pedido, int nuevoEstado);
    }
}
