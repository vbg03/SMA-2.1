using System;
using System.Collections.Generic;
using UnityEngine;

public class ProgressService
{

    /// Evento disparado cuando el usuario activo completa el Nivel 1 (transición de false -> true).
    public event Action<UserProfile> OnNivel1Completado;

    private readonly LocalUserRepository _repo;
    private readonly SessionManager _session;
    private readonly List<string> _requiredMinigameIds;

    /// <param name="repo">Repositorio para guardar cambios.</param>
    /// <param name="session">Gestor de sesión para acceder al usuario activo.</param>
    /// <param name="requiredMinigameIds">IDs de minijuegos que cuenta como requeridos para completar el Nivel 1.</param>
    public ProgressService(LocalUserRepository repo, SessionManager session, IEnumerable<string> requiredMinigameIds)
    {
        _repo = repo ?? throw new ArgumentNullException(nameof(repo));
        _session = session ?? throw new ArgumentNullException(nameof(session));
        _requiredMinigameIds = requiredMinigameIds != null ? new List<string>(requiredMinigameIds) : new List<string>();

        // Opcional: asegura que cuando cambie de usuario, su progreso tenga todos los IDs
        _session.OnUserChanged += EnsureMinigameKeysForActiveUser;
    }

    /// Marca un minijuego como completado (o no), recalcula Nivel 1 y guarda.
    public bool MarkMinigameCompleted(string minigameId, bool completed = true, bool autoSave = true)
    {
        var user = _session.ActiveUser;
        if (user == null)
        {
            Debug.LogWarning("[ProgressService] No hay usuario activo; no se puede marcar progreso.");
            return false;
        }

        if (string.IsNullOrWhiteSpace(minigameId))
        {
            Debug.LogWarning("[ProgressService] minigameId vacío.");
            return false;
        }

        bool oldNivel = user.progress.nivel1Completado;

        user.progress.SetMinigameCompleted(minigameId, completed);
        user.progress.RecalculateNivel1Completado(_requiredMinigameIds);

        if (autoSave)
            SaveActiveUser();

        // Si hubo transición de false -> true, dispara evento
        if (!oldNivel && user.progress.nivel1Completado)
        {
            try { OnNivel1Completado?.Invoke(user); }
            catch (Exception ex)
            {
                Debug.LogWarning($"[ProgressService] Excepción en OnNivel1Completado: {ex.Message}");
            }
        }

        return true;
    }

    /// Devuelve true si el minijuego está marcado como completado para el usuario activo.
    public bool IsMinigameCompleted(string minigameId)
    {
        var user = _session.ActiveUser;
        if (user == null || string.IsNullOrWhiteSpace(minigameId))
            return false;

        return user.progress.IsMinigameCompleted(minigameId);
    }

    /// Recalcula la bandera de Nivel 1 y guarda (útil tras migraciones o cambios de reglas).
    public bool RecalculateNivel1Completado(bool autoSave = true)
    {
        var user = _session.ActiveUser;
        if (user == null)
        {
            Debug.LogWarning("[ProgressService] No hay usuario activo; no se puede recalcular.");
            return false;
        }

        bool oldNivel = user.progress.nivel1Completado;

        user.progress.RecalculateNivel1Completado(_requiredMinigameIds);

        if (autoSave)
            SaveActiveUser();

        if (!oldNivel && user.progress.nivel1Completado)
        {
            try { OnNivel1Completado?.Invoke(user); }
            catch (Exception ex)
            {
                Debug.LogWarning($"[ProgressService] Excepción en OnNivel1Completado: {ex.Message}");
            }
        }
        return true;
    }

    /// Guarda el usuario activo en el repositorio.
    public bool SaveActiveUser()
    {
        var user = _session.ActiveUser;
        if (user == null)
        {
            Debug.LogWarning("[ProgressService] No hay usuario activo; no se puede guardar.");
            return false;
        }

        return _repo.UpdateUser(user, autoSave: true);
    }

    /// Devuelve la bandera global de Nivel 1 para el usuario activo.
    public bool IsNivel1Completado()
    {
        var user = _session.ActiveUser;
        return user != null && user.progress.nivel1Completado;
    }

    /// Asegura que el usuario activo tenga todas las entradas de minijuego en su lista (si faltan).
    /// Se llama automáticamente cuando cambia el usuario activo.
    private void EnsureMinigameKeysForActiveUser(UserProfile user)
    {
        if (user == null) return;

        if (_requiredMinigameIds.Count > 0)
        {
            user.progress.EnsureMinigames(_requiredMinigameIds.ToArray());
            // No forzamos recalcular aquí para no disparar eventos inesperados; solo guardamos si algo cambió.
            SaveActiveUser();
        }
    }
}