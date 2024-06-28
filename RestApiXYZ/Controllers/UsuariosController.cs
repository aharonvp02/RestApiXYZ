using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

using XYZ.Data.Repositorio;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using System.Text;
using XYZ.Model;
using Microsoft.AspNetCore.Authorization;

namespace RestApiXYZ.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly string _key;


        public UsuariosController(IUsuarioRepository usuarioRepository, IConfiguration config)
        {
            _usuarioRepository = usuarioRepository;
            _key = config.GetSection("Jwt").GetSection("key").ToString();
        }


        [HttpGet]
        public async Task<IActionResult> GetAllUsuarios()
        {
            
            var usuarios = await _usuarioRepository.GetAllUsuarios();
            return Ok(usuarios);
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] Login obj)
        
        {
            try
            {
                // Validar credenciales
                bool isValid = await _usuarioRepository.ValidarCredenciales(obj.Codigo, obj.Clave);

                if (isValid)
                {
                    // Generar y devolver un token JWT si la validación es exitosa
                    var keyBytes = Encoding.ASCII.GetBytes(_key);
                    var claims = new ClaimsIdentity();
                    claims.AddClaim(new Claim(ClaimTypes.NameIdentifier, obj.Codigo.ToString()));
                    var tokenDescriptor = new SecurityTokenDescriptor
                    {
                        Subject = claims,
                        Expires = DateTime.UtcNow.AddMinutes(3),
                        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256Signature)
                    };

                    var tokenHandler = new JwtSecurityTokenHandler();
                    var tokenConfig = tokenHandler.CreateToken(tokenDescriptor);

                    string tokenCreado = tokenHandler.WriteToken(tokenConfig);
                    

                    return StatusCode(StatusCodes.Status200OK, new { message = "Login Correcto", token = tokenCreado });
                }
                else
                {
                    
                    return StatusCode(StatusCodes.Status401Unauthorized, new { message = "Código o contraseña incorrectos.", token = "" });
                }
            }
            catch (Exception ex )
            {
                //  posibles excepciones
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Ocurrió un error al procesar la solicitud.", error = ex.Message });
            }



            
        }


        [HttpGet("validarToken")]
        [Authorize]
        public  IActionResult ValidarToken()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                
                var expirationClaim = identity.FindFirst("exp");

                if (expirationClaim != null)
                {
                    
                    var expirationTime = DateTimeOffset.FromUnixTimeSeconds(long.Parse(expirationClaim.Value)).UtcDateTime;
                    var currentTime = DateTime.UtcNow;

                    
                    var timeRemaining = expirationTime - currentTime;

                    return Ok(new
                    {
                        message = "Token válido",
                        expiresInSeconds = timeRemaining.TotalSeconds 
                    });
                }
            }
            return Unauthorized("Token inválido o falta de permisos.");
        }
    }
}
