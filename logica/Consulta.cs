using ClinicaVeterinaria.datos;

namespace ClinicaVeterinaria.logica
{
    public class Consulta
    {
        public int intNumero { get; set; }
        public Mascota objMascota { get; }
        public Veterinario objVeterinario { get; set; }
        public string strMotivo { get; set; }
        public string strSintomas { get; set; }
        public string strUrgencia { get; set; }
        public string strEstado { get; set; }
        public string strDiagnostico { get; set; }
        public string strTratamiento { get; set; }
        public bool blnEmergencia { get; set; }
        public DateTime dtmFecha { get; set; }
        public List<Examen> lstExamenes { get; } = new List<Examen>();

        private readonly FlujoConsulta objFlujo = new FlujoConsulta();

        public Consulta(Mascota objMascota)
        {
            this.objMascota = objMascota;
            strEstado = "Registrada";
            strDiagnostico = "";
            strTratamiento = "";
            dtmFecha = DateTime.Now;
        }

        /// <summary>Escribe el evento en la bitacora de la consulta y en el expediente de la mascota.</summary>
        public void registrarEvento(Sesion objSesion, string strEvento)
        {
            RepoConsulta.agregarBitacora(objSesion, intNumero, strEvento);
            RepoMascota.agregarExpediente(objSesion, objMascota.strCodigo, $"Consulta #{intNumero}: {strEvento}");
        }

        public void registrarExamen(string strTipo, string strResultado, string strSeveridad)
        {
            if (strEstado == "Cerrada")
            {
                throw new InvalidOperationException("No se pueden registrar examenes en una consulta cerrada.");
            }

            strTipo = Validador.normalizar(strTipo, Constantes.TiposExamen, "El tipo de examen");
            strSeveridad = Validador.normalizar(strSeveridad, Constantes.Urgencias, "La severidad");
            strResultado = Validador.requerido(strResultado, "El resultado");

            Examen objExamen = new Examen(strTipo, strResultado, strSeveridad);
            bool blnGrave = strSeveridad == "Alta" || strSeveridad == "Critica";

            string strUrgenciaAnterior = strUrgencia;
            bool blnEmergenciaAnterior = blnEmergencia;

            try
            {
                using (Sesion objSesion = new Sesion())
                {
                    RepoConsulta.crearExamen(objSesion, intNumero, objExamen);
                    registrarEvento(objSesion, $"Examen {strTipo} registrado (severidad {strSeveridad}): {strResultado}");

                    if (blnGrave && (!blnEmergencia || strUrgencia != "Critica"))
                    {
                        blnEmergencia = true;
                        strUrgencia = "Critica";
                        RepoConsulta.actualizar(objSesion, this);
                        registrarEvento(objSesion, "Severidad alta: consulta derivada a EMERGENCIA y urgencia elevada a Critica");
                    }

                    objSesion.Confirmar();
                }
            }
            catch
            {
                strUrgencia = strUrgenciaAnterior;
                blnEmergencia = blnEmergenciaAnterior;
                throw;
            }

            lstExamenes.Add(objExamen);
        }

        public void diagnosticar(string strDiagnosticoNuevo, string strTratamientoNuevo)
        {
            if (!objFlujo.puedeCambiarEstado(strEstado, "Diagnosticada"))
            {
                throw new InvalidOperationException("Solo se puede diagnosticar una consulta en estado Asignada.");
            }

            strDiagnosticoNuevo = Validador.requerido(strDiagnosticoNuevo, "El diagnostico");
            strTratamientoNuevo = Validador.requerido(strTratamientoNuevo, "El tratamiento");

            string strEstadoAnterior = strEstado;
            string strDiagnosticoAnterior = strDiagnostico;
            string strTratamientoAnterior = strTratamiento;

            strEstado = "Diagnosticada";
            strDiagnostico = strDiagnosticoNuevo;
            strTratamiento = strTratamientoNuevo;

            try
            {
                guardarCambio($"Diagnosticada: {strDiagnostico}. Tratamiento: {strTratamiento}");
            }
            catch
            {
                strEstado = strEstadoAnterior;
                strDiagnostico = strDiagnosticoAnterior;
                strTratamiento = strTratamientoAnterior;
                throw;
            }
        }

        public void cerrar()
        {
            if (!objFlujo.puedeCambiarEstado(strEstado, "Cerrada"))
            {
                throw new InvalidOperationException("Solo se puede cerrar una consulta en estado Diagnosticada.");
            }

            string strEstadoAnterior = strEstado;
            strEstado = "Cerrada";

            try
            {
                guardarCambio("Consulta cerrada");
            }
            catch
            {
                strEstado = strEstadoAnterior;
                throw;
            }
        }

        private void guardarCambio(string strEvento)
        {
            using (Sesion objSesion = new Sesion())
            {
                RepoConsulta.actualizar(objSesion, this);
                registrarEvento(objSesion, strEvento);
                objSesion.Confirmar();
            }
        }

        public void mostrarResumen()
        {
            Console.WriteLine($"  Consulta #{intNumero} | {dtmFecha:yyyy-MM-dd HH:mm} | Estado: {strEstado}");
            Console.WriteLine($"    Mascota: {objMascota.strNombre} ({objMascota.strCodigo}) - Propietario: {objMascota.objPropietario.strNombre}");
            Console.WriteLine($"    Motivo: {strMotivo} | Sintomas: {strSintomas}");
            Console.WriteLine($"    Urgencia: {strUrgencia}{(blnEmergencia ? " | EMERGENCIA" : "")}");
            Console.WriteLine($"    Veterinario: {(objVeterinario != null ? objVeterinario.strNombre : "(sin asignar)")} | Examenes: {lstExamenes.Count}");

            if (!string.IsNullOrEmpty(strDiagnostico))
            {
                Console.WriteLine($"    Diagnostico: {strDiagnostico} | Tratamiento: {strTratamiento}");
            }
        }

        public void mostrarBitacora()
        {
            Console.WriteLine($"\n  Bitacora de la consulta #{intNumero}:");
            foreach (string strLinea in RepoConsulta.listarBitacora(intNumero))
            {
                Console.WriteLine("    " + strLinea);
            }
        }
    }
}
