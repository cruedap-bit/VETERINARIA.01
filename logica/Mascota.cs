using ClinicaVeterinaria.datos;

namespace ClinicaVeterinaria.logica
{
    public class Mascota
    {
        public string strCodigo { get; }
        public string strNombre { get; }
        public string strEspecie { get; }
        public string strRaza { get; }
        public string strSexo { get; }
        public int intEdad { get; }
        public double dblPeso { get; }
        public Propietario objPropietario { get; }

        public Mascota(string strCodigo, string strNombre, string strEspecie, string strRaza,
                       string strSexo, int intEdad, double dblPeso, Propietario objPropietario)
        {
            this.strCodigo = strCodigo;
            this.strNombre = strNombre;
            this.strEspecie = strEspecie;
            this.strRaza = strRaza;
            this.strSexo = strSexo;
            this.intEdad = intEdad;
            this.dblPeso = dblPeso;
            this.objPropietario = objPropietario;
        }

        public void mostrarResumen()
        {
            Console.WriteLine($"  {strCodigo} - {strNombre} ({strEspecie}, {strRaza}, {strSexo}, {intEdad} anios, {dblPeso:0.0} kg)");
            Console.WriteLine($"    Propietario: {objPropietario.strNombre} ({objPropietario.strCodigo})");
        }

        public void aplicarVacuna(string strVacuna)
        {
            strVacuna = Validador.requerido(strVacuna, "El nombre de la vacuna");

            using (Sesion objSesion = new Sesion())
            {
                RepoMascota.crearVacuna(objSesion, strCodigo, strVacuna);
                RepoMascota.agregarExpediente(objSesion, strCodigo, "Vacuna aplicada: " + strVacuna);
                objSesion.Confirmar();
            }
        }

        public void mostrarExpediente()
        {
            Console.WriteLine($"\n===== EXPEDIENTE CLINICO: {strNombre} ({strCodigo}) =====");
            mostrarResumen();

            Console.WriteLine("\n  Vacunas aplicadas:");
            List<string> lstVacunas = RepoMascota.listarVacunas(strCodigo);
            if (lstVacunas.Count == 0)
            {
                Console.WriteLine("    (sin vacunas registradas)");
            }
            foreach (string strVacuna in lstVacunas)
            {
                Console.WriteLine("    " + strVacuna);
            }

            Console.WriteLine("\n  Historial:");
            List<string> lstEventos = RepoMascota.listarExpediente(strCodigo);
            if (lstEventos.Count == 0)
            {
                Console.WriteLine("    (sin eventos)");
            }
            foreach (string strEvento in lstEventos)
            {
                Console.WriteLine("    " + strEvento);
            }
        }
    }
}
