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
            // 1. Usamos un Diccionario para rastrear los presupuestos que ya vimos
            var presupuestosDict = new Dictionary<int, Presupuesto>();

            using var conexion = new SqliteConnection(_cadenaConexion);
            conexion.Open();

            string sql = @"
        SELECT 
            p.idPresupuesto, p.NombreDestinatario, p.FechaCreacion,
            pd.Cantidad,
            prod.idProducto, prod.Descripcion, prod.Precio
        FROM Presupuestos p
        INNER JOIN PresupuestosDetalle pd ON p.idPresupuesto = pd.idPresupuesto
        INNER JOIN Productos prod ON pd.idProducto = prod.idProducto
        ORDER BY p.idPresupuesto";

            using var cmd = new SqliteCommand(sql, conexion);
            using var lector = cmd.ExecuteReader();

            while (lector.Read())
            {
                int idPresupuestoActual = Convert.ToInt32(lector["idPresupuesto"]);
                Presupuesto presupuestoActual;

                // 2. Verificamos si ya tenemos este presupuesto en el diccionario
                if (!presupuestosDict.TryGetValue(idPresupuestoActual, out presupuestoActual))
                {
                    // 3. Si NO existe, es nuevo. Lo creamos y lo añadimos al diccionario.
                    presupuestoActual = new Presupuesto(
                        idPresupuestoActual,
                        lector["NombreDestinatario"].ToString(),
                        DateOnly.FromDateTime(Convert.ToDateTime(lector["FechaCreacion"])),
                        new List<PresupuestoDetalle>() // Empezamos con lista vacía
                    );
                    presupuestosDict.Add(idPresupuestoActual, presupuestoActual);
                }

                // 4. Creamos el Producto y el Detalle de ESTA fila
                Producto productoDeEstaFila = new Producto(
                    Convert.ToInt32(lector["idProducto"]),
                    lector["Descripcion"].ToString(),
                    Convert.ToInt32(lector["Precio"])
                );

                PresupuestoDetalle detalleDeEstaFila = new PresupuestoDetalle(
                    productoDeEstaFila,
                    Convert.ToInt32(lector["Cantidad"])
                );

                // 5. Añadimos el detalle al presupuesto (que ya sea que existía o lo acabamos de crear)
                presupuestoActual.AgregarPresupuestoDetalle(detalleDeEstaFila);
            }

            // 6. Al final, solo devolvemos todos los valores del diccionario.
            //    (Esto soluciona el "Error 2" de perder el último ítem)
            return presupuestosDict.Values.ToList();
        }

        //● Obtener detalles de un Presupuesto por su ID. (recibe un Id y devuelve un Presupuesto con sus productos y cantidades)
        public Presupuesto BuscarPresupuestoPorId(int id)
        {
            Presupuesto presupuesto = null;

            using var conexion = new SqliteConnection(_cadenaConexion);
            conexion.Open();

            string sql = "select idPresupuesto,NombreDestinatario,FechaCreacion, idProducto,Descripcion,Precio, Cantidad from Presupuestos p inner join PresupuestosDetalle pd using (idPresupuesto) inner join Productos prod using (idProducto) WHERE idPresupuesto = @idPres";

            using var cmd = new SqliteCommand(sql, conexion);

            cmd.Parameters.Add(new SqliteParameter("@idPres", id));

            using var lector = cmd.ExecuteReader();

            while (lector.Read())
            {
                if (presupuesto == null)
                {
                    presupuesto = new Presupuesto(
                        // ✅ SOLUCIÓN:
                        Convert.ToInt32(lector["idPresupuesto"]),
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

        public void AgregarProductoAlPresupuesto(int idPresupuesto, PresupuestoDetalle presupuestoDetalle)
        {
            using var conexion = new SqliteConnection(_cadenaConexion);
            conexion.Open();
            using var transaccion = conexion.BeginTransaction();


            string sqlUpdate = @"
            UPDATE PresupuestosDetalle 
            SET Cantidad = @Canti  
            WHERE idPresupuesto = @idPres AND idProducto = @idProd";

            using var cmdUpdate = new SqliteCommand(sqlUpdate, conexion, transaccion);

            cmdUpdate.Parameters.Add(new SqliteParameter("@idPres", idPresupuesto));
            cmdUpdate.Parameters.Add(new SqliteParameter("@idProd", presupuestoDetalle.producto.idProducto));
            cmdUpdate.Parameters.Add(new SqliteParameter("@Canti", presupuestoDetalle.cantidad));

            int filasAfectadas = cmdUpdate.ExecuteNonQuery();

            if (filasAfectadas == 0)
            {
                string sqlInsert = @"
                INSERT INTO PresupuestosDetalle (idPresupuesto, idProducto, Cantidad) 
                VALUES (@idPres, @idProd, @Canti)";

                using var cmdInsert = new SqliteCommand(sqlInsert, conexion, transaccion);
                cmdInsert.Parameters.Add(new SqliteParameter("@idPres", idPresupuesto));
                cmdInsert.Parameters.Add(new SqliteParameter("@idProd", presupuestoDetalle.producto.idProducto));
                cmdUpdate.Parameters.Add(new SqliteParameter("@Canti", presupuestoDetalle.cantidad));

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
