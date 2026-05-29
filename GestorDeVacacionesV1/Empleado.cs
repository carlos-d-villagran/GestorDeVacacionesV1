using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace GestorDeVacacionesV1
{
    internal class Empleado
    {
        private string conexion = "Data Source=GestorVacaciones.db";
        public void Agregar(string nombre, string fechaIngreso, string puesto, string telefono, string correo, int diasDisponibles)
        {
            using (SQLiteConnection db = new SQLiteConnection(conexion))
            {
                db.Open();
                string sql = "INSERT INTO TABLE Empleados" +
                    "(Nombre, FechaIngreso, Puesto, Telefono, Correo, DiasDisponibles)"
                    + "VALUES (@nombre, @fechaIngreso,@puesto,@telefono,@correo,@diasDisponibles)";
                SQLiteCommand comando = new SQLiteCommand(sql, db);
                comando.Parameters.AddWithValue("@nombre", nombre);
                comando.Parameters.AddWithValue("@fechaIngreso", fechaIngreso);
                comando.Parameters.AddWithValue("@puesto", puesto);
                comando.Parameters.AddWithValue("@telefono", telefono);
                comando.Parameters.AddWithValue("@correo", correo);
                comando.Parameters.AddWithValue("@diasDisponibles", diasDisponibles);
                comando.ExecuteNonQuery();
                Console.WriteLine("Empleado registrado correctamente...");

            }
        }
        public void Modificar(string nombre, string fechaIngreso, string puesto, string telefono, string correo, int id)
        {
            using (SQLiteConnection db = new SQLiteConnection(conexion))
            {
                db.Open();
                string sql = "UPDATE Empleados SET " +
                    "Nombre = @nombre, FechaIngreso = @fechaIngreso, Puesto= @puesto, Telefono = @telefono, Correo = @correo WHERE Id = @id";
                SQLiteCommand comando = new SQLiteCommand(sql, db);
                comando.Parameters.AddWithValue("@nombre", nombre);
                comando.Parameters.AddWithValue("@fechaIngreso", fechaIngreso);
                comando.Parameters.AddWithValue("@puesto", puesto);
                comando.Parameters.AddWithValue("@telefono", telefono);
                comando.Parameters.AddWithValue("@correo", correo);
                comando.Parameters.AddWithValue("@Id", id);
                int filas = comando.ExecuteNonQuery();
                if(filas > 0)
                 Console.WriteLine("Empleado modificado correctamente...");
                else
                 Console.WriteLine("Empleado no encontrado");
            }
        }
        public void MostrarEmpleados()
        {
            using (SQLiteConnection db = new SQLiteConnection(conexion))
            {
                db.Open();
                string sql = "SELECT *FROM Empleados";
                SQLiteCommand comando = new SQLiteCommand(sql, db);
                SQLiteDataReader lector = comando.ExecuteReader();
                Console.WriteLine("+++ Lista de empleados +++\n");
                bool hayEmpleado = false;
                while (lector.Read())
                {
                    hayEmpleado = true;
                    Console.WriteLine("Id: " + lector["Id"]);
                    Console.WriteLine("Nombre: " + lector["Nombre"]);
                    Console.WriteLine("Fecha de Ingreso: " + lector["FechaIngreso"]);
                    Console.WriteLine("Puesto: " + lector["Puesto"]);
                    Console.WriteLine("Teléfono: " + lector["Telefono"]);
                    Console.WriteLine("Correo: " + lector["Correo"]);
                    Console.WriteLine("Días disponibles: " + lector["DiasDisponibles"]);
                }
                if(!hayEmpleado)
                    Console.WriteLine("\nNo hay empleados...");
            }
        }
    }
}
