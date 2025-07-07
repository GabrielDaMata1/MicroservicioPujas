using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace Domain.Interfaces
{
    /// <summary>
    /// Clase interface que define las operaciones que se pueden realizar sobre productos, en el Microservicio Producto.
    /// </summary>
    public interface IProductoService
    {
        /// <summary>
        /// Método que se encarga de obtener el producto de un subastador en MongoDB.
        /// </summary>
        /// <param name="idProducto">Parametro que corresponde al ID del producto a consultar</param>
        /// <returns>Retorna el Producto con su detalle</returns>
        Task<Producto> ObtenerProductoPorGuid(Guid idProducto);
    }
}
