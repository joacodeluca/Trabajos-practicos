using System;
using MySql.Data.MySqlClient; 

namespace Progra3Card.Administrativo
{
    class Program
    {
        // Conexión directa a tu contenedor Docker expuesto en el puerto 3306
        private static string connectionString = "Server=localhost;Database=mi_banco_db;Uid=root;Pwd=abcde1234;";

        static void Main(string[] args)
        {
            bool salir = false;
            while (!salir)
            {
                Console.Clear();
                Console.WriteLine("========================================");
                Console.WriteLine("    SISTEMA ADMINISTRATIVO PROGRA3CARD   ");
                Console.WriteLine("========================================");
                Console.WriteLine("1. Emitir Nueva Tarjeta (Alta de Cliente)");
                Console.WriteLine("2. Listar Tarjetas");
                Console.WriteLine("3. Ver Detalle de una Tarjeta / Cliente");
                Console.WriteLine("4. Eliminar Tarjeta (Baja de Sistema)");
                Console.WriteLine("5. Emitir Nueva Liquidación Mensual");
                Console.WriteLine("6. Salir");
                Console.WriteLine("========================================");
                Console.Write("Seleccione una opción: ");

                switch (Console.ReadLine())
                {
                    case "1": MenuEmitirTarjeta(); break;
                    case "2": MenuListarTarjetas(); break;
                    case "3": MenuVerDetalleTarjeta(); break;
                    case "4": MenuEliminarTarjeta(); break;
                    case "5": MenuEmitirLiquidacion(); break;
                    case "6": salir = true; break;
                    default:
                        Console.WriteLine("Opción no válida. Presione una tecla para continuar...");
                        Console.ReadKey();
                        break;
                }
            }
        }

        // =========================================================================
        // MÉTODOS DE MENÚ
        // =========================================================================

        static void MenuEmitirTarjeta()
        {
            Console.Clear();
            Console.WriteLine("--- ALTA DE CLIENTE Y EMISIÓN DE TARJETA ---");
            
            try
            {
                Console.Write("Tipo de Documento (DNI/PASAPORTE): ");
                string tipoDoc = Console.ReadLine().ToUpper();
                Console.Write("Número de Documento: ");
                string documento = Console.ReadLine();
                Console.Write("Nombre: ");
                string nombre = Console.ReadLine();
                Console.Write("Apellido: ");
                string apellido = Console.ReadLine();
                Console.Write("Fecha Nacimiento (YYYY-MM-DD): ");
                string fechaNac = Console.ReadLine();
                Console.Write("Email: ");
                string email = Console.ReadLine();

                Console.Write("Número de Tarjeta (16 dígitos): ");
                string numTarjeta = Console.ReadLine();
                
                // Forzar selección estricta del ENUM
                Console.WriteLine("\nBancos Emisores Permitidos:");
                Console.WriteLine("1. Banco Nación | 2. Banco Provincia | 3. Banco Galicia");
                Console.WriteLine("4. Banco Santander | 5. Banco BBVA | 6. Banco Macro");
                Console.Write("Seleccione una opción (1-6): ");
                string opcionBanco = Console.ReadLine();
                
                string bancoEmisor = "";
                switch (opcionBanco)
                {
                    case "1": bancoEmisor = "Banco Nación"; break;
                    case "2": bancoEmisor = "Banco Provincia"; break;
                    case "3": bancoEmisor = "Banco Galicia"; break;
                    case "4": bancoEmisor = "Banco Santander"; break;
                    case "5": bancoEmisor = "Banco BBVA"; break;
                    case "6": bancoEmisor = "Banco Macro"; break;
                    default: throw new Exception("Banco seleccionado no válido.");
                }

                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    
                    // 1. Insertar Usuario (sin clave web, queda pendiente)
                    string queryUsuario = "INSERT INTO usuarios (documento, tipo_doc, nombre, apellido, fecha_nacimiento, email) VALUES (@doc, @tipo, @nom, @ape, @fecha, @email)";
                    using (MySqlCommand cmd = new MySqlCommand(queryUsuario, conn))
                    {
                        cmd.Parameters.AddWithValue("@doc", documento);
                        cmd.Parameters.AddWithValue("@tipo", tipoDoc);
                        cmd.Parameters.AddWithValue("@nom", nombre);
                        cmd.Parameters.AddWithValue("@ape", apellido);
                        cmd.Parameters.AddWithValue("@fecha", fechaNac);
                        cmd.Parameters.AddWithValue("@email", email);
                        cmd.ExecuteNonQuery();
                    }

                    // 2. Insertar Tarjeta
                    string queryTarjeta = "INSERT INTO tarjetas (numero_tarjeta, banco_emisor, dni_titular) VALUES (@numTar, @banco, @dni)";
                    using (MySqlCommand cmd = new MySqlCommand(queryTarjeta, conn))
                    {
                        cmd.Parameters.AddWithValue("@numTar", numTarjeta);
                        cmd.Parameters.AddWithValue("@banco", bancoEmisor);
                        cmd.Parameters.AddWithValue("@dni", documento);
                        cmd.ExecuteNonQuery();
                    }

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("\n¡Cliente y tarjeta registrados con éxito en la base de datos!");
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\nError al emitir tarjeta: " + ex.Message);
            }
            
            Console.ResetColor();
            Console.WriteLine("\nPresione una tecla para volver al menú...");
            Console.ReadKey();
        }

