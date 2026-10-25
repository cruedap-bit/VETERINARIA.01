using System.Globalization;
using Microsoft.Data.Sqlite;

namespace ClinicaVeterinaria.datos
{
    /// <summary>
    /// Una conexion SQLite con una transaccion abierta.
    /// Si no se llama a Confirmar() antes de cerrarla, todo se revierte.
    /// Los parametros se pasan alternados: nombre, valor, nombre, valor...
    /// </summary>
    public class Sesion : IDisposable
    {
        private readonly SqliteConnection objConexion;
        private readonly SqliteTransaction objTransaccion;

        public Sesion()
        {
            objConexion = new SqliteConnection(BaseDatos.strCadena);
            objConexion.Open();

            // Debe ejecutarse fuera de una transaccion.
            using (SqliteCommand objCmd = objConexion.CreateCommand())
            {
                objCmd.CommandText = "PRAGMA foreign_keys = ON;";
                objCmd.ExecuteNonQuery();
            }

            objTransaccion = objConexion.BeginTransaction();
        }

        private SqliteCommand crearComando(string strSql, object[] pars)
        {
            if (pars.Length % 2 != 0)
            {
                throw new ArgumentException("Los parametros deben venir en pares nombre/valor.");
            }

            SqliteCommand objCmd = objConexion.CreateCommand();
            objCmd.Transaction = objTransaccion;
            objCmd.CommandText = strSql;

            for (int i = 0; i < pars.Length; i += 2)
            {
                objCmd.Parameters.AddWithValue((string)pars[i], pars[i + 1] ?? DBNull.Value);
            }

            return objCmd;
        }

        public int Ejecutar(string strSql, params object[] pars)
        {
            using (SqliteCommand objCmd = crearComando(strSql, pars))
            {
                return objCmd.ExecuteNonQuery();
            }
        }

        /// <summary>Ejecuta un INSERT y devuelve el id autogenerado.</summary>
        public long Insertar(string strSql, params object[] pars)
        {
            Ejecutar(strSql, pars);
            using (SqliteCommand objCmd = crearComando("SELECT last_insert_rowid();", new object[0]))
            {
                return Convert.ToInt64(objCmd.ExecuteScalar());
            }
        }

        public List<Dictionary<string, object>> Consultar(string strSql, params object[] pars)
        {
            List<Dictionary<string, object>> lstFilas = new List<Dictionary<string, object>>();

            using (SqliteCommand objCmd = crearComando(strSql, pars))
            using (SqliteDataReader objLector = objCmd.ExecuteReader())
            {
                while (objLector.Read())
                {
                    Dictionary<string, object> dicFila = new Dictionary<string, object>();
                    for (int i = 0; i < objLector.FieldCount; i++)
                    {
                        dicFila[objLector.GetName(i)] = objLector.IsDBNull(i) ? null : objLector.GetValue(i);
                    }
                    lstFilas.Add(dicFila);
                }
            }

            return lstFilas;
        }

        public void Confirmar()
        {
            objTransaccion.Commit();
        }

        public void Dispose()
        {
            objTransaccion.Dispose();
            objConexion.Dispose();
        }

        // ---- Conversores para leer los valores de una fila ----
        public static string leerTexto(object objValor)
        {
            return objValor == null ? "" : Convert.ToString(objValor, CultureInfo.InvariantCulture);
        }

        public static int leerInt(object objValor)
        {
            return objValor == null ? 0 : Convert.ToInt32(objValor, CultureInfo.InvariantCulture);
        }

        public static double leerDouble(object objValor)
        {
            return objValor == null ? 0 : Convert.ToDouble(objValor, CultureInfo.InvariantCulture);
        }

        public static bool leerBool(object objValor)
        {
            return leerInt(objValor) == 1;
        }
    }
}
