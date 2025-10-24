
namespace TP7.Models
{
    public class Producto
    {

        int idProducto { get; set; }
        string descripcion { get; set; }
        int precio { get; set; }


        Producto() { }

        Producto(int id, string desc, int prec)
        {
            this.idProducto = id;
            this.descripcion = desc;
            this.precio = prec;
        }
        public int ObtenerPrecio()
        {
            return this.precio;
        }

    }
}