using ClinicaVeterinaria.logica;

namespace ClinicaVeterinaria.datos
{
    public static class RepoVeterinario
    {
        public static void crear(Sesion objSesion, Veterinario objVeterinario)
        {
            objSesion.Ejecutar(
                "INSERT INTO Veterinario(codigo, nombre, correo, telefono, especialidad, disponible, emergencia) VALUES(@c, @n, @co, @t, @e, @di, @em)",
                "@c", objVeterinario.strCodigo,
                "@n", objVeterinario.strNombre,
                "@co", objVeterinario.strCorreo,
                "@t", objVeterinario.strTelefono,
                "@e", objVeterinario.strEspecialidad,
                "@di", objVeterinario.blnDisponible ? 1 : 0,
                "@em", objVeterinario.blnEmergencia ? 1 : 0);
        }

        public static List<Veterinario> listar()
        {
            List<Veterinario> lstResultado = new List<Veterinario>();

            using (Sesion objSesion = new Sesion())
            {
                foreach (var f in objSesion.Consultar("SELECT * FROM Veterinario ORDER BY codigo"))
                {
                    lstResultado.Add(new Veterinario(
                        Sesion.leerTexto(f["codigo"]),
                        Sesion.leerTexto(f["nombre"]),
                        Sesion.leerTexto(f["correo"]),
                        Sesion.leerTexto(f["telefono"]),
                        Sesion.leerTexto(f["especialidad"]),
                        Sesion.leerBool(f["disponible"]),
                        Sesion.leerBool(f["emergencia"])));
                }
            }

            return lstResultado;
        }
    }
}
