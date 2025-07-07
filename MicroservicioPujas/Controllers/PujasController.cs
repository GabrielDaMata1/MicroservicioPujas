using Application.Command;
using Application.DTOs;
using Application.Query;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MicroservicioPujas.Controllers
{
    /// <summary>
    /// Clase controller API encargada de procesar las solicitudes de inserción y consulta,
    /// sobre las pujas.
    /// </summary>

    [ApiController]
    [Route("api/Pujas")]
    public class PujasController : ControllerBase
    {
        /// <summary>
        /// Atributo que se encarga de enviar solicitudes (commands/queries) mediante el patrón mediador
        /// </summary>
        private readonly IMediator _mediator;

        public PujasController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Endpoint encargado de registrar una nueva puja.
        /// </summary>
        /// <param name="pujaDto">Parametro de tipo DTO con los datos de la puja a registrar.</param>
        /// <returns>Resultado de la operación con mensaje y estado dependiendo del resultado.</returns>
        [HttpPost("registroPuja")]
        public async Task<IActionResult> RegistrarPuja([FromBody] RegistrarPujaDTO pujaDto)
        {
            var resultado = await _mediator.Send(new RegistrarPujaCommand(pujaDto));
            if (resultado)
            {
                return Ok(new ResultadoDTO { Mensaje = "La puja se registró exitosamente.", Exito = true });
            }

            return BadRequest(new ResultadoDTO { Mensaje = "La puja no pudo ser registrada.", Exito = false });
        }
        /// <summary>
        /// Endpoint encargado de obtener la puja ganadora de una subasta.
        /// </summary>
        /// <param name="idSubasta">Parametro que corresponde al ID  de la subasta a consultar la puja ganadora.</param>
        /// <returns>Retorna un objeto Puja con los detalles de la puja ganadora.</returns>
        [HttpGet("obtenerPujaGanadora/{idSubasta}")]
        public async Task<IActionResult> ConsultarPujaGanadora([FromRoute] Guid idSubasta)
        {
            var resultado = await _mediator.Send(new ConsultarMayorPujaSubastaQuery(idSubasta));
            return Ok(resultado);
        }

        /// <summary>
        /// Endpoint encargado de obtener las pujas de una subasta.
        /// </summary>
        /// <param name="idSubasta">Parametro que corresponde al ID  de la subasta a consultar las pujas.</param>
        /// <returns>Retorna una lista de objeto Puja con los detalles de la puja.</returns>
        [HttpGet("obtenerPujasSubasta/{idSubasta}")]
        public async Task<IActionResult> ConsultarPujasSubasta([FromRoute] Guid idSubasta)
        {
            var resultado = await _mediator.Send(new ConsultarPujasSubastaQuery(idSubasta));
            return Ok(resultado);
        }

        /// <summary>
        /// Endpoint encargado de obtener las pujas de un usuario en una subasta.
        /// </summary>
        /// <param name="pujasDto">Parametro que corresponde al ID  de la subasta a consultar las pujas y el correo del usuario.</param>
        /// <returns>Retorna una lista de objeto Puja con los detalles de la puja y el detalle de la subasta.</returns>
        [HttpPost("obtenerPujasSubastaUsuario")]
        public async Task<IActionResult> ConsultarPujasSubastaUsuario([FromBody] ConsultarPujasSubastaUsuarioDTO pujasDto)
        {
            var resultado = await _mediator.Send(new ConsultarPujasSubastaUsuarioQuery(pujasDto));
            return Ok(resultado);
        }

        /// <summary>
        /// Endpoint encargado de obtener las pujas de un usuario en todas las subastas.
        /// </summary>
        /// <param name="correo">Parametro que corresponde al correo del usuario.</param>
        /// <returns>Retorna una lista de objeto Puja con los detalles de la puja y el detalle de la subasta.</returns>
        [HttpGet("obtenerPujasUsuario/{correo}")]
        public async Task<IActionResult> ConsultarPujasUsuario([FromRoute] string correo)
        {
            var resultado = await _mediator.Send(new ConsultarPujasUsuarioQuery(correo));
            return Ok(resultado);
        }
    }
}
