using ClinicaVeterinaria.datos;

namespace ClinicaVeterinaria.logica
{
    public class GestorClinico
    {
        public List<Propietario> lstPropietarios { get; } = new List<Propietario>();
        public List<Veterinario> lstVeterinarios { get; } = new List<Veterinario>();
        public List<Mascota> lstMascotas { get; } = new List<Mascota>();
        public List<Consulta> lstConsultas { get; } = new List<Consulta>();

        // ---------------------------------------------------------------
        // Carga inicial desde SQLite
        // ---------------------------------------------------------------
        public void cargarDatosBase()
        {
            BaseDatos.inicializar();

            if (BaseDatos.contar("Propietario") == 0 && BaseDatos.contar("Veterinario") == 0)
            {
                sembrarDatos();
            }

            cargarDesdeBD();
        }

        private void cargarDesdeBD()
        {
            lstPropietarios.Clear();
            lstVeterinarios.Clear();
            lstMascotas.Clear();
            lstConsultas.Clear();

            lstPropietarios.AddRange(RepoPropietario.listar());
            lstVeterinarios.AddRange(RepoVeterinario.listar());

            Dictionary<string, Propietario> dicPropietarios = lstPropietarios.ToDictionary(p => p.strCodigo);
            lstMascotas.AddRange(RepoMascota.listar(dicPropietarios));

            Dictionary<string, Mascota> dicMascotas = lstMascotas.ToDictionary(m => m.strCodigo);
            Dictionary<string, Veterinario> dicVeterinarios = lstVeterinarios.ToDictionary(v => v.strCodigo);
            lstConsultas.AddRange(RepoConsulta.listar(dicMascotas, dicVeterinarios));
        }

        /// <summary>Datos de ejemplo: solo se insertan cuando la base esta vacia.</summary>
        private void sembrarDatos()
        {
            Propietario objP1 = new Propietario("P01", "Maria Lopez", "maria@correo.com", "5555-1111", "Zona 10, Guatemala", "2345678901101");
            Propietario objP2 = new Propietario("P02", "Jose Perez", "jose@correo.com", "5555-2222", "Zona 1, Guatemala", "2345678901202");

            using (Sesion objSesion = new Sesion())
            {
                RepoPropietario.crear(objSesion, objP1);
                RepoPropietario.crear(objSesion, objP2);

                RepoVeterinario.crear(objSesion, new Veterinario("V01", "Dra. Ana Morales", "ana@clinica.com", "5555-3001", "Canino", true, false));
                RepoVeterinario.crear(objSesion, new Veterinario("V02", "Dr. Luis Herrera", "luis@clinica.com", "5555-3002", "Felino", true, false));
                RepoVeterinario.crear(objSesion, new Veterinario("V03", "Dra. Sofia Castillo", "sofia@clinica.com", "5555-3003", "Ave", true, false));
                RepoVeterinario.crear(objSesion, new Veterinario("V04", "Dr. Pablo Ruiz", "pablo@clinica.com", "5555-3004", "General", true, false));
                RepoVeterinario.crear(objSesion, new Veterinario("V05", "Dr. Carlos Mendez", "carlos@clinica.com", "5555-3005", "General", true, true));

                Mascota objM1 = new Mascota("M01", "Firulais", "Canino", "Labrador", "Macho", 4, 28.5, objP1);
                Mascota objM2 = new Mascota("M02", "Michi", "Felino", "Siames", "Hembra", 2, 4.2, objP2);
                RepoMascota.crear(objSesion, objM1);
                RepoMascota.crear(objSesion, objM2);
                RepoMascota.agregarExpediente(objSesion, "M01", "Mascota registrada");
                RepoMascota.agregarExpediente(objSesion, "M02", "Mascota registrada");

                objSesion.Confirmar();
            }
        }

