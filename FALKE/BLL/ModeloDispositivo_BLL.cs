using System;
using System.Collections.Generic;
using BE;
using ORM;
using SERVICES;
using TE;
using TLL;

namespace BLL
{
    // Catálogo de modelos de dispositivo. Un modelo solo lleva su nombre; no se borra: se da de baja (baja lógica)
    // y deja de ofrecerse para dispositivos nuevos, pero los que ya lo tienen lo conservan.
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

        // Quien ve el inventario ve los modelos (para filtrar). Dar de alta un dispositivo elige solo entre los activos.
        public List<ModeloDispositivo_BE> Listar(ActorUsuario_TLL actor)
        {
            Exigir(actor, Patentes_TLL.VER_DISPOSITIVOS);

            return modeloRepo.ObtenerTodos();
        }

        public void Crear(ActorUsuario_TLL actor, string nombre)
        {
            Exigir(actor, Patentes_TLL.GESTIONAR_MODELOS_DISPOSITIVO);

            nombre = Normalizar(nombre);
            ValidarNombre(nombre);

            if (modeloRepo.ExisteNombre(nombre, 0)) throw new InvalidOperationException("Ya existe un modelo con ese nombre.");

            var modelo = new ModeloDispositivo_BE { Nombre = nombre, Activo = true };

            Transaccion_ORM.Ejecutar(() =>
            {
                modeloRepo.Alta(modelo);
                gestorIntegridad.ActualizarDVHRegistro(TablasBD.ModeloDispositivo, new[] { modelo.IdModelo.ToString() });

                Auditar(actor, "Alta del modelo de dispositivo \"" + nombre + "\".", CriticidadBitacora.Media);
            });
        }

        public void Renombrar(ActorUsuario_TLL actor, int idModelo, string nombre)
        {
            Exigir(actor, Patentes_TLL.GESTIONAR_MODELOS_DISPOSITIVO);

            ModeloDispositivo_BE modelo = ObtenerExistente(idModelo);

            nombre = Normalizar(nombre);
            ValidarNombre(nombre);

            if (string.Equals(modelo.Nombre, nombre, StringComparison.Ordinal)) throw new InvalidOperationException("No hay cambios para guardar.");
            if (modeloRepo.ExisteNombre(nombre, idModelo)) throw new InvalidOperationException("Ya existe un modelo con ese nombre.");

            string anterior = modelo.Nombre;
            modelo.Nombre = nombre;

            Transaccion_ORM.Ejecutar(() =>
            {
                modeloRepo.Modificar(modelo);
                gestorIntegridad.ActualizarDVHRegistro(TablasBD.ModeloDispositivo, new[] { idModelo.ToString() });

                Auditar(actor, "El modelo de dispositivo \"" + anterior + "\" pasó a llamarse \"" + nombre + "\".", CriticidadBitacora.Media);
            });
        }

        // Baja lógica: deja de ofrecerse para dispositivos nuevos. Los dispositivos que ya lo tienen no cambian.
        public void DarDeBaja(ActorUsuario_TLL actor, int idModelo)
        {
            CambiarActivo(actor, idModelo, false, "Baja del modelo de dispositivo \"{0}\".");
        }

        public void Reactivar(ActorUsuario_TLL actor, int idModelo)
        {
            CambiarActivo(actor, idModelo, true, "Se reactivó el modelo de dispositivo \"{0}\".");
        }

        private void CambiarActivo(ActorUsuario_TLL actor, int idModelo, bool activo, string descripcion)
        {
            Exigir(actor, Patentes_TLL.GESTIONAR_MODELOS_DISPOSITIVO);

            ModeloDispositivo_BE modelo = ObtenerExistente(idModelo);

            if (modelo.Activo == activo) throw new InvalidOperationException(activo ? "El modelo ya está activo." : "El modelo ya está dado de baja.");

            modelo.Activo = activo;

            Transaccion_ORM.Ejecutar(() =>
            {
                modeloRepo.Modificar(modelo);
                gestorIntegridad.ActualizarDVHRegistro(TablasBD.ModeloDispositivo, new[] { idModelo.ToString() });

                Auditar(actor, string.Format(descripcion, modelo.Nombre), CriticidadBitacora.Media);
            });
        }

        private ModeloDispositivo_BE ObtenerExistente(int idModelo)
        {
            ModeloDispositivo_BE modelo = modeloRepo.ObtenerPorPK(idModelo);

            if (modelo == null) throw new InvalidOperationException("El modelo no existe.");

            return modelo;
        }

        // Los espacios repetidos se juntan en uno: "Tobii  Eye" y "Tobii Eye" son el mismo modelo.
        private static string Normalizar(string nombre)
        {
            return string.Join(" ", (nombre ?? string.Empty).Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
        }

        private static void ValidarNombre(string nombre)
        {
            if (nombre.Length == 0) throw new InvalidOperationException("El nombre del modelo es obligatorio.");
            if (nombre.Length > LARGO_NOMBRE) throw new InvalidOperationException("El nombre del modelo no puede superar los " + LARGO_NOMBRE + " caracteres.");
        }

        private void Exigir(ActorUsuario_TLL actor, string patente)
        {
            if (actor != null && actor.Puede(patente)) return;

            bitacora.Registrar(actor != null ? actor.IdUsuario : 0, "Seguridad", "Acción rechazada por falta de permiso (" + patente + ")", CriticidadBitacora.Media);
            throw new UnauthorizedAccessException("No tenés permiso para realizar esta acción.");
        }

        // Estos eventos son de auditoría: si no se pueden guardar, el cambio completo se revierte.
        private void Auditar(ActorUsuario_TLL actor, string descripcion, CriticidadBitacora criticidad)
        {
            bitacora.Guardar(new Bitacora_TE(actor.IdUsuario, "Dispositivos", descripcion, criticidad, DateTime.Now));
        }
    }
}
