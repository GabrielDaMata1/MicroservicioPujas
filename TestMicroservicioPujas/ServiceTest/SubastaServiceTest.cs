using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Services;
using Moq.Protected;
using Moq;

namespace TestMicroservicioPujas.ServicesTest
{
    public class SubastaServiceTest
    {
        private const string BaseUrl = "http://localhost:5003/";
        private const string Endpoint = "api/Subastas/obtenerSubasta/";

        private HttpClient CrearHttpClient(Mock<HttpMessageHandler> handlerMock)
        {
            return new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri(BaseUrl)
            };
        }

        [Fact]
        public async Task ObtenerSubastaPorGuid_DeberiaRetornarSubasta_CuandoRespuestaEsExitosa()
        {
            var idSubasta = Guid.NewGuid();
            var idProducto = Guid.NewGuid();
            var idUsuario = Guid.NewGuid();

            var dto = new SubastaDTO
            {
                Id = idSubasta,
                nombreSubasta = "Subasta de prueba",
                descripcionSubasta = "Descripción",
                idProductoSubasta = idProducto,
                fechaInicioSubasta = DateTime.UtcNow.AddDays(-1),
                fechaFinSubasta = DateTime.UtcNow.AddDays(1),
                incrementoMinimoSubasta = 10,
                precioReservaSubasta = 100,
                estadoSubasta = "Activa",
            };

            var json = JsonSerializer.Serialize(dto);
            var expectedUrl = $"{BaseUrl}{Endpoint}{idSubasta}";

            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get &&
                        req.RequestUri.ToString() == expectedUrl), ItExpr.IsAny<CancellationToken>()).ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(json)
                });

            var httpClient = CrearHttpClient(handlerMock);
            var service = new SubastaService(httpClient);

            var resultado = await service.ObtenerSubastaPorGuid(idSubasta);

            Assert.NotNull(resultado);
            Assert.Equal(idSubasta, resultado.Id);
            Assert.Equal("Subasta de prueba", resultado.nombreSubasta.Nombre);
            Assert.Equal("Descripción", resultado.descripcionSubasta.descripcion);
            Assert.Equal(idProducto, resultado.idProductoSubasta);
            Assert.Equal(10, resultado.incrementoMinimoSubasta.incrementoMinimo);
            Assert.Equal(100, resultado.precioReservaSubasta.precioReserva);
            Assert.Equal("Activa", resultado.estadoSubasta.estado);
        }

        [Fact]
        public async Task ObtenerSubastaPorGuid_DeberiaRetornarNull_CuandoRespuestaEsFallida()
        {
            var idSubasta = Guid.NewGuid();
            var expectedUrl = $"{BaseUrl}{Endpoint}{idSubasta}";

            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && req.RequestUri.ToString() == expectedUrl),
                    ItExpr.IsAny<CancellationToken>()).ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.NotFound
                });

            var httpClient = CrearHttpClient(handlerMock);
            var service = new SubastaService(httpClient);

            var resultado = await service.ObtenerSubastaPorGuid(idSubasta);

            Assert.Null(resultado);
        }

        [Fact]
        public async Task ObtenerSubastaPorGuid_DeberiaRetornarNull_CuandoContenidoEsInvalido()
        {
            var idSubasta = Guid.NewGuid();
            var expectedUrl = $"{BaseUrl}{Endpoint}{idSubasta}";

            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && req.RequestUri.ToString() == expectedUrl),
                    ItExpr.IsAny<CancellationToken>()).ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("contenido-no-json")
                });

            var httpClient = CrearHttpClient(handlerMock);
            var service = new SubastaService(httpClient);

            var resultado = await service.ObtenerSubastaPorGuid(idSubasta);

            Assert.Null(resultado);
        }

        [Fact]
        public async Task ObtenerSubastaPorGuid_DeberiaRetornarNull_CuandoOcurreExcepcion()
        {
            var idSubasta = Guid.NewGuid();
            var expectedUrl = $"{BaseUrl}{Endpoint}{idSubasta}";

            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>()).ThrowsAsync(new HttpRequestException("Error de red"));

            var httpClient = CrearHttpClient(handlerMock);
            var service = new SubastaService(httpClient);

            var resultado = await service.ObtenerSubastaPorGuid(idSubasta);

            Assert.Null(resultado);
        }

    }
}
