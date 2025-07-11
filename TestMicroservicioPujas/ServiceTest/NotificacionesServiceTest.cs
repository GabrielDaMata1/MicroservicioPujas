using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Application.Service;
using Microsoft.Extensions.Configuration;
using Moq.Protected;
using Moq;

namespace TestMicroservicioPujas.ServiceTest
{
    public class NotificacionesServiceTest
    {
        private const string BaseUrl = "http://localhost:5287/";
        private const string Endpoint = "api/Notification/enviarCorreoUsuarioPujaAutomaticaAcabada";

        private HttpClient CrearHttpClient(Mock<HttpMessageHandler> handlerMock)
        {
            return new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri(BaseUrl)
            };
        }

        [Fact]
        public async Task EnviarCorreoUsuarioPujaAutomaticaFinalizada_DeberiaRetornarTrue_CuandoRespuestaEsExitosa()
        {
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Post &&
                        req.RequestUri.ToString() == $"{BaseUrl}{Endpoint}"),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK
                });

            var httpClient = CrearHttpClient(handlerMock);
            var service = new NotificacionService(httpClient, Mock.Of<IConfiguration>());

            var result = await service.EnviarCorreoUsuarioPujaAutomaticaFinalizada("usuario@test.com", "Subasta X", "Producto Y", 500);

            Assert.True(result);
        }

        [Fact]
        public async Task EnviarCorreoUsuarioPujaAutomaticaFinalizada_DeberiaRetornarFalse_CuandoRespuestaEsFallida()
        {
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.BadRequest
                });

            var httpClient = CrearHttpClient(handlerMock);
            var service = new NotificacionService(httpClient, Mock.Of<IConfiguration>());

            var result = await service.EnviarCorreoUsuarioPujaAutomaticaFinalizada("fallo@correo.com", "Subasta Y", "Producto Z", 300);

            Assert.False(result);
        }

        [Fact]
        public async Task EnviarCorreoUsuarioPujaAutomaticaFinalizada_DeberiaRetornarFalse_CuandoOcurreExcepcion()
        {
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ThrowsAsync(new HttpRequestException("Error de red"));

            var httpClient = CrearHttpClient(handlerMock);
            var service = new NotificacionService(httpClient, Mock.Of<IConfiguration>());

            var result = await service.EnviarCorreoUsuarioPujaAutomaticaFinalizada("error@correo.com", "Subasta Z", "Producto W", 100);

            Assert.False(result);
        }

    }
}
