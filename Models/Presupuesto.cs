namespace TP7.Models
{
   public class Presupuesto
   {
      public
          const double IVA = 0.21;
      public int idPresupuesto { get; set; }
      public string nombreDestinatario { get; set; }
      public DateOnly FechaCreacion { get; set; }
      public List<PresupuestoDetalle> PresupuestosDetalle { get; set; }

      public Presupuesto()
      {
         this.idPresupuesto = 0;
         this.nombreDestinatario = "Presupuesto vacio";
         this.FechaCreacion = new DateOnly(1900, 1, 1);
         this.PresupuestosDetalle = [];
      }
      public Presupuesto(string nombre, DateOnly fecha, List<PresupuestoDetalle> presupuestoDetalles)
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
      public void AgregarPresupuestoDetalle(PresupuestoDetalle detalle)
      {
         this.PresupuestosDetalle.Add(detalle);
      }
   }
}