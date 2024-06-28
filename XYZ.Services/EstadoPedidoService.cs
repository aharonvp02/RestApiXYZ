using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XYZ.Model;

namespace XYZ.Services
{
    public class EstadoPedidoService : IEstadoPedidoService
    {
        //Implementando la logica del negocio en la actualizacion de estados de Pedido
       public bool PuedeCambiarEstado(int estadoActual, int nuevoEstado)
        {
            var estadosValidos = new Dictionary<int, List<int>>
            {
                { 1, new List<int> { 2, 3, 4 } },
                { 2, new List<int> { 3, 4 } },
                { 3, new List<int> { 4 } }
            };

            return estadosValidos.ContainsKey(estadoActual) && estadosValidos[estadoActual].Contains(nuevoEstado);

        }

        void IEstadoPedidoService.RegistrarFechaGestion(Pedido pedido, int nuevoEstado)
        {
            switch (nuevoEstado)
            {
                case 1:
                    pedido.FechaPedido = DateTime.Now;
                    break;
                case 2:
                    pedido.FechaRecepcion = DateTime.Now;
                    break;
                case 3:
                    pedido.FechaDespacho = DateTime.Now;
                    break;
                case 4:
                    pedido.FechaEntrega = DateTime.Now;
                    break;
                default:
                    throw new ArgumentException("Estado de pedido no válido");
            }
        }
    }
}
