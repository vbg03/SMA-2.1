using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class UserProfile
{
    // Identidad y credenciales (password en texto plano según tu petición actual)
    public string id;
    public string username;
    public string password;

    // Tiempos como ISO-8601 string (JsonUtility no maneja bien DateTime)
    public string createdAtIso;
    public string lastLoginAtIso;

    // Progreso persistido
    public ProgressState progress = new ProgressState();

    // ---- Fábrica / Helpers ----
    public static UserProfile CreateNew(string username, string password)
    {
        var nowIso = DateTime.UtcNow.ToString("o"); // ISO-8601
        return new UserProfile
        {
            id = Guid.NewGuid().ToString(),
            username = username,
            password = password,
            createdAtIso = nowIso,
            lastLoginAtIso = nowIso,
            progress = new ProgressState()
        };
    }

    public void TouchLastLoginUtcNow()
    {
        lastLoginAtIso = DateTime.UtcNow.ToString("o");
    }
}

[Serializable]
public class ProgressState
{
    // Lista serializable para compatibilidad con JsonUtility (en lugar de Dictionary)
    public List<MinigameStatus> minigames = new List<MinigameStatus>();

    // Bandera de nivel
    public bool nivel1Completado = false;

    // ---- Utilidades de acceso ----

    /// Devuelve true/false si existe el minijuego; si no existe, retorna false.
    public bool IsMinigameCompleted(string minigameId)
    {
        var idx = IndexOf(minigameId);
        return idx >= 0 && minigames[idx].completed;
    }

    /// Marca/unmarca un minijuego; crea el registro si no existe.
    public void SetMinigameCompleted(string minigameId, bool completed = true)
    {
        var idx = IndexOf(minigameId);
        if (idx >= 0)
        {
            var item = minigames[idx];
            item.completed = completed;
            minigames[idx] = item;
        }
        else
        {
            minigames.Add(new MinigameStatus { id = minigameId, completed = completed });
        }
    }

    /// Garantiza que todos los IDs estén presentes en la lista (útil al iniciar sesión o migrar).
    public void EnsureMinigames(params string[] ids)
    {
        foreach (var id in ids)
        {
            if (IndexOf(id) < 0)
                minigames.Add(new MinigameStatus { id = id, completed = false });
        }
    }

    /// Recalcula y actualiza nivel1Completado según un conjunto requerido.
    public void RecalculateNivel1Completado(IReadOnlyList<string> requiredMinigameIds)
    {
        // Si no hay requeridos, no se considera completado
        if (requiredMinigameIds == null || requiredMinigameIds.Count == 0)
        {
            nivel1Completado = false;
            return;
        }

        // Asegura los registros faltantes
        EnsureMinigames(requiredMinigameIds as string[] ?? new List<string>(requiredMinigameIds).ToArray());

        // Evalúa que todos estén en true
        foreach (var id in requiredMinigameIds)
        {
            if (!IsMinigameCompleted(id))
            {
                nivel1Completado = false;
                return;
            }
        }
        nivel1Completado = true;
    }

    // ---- Privados ----
    private int IndexOf(string minigameId)
    {
        for (int i = 0; i < minigames.Count; i++)
        {
            if (string.Equals(minigames[i].id, minigameId, StringComparison.OrdinalIgnoreCase))
                return i;
        }
        return -1;
    }
}

[Serializable]
public struct MinigameStatus
{
    public string id;       // p. ej., "Minijuego1", "Minijuego2"
    public bool completed;  // true si el jugador lo completó
}
