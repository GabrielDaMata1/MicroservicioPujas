using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Service;
using Moq.Protected;
using Moq;

namespace TestMicroservicioPujas.ServiceTest
{
    public class ProductoServiceTest
    {
        private const string BaseUrl = "http://localhost:5002/";
        private const string EndpointProducto = "api/Productos/consultarProducto/";

        private HttpClient CrearHttpClient(Mock<HttpMessageHandler> handlerMock)
        {
            return new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri(BaseUrl)
            };
        }

        [Fact]
        public async Task ObtenerProductoPorGuid_DeberiaRetornarProducto_CuandoRespuestaEsExitosa()
        {
            var idProducto = Guid.NewGuid();
            var expectedUrl = $"{BaseUrl}{EndpointProducto}{idProducto}";

            var dto = new ProductoDTO
            {
                Id = idProducto,
                NombreProducto = "Producto 1",
                DescripcionProducto = "Descripción",
                ImagenURLProducto = "http://imagen.com",
                PrecioBaseProducto = 100,
                CategoriaProducto = "Electrónica",
                EstadoProducto = "Disponible"
            };

            var contenidoJson = JsonSerializer.Serialize(dto);

            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Get &&
                        req.RequestUri.ToString() == expectedUrl
                    ),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(contenidoJson, Encoding.UTF8, "application/json")
                });

            var httpClient = CrearHttpClient(handlerMock);
            var service = new ProductoService(httpClient);

            var resultado = await service.ObtenerProductoPorGuid(idProducto);

            Assert.NotNull(resultado);
            Assert.Equal(dto.Id, resultado.Id);
            Assert.Equal(dto.NombreProducto, resultado.NombreProducto.Nombre);
        }

        [Fact]
        public async Task ObtenerProductoPorGuid_DeberiaRetornarNull_CuandoRespuestaEsFallida()
        {
            var idProducto = Guid.NewGuid();
            var expectedUrl = $"{BaseUrl}{EndpointProducto}{idProducto}";

            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Get &&
                        req.RequestUri.ToString() == expectedUrl
                    ),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.NotFound
                });

            var httpClient = CrearHttpClient(handlerMock);
            var service = new ProductoService(httpClient);

            var resultado = await service.ObtenerProductoPorGuid(idProducto);

            Assert.Null(resultado);
        }

        [Fact]
        public async Task ObtenerProductoPorGuid_DeberiaRetornarNull_CuandoContenidoEsInvalido()
        {
            var idProducto = Guid.NewGuid();
            var expectedUrl = $"{BaseUrl}{EndpointProducto}{idProducto}";

            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Get &&
                        req.RequestUri.ToString() == expectedUrl
                    ),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("contenido inválido")
                });

            var httpClient = CrearHttpClient(handlerMock);
            var service = new ProductoService(httpClient);

            var resultado = await service.ObtenerProductoPorGuid(idProducto);

            Assert.Null(resultado);
        }

        [Fact]
        public async Task ObtenerProductoPorGuid_DeberiaRetornarNull_CuandoOcurreExcepcion()
        {
            var idProducto = Guid.NewGuid();
            var expectedUrl = $"{BaseUrl}{EndpointProducto}{idProducto}";

            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Get &&
                        req.RequestUri.ToString() == expectedUrl
                    ),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ThrowsAsync(new HttpRequestException("Error de red"));

            var httpClient = CrearHttpClient(handlerMock);
            var service = new ProductoService(httpClient);

            var resultado = await service.ObtenerProductoPorGuid(idProducto);

            Assert.Null(resultado);
        }

    }
}
