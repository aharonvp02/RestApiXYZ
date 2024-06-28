using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XYZ.Model
{
    public class Pedido
    {
        public int NumeroPedido { get; set; }
        public DateTime FechaPedido { get; set; }
        public DateTime? FechaRecepcion { get; set; }
        public DateTime? FechaDespacho { get; set; }
        public DateTime? FechaEntrega { get; set; }
        public int Vendedor { get; set; }
        public int? Repartidor { get; set; }
        public int IdEstadoPedido { get; set; }

        public List<DetallePedido> DetallesPedido { get; set; }
    }
}
