using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Events;
using Domain.Interfaces;
using MassTransit;

namespace Infrastructure.Consumer
{
    public class PujaRegistradaConsumer : IConsumer<PujaRegistradaEvent>
    {
        private readonly IPujaService _pujaService;

        public PujaRegistradaConsumer(IPujaService pujaService)
        {
            _pujaService = pujaService;
        }
        public async Task Consume(ConsumeContext<PujaRegistradaEvent> context)
        {
            await _pujaService.RegistrarPujaMongoAsync(context.Message.puja);

        }
    }
}
