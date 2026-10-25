namespace ClinicaVeterinaria.logica
{
    public class Examen
    {
        public string strTipo { get; }
        public string strResultado { get; }
        public string strSeveridad { get; }
        public DateTime dtmFecha { get; set; }

        public Examen(string strTipo, string strResultado, string strSeveridad)
        {
            this.strTipo = strTipo;
            this.strResultado = strResultado;
            this.strSeveridad = strSeveridad;
            dtmFecha = DateTime.Now;
        }
    }
}
