using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Data;
using Microsoft.SqlServer.Server;


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
                Console.Clear();
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
        static void MenuEmpleados(Empleado empleado)
        {
            bool salir = false;
            while (!salir)
            {
                Console.Clear();
                Console.WriteLine("***Menú Empleados***\n");
                Console.WriteLine("1. Registrar empleado");
                Console.WriteLine("2. Modificar empleado");
                Console.WriteLine("3. Buscar empleado por Id");
                Console.WriteLine("4. Listar todos los empleados");
                Console.WriteLine("0. regresar");
                Console.Write("\nSeleccione una opción: ");
                if (int.TryParse(Console.ReadLine(), out int opcion))
                {
                    switch (opcion)
                    {
                        case 0: salir = true; break;
                        case 1: RegistrarEmpleado(empleado); break;
                        case 2: ModificarEmpleado(empleado); break;
                        case 3:
                            Console.WriteLine("Ingrese Id del empleado:");
                            int id;
                            while (!int.TryParse(Console.ReadLine(), out id))
                            {
                                Console.Write("Error, vuelva a intentarlo: ");
                            }
                            empleado.BuscarPorId(id);
                            Console.ReadKey();
                            break;
                        case 4: empleado.MostrarEmpleados(); Console.ReadKey(); break;
                        default:
                            Console.WriteLine("Opcion incorrecta, vuelta a intentar...");
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
        static void RegistrarEmpleado(Empleado empleado)
        {
            Console.Clear();
            Console.WriteLine("***Registrar empleado**");
            string nombre = "";
            while(nombre.Trim()=="")
            {
                Console.Write("Nombre completo: \n");
                nombre = Console.ReadLine();
                if(nombre.Trim()=="")
                {
                    Console.WriteLine("El nombre no puede estar vacio...");
                }
            }
            string fechaIngreso = "";
            while (fechaIngreso.Trim() == "")
            {
                Console.Write("Fecha de ingreso (yyyy/mm/dd): \n");
                string entrada = Console.ReadLine();
                DateTime fecha;
                if(DateTime.TryParse(entrada, out fecha))
                {
                    fechaIngreso = fecha.ToString("yyyy-MM-dd");
                }
                else
                {
                    Console.WriteLine("Formato de fecha inválido...");
                }
            }
            string puesto = "";
            while (puesto.Trim() == "")
            {
                Console.Write("Puesto: \n");
                puesto = Console.ReadLine();
                if (puesto.Trim() == "")
                {
                    Console.WriteLine("El puesto no puede estar vacio...");
                }
            }
            string telefono = "";
            while (telefono.Trim() == "" || telefono.Length != 8 || !telefono.All(char.IsDigit))//Validación del número
            {
                Console.Write("Teléfono: \n");
                telefono = Console.ReadLine();
                if (telefono.Trim() == "" || telefono.Length != 8 || !telefono.All(char.IsDigit))
                {
                    Console.WriteLine("Error, el teléfono debe tener 8 dígitos y solo contener números");
                }
            }
            Console.Write("Correo: \n");
            string correo = Console.ReadLine();
            
            int dias = 0;
            while(dias<=0)
            {
                Console.Write("Días de vacaciones disponibles: \n");
                if(!int.TryParse(Console.ReadLine(), out dias) || dias<=0)
                {
                    Console.WriteLine("Error, ingrese un número válido");
                }
            }
            empleado.Agregar(nombre, fechaIngreso, puesto, telefono, correo, dias);
            Console.ReadKey();
        }
        static void ModificarEmpleado(Empleado empleado)
        {
            Console.Clear();
            Console.WriteLine("***Modificar empleado***\n");
            empleado.MostrarEmpleados();
            Console.Write("Ingrese el Id del empleado a modificar: ");
            int id;
            while (!int.TryParse(Console.ReadLine(), out id))
            {
                Console.Write("Error, vuelva a intentarlo: ");
            }
            if(!empleado.BuscarPorId(id))
            {
                Console.ReadKey();
                return;
            }
            Console.WriteLine("Nuevos datos: \n");
            string nombre = "";
            while (nombre.Trim() == "")
            {
                Console.Write("Nuevo Nombre: \n");
                nombre = Console.ReadLine();
                if (nombre.Trim() == "")
                {
                    Console.WriteLine("El nombre no puede estar vacio...");
                }
            }
            string fechaIngreso = "";
            while (fechaIngreso.Trim() == "")
            {
                Console.Write("Nueva fecha de ingreso (yyyy/mm/dd): \n");
                string entrada = Console.ReadLine();
                DateTime fecha;
                if (DateTime.TryParse(entrada, out fecha))
                {
                    fechaIngreso = fecha.ToString("yyyy-MM-dd");
                }
                else
                {
                    Console.WriteLine("Formato de fecha inválido...");
                }
            }
            string puesto = "";
            while (puesto.Trim() == "")
            {
                Console.Write("Nuevo puesto: \n");
                puesto = Console.ReadLine();
                if (puesto.Trim() == "")
                {
                    Console.WriteLine("El puesto no puede estar vacio...");
                }
            }
            string telefono = "";
            while (telefono.Trim() == "" || telefono.Length != 8 || !telefono.All(char.IsDigit))//Validación del número
            {
                Console.Write("Nuevo teléfono: \n");
                telefono = Console.ReadLine();
                if (telefono.Trim() == "" || telefono.Length != 8 || !telefono.All(char.IsDigit))
                {
                    Console.WriteLine("Error, el teléfono debe tener 8 dígitos y solo contener números");
                }
            }
            Console.Write("Nuevo correo: \n");
            string correo = Console.ReadLine();
            empleado.Modificar(nombre, fechaIngreso, puesto, telefono, correo, id);
            Console.ReadKey();
        }
        static void MenuUsuarios(ConexionBD db)
        {
            Console.Clear();
                Console.WriteLine("Crear usuario empleado");
            Empleado emp = new Empleado();
            emp.MostrarEmpleados();
            Console.Write("Id del empleado para crear usuario: ");
            int empId;
            if (!int.TryParse(Console.ReadLine(), out empId))
            {
                Console.WriteLine("Error en el ID");
                Console.ReadKey();
                return;
            }
            
            string usuario = "";
            while(usuario.Trim() == "")
            {
                Console.Write("Nombre de usuario: ");
                usuario = Console.ReadLine();
                if(usuario.Trim()=="")
                {
                    Console.WriteLine("El nombre no puede estar vacio");
                }
            }
            string contrasena = "";
            while (contrasena.Trim() == "" || contrasena.Length < 4)
            {
                Console.Write("Contraseña: ");
                contrasena = Console.ReadLine();
                if (contrasena.Trim() == "" || contrasena.Length < 4)
                {
                    Console.WriteLine("La contraseña no puede tener menos de 4 caracteres");
                }
            }
            db.CrearUsuarioEmpleado(empId, usuario, contrasena);
            Console.ReadKey();
        }
        static void MenuSolicitudesAdmin(Solicitud solicitud, Empleado empleado)
        {
            bool salir = false;
            while(!salir)
            {
                Console.Clear();
                Console.WriteLine("***Solicitud de vacaciones***\n");
                Console.WriteLine("1. Ver solicitudes pendientes");
                Console.WriteLine("2. Aprobar solicitud");
                Console.WriteLine("3. Rechazar solicitud");
                Console.WriteLine("4. Ver historial de un empleado");
                Console.WriteLine("0. Volver");
                Console.Write("\nSeleccione una opcion: ");
                if(int.TryParse(Console.ReadLine(), out int opcion))
                {
                    switch(opcion)
                    {
                        case 0: salir = true; break;
                        case 1: solicitud.VerPendiente(); Console.ReadKey(); break;
                        case 2: solicitud.VerPendiente();
                            Console.Write("Id de solicitud a aprobar: ");
                            int idAprobar;
                            if(!int.TryParse(Console.ReadLine(), out idAprobar))
                            {
                                Console.WriteLine("Id inválido...");
                                Console.ReadKey();
                                break;
                            }
                            solicitud.VerDiasSolicitud(idAprobar);
                            Console.Write("Comentario (opcional): ");
                            string comentario = Console.ReadLine();
                            solicitud.Aprobar(idAprobar, comentario, empleado);
                            Console.ReadKey();
                            break;
                            case 3 : solicitud.VerPendiente();
                            Console.Write("Id de solicitud a rechazar: ");
                            int idRechazar;
                            if(!int.TryParse(Console.ReadLine(), out idRechazar))
                            {
                                Console.WriteLine("Error en el Id");
                                Console.ReadKey();
                                break;
                            }
                            Console.Write("\nMotinvo del rechazo: ");
                            string comentarioRechazar = Console.ReadLine();
                            solicitud.RechazarSolictud(idRechazar, comentarioRechazar);
                            Console.ReadKey();
                            break;
                        case 4: empleado.MostrarEmpleados();
                            Console.Write("Id de empleado: ");
                            int idHistorial;
                            if(!int.TryParse(Console.ReadLine(), out idHistorial))
                            {
                                Console.WriteLine("Id inválido");
                                Console.ReadKey();
                                break;
                            }
                            solicitud.VerHistorial(idHistorial);
                            Console.ReadKey();
                            break;
                        default: Console.WriteLine("Error, opción inválida"); Console.ReadKey(); break;
                    }
                }
                else
                {
                    Console.WriteLine("Error, dato inválido");
                }
            }
        }
        static void MenuAsuetos(Asueto asueto)
        {
            bool salir = false;
            while(!salir)
            {
                Console.Clear();
                Console.WriteLine("***Menú Asuetos***");
                Console.WriteLine("1. Agregar asueto");
                Console.WriteLine("2. Listar asuetos");
                Console.WriteLine("3. Eliminar Asueto");
                Console.WriteLine("0. Regresar");
                Console.Write("\nSeleccione una opción: ");
                if(int.TryParse(Console.ReadLine(), out int opcion))
                {
                    switch (opcion)
                    {
                        case 0: salir = true; break;
                        case 1: Console.Clear();
                            string nombre = "";
                            while (nombre.Trim() == "")
                            {
                                Console.Write("Nombre de asueto: ");
                                nombre = Console.ReadLine();
                                if (nombre.Trim() == "")
                                {
                                    Console.WriteLine("El nombre no puede estar vacío");
                                }
                            }
                            string fechaAsueto = "";
                            while (fechaAsueto.Trim() == "")
                            {
                                Console.Write("Nueva fecha de asueto (yyyy/mm/dd): ");
                                string entrada = Console.ReadLine();
                                DateTime fecha;
                                if (DateTime.TryParse(entrada, out fecha))
                                {
                                    fechaAsueto = fecha.ToString("yyyy-MM-dd");
                                }
                                else
                                {
                                    Console.WriteLine("Formato de fecha inválido...");
                                }
                            }
                            Console.Write("Descripción (opcional): ");
                            string descripcion = Console.ReadLine();
                            asueto.Agregar(nombre, fechaAsueto, descripcion);
                            Console.ReadKey();
                            break;
                        case 2: asueto.ListarTodos(); Console.ReadKey(); break;
                        case 3: asueto.ListarTodos();
                            Console.Write("\nId de asueto a eliminar: ");
                            int idAsuetoElimar;
                            if(!int.TryParse(Console.ReadLine(), out idAsuetoElimar))
                            {
                                Console.WriteLine("Id inválido");
                                Console.ReadKey();
                                break;
                            }
                            asueto.Eliminar(idAsuetoElimar);
                            Console.ReadKey();
                            break;
                        default:
                            Console.WriteLine("Opción no encontrada");
                            Console.ReadKey();
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("ERROR, Dato inválido");
                    Console.ReadKey();
                }
            }
        }
        static void MenuEmpleado()
        {
            Empleado empleado = new Empleado();
            Solicitud solicitud = new Solicitud();
            Asueto asueto = new Asueto();
            bool salir = false;
            while(!salir)
            {
                Console.Clear();
                Console.WriteLine("*** Menú Empleados ***");
                Console.WriteLine("1. Ver mi información");
                Console.WriteLine("2. Solicitar vacaciones");
                Console.WriteLine("3. Ver mis solicitudes");
                Console.WriteLine("4. Ver asuetos del año");
                Console.WriteLine("0. Salir");
                Console.Write("\nSeleccione una opción: ");
                if(int.TryParse(Console.ReadLine(), out int opcion))
                {
                    switch (opcion)
                    {
                        case 0: salir = true; break;
                        case 1: empleado.BuscarPorId(EmpleadoIdActual); Console.ReadKey(); break;
                            case 2: HacerSolicitud(solicitud, empleado); break;
                            case 3: solicitud.VerHistorial(EmpleadoIdActual); Console.ReadKey(); break;
                            case 4: asueto.ListarTodos(); Console.ReadKey(); break;
                        default: Console.WriteLine("Opcion no encontrada"); Console.ReadKey(); break;
                    }
                }
                else
                {
                    Console.WriteLine("ERROR, dato inválido");
                    Console.ReadKey();
                }
            }
        }
        static void HacerSolicitud(Solicitud solicitud, Empleado empleado)
        {
            Console.Clear();
            Console.WriteLine("Solicitar Vacaciones");
            int diasDisponibles = empleado.ObtenerDiasDisponibles(EmpleadoIdActual);
            Console.WriteLine("Dias disponibles: "+diasDisponibles);
            if(diasDisponibles<=0)
            {
                Console.WriteLine("No tienes días disponibles");
                Console.ReadKey();
                return;
            }
            Console.WriteLine("Ingresa las fechas que deseas solicitar");
            Console.WriteLine("Formato: yyyy-MM-dd\nEscribe 'fin' para terminar");
            List<string> diasSeleccionados = new List<string>();
            while(true)
            {
                Console.Write("Fecha " + (diasSeleccionados.Count+1) + ": ");
                string entrada = Console.ReadLine().Trim();
                if(entrada.Equals("fin", StringComparison.OrdinalIgnoreCase))
                {
                    if(diasSeleccionados.Count == 0)
                    {
                        Console.WriteLine("Agregar al menos un día");
                        continue;
                    }
                    break;
                }
                DateTime fecha;
                if(!DateTime.TryParse(entrada, out fecha))
                {
                    Console.WriteLine("Formato fecha inválida");
                    continue;
                }
                string fechaSTR = fecha.ToString("yyyy-MM-dd");
                if(fecha.Date < DateTime.Today)
                {
                    Console.WriteLine("No puedes solicitar una fecha pasada");
                    continue;
                }
                if(fecha.DayOfWeek == DayOfWeek.Sunday)
                {
                    Console.WriteLine("No puede solicitar un domingo");
                    continue;
                }
                if(solicitud.EsAsueto(fechaSTR))
                {
                    Console.WriteLine("La fecha seleccionada es asueto");
                    continue;
                }
                if(solicitud.FechaOcupada(fechaSTR, EmpleadoIdActual))
                {
                    Console.WriteLine("Esa fecha ya fue ocupada por otro empleado");
                    continue;
                }
                if(diasSeleccionados.Contains(fechaSTR))
                {
                    Console.WriteLine("Ya agregaste esta fecha");
                    continue;
                }
                if(diasSeleccionados.Count + 1 > diasDisponibles)
                {
                    Console.WriteLine($"No tienes los suficientes días\nDias disponibles: {diasDisponibles}");
                    continue;
                }
                diasSeleccionados.Add(fechaSTR);
                Console.WriteLine("fecha agregada correctamente...");
            }
            Console.WriteLine("Resumen de solicitud:");
            Console.WriteLine("Dias seleccionados: " + diasSeleccionados.Count);
            foreach(string d in diasSeleccionados)
            {
                Console.WriteLine("-> " + d);
            }
            Console.WriteLine("Dias que quedarán: " + (diasDisponibles-diasSeleccionados.Count));
            Console.Write("\nMotivo (opcional): ");
            string motivo = Console.ReadLine();
            Console.Write("Confirmar solicitud (s/n): ");
            string confirmar = Console.ReadLine().Trim();
            if(confirmar.Equals("s", StringComparison.OrdinalIgnoreCase))
            {
                solicitud.CrearSolicitud(EmpleadoIdActual, diasSeleccionados, motivo);
            }
            else
            {
                Console.WriteLine("Solicitud cancelada");
            }
            Console.ReadKey();
        }
    }
}
