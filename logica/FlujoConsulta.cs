namespace ClinicaVeterinaria.logica
{
    /// <summary>Registrada -> Asignada -> Diagnosticada -> Cerrada</summary>
    public class FlujoConsulta
    {
        private readonly Dictionary<string, string> dicSiguiente = new Dictionary<string, string>
        {
            { "Registrada", "Asignada" },
            { "Asignada", "Diagnosticada" },
            { "Diagnosticada", "Cerrada" }
        };

        public bool puedeCambiarEstado(string strActual, string strNuevo)
        {
            return dicSiguiente.TryGetValue(strActual ?? "", out string strSiguiente) && strSiguiente == strNuevo;
        }

        public void mostrarFlujo()
        {
            Console.WriteLine("\nFLUJO DE ESTADOS DE LA CONSULTA");
            Console.WriteLine("  Registrada    -> se crea la consulta");
            Console.WriteLine("  Asignada      -> un veterinario la atiende (automatico, o emergencia)");
            Console.WriteLine("  Diagnosticada -> se registra diagnostico y tratamiento");
            Console.WriteLine("  Cerrada       -> consulta finalizada");
            Console.WriteLine("\n  Registrada -> Asignada -> Diagnosticada -> Cerrada");
        }
    }
}