        static void MenuListarTarjetas()
        {
            Console.Clear();
            Console.WriteLine("--- LISTADO GENERAL DE TARJETAS ---");
            Console.WriteLine("{0,-12} {1,-18} {2,-20} {3,-15}", "Nro Cuenta", "Nro Tarjeta", "Banco Emisor", "DNI Titular");
            Console.WriteLine("----------------------------------------------------------------------");

            ObtenerYMostrarTarjetas();

            Console.WriteLine("\nPresione una tecla para volver al menú...");
            Console.ReadKey();
        }

        static void MenuVerDetalleTarjeta()
        {
            Console.Clear();
            Console.WriteLine("--- DETALLE DE TARJETA Y CLIENTE ---");
            Console.Write("Ingrese el Número de Cuenta a consultar: ");
            
            if (int.TryParse(Console.ReadLine(), out int numCuenta))
            {
                MostrarDetalleCompleto(numCuenta);
            }
            else
            {
                Console.WriteLine("Formato de cuenta inválido.");
            }

            Console.WriteLine("\nPresione una tecla para volver al menú...");
            Console.ReadKey();
        }

        static void MenuEliminarTarjeta()
        {
            Console.Clear();
            Console.WriteLine("--- ELIMINAR TARJETA DEL SISTEMA ---");
            Console.Write("Ingrese el Número de Cuenta de la tarjeta a dar de baja: ");
            
            if (int.TryParse(Console.ReadLine(), out int numCuenta))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\n⚠️ ADVERTENCIA: Se eliminará la tarjeta, sus liquidaciones y los datos de acceso web vinculados.");
                Console.ResetColor();
                Console.Write("¿Está seguro de continuar? (S/N): ");
                
                if (Console.ReadLine().ToUpper() == "S")
                {
                    bool exito = DarDeBajaTarjeta(numCuenta);

                    if (exito)
                        Console.WriteLine("\nTarjeta eliminada correctamente del sistema.");
                    else
                        Console.WriteLine("\nError al intentar eliminar la tarjeta. Verifique el número de cuenta.");
                }
                else
                {
                    Console.WriteLine("\nOperación cancelada.");
                }
            }

            Console.WriteLine("\nPresione una tecla para volver al menú...");
            Console.ReadKey();
        }

