using System;
using System.Collections.Generic;
using ORM;
using SECURITY;

namespace SERVICES
{
    public class GestorIntegridad_SERVICE
    {
        private readonly Integridad_ORM integridadRepo;
        private readonly Cifrador_SECURITY cifrador;

        public GestorIntegridad_SERVICE()
        {
            integridadRepo = new Integridad_ORM();
            cifrador = Cifrador_SECURITY.CifradorSingleton;
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

        public DetalleTablaRecalculo_SERVICE RecalcularTabla(TablasBD tabla)
        {
            var filas = integridadRepo.ObtenerDatosTabla(tabla);

            foreach (var fila in filas)
            {
                string dvh = CalcularDigitoVerificador(fila.Datos);
                integridadRepo.GuardarNuevoDVH(tabla, fila.ClavePK, dvh);
            }

            GuardarIntegridadTabla(tabla);

            var registro = integridadRepo.LeerRegistroIntegridad(tabla);

            return new DetalleTablaRecalculo_SERVICE
            {
                Tabla = tabla.ToString(),
                Registros = filas.Count,
                Dvv = registro.HasValue ? registro.Value.DVV : null
            };
        }

        public ResultadoRecalculoIntegridad_SERVICE RecalcularTodasLasTablas()
        {
            var resultado = new ResultadoRecalculoIntegridad_SERVICE();

            foreach (TablasBD tabla in (TablasBD[])Enum.GetValues(typeof(TablasBD)))
            {
                var detalle = RecalcularTabla(tabla);

                resultado.TablasProcesadas++;
                resultado.RegistrosProcesados += detalle.Registros;
                resultado.Tablas.Add(detalle);
            }

            return resultado;
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
                    //TODO: Traducir.
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
                    //TODO: Traducir.
                    Detalle = $"Se detectaron registros agregados de forma externa en la tabla {tabla}."
                });
            }

            else if (filas.Count < registroGuardado.Value.CR)
            {
                inconsistencias.Add(new InconsistenciaIntegridad_SERVICE
                {
                    Tabla = tabla,
                    Tipo = TipoInconsistencia.RegistrosEliminados,
                    //TODO: Traducir.
                    Detalle = $"Se detectaron registros eliminados de forma externa en la tabla {tabla}."
                });
            }

            var dvhsRecalculados = new List<string>();
            var columnas = integridadRepo.ObtenerNombresColumnas(tabla).ToArray();
            int numeroRegistro = 0;

            foreach (var fila in filas)
            {
                numeroRegistro++;
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
                        NumeroRegistro = numeroRegistro,
                        Columnas = columnas,
                        Datos = fila.Datos,
                        //TODO: Traducir.
                        DvhAlmacenado = string.IsNullOrEmpty(fila.Dvh) ? "(vacio)" : fila.Dvh,
                        DvhRecalculado = dvhCalculado,
                        //TODO: Traducir.
                        Detalle = $"El registro #{numeroRegistro} (clave \"{clave}\") de la tabla {tabla} fue alterado o agregado externamente."
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
                    //TODO: Traducir.
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