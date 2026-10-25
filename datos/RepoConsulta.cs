using System.Globalization;
using ClinicaVeterinaria.logica;

namespace ClinicaVeterinaria.datos
{
    public static class RepoConsulta
    {
        /// <summary>Inserta la consulta y deja el numero autogenerado en objConsulta.intNumero.</summary>
        public static void crear(Sesion objSesion, Consulta objConsulta)
        {
            objConsulta.intNumero = (int)objSesion.Insertar(
                "INSERT INTO Consulta(mascota, veterinario, motivo, sintomas, urgencia, estado, emergencia, diagnostico, tratamiento, fecha) " +
                "VALUES(@m, @v, @mo, @si, @u, @e, @em, @d, @t, @f)",
                "@m", objConsulta.objMascota.strCodigo,
                "@v", objConsulta.objVeterinario?.strCodigo,
                "@mo", objConsulta.strMotivo,
                "@si", objConsulta.strSintomas,
                "@u", objConsulta.strUrgencia,
                "@e", objConsulta.strEstado,
                "@em", objConsulta.blnEmergencia ? 1 : 0,
                "@d", objConsulta.strDiagnostico,
                "@t", objConsulta.strTratamiento,
                "@f", objConsulta.dtmFecha.ToString(Constantes.FORMATO_FECHA, CultureInfo.InvariantCulture));
        }

        public static void actualizar(Sesion objSesion, Consulta objConsulta)
        {
            objSesion.Ejecutar(
                "UPDATE Consulta SET veterinario = @v, urgencia = @u, estado = @e, emergencia = @em, " +
                "diagnostico = @d, tratamiento = @t WHERE numero = @n",
                "@v", objConsulta.objVeterinario?.strCodigo,
                "@u", objConsulta.strUrgencia,
                "@e", objConsulta.strEstado,
                "@em", objConsulta.blnEmergencia ? 1 : 0,
                "@d", objConsulta.strDiagnostico,
                "@t", objConsulta.strTratamiento,
                "@n", objConsulta.intNumero);
        }

        public static void crearExamen(Sesion objSesion, int intConsulta, Examen objExamen)
        {
            objSesion.Ejecutar(
                "INSERT INTO Examen(consulta, tipo, resultado, severidad, fecha) VALUES(@c, @t, @r, @s, @f)",
                "@c", intConsulta,
                "@t", objExamen.strTipo,
                "@r", objExamen.strResultado,
                "@s", objExamen.strSeveridad,
                "@f", objExamen.dtmFecha.ToString(Constantes.FORMATO_FECHA, CultureInfo.InvariantCulture));
        }

        public static void agregarBitacora(Sesion objSesion, int intConsulta, string strEvento)
        {
            objSesion.Ejecutar(
                "INSERT INTO Bitacora(consulta, fecha, evento) VALUES(@c, @f, @e)",
                "@c", intConsulta, "@f", Constantes.ahora(), "@e", strEvento);
        }

        public static List<string> listarBitacora(int intConsulta)
        {
            List<string> lstResultado = new List<string>();

            using (Sesion objSesion = new Sesion())
            {
                foreach (var f in objSesion.Consultar(
                    "SELECT fecha, evento FROM Bitacora WHERE consulta = @c ORDER BY id", "@c", intConsulta))
                {
                    lstResultado.Add(Sesion.leerTexto(f["fecha"]) + " - " + Sesion.leerTexto(f["evento"]));
                }
            }

            return lstResultado;
        }

        /// <summary>Carga todas las consultas con sus examenes, enlazando mascotas y veterinarios ya cargados.</summary>
        public static List<Consulta> listar(Dictionary<string, Mascota> dicMascotas, Dictionary<string, Veterinario> dicVeterinarios)
        {
            List<Consulta> lstResultado = new List<Consulta>();
            Dictionary<int, Consulta> dicPorNumero = new Dictionary<int, Consulta>();

            using (Sesion objSesion = new Sesion())
            {
                foreach (var f in objSesion.Consultar("SELECT * FROM Consulta ORDER BY numero"))
                {
                    Consulta objConsulta = new Consulta(dicMascotas[Sesion.leerTexto(f["mascota"])])
                    {
                        intNumero = Sesion.leerInt(f["numero"]),
                        strMotivo = Sesion.leerTexto(f["motivo"]),
                        strSintomas = Sesion.leerTexto(f["sintomas"]),
                        strUrgencia = Sesion.leerTexto(f["urgencia"]),
                        strEstado = Sesion.leerTexto(f["estado"]),
                        blnEmergencia = Sesion.leerBool(f["emergencia"]),
                        strDiagnostico = Sesion.leerTexto(f["diagnostico"]),
                        strTratamiento = Sesion.leerTexto(f["tratamiento"]),
                        dtmFecha = Constantes.leerFecha(Sesion.leerTexto(f["fecha"]))
                    };

                    string strVet = Sesion.leerTexto(f["veterinario"]);
                    if (strVet != "" && dicVeterinarios.ContainsKey(strVet))
                    {
                        objConsulta.objVeterinario = dicVeterinarios[strVet];
                    }

                    lstResultado.Add(objConsulta);
                    dicPorNumero[objConsulta.intNumero] = objConsulta;
                }

                foreach (var f in objSesion.Consultar("SELECT * FROM Examen ORDER BY id"))
                {
                    int intNumero = Sesion.leerInt(f["consulta"]);
                    if (dicPorNumero.ContainsKey(intNumero))
                    {
                        Examen objExamen = new Examen(
                            Sesion.leerTexto(f["tipo"]),
                            Sesion.leerTexto(f["resultado"]),
                            Sesion.leerTexto(f["severidad"]));
                        objExamen.dtmFecha = Constantes.leerFecha(Sesion.leerTexto(f["fecha"]));
                        dicPorNumero[intNumero].lstExamenes.Add(objExamen);
                    }
                }
            }

            return lstResultado;
        }
    }
}