        static void MenuEmitirLiquidacion()
        {
            Console.Clear();
            Console.WriteLine("--- EMITIR NUEVA LIQUIDACIÓN MENSUAL ---");
            
            try
            {
                Console.Write("Número de Cuenta: ");
                int numCuenta = Convert.ToInt32(Console.ReadLine());
                Console.Write("Período (YYYY-MM): ");
                string periodo = Console.ReadLine();
                Console.Write("Fecha Vencimiento (YYYY-MM-DD): ");
                string fechaVenc = Console.ReadLine();
                Console.Write("Total a Pagar: ");
                decimal total = Convert.ToDecimal(Console.ReadLine().Replace('.', ',')); // Manejo simple de coma decimal
                Console.Write("Pago Mínimo: ");
                decimal minimo = Convert.ToDecimal(Console.ReadLine().Replace('.', ','));

                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "INSERT INTO liquidaciones (num_cuenta, periodo, fecha_vencimiento, total_a_pagar, pago_minimo) VALUES (@cuenta, @per, @venc, @tot, @min)";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@cuenta", numCuenta);
                        cmd.Parameters.AddWithValue("@per", periodo);
                        cmd.Parameters.AddWithValue("@venc", fechaVenc);
                        cmd.Parameters.AddWithValue("@tot", total);
                        cmd.Parameters.AddWithValue("@min", minimo);
                        
                        cmd.ExecuteNonQuery();
                    }
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("\n¡Liquidación impactada con éxito en la Base de Datos!");
                    Console.WriteLine("El cliente ya puede visualizarla en el Portal Web.");
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\nError al emitir liquidación: " + ex.Message);
            }
            
            Console.ResetColor();
            Console.WriteLine("\nPresione una tecla para volver al menú...");
            Console.ReadKey();
        }

        // =========================================================================
        // MÉTODOS DE BASE DE DATOS
        // =========================================================================

        static void ObtenerYMostrarTarjetas()
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT num_cuenta, numero_tarjeta, banco_emisor, dni_titular FROM tarjetas";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Console.WriteLine("{0,-12} {1,-18} {2,-20} {3,-15}", 
                                reader["num_cuenta"], 
                                reader["numero_tarjeta"], 
                                reader["banco_emisor"], 
                                reader["dni_titular"]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error leyendo base de datos: " + ex.Message);
            }
        }

        static void MostrarDetalleCompleto(int cuenta)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"SELECT t.*, u.nombre, u.apellido, u.email 
                                     FROM tarjetas t 
                                     JOIN usuarios u ON t.dni_titular = u.documento 
                                     WHERE t.num_cuenta = @cuenta";
                    
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@cuenta", cuenta);
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                Console.WriteLine("\n-- DATOS DEL TITULAR --");
                                Console.WriteLine($"Nombre y Apellido: {reader["nombre"]} {reader["apellido"]}");
                                Console.WriteLine($"DNI: {reader["dni_titular"]}");
                                Console.WriteLine($"Email: {reader["email"]}");
                                
                                Console.WriteLine("\n-- DATOS DE LA TARJETA --");
                                Console.WriteLine($"Número: {reader["numero_tarjeta"]}");
                                Console.WriteLine($"Banco: {reader["banco_emisor"]}");
                                Console.WriteLine($"Estado: {reader["estado"]}");
                                Console.WriteLine($"Saldo Actual: ${reader["saldo"]}");
                            }
                            else
                            {
                                Console.WriteLine("\nNo se encontró ninguna tarjeta con ese número de cuenta.");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("\nError buscando detalles: " + ex.Message);
            }
        }

        static bool DarDeBajaTarjeta(int cuenta)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    // Al eliminar la tarjeta, el ON DELETE CASCADE elimina las liquidaciones asociadas
                    string query = "DELETE FROM tarjetas WHERE num_cuenta = @cuenta";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@cuenta", cuenta);
                        int filasAfectadas = cmd.ExecuteNonQuery();
                        return filasAfectadas > 0;
                    }
                }
            }
            catch
            {
                return false;
            }
        }
    }
}