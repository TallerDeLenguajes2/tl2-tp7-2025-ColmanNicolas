using Microsoft.AspNetCore.Mvc;
using TP7.Repositorios;
using TP7.Models;

namespace TP7.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PresupuestoController : ControllerBase
    {
        private readonly PresupuestosRepository presupuestosRepository;

        public PresupuestoController()
        {
            presupuestosRepository = new PresupuestosRepository();
        }


        // ● POST /api/Presupuesto: Permite crear un Presupuesto.
        [HttpPost()]
        public IActionResult InsertarPresupuesto([FromBody] Presupuesto presupuesto)
        {
            presupuestosRepository.InsertarPresupuesto(presupuesto);

            return Ok("Presupuesto registrado");
        }
        // ● POST /api/Presupuesto/{ id}/ProductoDetalle: Permite agregar un Producto existente y una cantidad al presupuesto.
        [HttpPost("{idPresupuesto}")]
        public IActionResult AgregarProductosAPresupuesto([FromRoute]int idPresupuesto, [FromBody] PresupuestoDetalle presupuestoDetalle)
        {
            presupuestosRepository.AgregarProductoAlPresupuesto(idPresupuesto, presupuestoDetalle);
            return Ok("Operacion completada con exito");

        }

        // ● GET /api/Presupuesto/{ id}: Obtener detalles de un Presupuesto por su ID.
        [HttpGet("{id}")]
        public IActionResult ObtenerPresupuestoPorId(int id)
        {
            Presupuesto presupuesto = presupuestosRepository.BuscarPresupuestoPorId(id);

            if (presupuesto == null)
            {
                return NotFound($"No se encontró el presupuesto con ID {id}.");
            }
            return Ok(presupuesto);
        }

        // ● GET /api/presupuesto: Permite listar los presupuestos existentes.
        [HttpGet()]
        public IActionResult ObtenerPresupuestos()
        {
            List<Presupuesto> presupuestos = presupuestosRepository.ListarPresupuestos();

            /*if (presupuestos.Count == 0)
            {
                return Ok("No se encontraron presupuestos regitrados");
            }*/
            return Ok(presupuestos);

        }
        // ● DETELE /api/Presupuesto/{ id}: Permite eliminar un Presupuesto.
        [HttpDelete("{id}")]
        public IActionResult EliminarPresupuesto(int id)
        {
            presupuestosRepository.EliminarPresupuesto(id);
            return NoContent();// HTTP 204 (Eliminación exitosa)

        }
    }
}