        // ---------------------------------------------------------------
        // Propietarios y mascotas
        // ---------------------------------------------------------------
        public void registrarPropietario(Propietario objPropietario)
        {
            Validador.requerido(objPropietario.strCodigo, "El codigo del propietario");
            Validador.requerido(objPropietario.strNombre, "El nombre del propietario");

            if (lstPropietarios.Any(p => p.strCodigo.Equals(objPropietario.strCodigo, StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException("Ya existe un propietario con el codigo " + objPropietario.strCodigo + ".");
            }

            using (Sesion objSesion = new Sesion())
            {
                RepoPropietario.crear(objSesion, objPropietario);
                objSesion.Confirmar();
            }

            lstPropietarios.Add(objPropietario);
        }

        public Mascota registrarMascota(string strNombre, string strEspecie, string strRaza, string strSexo,
                                        int intEdad, double dblPeso, Propietario objPropietario)
        {
            strNombre = Validador.requerido(strNombre, "El nombre de la mascota");
            strEspecie = Validador.normalizar(strEspecie, Constantes.Especies, "La especie");
            strSexo = Validador.normalizar(strSexo, Constantes.Sexos, "El sexo");
            strRaza = (strRaza ?? "").Trim();

            if (intEdad < 0)
            {
                throw new ArgumentException("La edad no puede ser negativa.");
            }

            if (dblPeso <= 0)
            {
                throw new ArgumentException("El peso debe ser mayor que cero.");
            }

            Mascota objMascota = new Mascota(siguienteCodigoMascota(), strNombre, strEspecie, strRaza, strSexo, intEdad, dblPeso, objPropietario);

            using (Sesion objSesion = new Sesion())
            {
                RepoMascota.crear(objSesion, objMascota);
                RepoMascota.agregarExpediente(objSesion, objMascota.strCodigo, "Mascota registrada");
                objSesion.Confirmar();
            }

            lstMascotas.Add(objMascota);
            return objMascota;
        }

        private string siguienteCodigoMascota()
        {
            int intMayor = 0;

            foreach (Mascota objMascota in lstMascotas)
            {
                if (objMascota.strCodigo.Length > 1 && int.TryParse(objMascota.strCodigo.Substring(1), out int intNumero) && intNumero > intMayor)
                {
                    intMayor = intNumero;
                }
            }

            return "M" + (intMayor + 1).ToString("00");
        }

        public Mascota buscarMascota(string strCodigo)
        {
            Mascota objMascota = lstMascotas.FirstOrDefault(m => m.strCodigo.Equals((strCodigo ?? "").Trim(), StringComparison.OrdinalIgnoreCase));

            if (objMascota == null)
            {
                throw new KeyNotFoundException("No existe una mascota con el codigo '" + strCodigo + "'.");
            }

            return objMascota;
        }

        public Consulta buscarConsulta(int intNumero)
        {
            Consulta objConsulta = lstConsultas.FirstOrDefault(c => c.intNumero == intNumero);

            if (objConsulta == null)
            {
                throw new KeyNotFoundException("No existe la consulta numero " + intNumero + ".");
            }

            return objConsulta;
        }

        // ---------------------------------------------------------------
        // Consultas: asignacion automatica
        // ---------------------------------------------------------------
        /// <summary>Carga = consultas Asignadas o Diagnosticadas sin cerrar (se calcula siempre, nunca queda desfasada).</summary>
        public int calcularCarga(Veterinario objVeterinario)
        {
            return lstConsultas.Count(c => c.objVeterinario != null
                                        && c.objVeterinario.strCodigo == objVeterinario.strCodigo
                                        && (c.strEstado == "Asignada" || c.strEstado == "Diagnosticada"));
        }

        private Veterinario elegirVeterinario(string strEspecie)
        {
            return lstVeterinarios
                .Where(v => v.blnDisponible && !v.blnEmergencia && v.atiende(strEspecie) && calcularCarga(v) < Constantes.CargaMaxima)
                .OrderBy(v => calcularCarga(v))
                .ThenBy(v => v.strEspecialidad == "General" ? 1 : 0)   // a igual carga, primero el especialista
                .ThenBy(v => v.strCodigo)
                .FirstOrDefault();
        }

        public Consulta crearConsulta(string strMotivo, string strSintomas, string strUrgencia, Mascota objMascota)
        {
            strMotivo = Validador.requerido(strMotivo, "El motivo de la consulta");
            strUrgencia = Validador.normalizar(strUrgencia, Constantes.Urgencias, "La urgencia");

            Consulta objConsulta = new Consulta(objMascota)
            {
                strMotivo = strMotivo,
                strSintomas = (strSintomas ?? "").Trim(),
                strUrgencia = strUrgencia
            };

            string strEventoAsignacion;
            Veterinario objVet = elegirVeterinario(objMascota.strEspecie);

            if (objVet != null)
            {
                objConsulta.objVeterinario = objVet;
                objConsulta.strEstado = "Asignada";
                strEventoAsignacion = "Asignada automaticamente a " + objVet.strNombre;
            }
            else
            {
                // Ningun veterinario disponible para la especie: pasa a emergencia.
                objConsulta.blnEmergencia = true;
                Veterinario objEmergencia = lstVeterinarios.FirstOrDefault(v => v.blnEmergencia && v.blnDisponible);

                if (objEmergencia != null)
                {
                    objConsulta.objVeterinario = objEmergencia;
                    objConsulta.strEstado = "Asignada";
                    strEventoAsignacion = "Sin veterinario disponible: derivada a EMERGENCIA (" + objEmergencia.strNombre + ")";
                }
                else
                {
                    strEventoAsignacion = "Sin veterinario disponible: queda en espera en EMERGENCIA";
                }
            }

            using (Sesion objSesion = new Sesion())
            {
                RepoConsulta.crear(objSesion, objConsulta);
                objConsulta.registrarEvento(objSesion, "Consulta registrada: " + strMotivo);
                objConsulta.registrarEvento(objSesion, strEventoAsignacion);
                objSesion.Confirmar();
            }

            lstConsultas.Add(objConsulta);
            return objConsulta;
        }

        // ---------------------------------------------------------------
        // Reportes en consola
        // ---------------------------------------------------------------
        public void mostrarUsuariosPolimorfismo()
        {
            List<Usuario> lstUsuarios = new List<Usuario>();
            lstUsuarios.AddRange(lstPropietarios);
            lstUsuarios.AddRange(lstVeterinarios);

            Console.WriteLine("\nUSUARIOS DEL SISTEMA");
            foreach (Usuario objUsuario in lstUsuarios)
            {
                objUsuario.mostrarInformacion();
            }
        }

        public void mostrarMascotas()
        {
            Console.WriteLine("\nMASCOTAS REGISTRADAS");

            if (lstMascotas.Count == 0)
            {
                Console.WriteLine("  (no hay mascotas)");
                return;
            }

            foreach (Mascota objMascota in lstMascotas)
            {
                objMascota.mostrarResumen();
            }
        }

        public void mostrarConsultas()
        {
            Console.WriteLine("\nCONSULTAS");

            if (lstConsultas.Count == 0)
            {
                Console.WriteLine("  (no hay consultas)");
                return;
            }

            foreach (Consulta objConsulta in lstConsultas)
            {
                objConsulta.mostrarResumen();
            }
        }

        public void generarResumenControl()
        {
            Console.WriteLine("\n========== RESUMEN DE CONTROL ==========");
            Console.WriteLine($"  Propietarios: {lstPropietarios.Count} | Mascotas: {lstMascotas.Count} | Veterinarios: {lstVeterinarios.Count}");
            Console.WriteLine($"  Consultas: {lstConsultas.Count} | En emergencia: {lstConsultas.Count(c => c.blnEmergencia)}");

            Console.WriteLine("\n  Consultas por estado:");
            foreach (string strEstado in new[] { "Registrada", "Asignada", "Diagnosticada", "Cerrada" })
            {
                Console.WriteLine($"    {strEstado,-14}: {lstConsultas.Count(c => c.strEstado == strEstado)}");
            }

            Console.WriteLine("\n  Mascotas por especie:");
            foreach (string strEspecie in Constantes.Especies)
            {
                Console.WriteLine($"    {strEspecie,-14}: {lstMascotas.Count(m => m.strEspecie == strEspecie)}");
            }

            Console.WriteLine("\n  Carga de veterinarios (maximo " + Constantes.CargaMaxima + "):");
            foreach (Veterinario objVet in lstVeterinarios)
            {
                Console.WriteLine($"    {objVet.strNombre,-22} {objVet.strEspecialidad,-9} carga: {calcularCarga(objVet)}{(objVet.blnEmergencia ? "  (emergencia)" : "")}");
            }
        }
    }
}
