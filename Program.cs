using ClinicaVeterinaria.datos;
using ClinicaVeterinaria.logica;

namespace ClinicaVeterinaria
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // 1. Gestion centralizada: el gestor administra veterinarios, propietarios, mascotas y consultas.
            GestorClinico objGestor = new GestorClinico();
            try
            {
                // Crea clinica_veterinaria.db y sus tablas la primera vez; luego carga todo desde SQLite.
                objGestor.cargarDatosBase();
                Console.WriteLine("Base de datos SQLite: " + BaseDatos.strRuta);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("No se pudo inicializar la base de datos SQLite: " + ex.Message);
                Console.ResetColor();
                return;
            }

            // 2. Flujo de la consulta: Registrada -> Asignada -> Diagnosticada -> Cerrada.
            FlujoConsulta objFlujoConsulta = new FlujoConsulta();

            bool blnContinuar = true;

            while (blnContinuar)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("\n========================================================");
                Console.WriteLine("      SISTEMA DE CONTROL CLINICO VETERINARIO");
                Console.WriteLine("========================================================");
                Console.ResetColor();
                Console.WriteLine("  1. Ver usuarios del sistema (polimorfismo)");
                Console.WriteLine("  2. Ver flujo de estados de la consulta");
                Console.WriteLine("  3. Registrar propietario");
                Console.WriteLine("  4. Registrar mascota");
                Console.WriteLine("  5. Crear consulta y asignar automaticamente");
                Console.WriteLine("  6. Ver mascotas");
                Console.WriteLine("  7. Ver consultas");
                Console.WriteLine("  8. Registrar examen clinico");
                Console.WriteLine("  9. Diagnosticar consulta");
                Console.WriteLine(" 10. Cerrar consulta");
                Console.WriteLine(" 11. Aplicar vacuna a mascota");
                Console.WriteLine(" 12. Ver expediente clinico de una mascota");
                Console.WriteLine(" 13. Generar resumen de control");
                Console.WriteLine(" 14. Salir");
                Console.Write("\n Seleccione una opcion (1-14): ");

                try
                {
                    string strOpcion = Console.ReadLine()?.Trim();

                    switch (strOpcion)
                    {
                        case "1":
                            objGestor.mostrarUsuariosPolimorfismo();
                            break;

                        case "2":
                            objFlujoConsulta.mostrarFlujo();
                            break;

                        case "3":
                            registrarPropietario(objGestor);
                            break;

                        case "4":
                            registrarMascota(objGestor);
                            break;

                        case "5":
                            crearConsulta(objGestor);
                            break;

                        case "6":
                            objGestor.mostrarMascotas();
                            break;

                        case "7":
                            objGestor.mostrarConsultas();
                            break;

                        case "8":
                            registrarExamen(objGestor);
                            break;

                        case "9":
                            diagnosticarConsulta(objGestor, objFlujoConsulta);
                            break;

                        case "10":
                            cerrarConsulta(objGestor, objFlujoConsulta);
                            break;

                        case "11":
                            aplicarVacuna(objGestor);
                            break;

                        case "12":
                            solicitarMascota(objGestor).mostrarExpediente();
                            break;

                        case "13":
                            objGestor.generarResumenControl();
                            break;

                        case "14":
                            blnContinuar = false;
                            Console.WriteLine("\nGracias por utilizar el sistema de control clinico veterinario.");
                            break;

                        default:
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Opcion no valida. Ingrese un numero del 1 al 14.");
                            Console.ResetColor();
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Error controlado por try/catch: " + ex.Message);
                    Console.ResetColor();
                }
            }
        }

        static void registrarPropietario(GestorClinico objGestor)
        {
            Console.Write("Codigo del propietario: ");
            string strCodigo = Console.ReadLine();

            Console.Write("Nombre completo: ");
            string strNombre = Console.ReadLine();

            Console.Write("Correo: ");
            string strCorreo = Console.ReadLine();

            Console.Write("Telefono: ");
            string strTelefono = Console.ReadLine();

            Console.Write("Direccion: ");
            string strDireccion = Console.ReadLine();

            Console.Write("DPI: ");
            string strDpi = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(strCodigo) || string.IsNullOrWhiteSpace(strNombre))
            {
                throw new ArgumentException("El codigo y el nombre del propietario son obligatorios.");
            }

            Propietario objPropietario = new Propietario(strCodigo, strNombre, strCorreo, strTelefono, strDireccion, strDpi);
            objGestor.registrarPropietario(objPropietario);   // valida y guarda en SQLite

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\nPropietario registrado correctamente.");
            Console.ResetColor();
            objPropietario.mostrarInformacion();
        }

        static void registrarMascota(GestorClinico objGestor)
        {
            Propietario objPropietario = solicitarPropietario(objGestor);

            Console.Write("Nombre de la mascota: ");
            string strNombre = Console.ReadLine();

            Console.Write("Especie (Canino/Felino/Ave/Exotico): ");
            string strEspecie = Console.ReadLine();

            Console.Write("Raza: ");
            string strRaza = Console.ReadLine();

            Console.Write("Sexo (Macho/Hembra): ");
            string strSexo = Console.ReadLine();

            Console.Write("Edad en anios: ");
            int intEdad = int.Parse(Console.ReadLine() ?? "0");

            Console.Write("Peso en kg: ");
            double dblPeso = double.Parse(Console.ReadLine() ?? "0");

            Mascota objMascota = objGestor.registrarMascota(strNombre, strEspecie, strRaza, strSexo, intEdad, dblPeso, objPropietario);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\nMascota registrada correctamente.");
            Console.ResetColor();
            objMascota.mostrarResumen();
        }

        static void crearConsulta(GestorClinico objGestor)
        {
            Mascota objMascota = solicitarMascota(objGestor);

            Console.Write("Motivo de la consulta: ");
            string strMotivo = Console.ReadLine();

            Console.Write("Sintomas observados: ");
            string strSintomas = Console.ReadLine();

            Console.Write("Urgencia (Baja/Media/Alta/Critica): ");
            string strUrgencia = Console.ReadLine();

            Consulta objConsulta = objGestor.crearConsulta(strMotivo, strSintomas, strUrgencia, objMascota);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\nConsulta creada correctamente.");
            Console.ResetColor();
            objConsulta.mostrarResumen();
        }

        static void registrarExamen(GestorClinico objGestor)
        {
            Consulta objConsulta = solicitarConsulta(objGestor);

            Console.Write("Tipo de examen (Sangre/Radiografia/Ultrasonido/Fisico): ");
            string strTipo = Console.ReadLine();

            Console.Write("Resultado del examen: ");
            string strResultado = Console.ReadLine();

            Console.Write("Severidad (Baja/Media/Alta/Critica): ");
            string strSeveridad = Console.ReadLine();

            objConsulta.registrarExamen(strTipo, strResultado, strSeveridad);
            Console.WriteLine("Examen registrado correctamente.");
            objConsulta.mostrarBitacora();
        }

        static void diagnosticarConsulta(GestorClinico objGestor, FlujoConsulta objFlujoConsulta)
        {
            Consulta objConsulta = solicitarConsulta(objGestor);

            if (!objFlujoConsulta.puedeCambiarEstado(objConsulta.strEstado, "Diagnosticada"))
            {
                throw new InvalidOperationException("El flujo no permite diagnosticar la consulta desde el estado actual.");
            }

            Console.Write("Diagnostico: ");
            string strDiagnostico = Console.ReadLine();

            Console.Write("Tratamiento indicado: ");
            string strTratamiento = Console.ReadLine();

            objConsulta.diagnosticar(strDiagnostico, strTratamiento);
            Console.WriteLine("Consulta diagnosticada correctamente.");
            objConsulta.mostrarBitacora();
        }

        static void cerrarConsulta(GestorClinico objGestor, FlujoConsulta objFlujoConsulta)
        {
            Consulta objConsulta = solicitarConsulta(objGestor);

            if (!objFlujoConsulta.puedeCambiarEstado(objConsulta.strEstado, "Cerrada"))
            {
                throw new InvalidOperationException("El flujo no permite cerrar la consulta desde el estado actual.");
            }

            objConsulta.cerrar();
            Console.WriteLine("Consulta cerrada correctamente.");
            objConsulta.mostrarBitacora();
        }

        static void aplicarVacuna(GestorClinico objGestor)
        {
            Mascota objMascota = solicitarMascota(objGestor);

            Console.Write("Nombre de la vacuna: ");
            string strVacuna = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(strVacuna))
            {
                throw new ArgumentException("Debe indicar el nombre de la vacuna.");
            }

            objMascota.aplicarVacuna(strVacuna);
            Console.WriteLine("Vacuna aplicada y registrada en el expediente.");
        }

        static Propietario solicitarPropietario(GestorClinico objGestor)
        {
            Console.WriteLine("\nPropietarios disponibles:");
            for (int i = 0; i < objGestor.lstPropietarios.Count; i++)
            {
                Console.WriteLine($" {i + 1}. {objGestor.lstPropietarios[i].strNombre} - {objGestor.lstPropietarios[i].strCodigo}");
            }

            Console.Write("Seleccione propietario: ");
            int intIndice = int.Parse(Console.ReadLine() ?? "0") - 1;

            if (intIndice < 0 || intIndice >= objGestor.lstPropietarios.Count)
            {
                throw new ArgumentOutOfRangeException("Propietario", "Seleccion fuera de rango.");
            }

            return objGestor.lstPropietarios[intIndice];
        }

        static Mascota solicitarMascota(GestorClinico objGestor)
        {
            objGestor.mostrarMascotas();
            Console.Write("Ingrese codigo de la mascota (ejemplo M01): ");
            string strCodigo = Console.ReadLine()?.Trim();
            return objGestor.buscarMascota(strCodigo);
        }

        static Consulta solicitarConsulta(GestorClinico objGestor)
        {
            Console.Write("Ingrese numero de consulta: ");
            int intNumero = int.Parse(Console.ReadLine() ?? "0");
            return objGestor.buscarConsulta(intNumero);
        }
    }
}
