using System;
using System.Collections.Generic;
using ORM;
using SECURITY;

namespace SERVICIOS
{
    public class GestorIntegridad_SERVICE
    {
        private readonly Integridad_ORM integridadRepo;
        private readonly Cifrador cifrador;

        public GestorIntegridad_SERVICE()
        {
            integridadRepo = new Integridad_ORM();
            cifrador = Cifrador.CypherInstance;
        }

        public string CalcularDigitoVerificador(string[] datos)
        {
            string acumulado = string.Empty;

            foreach (var item in datos)
            {
                acumulado += item;
                acumulado = cifrador.Encoder(acumulado);
            }

            return string.IsNullOrEmpty(acumulado) ? "0" : acumulado;
        }

        public void GuardarIntegridadTabla(TablasBD tabla)
        {
            var dvhs = integridadRepo.ObtenerDVHs(tabla);
            string dvv = CalcularDigitoVerificador(dvhs.ToArray());
            integridadRepo.GuardarRegistroIntegridad(tabla, dvv, dvhs.Count);
        }

        public void RecalcularTodasLasTablas()
        {
            foreach (TablasBD tabla in (TablasBD[])Enum.GetValues(typeof(TablasBD)))
            {
                var filas = integridadRepo.ObtenerDatosTabla(tabla);

                foreach (var fila in filas)
                {
                    string dvh = CalcularDigitoVerificador(fila.Datos);
                    integridadRepo.GuardarNuevoDVH(tabla, fila.ClavePK, dvh);
                }

                GuardarIntegridadTabla(tabla);
            }
        }

        public void ActualizarDVHRegistro(TablasBD tabla, string[] clavesPK)
        {
            var datos = integridadRepo.ObtenerDatosRegistro(tabla, clavesPK);

            if (datos == null) return;

            string dvh = CalcularDigitoVerificador(datos);
            integridadRepo.GuardarNuevoDVH(tabla, clavesPK, dvh);

            GuardarIntegridadTabla(tabla);
        }

        public List<InconsistenciaIntegridad_SERVICE> VerificarIntegridadTabla(TablasBD tabla)
        {
            var inconsistencias = new List<InconsistenciaIntegridad_SERVICE>();

            var filas = integridadRepo.ObtenerDatosTabla(tabla);
            var registroGuardado = integridadRepo.LeerRegistroIntegridad(tabla);

            if (registroGuardado == null)
            {
                inconsistencias.Add(new InconsistenciaIntegridad_SERVICE
                {
                    Tabla = tabla,
                    Tipo = TipoInconsistencia.ErrorLectura,
                    Detalle = $"No se encontró un registro de integridad previo para la tabla {tabla}."
                });
                return inconsistencias;
            }

            if (filas.Count > registroGuardado.Value.CR)
            {
                inconsistencias.Add(new InconsistenciaIntegridad_SERVICE
                {
                    Tabla = tabla,
                    Tipo = TipoInconsistencia.RegistrosAgregados,
                    Detalle = $"Se detectaron registros agregados de forma externa en la tabla {tabla}."
                });
            }

            else if (filas.Count < registroGuardado.Value.CR)
            {
                inconsistencias.Add(new InconsistenciaIntegridad_SERVICE
                {
                    Tabla = tabla,
                    Tipo = TipoInconsistencia.RegistrosEliminados,
                    Detalle = $"Se detectaron registros eliminados de forma externa en la tabla {tabla}."
                });
            }

            var dvhsRecalculados = new List<string>();

            foreach (var fila in filas)
            {
                string dvhCalculado = CalcularDigitoVerificador(fila.Datos);
                dvhsRecalculados.Add(dvhCalculado);

                if (dvhCalculado != fila.Dvh)
                {
                    string clave = string.Join("|", fila.ClavePK);
                    inconsistencias.Add(new InconsistenciaIntegridad_SERVICE
                    {
                        Tabla = tabla,
                        Tipo = TipoInconsistencia.RegistroAlterado,
                        ClaveRegistro = clave,
                        Detalle = $"El registro con clave \"{clave}\" de la tabla {tabla} fue alterado."
                    });
                }
            }

            string dvvCalculado = CalcularDigitoVerificador(dvhsRecalculados.ToArray());

            if (dvvCalculado != registroGuardado.Value.DVV)
            {
                inconsistencias.Insert(0, new InconsistenciaIntegridad_SERVICE
                {
                    Tabla = tabla,
                    Tipo = TipoInconsistencia.FirmaTablaInvalida,
                    Detalle = $"La tabla {tabla} posee datos corruptos (firma global inválida)."
                });
            }

            return inconsistencias;
        }

        public List<InconsistenciaIntegridad_SERVICE> VerificarIntegridadTodasLasTablas()
        {
            var todas = new List<InconsistenciaIntegridad_SERVICE>();

            foreach (TablasBD tabla in (TablasBD[])Enum.GetValues(typeof(TablasBD)))
            {
                todas.AddRange(VerificarIntegridadTabla(tabla));
            }

            return todas;
        }
    }
}