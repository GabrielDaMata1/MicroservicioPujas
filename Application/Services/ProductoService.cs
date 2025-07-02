using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Application.DTOs;
using Domain.Entities;
using Domain.Interfaces;
using Domain.Value_Object;

namespace Application.Service
{
    public class ProductoService: IProductoService
    {
        private readonly HttpClient _httpClient;

        public ProductoService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<Producto> ObtenerProductoPorGuid(Guid idProducto)
        {
            try
            {
                var response = await _httpClient.GetAsync($"http://localhost:5002/api/Productos/consultarProducto/{idProducto}");

                if (!response.IsSuccessStatusCode)
                {
                    return null;
                }

                var contenido = await response.Content.ReadAsStringAsync();
                Console.WriteLine(contenido);
                var dto = JsonSerializer.Deserialize<ProductoDTO>(contenido, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (dto == null)
                {
                    return null;
                }

                var producto = new Producto(
                    dto.Id,
                    new NombreProductoVO(dto.NombreProducto),
                    new DescripcionProductoVO(dto.DescripcionProducto),
                    new ImagenURLProductoVO(dto.ImagenURLProducto),
                    new PrecioBaseProductoVO(dto.PrecioBaseProducto),
                    new CategoriaProductoVO(dto.CategoriaProducto),
                    new EstadoProductoVO(dto.EstadoProducto)

                );

                return producto;
            }
            catch (System.Exception ex)
            {
                return null;
            }
        }


    }
}
