using System;
using System.Collections.Generic;
using BE;
using ORM;
using SERVICES;
using TE;
using TLL;

namespace BLL
{
    public class AnalisisMultiple_BLL
    {
        private readonly AnalisisMultipleRepository analisisRepo;
        private readonly SesionGrabada_BLL sesionBll;
        private readonly Integridad_ORM integridadRepo;
        private readonly GestorIntegridad_SERVICE gestorIntegridad;
        private readonly BitacoraGestor_TLL bitacora;

        public AnalisisMultiple_BLL()
        {
            analisisRepo = new AnalisisMultipleRepository();
            sesionBll = new SesionGrabada_BLL();
            integridadRepo = new Integridad_ORM();
            gestorIntegridad = new GestorIntegridad_SERVICE();
            bitacora = new BitacoraGestor_TLL();
        }

        public AnalisisMultiple_BE Registrar(ActorUsuario_TE actor, List<int> idSesiones)
        {
            actor.Exigir(Patentes_TLL.OPERAR_ANALISIS);

            if (actor.IdEmpresa <= 0) throw new ExcepcionNoAutorizada_TLL("comun_sesion_sin_empresa");
            if (idSesiones == null || idSesiones.Count == 0) throw new ExcepcionTraducible_TLL("sesion_no_existe");

            var ids = new List<int>();

            foreach (int id in idSesiones)
            {
                if (!ids.Contains(id)) ids.Add(id);
            }

            var sesiones = new List<SesionGrabada_BE>();

            foreach (int id in ids) sesiones.Add(sesionBll.ObtenerPropia(actor, id));

            foreach (SesionGrabada_BE s in sesiones)
            {
                if (s.IdCategoria != sesiones[0].IdCategoria) throw new ExcepcionTraducible_TLL("analisismultiple_todas_las_sesiones_tienen_que_ser");
            }

            var analisis = new AnalisisMultiple_BE
            {
                IdEmpresa = actor.IdEmpresa,
                IdUsuario = actor.IdUsuario,
                Fecha = DateTime.Now,
                IdSesiones = ids
            };

            Transaccion_ORM.Ejecutar(() =>
            {
                analisisRepo.Alta(analisis);

                GuardarDvh(TablasBD.AnalisisMultiple, new[] { analisis.IdAnalisis.ToString() });

                foreach (int id in ids) GuardarDvh(TablasBD.AnalisisMultiple_Sesion, new[] { analisis.IdAnalisis.ToString(), id.ToString() });

                gestorIntegridad.GuardarIntegridadTabla(TablasBD.AnalisisMultiple);
                gestorIntegridad.GuardarIntegridadTabla(TablasBD.AnalisisMultiple_Sesion);

                bitacora.Auditar(actor, "Grabaciones", actor.IdEmpresa, EventosBitacora_TLL.ANALISIS_MULTIPLE_GENERADO, CriticidadBitacora.Baja,
                    DetalleBitacora_TLL.Armar(sesiones[0].NombreCategoria, " (", ids.Count, ")"));
            });

            return analisis;
        }

        private void GuardarDvh(TablasBD tabla, string[] clave)
        {
            string[] datos = integridadRepo.ObtenerDatosRegistro(tabla, clave);

            if (datos == null) return;

            integridadRepo.GuardarNuevoDVH(tabla, clave, gestorIntegridad.CalcularDigitoVerificador(datos));
        }
    }
}
