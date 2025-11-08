using Microsoft.Data.Sqlite;
using TP7.Models;

namespace TP7.Repositorios
{

    public class ProductoRepository
    {
        private readonly string _cadenaConexion = "Data Source=Tienda_final.db";

        // ● Crear un nuevo Producto. (recibe un objeto Producto)
        public void InsertarProducto(Producto producto)
        {
            using var conexion = new SqliteConnection(_cadenaConexion);
            conexion.Open();

            string sql = "INSERT INTO Productos (Descripcion, Precio) VALUES (@descripcion, @precio)";

            using var comando = new SqliteCommand(sql, conexion);

            comando.Parameters.Add(new SqliteParameter("@descripcion", producto.descripcion));
            comando.Parameters.Add(new SqliteParameter("@precio", producto.precio));

            comando.ExecuteNonQuery();
        }

        // ● Modificar un Producto existente. (recibe un Id y un objeto Producto)
        public void ActualizarProducto(int id, Producto producto)
        {
            using var conexion = new SqliteConnection(_cadenaConexion);
            conexion.Open();

            string sql = "UPDATE Productos SET Descripcion = @desc, Precio = @prec WHERE idProducto = @id";

            using var comando = new SqliteCommand(sql, conexion);

            comando.Parameters.Add(new SqliteParameter("@desc", producto.descripcion));
            comando.Parameters.Add(new SqliteParameter("@prec", producto.precio));

            comando.Parameters.Add(new SqliteParameter("@id", id));

            comando.ExecuteNonQuery();
        }

        //● Listar todos los Productos registrados. (devuelve un List de Producto)
        public List<Producto> ListarProductos()
        {
            List<Producto> productos = new List<Producto>();
            using var conexion = new SqliteConnection(_cadenaConexion);
            conexion.Open();

            string sql = "select * from Productos";

            using var comando = new SqliteCommand(sql, conexion);

            using var lector = comando.ExecuteReader();

            while (lector.Read())
            {
                var p = new Producto(Convert.ToInt32(lector["idProducto"])
                , lector["Descripcion"].ToString()
                , Convert.ToInt32(lector["Precio"]));

                productos.Add(p);
            }

            return productos;
        }
        //● Obtener detalles de un Productos por su ID. (recibe un Id y devuelve un Producto)
        public Producto BuscarProductoPorId(int id)
        {
            using var conexion = new SqliteConnection(_cadenaConexion);
            conexion.Open();

            string sql = "select idProducto, Descripcion, Precio from Productos where idProducto =@idProd";

            using var comando = new SqliteCommand(sql, conexion);

            comando.Parameters.Add(new SqliteParameter("@idProd", id));

            using var lector = comando.ExecuteReader();

            if (lector.Read())
            {
                Producto producto = new Producto(id,
                lector["Descripcion"].ToString(),
                Convert.ToInt32(lector["Precio"]));

                return producto;
            }

            return null;
        }
        //● Eliminar un Producto por ID
        public int EliminarProducto(int id)
        {
            using var conexion = new SqliteConnection(_cadenaConexion);
            conexion.Open();

            string sql = "DELETE From Productos WHERE idProducto = @idProd";

            using var comando = new SqliteCommand(sql, conexion);

            comando.Parameters.Add(new SqliteParameter("@idProd", id));

            int filasAfectadas = comando.ExecuteNonQuery();

            return filasAfectadas;
        }
    }
}
