using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XYZ.Model;

namespace XYZ.Data.Repositorio
{
    public interface IPedidoRepository
    {
        Task<int> CrearPedido(Pedido pedido);
        Task<Pedido> ObtenerPedidoporNumero(int numeroPedido);
        Task<bool> ActualizarPedido(Pedido pedido);

    }
}
