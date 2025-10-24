namespace TP7.Models
{
    public class Presupuesto
    {
        public 
            int idPresupuesto {get;set;}
            string nombreDestinatario {get;set;}
            DateOnly FechaCreacion {get;set;}
            List<PresupuestoDetalle> presupuestosDetalle {get;set;}

         public int MontoPresupuesto(){
            int presupuesto =0;
            foreach (var itemDetalle in presupuestosDetalle)
            {
                presupuesto+= itemDetalle.producto.ObtenerPrecio();
            }
            return presupuesto;
         }
         int MontoPresupuestoConIVA(){
            return (int) (this.MontoPresupuesto() * 1.21); 
         }
         int CantidaDeProductos(){
            int cantidad =0;
            foreach (var itemDetalle in presupuestosDetalle)
            {
                cantidad += itemDetalle.cantidad;
            }
            return cantidad;
         }
    }
 }