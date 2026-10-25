namespace ClinicaVeterinaria.logica
{
    public abstract class Usuario
    {
        public string strCodigo { get; }
        public string strNombre { get; }
        public string strCorreo { get; }
        public string strTelefono { get; }

        protected Usuario(string strCodigo, string strNombre, string strCorreo, string strTelefono)
        {
            this.strCodigo = (strCodigo ?? "").Trim();
            this.strNombre = (strNombre ?? "").Trim();
            this.strCorreo = (strCorreo ?? "").Trim();
            this.strTelefono = (strTelefono ?? "").Trim();
        }

        public abstract string strRol { get; }

        public abstract void mostrarInformacion();
    }
}
