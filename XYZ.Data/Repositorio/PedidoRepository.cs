using Dapper;
using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XYZ.Model;

namespace XYZ.Data.Repositorio
{
    public class PedidoRepository : IPedidoRepository
    {
        private readonly DBOConexion _conexionbd;
       

        public PedidoRepository(DBOConexion conexionbd)
        {
            _conexionbd = conexionbd;
            
        }

        protected MySqlConnection dbConexion()
        {

            return new MySqlConnection(_conexionbd.ConnectionString);
            
        }


        public async Task<int> CrearPedido(Pedido pedido)
        {
            try
            {
                using (var cnx=dbConexion())
                {
                    //Uso de transaccion para asegurar todas las operaciones se realizan correctamente, caso contrario se realizara un rollBack
                    await cnx.OpenAsync();
                    using (var transaction = cnx.BeginTransaction())
                    {
                        try
                        {
                            var parameters = new DynamicParameters();
                            parameters.Add("@p_FechaPedido", pedido.FechaPedido, DbType.DateTime, ParameterDirection.Input);
                            parameters.Add("@p_Vendedor", pedido.Vendedor, DbType.Int32, ParameterDirection.Input);
                            parameters.Add("@p_IdEstadoPedido", pedido.IdEstadoPedido, DbType.Int32, ParameterDirection.Input);
                            parameters.Add("@p_NumeroPedido", DbType.Int32, direction: ParameterDirection.Output);

                            await cnx.ExecuteAsync("sp_CrearPedido", parameters, commandType: CommandType.StoredProcedure);
                            int numeroPedido = parameters.Get<int>("@p_NumeroPedido");

                            foreach (var detalle in pedido.DetallesPedido)
                            {
                                detalle.NumeroPedido = numeroPedido;
                                var detalleParameters = new DynamicParameters();
                                detalleParameters.Add("@p_NumeroPedido", detalle.NumeroPedido, DbType.Int32, ParameterDirection.Input);
                                detalleParameters.Add("@p_Sku", detalle.Sku, DbType.Int32, ParameterDirection.Input);
                                detalleParameters.Add("@p_Cantidad", detalle.Cantidad, DbType.Int32, ParameterDirection.Input);

                                await cnx.ExecuteAsync("sp_CrearDetallePedido", detalleParameters, commandType: CommandType.StoredProcedure);
                            }
                            transaction.Commit();
                            return numeroPedido;
                        }                        
                        catch (Exception ex )
                        {
                            transaction.Rollback();
                            Console.WriteLine("Error en sp_CrearPedido,sp_CrearDetallePedido: " + ex.Message + ", StackTrace: " + ex.StackTrace);
                            return 0;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en sp_CrearPedido: " + ex.Message + ", StackTrace: " + ex.StackTrace);
                return 0;
            }
        }

        public async Task<Pedido> ObtenerPedidoporNumero(int numeroPedido)
        {
            try
            {
                using (var cnx = dbConexion())
                {
                   
                    return await cnx.QueryFirstOrDefaultAsync<Pedido>("sp_ObtenerPedidoByNumero", new { p_NumeroPedido = numeroPedido }, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en sp_ObtenerPedidoByNumero: " + ex.Message + ", StackTrace: " + ex.StackTrace);
                return null;
            }
        }

        public async Task<bool> ActualizarPedido(Pedido pedido)
        {

            try
            {
                using (var cnx = dbConexion())
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@p_NumeroPedido", pedido.NumeroPedido, DbType.Int32, ParameterDirection.Input);
                    parameters.Add("@p_FechaPedido", pedido.FechaPedido, DbType.DateTime, ParameterDirection.Input);
                    parameters.Add("@p_FechaRecepcion", pedido.FechaRecepcion, DbType.DateTime, ParameterDirection.Input);
                    parameters.Add("@p_FechaDespacho", pedido.FechaDespacho, DbType.DateTime, ParameterDirection.Input);
                    parameters.Add("@p_FechaEntrega", pedido.FechaEntrega, DbType.DateTime, ParameterDirection.Input);
                    parameters.Add("@p_IdEstadoPedido", pedido.IdEstadoPedido, DbType.Int32, ParameterDirection.Input);

                    int rowsAffected = await cnx.ExecuteAsync("sp_ActualizarPedido", parameters, commandType: CommandType.StoredProcedure);
                    return rowsAffected > 0;  // Devuelve true si la actualización fue exitosa

                }
            }
            catch (Exception ex )
            {
                Console.WriteLine("Error en ActualizarPedido: " + ex.Message);
                return false;  // Devuelve false si hubo un error al actualizar
            }
           
        }



    }
}
