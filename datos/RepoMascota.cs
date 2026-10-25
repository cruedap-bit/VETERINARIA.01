using ClinicaVeterinaria.logica;

namespace ClinicaVeterinaria.datos
{
    public static class RepoMascota
    {
        public static void crear(Sesion objSesion, Mascota objMascota)
        {
            objSesion.Ejecutar(
                "INSERT INTO Mascota(codigo, nombre, especie, raza, sexo, edad, peso, propietario) VALUES(@c, @n, @es, @r, @s, @ed, @p, @pr)",
                "@c", objMascota.strCodigo,
                "@n", objMascota.strNombre,
                "@es", objMascota.strEspecie,
                "@r", objMascota.strRaza,
                "@s", objMascota.strSexo,
                "@ed", objMascota.intEdad,
                "@p", objMascota.dblPeso,
                "@pr", objMascota.objPropietario.strCodigo);
        }

        public static List<Mascota> listar(Dictionary<string, Propietario> dicPropietarios)
        {
            List<Mascota> lstResultado = new List<Mascota>();

            using (Sesion objSesion = new Sesion())
            {
                foreach (var f in objSesion.Consultar("SELECT * FROM Mascota ORDER BY codigo"))
                {
                    lstResultado.Add(new Mascota(
                        Sesion.leerTexto(f["codigo"]),
                        Sesion.leerTexto(f["nombre"]),
                        Sesion.leerTexto(f["especie"]),
                        Sesion.leerTexto(f["raza"]),
                        Sesion.leerTexto(f["sexo"]),
                        Sesion.leerInt(f["edad"]),
                        Sesion.leerDouble(f["peso"]),
                        dicPropietarios[Sesion.leerTexto(f["propietario"])]));
                }
            }

            return lstResultado;
        }

        // ---- Vacunas ----
        public static void crearVacuna(Sesion objSesion, string strMascota, string strVacuna)
        {
            objSesion.Ejecutar(
                "INSERT INTO VacunaAplicada(mascota, vacuna, fecha) VALUES(@m, @v, @f)",
                "@m", strMascota, "@v", strVacuna, "@f", Constantes.ahora());
        }

        public static List<string> listarVacunas(string strMascota)
        {
            List<string> lstResultado = new List<string>();

            using (Sesion objSesion = new Sesion())
            {
                foreach (var f in objSesion.Consultar(
                    "SELECT fecha, vacuna FROM VacunaAplicada WHERE mascota = @m ORDER BY id", "@m", strMascota))
                {
                    lstResultado.Add(Sesion.leerTexto(f["fecha"]) + " - " + Sesion.leerTexto(f["vacuna"]));
                }
            }

            return lstResultado;
        }

        // ---- Expediente (historial) ----
        public static void agregarExpediente(Sesion objSesion, string strMascota, string strEvento)
        {
            objSesion.Ejecutar(
                "INSERT INTO Expediente(mascota, fecha, evento) VALUES(@m, @f, @e)",
                "@m", strMascota, "@f", Constantes.ahora(), "@e", strEvento);
        }

        public static List<string> listarExpediente(string strMascota)
        {
            List<string> lstResultado = new List<string>();

            using (Sesion objSesion = new Sesion())
            {
                foreach (var f in objSesion.Consultar(
                    "SELECT fecha, evento FROM Expediente WHERE mascota = @m ORDER BY id", "@m", strMascota))
                {
                    lstResultado.Add(Sesion.leerTexto(f["fecha"]) + " - " + Sesion.leerTexto(f["evento"]));
                }
            }

            return lstResultado;
        }
    }
}
