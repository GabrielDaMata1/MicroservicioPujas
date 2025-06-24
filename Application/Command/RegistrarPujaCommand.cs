using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
using MediatR;

namespace Application.Command
{
    public class RegistrarPujaCommand : IRequest<bool>
    {
        public RegistrarPujaDTO pujaDTO;

        public RegistrarPujaCommand(RegistrarPujaDTO pujadto)
        {
            pujaDTO = pujadto;

        }
    }
}
