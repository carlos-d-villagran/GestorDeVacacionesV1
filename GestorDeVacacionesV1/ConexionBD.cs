using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Runtime.InteropServices.ComTypes;

namespace GestorDeVacacionesV1
{
    class ConexionBD
    {
        // CONEXION Y CREACION DE LA BASE DE DATOS
        private string conexion = "Data Source=GestorVacaciones.db;Version=3;BusyTimeout=5000;";
        public void IniciarBD()
        {
            using (SQLiteConnection db =
                new SQLiteConnection(conexion))
            {
                db.Open();
                string sql = @"
             CREATE TABLE IF NOT EXISTS Empleados(
            
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Nombre TEXT NOT NULL,
                FechaIngreso TEXT NOT NULL,
                Puesto TEXT NOT NULL,
                Telefono TEXT NOT NULL,
                Correo TEXT,
                DiasDisponibles INTEGER NOT NULL
            
             );
             CREATE TABLE IF NOT EXISTS Usuarios(
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                NombreUsuario TEXT NOT NULL,
                Contrasena TEXT NOT NULL,
                Rol TEXT NOT NULL,
                EmpleadoId INTEGER,
                FOREIGN KEY (EmpleadoId) REFERENCES Empleados(Id)
             );
             CREATE TABLE IF NOT EXISTS SolicitudesVacaciones(
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                EmpleadoId INTEGER NOT NULL,
                DiasSolicitados INTEGER NOT NULL,
                Estado TEXT NOT NULL,
                Motivo TEXT,
                FechaSolicitud TEXT NOT NULL,
                ComentarioAdmin TEXT,
                FOREIGN KEY (EmpleadoId) REFERENCES Empleados(Id)
             );
             CREATE TABLE IF NOT EXISTS DetalleDiasSolicitud(
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                SolicitudId INTEGER NOT NULL,
                Fecha TEXT NOT NULL,
                FOREIGN KEY (SolicitudId) REFERENCES SolicitudesVacaciones(Id)
             );
             CREATE TABLE IF NOT EXISTS Asuetos(
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Nombre TEXT NOT NULL,
                Fecha TEXT NOT NULL,
                Descripcion TEXT
             );";
                using (SQLiteCommand comando = new SQLiteCommand(sql, db))
                {
                    comando.ExecuteNonQuery();
                }

                Console.WriteLine(
                    "Base de datos y tabla creadas");
            }
        }
        public void CrearAdmin()
        {
            using (SQLiteConnection db =
                new SQLiteConnection(conexion))
            {
                db.Open();
                string sql = "INSERT OR IGNORE INTO Usuarios "
                    + "(Id, NombreUsuario, Contrasena, Rol, EmpleadoId) "
                    + "VALUES (1,'Admin','Admin123','Admin',null)";
                using (SQLiteCommand comando = new SQLiteCommand(sql, db))
                {
                    comando.ExecuteNonQuery();
                }
            }
        }
        //CREAMOS EL LOGIN
        public string Login(string nombreUsuario, string contrasena)
        {
            using (SQLiteConnection db = new SQLiteConnection(conexion))
            {  
                db.Open();
                string sql = "SELECT Rol FROM Usuarios "
                    + "WHERE NombreUsuario = @usuario AND Contrasena = @contrasena";
                using (SQLiteCommand comando = new SQLiteCommand(sql, db))
                {
                    comando.Parameters.AddWithValue("@usuario", nombreUsuario);
                    comando.Parameters.AddWithValue("@contrasena", contrasena);
                    using (SQLiteDataReader lector = comando.ExecuteReader())
                    {
                        if(lector.Read())
                        {
                            return lector["Rol"].ToString();
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
            }    
        }
        public int ObtenerEmpleadoId(string nombreUsuario, string contrasena)
        {
            using (SQLiteConnection db =
                new SQLiteConnection(conexion))
            {
                db.Open();
                string sql = "SELECT EmpleadoId FROM Usuarios WHERE NombreUsuario = @usuario AND Contrasena = @contrasena";
                object resultado;
                using (SQLiteCommand comando = new SQLiteCommand(sql, db))
                {
                    comando.Parameters.AddWithValue("@usuario", nombreUsuario);
                    comando.Parameters.AddWithValue("@contrasena", contrasena);
                    resultado = comando.ExecuteScalar();
                }
                if(resultado != null && resultado != DBNull.Value)
                {
                    return Convert.ToInt32(resultado);
                }
                else
                {
                    return 0;
                }
            }
        }
        public void CrearUsuarioEmpleado(int empleadoId, string nombreUsuario, string contrasena)
        {
            using (SQLiteConnection db =
                new SQLiteConnection(conexion))
            {
                db.Open();
                string sql = "INSERT INTO Usuarios(NombreUsuario, Contrasena, Rol, EmpleadoId) " +
                    "VALUES(@usuario, @contrasena, 'Empleado', @empleadoId)";
                using (SQLiteCommand comando = new SQLiteCommand (sql, db))
                {
                    comando.Parameters.AddWithValue("@usuario", nombreUsuario);
                    comando.Parameters.AddWithValue("@contrasena", contrasena);
                    comando.Parameters.AddWithValue("@empleadoId", empleadoId);
                    comando.ExecuteNonQuery();
                }
                Console.WriteLine("Usuario empleado creado correctamente...");
            }
        }
    }
}
