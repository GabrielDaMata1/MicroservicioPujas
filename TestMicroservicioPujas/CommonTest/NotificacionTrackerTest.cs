using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Commons;

namespace TestMicroservicioPujas.CommonTest
{
    public class NotificacionTrackerTest
    {
        [Fact]
        public void YaFueEnviada_DeberiaRetornarFalse_CuandoNoSeHaRegistrado()
        {
            var tracker = new NotificacionTracker();
            var usuarioId = "usuario123";
            var subastaId = Guid.NewGuid();

            var resultado = tracker.YaFueEnviada(usuarioId, subastaId);

            Assert.False(resultado);
        }

        [Fact]
        public void YaFueEnviada_DeberiaRetornarTrue_CuandoYaSeHaRegistrado()
        {
            var tracker = new NotificacionTracker();
            var usuarioId = "usuario456";
            var subastaId = Guid.NewGuid();

            tracker.Registrar(usuarioId, subastaId);
            var resultado = tracker.YaFueEnviada(usuarioId, subastaId);

            Assert.True(resultado);
        }

        [Fact]
        public void Registrar_DeberiaGuardarClaveCorrectamente()
        {
            var tracker = new NotificacionTracker();
            var usuarioId = "usuario789";
            var subastaId = Guid.NewGuid();
            var claveEsperada = $"{usuarioId}:{subastaId}";

            tracker.Registrar(usuarioId, subastaId);

            var resultado = tracker.YaFueEnviada(usuarioId, subastaId);
            Assert.True(resultado);
        }

    }
}
