using System;
using System.Collections.Generic;
using BE;
using ORM;
using SERVICES;
using TE;
using TLL;

namespace BLL
{
    public class ModeloDispositivo_BLL
    {
        private const int LARGO_NOMBRE = 50;

        private readonly ModeloDispositivoRepository modeloRepo;
        private readonly GestorIntegridad_SERVICE gestorIntegridad;
        private readonly BitacoraGestor_TLL bitacora;

        public ModeloDispositivo_BLL()
        {
            modeloRepo = new ModeloDispositivoRepository();
            gestorIntegridad = new GestorIntegridad_SERVICE();
            bitacora = new BitacoraGestor_TLL();
        }

        public List<ModeloDispositivo_BE> Listar(ActorUsuario_TE actor)
        {
            actor.Exigir(Patentes_TLL.VER_DISPOSITIVOS);

            return modeloRepo.ObtenerTodos();
        }

        public void Crear(ActorUsuario_TE actor, string nombre)
        {
            actor.Exigir(Patentes_TLL.GESTIONAR_MODELOS_DISPOSITIVO);

            nombre = Normalizar(nombre);
            ValidarNombre(nombre);

            if (modeloRepo.ExisteNombre(nombre, 0)) throw new ExcepcionTraducible_TLL("modelo_duplicado");

            var modelo = new ModeloDispositivo_BE { Nombre = nombre, Activo = true };

            Transaccion_ORM.Ejecutar(() =>
            {
                modeloRepo.Alta(modelo);
                gestorIntegridad.ActualizarDVHRegistro(TablasBD.ModeloDispositivo, new[] { modelo.IdModelo.ToString() });

                bitacora.Auditar(actor, "Dispositivos", null, EventosBitacora_TLL.MODELO_ALTA, CriticidadBitacora.Media, DetalleBitacora_TLL.Armar(nombre));
            });
        }

        public void Renombrar(ActorUsuario_TE actor, int idModelo, string nombre)
        {
            actor.Exigir(Patentes_TLL.GESTIONAR_MODELOS_DISPOSITIVO);

            ModeloDispositivo_BE modelo = ObtenerExistente(idModelo);

            nombre = Normalizar(nombre);
            ValidarNombre(nombre);

            if (string.Equals(modelo.Nombre, nombre, StringComparison.Ordinal)) throw new ExcepcionTraducible_TLL("comun_sin_cambios");
            if (modeloRepo.ExisteNombre(nombre, idModelo)) throw new ExcepcionTraducible_TLL("modelo_duplicado");

            string anterior = modelo.Nombre;
            modelo.Nombre = nombre;

            Transaccion_ORM.Ejecutar(() =>
            {
                modeloRepo.Modificar(modelo);
                gestorIntegridad.ActualizarDVHRegistro(TablasBD.ModeloDispositivo, new[] { idModelo.ToString() });

                bitacora.Auditar(actor, "Dispositivos", null, EventosBitacora_TLL.MODELO_MODIFICACION, CriticidadBitacora.Media, DetalleBitacora_TLL.Armar(anterior, " → ", nombre));
            });
        }

        public void DarDeBaja(ActorUsuario_TE actor, int idModelo)
        {
            CambiarActivo(actor, idModelo, false, EventosBitacora_TLL.MODELO_BAJA);
        }

        public void Reactivar(ActorUsuario_TE actor, int idModelo)
        {
            CambiarActivo(actor, idModelo, true, EventosBitacora_TLL.MODELO_REACTIVADO);
        }

        private void CambiarActivo(ActorUsuario_TE actor, int idModelo, bool activo, string evento)
        {
            actor.Exigir(Patentes_TLL.GESTIONAR_MODELOS_DISPOSITIVO);

            ModeloDispositivo_BE modelo = ObtenerExistente(idModelo);

            if (modelo.Activo == activo) throw new ExcepcionTraducible_TLL(activo ? "modelo_ya_activo" : "modelo_ya_de_baja");

            modelo.Activo = activo;

            Transaccion_ORM.Ejecutar(() =>
            {
                modeloRepo.Modificar(modelo);
                gestorIntegridad.ActualizarDVHRegistro(TablasBD.ModeloDispositivo, new[] { idModelo.ToString() });

                bitacora.Auditar(actor, "Dispositivos", null, evento, CriticidadBitacora.Media, DetalleBitacora_TLL.Armar(modelo.Nombre));
            });
        }

        private ModeloDispositivo_BE ObtenerExistente(int idModelo)
        {
            ModeloDispositivo_BE modelo = modeloRepo.ObtenerPorPK(idModelo);

            if (modelo == null) throw new ExcepcionTraducible_TLL("modelo_no_existe");

            return modelo;
        }

        private static string Normalizar(string nombre)
        {
            return string.Join(" ", (nombre ?? string.Empty).Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
        }

        private static void ValidarNombre(string nombre)
        {
            if (nombre.Length == 0) throw new ExcepcionTraducible_TLL("modelo_nombre_obligatorio");
            if (nombre.Length > LARGO_NOMBRE) throw new ExcepcionTraducible_TLL("modelo_nombre_largo", LARGO_NOMBRE);
        }
    }
}
