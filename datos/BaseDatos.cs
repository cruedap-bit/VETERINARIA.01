namespace ClinicaVeterinaria.datos
{
    public static class BaseDatos
    {
        /// <summary>Ruta del archivo .db (junto al ejecutable).</summary>
        public static string strRuta
        {
            get { return Path.Combine(AppContext.BaseDirectory, "clinica_veterinaria.db"); }
        }

        public static string strCadena
        {
            get { return "Data Source=" + strRuta; }
        }

        private const string ESQUEMA = @"
CREATE TABLE IF NOT EXISTS Propietario (
    codigo    TEXT PRIMARY KEY,
    nombre    TEXT NOT NULL,
    correo    TEXT,
    telefono  TEXT,
    direccion TEXT,
    dpi       TEXT
);

CREATE TABLE IF NOT EXISTS Veterinario (
    codigo       TEXT PRIMARY KEY,
    nombre       TEXT NOT NULL,
    correo       TEXT,
    telefono     TEXT,
    especialidad TEXT NOT NULL DEFAULT 'General',
    disponible   INTEGER NOT NULL DEFAULT 1,
    emergencia   INTEGER NOT NULL DEFAULT 0
);

CREATE TABLE IF NOT EXISTS Mascota (
    codigo      TEXT PRIMARY KEY,
    nombre      TEXT NOT NULL,
    especie     TEXT NOT NULL,
    raza        TEXT,
    sexo        TEXT,
    edad        INTEGER NOT NULL DEFAULT 0,
    peso        REAL NOT NULL DEFAULT 0,
    propietario TEXT NOT NULL REFERENCES Propietario(codigo)
);

CREATE TABLE IF NOT EXISTS Consulta (
    numero      INTEGER PRIMARY KEY AUTOINCREMENT,
    mascota     TEXT NOT NULL REFERENCES Mascota(codigo),
    veterinario TEXT REFERENCES Veterinario(codigo),
    motivo      TEXT NOT NULL,
    sintomas    TEXT,
    urgencia    TEXT NOT NULL,
    estado      TEXT NOT NULL,
    emergencia  INTEGER NOT NULL DEFAULT 0,
    diagnostico TEXT,
    tratamiento TEXT,
    fecha       TEXT NOT NULL
);

CREATE TABLE IF NOT EXISTS Examen (
    id        INTEGER PRIMARY KEY AUTOINCREMENT,
    consulta  INTEGER NOT NULL REFERENCES Consulta(numero) ON DELETE CASCADE,
    tipo      TEXT NOT NULL,
    resultado TEXT,
    severidad TEXT NOT NULL,
    fecha     TEXT NOT NULL
);

CREATE TABLE IF NOT EXISTS VacunaAplicada (
    id      INTEGER PRIMARY KEY AUTOINCREMENT,
    mascota TEXT NOT NULL REFERENCES Mascota(codigo) ON DELETE CASCADE,
    vacuna  TEXT NOT NULL,
    fecha   TEXT NOT NULL
);

CREATE TABLE IF NOT EXISTS Expediente (
    id      INTEGER PRIMARY KEY AUTOINCREMENT,
    mascota TEXT NOT NULL REFERENCES Mascota(codigo) ON DELETE CASCADE,
    fecha   TEXT NOT NULL,
    evento  TEXT NOT NULL
);

CREATE TABLE IF NOT EXISTS Bitacora (
    id       INTEGER PRIMARY KEY AUTOINCREMENT,
    consulta INTEGER NOT NULL REFERENCES Consulta(numero) ON DELETE CASCADE,
    fecha    TEXT NOT NULL,
    evento   TEXT NOT NULL
);
";

        /// <summary>Crea el archivo .db y las tablas la primera vez que se ejecuta.</summary>
        public static void inicializar()
        {
            using (Sesion objSesion = new Sesion())
            {
                objSesion.Ejecutar(ESQUEMA);
                objSesion.Confirmar();
            }
        }

        /// <summary>Cantidad de filas de una tabla (el nombre es siempre una constante del codigo).</summary>
        public static int contar(string strTabla)
        {
            using (Sesion objSesion = new Sesion())
            {
                var lstFilas = objSesion.Consultar("SELECT COUNT(*) AS n FROM " + strTabla);
                return Sesion.leerInt(lstFilas[0]["n"]);
            }
        }
    }
}
