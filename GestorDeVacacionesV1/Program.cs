using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;


namespace GestorDeVacacionesV1
{
    class Program
    {
        //VARIABLES GLOBALES
        static string RolActual = "";
        static int EmpleadoIdActual = 0;
        static void Main(string[] args)
        {
            ConexionBD db = new ConexionBD();
            db.IniciarBD();
            db.CrearAdmin();
            //login
            int intentos = 0;
            bool loginExitoso = false;
            while(intentos < 3 && !loginExitoso)
            {
                Console.Clear();
                Console.WriteLine("****GESTOR DE VACACIONES****\n");
                if(intentos>0)
                {
                    Console.WriteLine($"Credenciales incorrectas, le quedan: {intentos} de 3");
                }
                Console.Write("Usuario: ");
                string usuario = Console.ReadLine();
                Console.Write("\nContraseña: ");
                string contrasena = Console.ReadLine();

                //Aquí obtenemos el Rool y el Id del empleado
                string rol = db.Login(usuario, contrasena);
                int empId = db.ObtenerEmpleadoId(usuario, contrasena);
                if(rol != null)
                {
                    RolActual = rol;
                    EmpleadoIdActual = empId;
                    loginExitoso = true;
                }
                else
                {
                    intentos++;
                }
            }
            if(!loginExitoso)
            {
                Console.WriteLine("Demasiados intentos fallidos, intentelo más tarde");
                Console.ReadKey();
                return;
            }

            //redirigir según el rol
            if(RolActual == "Admin")
            {
                MenuAdmin();
            }
            else
            {
                MenuEmpleado();
            }
        }

        //Menu Admin
        static void MenuAdmin()
        {
            Empleado empleado = new Empleado();
            Solicitud solicitud = new Solicitud();
            Asueto asueto = new Asueto();
            ConexionBD db = new ConexionBD();
            bool salir = false;
            while(!salir)
            {
                Console.WriteLine("***Menú Administrador***\n");
                Console.WriteLine("1. Gestión de empleados");
                Console.WriteLine("2. Gestión de usuario");
                Console.WriteLine("3. Solicitudes de vacaciones");
                Console.WriteLine("4. Asuetos");
                Console.WriteLine("5. Reporte de días disponibles");
                Console.WriteLine("0. Salir");
                Console.Write("\nSeleccione una opción: ");
                if(int.TryParse(Console.ReadLine(), out int opcion))
                {
                    switch(opcion)
                    {
                        case 0: salir = true; break;
                        case 1: MenuEmpleados(empleado); break;
                        case 2: MenuUsuarios(db); break;
                        case 3: MenuSolicitudesAdmin(solicitud, empleado); break;
                        case 4: MenuAsuetos(asueto); break;
                            case 5: empleado.ReporteDiasDisponibles(); Console.ReadKey();  break;
                        default: Console.WriteLine("Opcion incorrecta, vuelta a intentar..."); 
                                Console.ReadKey();
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("ERROR. Dato inválido");
                    Console.ReadKey();
                }
                
            }
        }
    }
}
