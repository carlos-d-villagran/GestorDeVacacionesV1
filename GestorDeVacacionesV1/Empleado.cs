using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace GestorDeVacacionesV1
{
    class Empleado
    {
        private string conexion = "Data Source=GestorVacaciones.db;Version=3;BusyTimeout=5000;";
        public void Agregar(string nombre, string fechaIngreso, string puesto, string telefono, string correo, int diasDisponibles)
        {
            using (SQLiteConnection db = new SQLiteConnection(conexion))
            {
                db.Open();
                string sql = "INSERT INTO Empleados "
                    + "(Nombre, FechaIngreso, Puesto, Telefono, Correo, DiasDisponibles) "
                    + "VALUES (@nombre, @fechaIngreso, @puesto, @telefono, @correo, @diasDisponibles)";
                using (SQLiteCommand comando = new SQLiteCommand(sql, db))
                {
                    comando.Parameters.AddWithValue("@nombre", nombre);
                    comando.Parameters.AddWithValue("@fechaIngreso", fechaIngreso);
                    comando.Parameters.AddWithValue("@puesto", puesto);
                    comando.Parameters.AddWithValue("@telefono", telefono);
                    comando.Parameters.AddWithValue("@correo", correo);
                    comando.Parameters.AddWithValue("@diasDisponibles", diasDisponibles);
                    comando.ExecuteNonQuery();
                }
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
                int filas;
                using (SQLiteCommand comando = new SQLiteCommand(sql, db))
                {
                    comando.Parameters.AddWithValue("@nombre", nombre);
                    comando.Parameters.AddWithValue("@fechaIngreso", fechaIngreso);
                    comando.Parameters.AddWithValue("@puesto", puesto);
                    comando.Parameters.AddWithValue("@telefono", telefono);
                    comando.Parameters.AddWithValue("@correo", correo);
                    comando.Parameters.AddWithValue("@id", id);
                    filas = comando.ExecuteNonQuery();
                }
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
                string sql = "SELECT * FROM Empleados";
                using (SQLiteCommand comando = new SQLiteCommand(sql, db))
                using (SQLiteDataReader lector = comando.ExecuteReader())
                {
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
        public bool BuscarPorId(int id)
        {
            using (SQLiteConnection db = new SQLiteConnection(conexion))
            {
                db.Open();
                string sql = "SELECT * FROM Empleados WHERE Id = @id";
                using (SQLiteCommand comando = new SQLiteCommand (sql, db))
                {
                    comando.Parameters.AddWithValue("@id", id);
                    using (SQLiteDataReader lector = comando.ExecuteReader())
                    {
                        if(lector.Read())
                        { Console.WriteLine("Id: " + lector["Id"]);
                          Console.WriteLine("Nombre: " + lector["Nombre"]);
                          Console.WriteLine("Fecha de Ingreso: " + lector["FechaIngreso"]);
                          Console.WriteLine("Puesto: " + lector["Puesto"]);
                          Console.WriteLine("Teléfono: " + lector["Telefono"]);
                          Console.WriteLine("Correo: " + lector["Correo"]);
                          Console.WriteLine("Días disponibles: " + lector["DiasDisponibles"]);
                            return true;
                        }
                        else
                        {
                            Console.WriteLine("Empleado no encontrado...");
                            return false;
                        }
                    }
                }
            }
        }
        public int ObtenerDiasDisponibles(int empleadoId)
        {
            using (SQLiteConnection db = new SQLiteConnection(conexion))
            {
                db.Open();
                string sql = "SELECT DiasDisponibles FROM Empleados WHERE Id = @id";
                object resultado;
                using (SQLiteCommand comando = new SQLiteCommand (sql, db))
                {
                    comando.Parameters.AddWithValue ("@id", empleadoId);
                    resultado = comando.ExecuteScalar();
                }
                if(resultado!= null)
                {
                    return Convert.ToInt32(resultado);
                }
                else
                {
                    return 0;
                }
            }
        }
        public void DescontarDias(int empleadoId, int diasADescontar)
        {
            using (SQLiteConnection db = new SQLiteConnection(conexion))
            {
                db.Open();
                string sql = "UPDATE Empleados SET DiasDisponibles = DiasDisponibles - @dias WHERE Id = @id";
                using (SQLiteCommand comando = new SQLiteCommand(sql, db))
                {
                    comando.Parameters.AddWithValue("@dias", diasADescontar);
                    comando.Parameters.AddWithValue("@id", empleadoId);
                    comando.ExecuteNonQuery();
                }
            }
        }
        public void ReporteDiasDisponibles()
        {
            using (SQLiteConnection db = new SQLiteConnection(conexion))
            {
                db.Open();
                string sql = "SELECT Nombre, Puesto, DiasDisponibles FROM Empleados ORDER BY Nombre";
                using (SQLiteCommand comando = new SQLiteCommand(sql, db))
                using (SQLiteDataReader lector = comando.ExecuteReader())
                {
                    Console.WriteLine("REPORTE DÍAS DISPONIBLES: \n");
                    while(lector.Read())
                    {
                        Console.WriteLine("Nombre: "+ lector["Nombre"]);
                        Console.WriteLine("Puesto: "+ lector["Puesto"]);
                        Console.WriteLine("Dias disponibles: "+ lector["DiasDisponibles"]);
                    }
                }
            }
        }
    }
}
