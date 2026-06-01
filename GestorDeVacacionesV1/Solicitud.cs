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
        private string conexion = "Data Source=GestorVacaciones.db";
        public bool EsAsueto(string fecha)
        {
            using (SQLiteConnection db = new SQLiteConnection(conexion))
            {
                db.Open();
                string sql = "SELECT COUNT(*)FROM Asuetos WHERE Fecha = @fecha";
                SQLiteCommand comando = new SQLiteCommand(sql, db);
                comando.Parameters.AddWithValue("@fecha", fecha);
                int count = Convert.ToInt32(comando.ExecuteScalar());
                return count > 0;
            }
        }
        public bool FechaOcupada(string fecha, int empleadoActual)
        {
            using (SQLiteConnection db = new SQLiteConnection(conexion))
            {
                db.Open();
                string sql = "SELECT COUNT(*)FROM DetallesDiasSolicitud d INNER JOIN SolicitudesVacaciones " +
                    "s ON d.SolicitudId = s.Id WHERE d.Fecha = @fecha AND s.Estado = 'Aprobada' AND s.EmpleadoID != @empleadoId";
                SQLiteCommand comando = new SQLiteCommand(sql,db);
                comando.Parameters.AddWithValue("@fecha",fecha);
                comando.Parameters.AddWithValue("@empleadoId",empleadoActual);
                int count = Convert.ToInt32(comando.ExecuteScalar()); 
                return count > 0;
            }
        }
        public void CrearSolicitud(int empleadoId, List<string> dias, string motivo)
        {
            using (SQLiteConnection db = new SQLiteConnection(conexion))
            {
                db.Open();
                string sqlSolicitud = "INSERT INTO SolicitudesVacaciones (EmpleadoID, DiasSolicitados, Estado, Motivo, FechaSolicitud)" +
                    "VALUES (@empleadoId, @dias, 'Pendiente', @motivo, @fechaSolicitud)";
                SQLiteCommand comando = new SQLiteCommand(sqlSolicitud, db);
                comando.Parameters.AddWithValue("@empleadoId", empleadoId);
                comando.Parameters.AddWithValue("@dias", dias.Count);
                comando.Parameters.AddWithValue("@fechaSolicitud", DateTime.Now.ToString("yyyy-mm-dd"));
                comando.ExecuteNonQuery();
                long solicitudId = db.LastInsertRowId;
                foreach(string dia in dias)
                {
                    string sqlDetalle = "INSERT INTO DellaSolicitud (SolicitudId, Fecha) " +
                        "VALUES (@solicitudId, @fecha)";
                    SQLiteCommand comandoD = new SQLiteCommand(sqlDetalle, db);
                    comandoD.Parameters.AddWithValue("@solicitudId",solicitudId);
                    comandoD.Parameters.AddWithValue("@fecha", dia);
                    comando.ExecuteNonQuery();
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
                SQLiteCommand comando = new SQLiteCommand(sql, db);
                SQLiteDataReader lector = comando.ExecuteReader();
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
        public void VerDiasSolicitud(int solicitudId)
        {
            using (SQLiteConnection db = new SQLiteConnection(conexion))
            {
                db.Open();
                string sql = "SELECT Fecha FROM DetalleDiasSolicitud WHERE SoliditudId = @id ORDER BY Fecha";
                SQLiteCommand comando = new SQLiteCommand(sql, db);
                comando.Parameters.AddWithValue("@id", solicitudId);
                SQLiteDataReader lector =comando.ExecuteReader();
                Console.WriteLine("Dias solicitados");
                while(lector.Read())
                {
                    Console.WriteLine("-> "+ lector["Fecha"]);
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
                SQLiteCommand comando = new SQLiteCommand(sql, db);
                comando.Parameters.AddWithValue("@id", solicitudId);
                SQLiteDataReader lector = comando.ExecuteReader();
                if(!lector.Read())
                {
                    Console.WriteLine("Solicitud no encontrada...");
                    return;
                }
                int empleadoId = Convert.ToInt32(lector["EmpleadoId"]);
                int diasSolicitados = Convert.ToInt32(lector["DiasSolicitados"]);
                lector.Close();
                int diasDisponibles = empleado.ObtenerDiasDisponibles(empleadoId);
                if(diasDisponibles < diasSolicitados)
                {
                    Console.WriteLine("El empleado no tiene días suficientes. Disponibles: "+ diasDisponibles);
                    return ;
                }
                string sqlUpdate = "UPDATE SolicitudesVacaciones " +
                    "SET Estado = 'Aprobada', ComentarioAdmin = @comentario WHERE Id = @id";
                SQLiteCommand comandoU = new SQLiteCommand(sqlUpdate, db);
                comandoU.Parameters.AddWithValue("@comentario", comentario);
                comandoU.Parameters.AddWithValue("@id", solicitudId);
                comandoU.ExecuteNonQuery();
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
                SQLiteCommand comando = new SQLiteCommand(sql, db);
                comando.Parameters.AddWithValue("@comentario", comentario);
                comando.Parameters.AddWithValue("@id", solicitudId);
                int filas = comando.ExecuteNonQuery();
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
                string sql = "SELECT s.Id, s.DiasSolicitados, s.Estado, s.Motivo, s.FechaSolicitud, s.ComentarioAdmin" +
                    "FROM SolicitudesVacaciones s WHERE s.EmpleadoId = @empleadoId ORDER BY s.FechaSolicitud DESC";
                SQLiteCommand comando = new SQLiteCommand(sql, db);
                comando.Parameters.AddWithValue("@empleadoId", empleadoId);
                SQLiteDataReader lector = comando.ExecuteReader();
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
