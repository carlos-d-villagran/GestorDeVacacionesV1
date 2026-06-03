using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Web;

namespace GestorDeVacacionesV1
{
    class Asueto
    {
        private string conexion = "Data Source=GestorVacaciones.db";
        public void Agregar(string nombre, string fecha, string descripcion)
        {
            using (SQLiteConnection db = new SQLiteConnection(conexion))
            {
                db.Open();
                string sql = "INSERT INTO Asuetos(Nombre, Fecha, Descripcion) VALUES(@nombre, @fecha, @descripcion)";
                SQLiteCommand comando = new SQLiteCommand(sql,db);
                comando.Parameters.AddWithValue("@nombre",nombre    );
                comando.Parameters.AddWithValue("@fecha",fecha);
                comando.Parameters.AddWithValue("@descripcion",descripcion);
                comando.ExecuteNonQuery();
                Console.WriteLine("Asueto registrado correctamente...");
            }
        }
        public void ListarTodos()
        {
            using (SQLiteConnection db = new SQLiteConnection(conexion))
            {
                db.Open();
                string sql = "SELECT *FROM Asuetos ORDER BY Fecha";
                SQLiteCommand comando = new SQLiteCommand(sql,db);
                SQLiteDataReader lector = comando.ExecuteReader();
                Console.WriteLine("Asuetos:\n");
                bool hay = false;
                while (lector.Read())
                {
                    hay= true;
                    Console.WriteLine("Id:" + lector["Id"]);
                    Console.WriteLine("Nombre:" + lector["Nombre"]);
                    Console.WriteLine("Fecha:" + lector["Fecha"]);
                    Console.WriteLine("Descripcion:" + lector["Descripcion"]);
                }
                if(!hay)
                {
                    Console.WriteLine("No hay asuetos resgistrados...");
                }
            }
        }
        public void Eliminar(int id)
        {
            using (SQLiteConnection db = new SQLiteConnection(conexion))
            {
                db.Open();
                string sql = "DELETE FROM Asuetos WHERE Id = @id";
                SQLiteCommand comando = new SQLiteCommand(sql, db);
                comando.Parameters.AddWithValue("@id", id);
                int filas = comando.ExecuteNonQuery();
                if(filas>0)
                {
                    Console.WriteLine("Asueto eliminado correctamente...");
                }
                else
                {
                    Console.WriteLine("Id no encontrado...");
                }
            }
        }
    }
}
