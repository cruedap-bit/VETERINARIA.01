using ClinicaVeterinaria.logica;

namespace ClinicaVeterinaria.datos
{
    public static class RepoPropietario
    {
        public static void crear(Sesion objSesion, Propietario objPropietario)
        {
            objSesion.Ejecutar(
                "INSERT INTO Propietario(codigo, nombre, correo, telefono, direccion, dpi) VALUES(@c, @n, @co, @t, @d, @dpi)",
                "@c", objPropietario.strCodigo,
                "@n", objPropietario.strNombre,
                "@co", objPropietario.strCorreo,
                "@t", objPropietario.strTelefono,
                "@d", objPropietario.strDireccion,
                "@dpi", objPropietario.strDpi);
        }

        public static List<Propietario> listar()
        {
            List<Propietario> lstResultado = new List<Propietario>();

            using (Sesion objSesion = new Sesion())
            {
                foreach (var f in objSesion.Consultar("SELECT * FROM Propietario ORDER BY codigo"))
                {
                    lstResultado.Add(new Propietario(
                        Sesion.leerTexto(f["codigo"]),
                        Sesion.leerTexto(f["nombre"]),
                        Sesion.leerTexto(f["correo"]),
                        Sesion.leerTexto(f["telefono"]),
                        Sesion.leerTexto(f["direccion"]),
                        Sesion.leerTexto(f["dpi"])));
                }
            }

            return lstResultado;
        }
    }
}
