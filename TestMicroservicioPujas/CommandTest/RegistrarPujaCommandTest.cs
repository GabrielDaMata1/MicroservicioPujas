using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Command;
using Application.DTOs;

namespace TestMicroservicioPujas.CommandTest
{
    public class RegistrarPujaCommandTest
    {
        [Fact]
        public void Constructor_DeberiaAsignarDTO_Correctamente()
        {
            var dto = new RegistrarPujaDTO
            {
                idSubasta = Guid.NewGuid(),
                correoUsuario = "usuarioPrueba@gmail.com",
                montoPuja = 150,
                montoMaximo = 200,
                tipoPuja = "Manual",
                montoPredeterminado = 10
            };

            var command = new RegistrarPujaCommand(dto);

            Assert.Equal(dto, command.pujaDTO);
            Assert.Equal(dto.idSubasta, command.pujaDTO.idSubasta);
            Assert.Equal(dto.montoPuja, command.pujaDTO.montoPuja);
        }

    }
}
