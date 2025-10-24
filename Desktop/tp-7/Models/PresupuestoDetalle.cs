namespace TP7.Models
{
    public class PresupuestoDetalle
    {
         
        public Producto producto;
        public int cantidad;

        PresupuestoDetalle(Producto prod, int cant){
            this.producto = prod;
            this.cantidad = cant;
        }
    }
 }