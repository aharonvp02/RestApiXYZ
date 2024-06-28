using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using XYZ.Data.Repositorio;
using XYZ.Model;
using XYZ.Services;

namespace RestApiXYZ.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PedidosController : ControllerBase
    {
        private readonly IPedidoRepository _pedidoRepository;
        private readonly string _key;
        private readonly IEstadoPedidoService _estadoPedidoService;

        public PedidosController(IPedidoRepository pedidoRepository, IConfiguration config, IEstadoPedidoService estadoPedidoService)
        {
            _pedidoRepository = pedidoRepository;
            _key = config.GetSection("Jwt").GetSection("key").ToString();
            _estadoPedidoService = estadoPedidoService;
        }


        [HttpPost("crearPedido")]
        [Authorize]
        public async Task<IActionResult> CrearPedido([FromBody] Pedido pedido)
        {
            try
            {
                pedido.FechaPedido = DateTime.UtcNow;
                pedido.IdEstadoPedido = 1; // Estado 'Por atender'
                var numeroPedido = await _pedidoRepository.CrearPedido(pedido);
                if (numeroPedido<0)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error al crear el pedido" });
                }

                return Ok(new { message = "Pedido creado exitosamente", numeroPedido });

            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error al crear el pedido", error = ex.Message });
                
            }
        }


        [HttpPost("cambiarEstadoPedido")]
        [Authorize]
        public async Task<IActionResult> CambiarEstadoPedido(int numeroPedido, int nuevoEstado)
        {
            var pedido = await _pedidoRepository.ObtenerPedidoporNumero(numeroPedido);
            if (pedido == null)
            {
                return NotFound(new { message = "Pedido no encontrado" });
            }

            if (!_estadoPedidoService.PuedeCambiarEstado(pedido.IdEstadoPedido, nuevoEstado))
            {
                return BadRequest(new { message = "Cambio de estado no permitido" });
            }

            _estadoPedidoService.RegistrarFechaGestion(pedido, nuevoEstado);
            pedido.IdEstadoPedido = nuevoEstado;

            var resultado = await _pedidoRepository.ActualizarPedido(pedido);
            if (!resultado)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error al actualizar el pedido" });
            }

            return Ok(new { message = "Estado del pedido actualizado correctamente" });

        }
    }
}
