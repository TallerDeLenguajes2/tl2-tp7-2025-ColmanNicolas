namespace TP7.Models
{
    public class PresupuestoDetalle
    {
         
        public Producto producto;
        public int cantidad;

        public PresupuestoDetalle(){
            this.producto = null;
            this.cantidad = 0;
        }
        public PresupuestoDetalle(Producto prod, int cant)
        {
            this.producto = prod;
            this.cantidad = cant;
        }
    }
 }