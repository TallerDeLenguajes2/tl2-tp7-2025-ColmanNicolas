using Microsoft.Data.Sqlite;
using SQLitePCL;
using TP7.Models;

namespace TP7.Repositorios
{
    public class PresupuestosRepository
    {
        private readonly string _cadenaConexion = "Data Source=Tienda_final.db";

        //● Crear un nuevo Presupuesto. (recibe un objeto Presupuesto)
        public void InsertarPresupuesto(Presupuesto presupuesto)
        {
            using var conexion = new SqliteConnection(_cadenaConexion);
            conexion.Open();
            using var transaccion = conexion.BeginTransaction();


            string sqlCabecera = "INSERT INTO Presupuestos (NombreDestinatario, FechaCreacion) VALUES (@nombreD, @fechaC)";

            using var cmdCabecera = new SqliteCommand(sqlCabecera, conexion, transaccion);

            cmdCabecera.Parameters.Add(new SqliteParameter("@nombreD", presupuesto.nombreDestinatario));
            cmdCabecera.Parameters.Add(new SqliteParameter("@fechaC", presupuesto.FechaCreacion));

            long nuevoId = (long)cmdCabecera.ExecuteScalar();

            presupuesto.idPresupuesto = (int)nuevoId;

            foreach (var item in presupuesto.PresupuestosDetalle)
            {

                string sqlDetalle = "INSERT INTO PresupuestosDetalle (idPresupuesto, idProducto,Cantidad) VALUES (@idPres, @idProd,@cantidad)";
                using var cmdDetalle = new SqliteCommand(sqlDetalle, conexion, transaccion);

                cmdDetalle.Parameters.Add(new SqliteParameter("@idPres", nuevoId));
                cmdDetalle.Parameters.Add(new SqliteParameter("@idProd", item.producto.idProducto));
                cmdDetalle.Parameters.Add(new SqliteParameter("@cantidad", item.cantidad));

                cmdDetalle.ExecuteNonQuery();

            }
            transaccion.Commit();

        }

        // Listar todos los Presupuestos registrados. (devuelve un List de Presupuestos)
        public List<Presupuesto> ListarPresupuestos()
        {
            List<Presupuesto> presupuestos = new List<Presupuesto>();

            using var conexion = new SqliteConnection(_cadenaConexion);
            conexion.Open();

            string sql = "select idPresupuesto,NombreDestinatario,FechaCreacion, idProducto,Descripcion,Precio, Cantidad from Presupuestos p inner join PresupuestosDetalle pd using (idPresupuesto) inner join Producto prod using (idProducto) ORDER BY p.idPresupuesto";

            using var cmd = new SqliteCommand(sql, conexion);

            using var lector = cmd.ExecuteReader();

            bool mismoPresupuesto = false;

            Producto auxProducto = null;
            Presupuesto auxPresupuesto = null;
            int auxIdPresupuesto = -1;

            while (lector.Read())
            {
                int idPresupuestoActual = Convert.ToInt32(lector["idPresupuesto"]);

                if (idPresupuestoActual != auxIdPresupuesto)
                {

                    if (auxIdPresupuesto != -1)  // en la primera lectura evito hacer un Add del nuevo presupuesto
                    {
                        presupuestos.Add(auxPresupuesto);  // agrego el presupuesto al leer que empieza una nueva fila de presupuesto
                    }

                    auxProducto = new Producto(Convert.ToInt32(lector["idProducto"]), lector["Descripcion"].ToString(), Convert.ToInt32(lector["Precio"]));
                    PresupuestoDetalle detalle = new PresupuestoDetalle(auxProducto, Convert.ToInt32(lector["Cantidad"]));
                    List<PresupuestoDetalle> detalles = [detalle];

                    auxPresupuesto = new Presupuesto(Convert.ToInt32(lector["idPresupuesto"]), lector["NombreDestinatario"].ToString(),
                    DateOnly.FromDateTime(Convert.ToDateTime(lector["FechaCreacion"])), detalles);
                }
                else
                {
                    PresupuestoDetalle detalle = new PresupuestoDetalle(auxProducto, Convert.ToInt32(lector["Cantidad"]));
                    auxPresupuesto.AgregarPresupuestoDetalle(detalle);
                }
            }
            return presupuestos;
        }

