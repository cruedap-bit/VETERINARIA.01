namespace ClinicaVeterinaria.logica
{
    public class Veterinario : Usuario
    {
        public string strEspecialidad { get; }
        public bool blnDisponible { get; }
        public bool blnEmergencia { get; }

        public Veterinario(string strCodigo, string strNombre, string strCorreo, string strTelefono,
                           string strEspecialidad, bool blnDisponible, bool blnEmergencia)
            : base(strCodigo, strNombre, strCorreo, strTelefono)
        {
            this.strEspecialidad = string.IsNullOrWhiteSpace(strEspecialidad) ? "General" : strEspecialidad.Trim();
            this.blnDisponible = blnDisponible;
            this.blnEmergencia = blnEmergencia;
        }

        public override string strRol
        {
            get { return blnEmergencia ? "Veterinario de emergencia" : "Veterinario"; }
        }

        /// <summary>Un veterinario General atiende cualquier especie.</summary>
        public bool atiende(string strEspecie)
        {
            if (strEspecialidad.Equals("General", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            return Validador.quitarAcentos(strEspecialidad)
                .Equals(Validador.quitarAcentos(strEspecie), StringComparison.OrdinalIgnoreCase);
        }

        public override void mostrarInformacion()
        {
            Console.WriteLine($"[{strRol}] {strCodigo} - {strNombre}");
            Console.WriteLine($"    Especialidad: {strEspecialidad} | Disponible: {(blnDisponible ? "Si" : "No")}");
            Console.WriteLine($"    Correo: {strCorreo} | Tel: {strTelefono}");
        }
    }
}
