using Microsoft.AspNetCore.Mvc;
using TP7.Repositorios;
using TP7.Models;

namespace TP7.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class ProductoController : ControllerBase
    {

        private readonly ProductoRepository productoRepository;
        public ProductoController()
        {
            productoRepository = new ProductoRepository();

        }

        [HttpPost()]
        public ActionResult<string> AltaProducto([FromBody] Producto nuevoProducto)
        {
            productoRepository.InsertarProducto(nuevoProducto);
            return Ok("Producto dado de alta exitosamente");
        }

        [HttpPut("{id}")]
        public IActionResult ActualizarNombre(int id, [FromBody] string nombreActualizado)
        {

            Producto productoBD = productoRepository.BuscarProductoPorId(id);

            if (productoBD != null)
            {
                productoBD.descripcion = nombreActualizado;
                productoRepository.ActualizarProducto(id, productoBD);

                return Ok(new { msj = "Producto actualizado con exito", productoBD });
            }
            else
            {
                return NotFound($"No se encontró el producto con ID {id} para actualizar.");
            }
        }
        [HttpGet()]
        public IActionResult ListarProductos()
        {
            List<Producto> productos = productoRepository.ListarProductos();

            return Ok(productos);
        }

        [HttpGet("{id}")]
        public IActionResult ObtenerProductoPorId(int id)
        {
            Producto producto = productoRepository.BuscarProductoPorId(id);

            if (producto != null)
            {
                return Ok(producto);
            }

            return NotFound($"No se encontró el producto con ID {id}");
        }
        [HttpDelete("{id}")]
        public IActionResult EliminarProducto(int id)
        {
            int filasAfectadas = productoRepository.EliminarProducto(id);

            if (filasAfectadas > 0)
            {
                return NoContent();// HTTP 204 (Eliminación exitosa)
            }
            else
            {
                return NotFound($"No se encontró el producto con ID {id} para eliminar.");
            }
        }
    }
}
