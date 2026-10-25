using System.Globalization;
using System.Text;

namespace ClinicaVeterinaria.logica
{
    public static class Constantes
    {
        public static readonly string[] Especies = { "Canino", "Felino", "Ave", "Exotico" };
        public static readonly string[] Sexos = { "Macho", "Hembra" };
        public static readonly string[] Urgencias = { "Baja", "Media", "Alta", "Critica" };
        public static readonly string[] TiposExamen = { "Sangre", "Radiografia", "Ultrasonido", "Fisico" };

        /// <summary>Maximo de consultas activas (Asignada/Diagnosticada) por veterinario.</summary>
        public const int CargaMaxima = 3;

        public const string FORMATO_FECHA = "yyyy-MM-dd HH:mm:ss";

        public static string ahora()
        {
            return DateTime.Now.ToString(FORMATO_FECHA, CultureInfo.InvariantCulture);
        }

        public static DateTime leerFecha(string strFecha)
        {
            return DateTime.ParseExact(strFecha, FORMATO_FECHA, CultureInfo.InvariantCulture);
        }
    }

    public static class Validador
    {
        public static string quitarAcentos(string strTexto)
        {
            string strDescompuesto = (strTexto ?? "").Normalize(NormalizationForm.FormD);
            StringBuilder objSb = new StringBuilder();

            foreach (char c in strDescompuesto)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                {
                    objSb.Append(c);
                }
            }

            return objSb.ToString().Normalize(NormalizationForm.FormC);
        }

        public static string requerido(string strValor, string strCampo)
        {
            if (string.IsNullOrWhiteSpace(strValor))
            {
                throw new ArgumentException(strCampo + " es obligatorio.");
            }

            return strValor.Trim();
        }

        /// <summary>Devuelve el valor permitido que coincide (sin importar mayusculas ni acentos).</summary>
        public static string normalizar(string strValor, string[] arrPermitidos, string strCampo)
        {
            string strLimpio = requerido(strValor, strCampo);

            foreach (string strPermitido in arrPermitidos)
            {
                if (quitarAcentos(strPermitido).Equals(quitarAcentos(strLimpio), StringComparison.OrdinalIgnoreCase))
                {
                    return strPermitido;
                }
            }

            throw new ArgumentException(strCampo + " no es valido. Opciones: " + string.Join("/", arrPermitidos) + ".");
        }
    }
}
