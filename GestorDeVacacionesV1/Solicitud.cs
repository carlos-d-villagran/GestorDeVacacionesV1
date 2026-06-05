using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace GestorDeVacacionesV1
{
    class Solicitud
    {
        private string conexion = "Data Source=GestorVacaciones.db;Version=3;BusyTimeout=5000;";
        public bool EsAsueto(string fecha)
        {
            using (SQLiteConnection db = new SQLiteConnection(conexion))
            {
                db.Open();
                string sql = "SELECT COUNT(*) FROM Asuetos WHERE Fecha = @fecha";
                int count;
                using (SQLiteCommand comando = new SQLiteCommand(sql, db))
                {
                    comando.Parameters.AddWithValue("@fecha", fecha);
                    count = Convert.ToInt32(comando.ExecuteScalar());
                }
                return count > 0;
            }
        }
        public bool FechaOcupada(string fecha, int empleadoActual)
        {
            using (SQLiteConnection db = new SQLiteConnection(conexion))
            {
                db.Open();
                string sql = "SELECT COUNT(*) FROM DetalleDiasSolicitud d INNER JOIN SolicitudesVacaciones s "
                    + "ON d.SolicitudId = s.Id WHERE d.Fecha = @fecha AND s.Estado = 'Aprobada' AND s.EmpleadoId != @empleadoId";
                int count;
                using (SQLiteCommand comando = new SQLiteCommand(sql,db))
                {
                    comando.Parameters.AddWithValue("@fecha",fecha);
                    comando.Parameters.AddWithValue("@empleadoId",empleadoActual);
                    count = Convert.ToInt32(comando.ExecuteScalar());
                }
                return count > 0;
            }
        }
        public void CrearSolicitud(int empleadoId, List<string> dias, string motivo)
        {
            using (SQLiteConnection db = new SQLiteConnection(conexion))
            {
                db.Open();
                string sqlSolicitud = "INSERT INTO SolicitudesVacaciones (EmpleadoId, DiasSolicitados, Estado, Motivo, FechaSolicitud) " +
                    "VALUES (@empleadoId, @dias, 'Pendiente', @motivo, @fechaSolicitud)";
                using (SQLiteCommand comando = new SQLiteCommand(sqlSolicitud, db))
                {
                    comando.Parameters.AddWithValue("@empleadoId", empleadoId);
                    comando.Parameters.AddWithValue("@dias", dias.Count);
                    comando.Parameters.AddWithValue("@motivo", motivo);
                    comando.Parameters.AddWithValue("@fechaSolicitud", DateTime.Now.ToString("yyyy-MM-dd"));
                    comando.ExecuteNonQuery();
                }
                long solicitudId = db.LastInsertRowId;
                foreach(string dia in dias)
                {
                    string sqlDetalle = "INSERT INTO DetalleDiasSolicitud (SolicitudId, Fecha) " +
                        "VALUES (@solicitudId, @fecha)";
                    using (SQLiteCommand comandoD = new SQLiteCommand(sqlDetalle, db))
                    {
                        comandoD.Parameters.AddWithValue("@solicitudId",solicitudId);
                        comandoD.Parameters.AddWithValue("@fecha", dia);
                        comandoD.ExecuteNonQuery();
                    }
                }
                Console.WriteLine("Solicitud generada, días solicitados: " + dias.Count);
            }
        }
        public void VerPendiente()
        {
            using (SQLiteConnection db = new SQLiteConnection(conexion))
            {
                db.Open();
                string sql = "SELECT s.Id, e.Nombre, s.DiasSolicitados, s.Motivo, s.FechaSolicitud " +
                    "FROM SolicitudesVacaciones s INNER JOIN Empleados e ON s.EmpleadoId = e.Id WHERE s.Estado = 'Pendiente'";
                using (SQLiteCommand comando = new SQLiteCommand(sql, db))
                using (SQLiteDataReader lector = comando.ExecuteReader())
                {
                    Console.WriteLine("Solicitudes pendientes");
                    bool hay = false;
                    while (lector.Read())
                    {
                        hay = true;
                        Console.WriteLine("Solicitud: " + lector["Id"]);
                        Console.WriteLine("Nombre: " + lector["Nombre"]);
                        Console.WriteLine("Dias Pedidos: " + lector["DiasSolicitados"]);
                        Console.WriteLine("Motivo: " + lector["Motivo"]);
                        Console.WriteLine("Fecha: " + lector["FechaSolicitud"]);
                    }
                    if (!hay)
                    {
                        Console.WriteLine("No hay solicitudes pendientes...");
                    }
                }

            }
        }
        public void VerDiasSolicitud(int solicitudId)
        {
            using (SQLiteConnection db = new SQLiteConnection(conexion))
            {
                db.Open();
                string sql = "SELECT Fecha FROM DetalleDiasSolicitud WHERE SolicitudId = @id ORDER BY Fecha";
                using (SQLiteCommand comando = new SQLiteCommand(sql, db))
                {
                    comando.Parameters.AddWithValue("@id", solicitudId);
                    using (SQLiteDataReader lector =comando.ExecuteReader())
                    {
                        Console.WriteLine("Dias solicitados");
                        while(lector.Read())
                        {
                            Console.WriteLine("-> "+ lector["Fecha"]);
                        }
                    }
                }

            }
        }
        public void Aprobar(int solicitudId, string comentario, Empleado empleado)
        {
            using (SQLiteConnection db = new SQLiteConnection(conexion))
            {
                db.Open();
                string sql = "SELECT EmpleadoId, DiasSolicitados FROM " +
                    "SolicitudesVacaciones WHERE Id = @id AND Estado = 'Pendiente'";
                int empleadoId;
                int diasSolicitados;
                using (SQLiteCommand comando = new SQLiteCommand(sql, db))
                {
                    comando.Parameters.AddWithValue("@id", solicitudId);
                    using (SQLiteDataReader lector = comando.ExecuteReader())
                    {
                        if(!lector.Read())
                        {
                            Console.WriteLine("Solicitud no encontrada...");
                            return;
                        }
                        empleadoId = Convert.ToInt32(lector["EmpleadoId"]);
                        diasSolicitados = Convert.ToInt32(lector["DiasSolicitados"]);
                    }
                }
                int diasDisponibles = empleado.ObtenerDiasDisponibles(empleadoId);
                if(diasDisponibles < diasSolicitados)
                {
                    Console.WriteLine("El empleado no tiene días suficientes. Disponibles: "+ diasDisponibles);
                    return ;
                }
                string sqlUpdate = "UPDATE SolicitudesVacaciones " +
                    "SET Estado = 'Aprobada', ComentarioAdmin = @comentario WHERE Id = @id";
                using (SQLiteCommand comandoU = new SQLiteCommand(sqlUpdate, db))
                {
                    comandoU.Parameters.AddWithValue("@comentario", comentario);
                    comandoU.Parameters.AddWithValue("@id", solicitudId);
                    comandoU.ExecuteNonQuery();
                }
                empleado.DescontarDias(empleadoId, diasSolicitados);
                Console.WriteLine("Solicitud Aprobada. Dias descontados: "+ diasSolicitados);
                
            }
        }
        public void RechazarSolictud(int solicitudId, string comentario)
        {
            using (SQLiteConnection db = new SQLiteConnection(conexion))
            {
                db.Open();
                string sql = "UPDATE SolicitudesVacaciones " +
                    "SET Estado = 'Rechazado', ComentarioAdmin = @comentario WHERE Id = @id AND Estado = 'Pendiente'";
                int filas;
                using (SQLiteCommand comando = new SQLiteCommand(sql, db))
                {
                    comando.Parameters.AddWithValue("@comentario", comentario);
                    comando.Parameters.AddWithValue("@id", solicitudId);
                    filas = comando.ExecuteNonQuery();
                }
                if(filas > 0)
                {
                    Console.WriteLine("Solicitud rechazada");
                }
                else
                {
                    Console.WriteLine("Solicitud no encontrada");
                }
            }
        }
        public void VerHistorial(int empleadoId)
        {
            using (SQLiteConnection db = new SQLiteConnection(conexion))
            {
                db.Open();
                string sql = "SELECT s.Id, s.DiasSolicitados, s.Estado, s.Motivo, s.FechaSolicitud, s.ComentarioAdmin " +
                    "FROM SolicitudesVacaciones s WHERE s.EmpleadoId = @empleadoId ORDER BY s.FechaSolicitud DESC";
                using (SQLiteCommand comando = new SQLiteCommand(sql, db))
                {
                    comando.Parameters.AddWithValue("@empleadoId", empleadoId);
                    using (SQLiteDataReader lector = comando.ExecuteReader())
                    {
                        Console.WriteLine("HISTORIAL DE VACACIONES:");
                        bool hay = false;
                        while(lector.Read())
                        {
                            hay = true;
                            Console.WriteLine("Solicitud: " + lector["Id"]);
                            Console.WriteLine("Días: "+ lector["DiasSolicitados"]);
                            Console.WriteLine("Estado: " + lector["Estado"]);
                            Console.WriteLine("Motivo: " + lector["Motivo"]);
                            Console.WriteLine("Fecha solicitud: " + lector["FechaSolicitud"]);
                            Console.WriteLine("Comentario: " + lector["ComentarioAdmin"]);
                        }
                        if(!hay)
                        {
                            Console.WriteLine("No hay solicitudes registradas");
                        }
                    }
                }
            }
        }
    }
}
