/*===============================================================================
Trabajo de Clase: Clase 22 C# y MySQL
App: BuscaBDAlumnos
Descripción: Busca alumnos en bucle en la base de datos miBD (Docker).
===============================================================================*/

using System;
using MySqlConnection = MySql.Data.MySqlClient.MySqlConnection;
using MySqlCommand = MySql.Data.MySqlClient.MySqlCommand;
using MySqlDataReader = MySql.Data.MySqlClient.MySqlDataReader;

namespace BuscaBDAlumnos
{
    class Program
    {
        static void Main (string [] args)
        {
            
            bool continuar = true;

            
            while (continuar)
            {
                Console.Clear();
                Console.WriteLine("======================================");
                Console.WriteLine("    SISTEMA DE BÚSQUEDA DE ALUMNOS    ");
                Console.WriteLine("======================================");

                // 1. Solicitud del legajo al usuario
                Console.Write("Ingrese el legajo a buscar: ");
                string legajoBuscado = Console.ReadLine() ?? "";

                // Cadena de conexión apuntando a 'miBD' con las credenciales de tu Docker
                string connectionString = "Server=localhost;Port=3306;Database=miBD;Uid=root;Pwd=abcde1234;";

                Console.WriteLine("\nConectando con la base de datos...");

                // 2. Control de Resource Leakage
                using (MySqlConnection conexion = new MySqlConnection(connectionString))
                {
                    try
                    {
                        conexion.Open();
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("¡Conexión exitosa al servidor MySQL! \n");
                        Console.ResetColor();

                        
                        string query = "SELECT legajo, nombre, apellido, email, carrera, turno, fecha_inscripcion FROM alumnos WHERE legajo = @legajo";
                        
                        using (MySqlCommand comando = new MySqlCommand(query, conexion))
                        {
                            comando.Parameters.AddWithValue("@legajo", legajoBuscado);

                      
                            using (MySqlDataReader lector = comando.ExecuteReader())
                            {
                                if (lector.Read())
                                {
                                    Console.WriteLine("======================================");
                                    Console.WriteLine("      DATOS DEL ALUMNO ENCONTRADO     ");
                                    Console.WriteLine("======================================");
                                    Console.WriteLine($"Legajo:      {lector["legajo"]}");
                                    Console.WriteLine($"Nombre:      {lector["nombre"]}");
                                    Console.WriteLine($"Apellido:    {lector["apellido"]}");
                                    Console.WriteLine($"Email:       {lector["email"]}");
                                    Console.WriteLine($"Carrera:     {lector["carrera"]}");
                                    Console.WriteLine($"Turno:       {lector["turno"]}");

                                    if (DateTime.TryParse(lector["fecha_inscripcion"].ToString(), out DateTime fecha))
                                    {
                                        Console.WriteLine($"Inscripto:   {fecha:dd/MM/yyyy}");  
                                    }
                                    
                                    Console.WriteLine("======================================");
                                }  
                                else
                                {
                                    Console.ForegroundColor = ConsoleColor.Yellow;
                                    Console.WriteLine($"Error: No se encontró ningún alumno registrado con el legajo '{legajoBuscado}'.");
                                    Console.ResetColor();
                                }  
                            }
                        }
                    }
                    catch (Exception ex) 
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Ocurrió un error al intentar operar con la base de datos:");
                        Console.WriteLine(ex.Message);
                        Console.ResetColor();
                    }
                }
                
                
                Console.WriteLine("\n¿Desea buscar otro alumno? (S/N): ");
                string respuesta = Console.ReadLine() ?? "";

               
                if (respuesta.Trim().ToUpper() != "S")
                {
                    continuar = false; 
                }
            } // Fin del while

           
            Console.Clear();
            Console.WriteLine("¡Saliendo del sistema! Hasta luego.");
        }
    }
}
