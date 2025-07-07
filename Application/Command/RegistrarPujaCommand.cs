using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
using MediatR;

namespace Application.Command
{
    /// <summary>
    /// Clase Command que se encarga de enviar la solicitud de para registrar una nueva puja en una subasta.
    /// </summary>
    public class RegistrarPujaCommand : IRequest<bool>
    {
        /// <summary>
        /// Atributo DTO que se encarga de recibir la información de la nueva puja a registrar.
        /// </summary>
        public RegistrarPujaDTO pujaDTO;

        public RegistrarPujaCommand(RegistrarPujaDTO pujadto)
        {
            pujaDTO = pujadto;

        }
    }
}
