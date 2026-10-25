# Sistema de Control Clinico Veterinario (consola + SQLite)

Aplicacion de consola **.NET 6** que guarda todo en una base **SQLite** (`clinica_veterinaria.db`), por lo que los datos ya no se pierden al cerrar el programa.

## Como ejecutar

```bash
dotnet run
```

La primera vez se crea `clinica_veterinaria.db` junto al ejecutable (`bin/Debug/net6.0/`) con las tablas y datos de ejemplo. Para empezar de cero, cierra el programa y borra ese archivo. El paquete `Microsoft.Data.Sqlite` se restaura solo.

## Estructura

```
Program.cs              Menu de consola (14 opciones)
datos/                  Capa de datos (SQLite)
  BaseDatos.cs          Ruta, esquema (CREATE TABLE) e inicializacion
  Sesion.cs             Conexion + transaccion (Ejecutar / Insertar / Consultar / Confirmar)
  Repo*.cs              Acceso a tablas
logica/                 Capa de negocio
  Usuario (abstracta) · Propietario · Veterinario · Mascota · Consulta · Examen
  FlujoConsulta         Registrada -> Asignada -> Diagnosticada -> Cerrada
  GestorClinico         Reglas del flujo y asignacion automatica
  Constantes · Validador
```

## Tablas

`Propietario`, `Veterinario`, `Mascota`, `Consulta`, `Examen`, `VacunaAplicada`, `Expediente`, `Bitacora`. Las llaves foraneas estan activas.

## Reglas

- Al crear una consulta se asigna sola al veterinario disponible con menos carga que atienda la especie (maximo 3 consultas activas); si no hay, pasa a **emergencia**.
- Un examen con severidad Alta o Critica marca la consulta como emergencia y sube su urgencia a Critica.
- Solo se diagnostica una consulta *Asignada* y solo se cierra una *Diagnosticada*.
- La carga del veterinario se calcula con las consultas Asignadas/Diagnosticadas, asi que nunca queda desfasada.
- Un veterinario `General` atiende cualquier especie.
- Cada operacion que escribe en varias tablas usa una transaccion: o se guarda todo o nada.
