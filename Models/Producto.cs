
namespace TP7.Models
{
    public class Producto
    {

        public int idProducto { get; set; }
        public string descripcion { get; set; }
        public int precio { get; set; }


        public Producto(string desc, int prec)
        {
            this.idProducto = 0;
            this.descripcion = desc;
            this.precio = prec;
        }

        public Producto(int id, string desc, int prec)
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