namespace TP7.Models
{
   public class Presupuesto
   {
      public
          const double IVA = 0.21;
      int idPresupuesto { get; set; }
      string nombreDestinatario { get; set; }
      DateOnly FechaCreacion { get; set; }
      List<PresupuestoDetalle> PresupuestosDetalle { get; set; }

      public Presupuesto( string nombre, DateOnly fecha, List<PresupuestoDetalle> presupuestoDetalles)
      {
         this.idPresupuesto = 0;
         this.nombreDestinatario = nombre;
         this.FechaCreacion = fecha;
         this.PresupuestosDetalle = presupuestoDetalles;
      }
      public Presupuesto(int id, string nombre, DateOnly fecha, List<PresupuestoDetalle> presupuestoDetalles)
      {
         this.idPresupuesto = id;
         this.nombreDestinatario = nombre;
         this.FechaCreacion = fecha;
         this.PresupuestosDetalle = presupuestoDetalles;
      }
      public int MontoPresupuesto()
      {
         int presupuesto = 0;
         foreach (var itemDetalle in PresupuestosDetalle)
         {
            presupuesto += itemDetalle.producto.ObtenerPrecio();
         }
         return presupuesto;
      }
      public int MontoPresupuestoConIVA()
      {
         return (int)(this.MontoPresupuesto() * (1 + IVA));
      }
      public int CantidaDeProductos()
      {
         int cantidad = 0;
         foreach (var itemDetalle in PresupuestosDetalle)
         {
            cantidad += itemDetalle.cantidad;
         }
         return cantidad;
      }
   }
}