using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace GestorDeVacacionesV1
{
    class ConexionBD
    {
        // CONEXION Y CREACION DE LA BASE DE DATOS
        private string conexion = "Data Source=GestorVacaciones.db";
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
                Correo TEXT NOT NULL,
                DiasDisponibles INTEGER NOT NULL
            
             );
             CREATE TABLE IF NOT EXIST Usuarios(
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                NombreUsuario TEXT NOT NULL,
                Contrasena TEXT NOT NULL,
                Rol TEXT NOT NULL,
                EmpleadoId INTEGER,
                FOREIGN KEY (EmpleadoId) REFERENCES Empleados(Id)
             );
             CREATE TABLE IF NOT EXIST SolicitudesVacaciones(
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                EmpleadoId INTEGER NOT NULL,
                DiasSolicitados INTEGER NOT NULL,
                Estado TEXT NOT NULL,
                Motivo TEXT,
                FechasSolicitud TEXT NOT NULL,
                ComentarioAdmin TEXT,
                FOREIGN KEY (EmpleadoId) REFERENCES Empleados(Id)
             );
             CREATE TABLE IF NOT EXIST DetalleDiaSolicitud(
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                SolicitudID INTEGER NOT NULL,
                Fecha TEXT NOT NULL,
                FOREIGN KEY (SolicitudID) REFERENCES SolicitudesVacaciones(Id)
             );
             CREATE TABLE IF NOT EXIST Asuetos(
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Nombre TEXT NOT NULL,
                Fecha TEXT NOT NULL,
                Descripcion TEXT
             );";
                SQLiteCommand comando =
                new SQLiteCommand(sql, db);

                comando.ExecuteNonQuery();

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
                    + "(Id, NombreUsuario, Contrasena, Rol, EmpleadoId)"
                    + "VALUES (1,'Admin','Admin123','Admin',null)";
                SQLiteCommand comando =
                new SQLiteCommand(sql, db);
                comando.ExecuteNonQuery();
            }
        }
        //CREAMOS EL LOGIN
        public string Login(string nombreUsuario, string contrasena)
        {
            using (SQLiteConnection db = new SQLiteConnection(conexion))
            {  
                db.Open();
                string sql = "SELECT Rol FROM Usuarios"
                    + "WHERE NombreUsuario = @usuario AND Contrasena = @contrasena";
                SQLiteCommand comando = new SQLiteCommand(sql, db);
                comando.Parameters.AddWithValue("@usuario", nombreUsuario);
                comando.Parameters.AddWithValue("@contrasena", contrasena);
                SQLiteDataReader lector = comando.ExecuteReader();
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
