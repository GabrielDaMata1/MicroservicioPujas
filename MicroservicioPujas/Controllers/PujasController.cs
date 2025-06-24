using Application.Command;
using Application.DTOs;
using Application.Query;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MicroservicioPujas.Controllers
{

    [ApiController]
    [Route("api/Pujas")]
    public class PujasController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PujasController(IMediator mediator)
        {
            _mediator = mediator;
        }

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

        [HttpGet("obtenerPujaGanadora/{idSubasta}")]
        public async Task<IActionResult> RegistrarPuja([FromRoute] Guid idSubasta)
        {
            var resultado = await _mediator.Send(new ConsultarMayorPujaSubastaQuery(idSubasta));
            return Ok(resultado);
        }
    }
}