        //● Obtener detalles de un Presupuesto por su ID. (recibe un Id y devuelve un Presupuesto con sus productos y cantidades)
        public Presupuesto BuscarPresupuestoPorId(int id)
        {
            Presupuesto presupuesto = null;

            using var conexion = new SqliteConnection(_cadenaConexion);
            conexion.Open();

            string sql = "select idPresupuesto,NombreDestinatario,FechaCreacion, idProducto,Descripcion,Precio, Cantidad from Presupuestos p inner join PresupuestosDetalle pd using (idPresupuesto) inner join Producto prod using (idProducto) WHERE idPresupuesto = @idPres";

            using var cmd = new SqliteCommand(sql, conexion);

            cmd.Parameters.Add(new SqliteParameter("@idPres", id));

            using var lector = cmd.ExecuteReader();

            while (lector.Read())
            {
                if (presupuesto == null)
                {
                    presupuesto = new Presupuesto(
                        lector.GetInt32(Convert.ToInt32(lector["idPresupuesto"])),
                        lector["NombreDestinatario"].ToString(),
                        DateOnly.FromDateTime(Convert.ToDateTime(lector["FechaCreacion"])),
                        new List<PresupuestoDetalle>()
                    );
                }

                Producto productoDeEstaFila = new Producto(
                Convert.ToInt32(lector["idProducto"]),
                lector["Descripcion"].ToString(),
                Convert.ToInt32(lector["Precio"]));

                PresupuestoDetalle detalleDeEstaFila = new PresupuestoDetalle(
                    productoDeEstaFila,
                    Convert.ToInt32(lector["Cantidad"])
                );

                // 4. Añadir el nuevo detalle a la lista
                presupuesto.AgregarPresupuestoDetalle(detalleDeEstaFila);

            }
            return presupuesto;
        }

        public void AgregarProductoAPresupuesto(int idProducto, int idPresupuesto)
        {
            using var conexion = new SqliteConnection(_cadenaConexion);
            conexion.Open();
            using var transaccion = conexion.BeginTransaction();


            string sqlUpdate = @"
            UPDATE PresupuestosDetalle 
            SET Cantidad = Cantidad + 1 
            WHERE idPresupuesto = @idPres AND idProducto = @idProd";

            using var cmdUpdate = new SqliteCommand(sqlUpdate, conexion, transaccion);

            cmdUpdate.Parameters.Add(new SqliteParameter("@idPres", idPresupuesto));
            cmdUpdate.Parameters.Add(new SqliteParameter("@idProd", idProducto));

            int filasAfectadas = cmdUpdate.ExecuteNonQuery();

            if (filasAfectadas == 0)
            {
                string sqlInsert = @"
                INSERT INTO PresupuestosDetalle (idPresupuesto, idProducto, Cantidad) 
                VALUES (@idPres, @idProd, 1)";

                using var cmdInsert = new SqliteCommand(sqlInsert, conexion, transaccion);
                cmdInsert.Parameters.Add(new SqliteParameter("@idPres", idPresupuesto));
                cmdInsert.Parameters.Add(new SqliteParameter("@idProd", idProducto));

                cmdInsert.ExecuteNonQuery();

                transaccion.Commit();
            }

        }

        public void EliminarPresupuesto(int id)
        {
            using var conexion = new SqliteConnection(_cadenaConexion);
            conexion.Open();

            string sqlDetalle = "delete from PresupuestosDetalle where idPresupuesto = @idPres";
            using var cmdDetalle = new SqliteCommand(sqlDetalle, conexion);
            cmdDetalle.Parameters.Add(new SqliteParameter("@idPres", id));
            // elimino registros donde este idpresupuesto como fk
            cmdDetalle.ExecuteNonQuery();

            string sqlPresupuesto = "delete from Presupuestos where idPresupuesto = @idPres";
            using var cmdPresupuesto = new SqliteCommand(sqlPresupuesto, conexion);
            cmdPresupuesto.Parameters.Add(new SqliteParameter("@idPres", id));
            // elimino el presupuesto
            cmdPresupuesto.ExecuteNonQuery();

        }
    }
}




● Agregar un producto y una cantidad a un presupuesto (recibe un Id)
● Eliminar un Presupuesto por ID*/