using Dapper;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XYZ.Model;

namespace XYZ.Data.Repositorio
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly DBOConexion _conexionbd;
       

        public UsuarioRepository(DBOConexion conexionbd)
        {

            _conexionbd = conexionbd;

        }

        protected MySqlConnection dbConexion()
        {

            return new MySqlConnection(_conexionbd.ConnectionString);

        }


        public async Task<IEnumerable<Usuario>> GetAllUsuarios()
        {
            try
            {
                using (var cnx= dbConexion())
                {

                    return await cnx.QueryAsync<Usuario>("sp_ListarUsuarios", new { }, commandType: CommandType.StoredProcedure);
                 
                }
            }
            catch (Exception ex )
            {
                Console.WriteLine("Error en sp_ListarUsuarios: " + ex.Message + ", StackTrace: " + ex.StackTrace);
                return null;
            }      
        }

        public async Task<bool> ValidarCredenciales(int codigo, string clave)
        {
            try
            {
                using (var cnx = dbConexion())
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@p_codigo", codigo);
                    parameters.Add("@p_clave", clave);
                    parameters.Add("@p_isValid", dbType: DbType.Int32, direction: ParameterDirection.Output);

                    await cnx.ExecuteAsync("sp_ValidarCredenciales", parameters, commandType: CommandType.StoredProcedure);

                    int isValid = parameters.Get<int>("@p_isValid");
                    return isValid > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en sp_ValidarCredenciales: " + ex.Message + ", StackTrace: " + ex.StackTrace);
                return false;
            }       
        }
    }
}
