namespace ClinicaVeterinaria.logica
{
    public class Propietario : Usuario
    {
        public string strDireccion { get; }
        public string strDpi { get; }

        public Propietario(string strCodigo, string strNombre, string strCorreo, string strTelefono, string strDireccion, string strDpi)
            : base(strCodigo, strNombre, strCorreo, strTelefono)
        {
            this.strDireccion = (strDireccion ?? "").Trim();
            this.strDpi = (strDpi ?? "").Trim();
        }

        public override string strRol
        {
            get { return "Propietario"; }
        }

        public override void mostrarInformacion()
        {
            Console.WriteLine($"[{strRol}] {strCodigo} - {strNombre}");
            Console.WriteLine($"    Correo: {strCorreo} | Tel: {strTelefono}");
            Console.WriteLine($"    Direccion: {strDireccion} | DPI: {strDpi}");
        }
    }
}
