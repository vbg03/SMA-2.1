using System;
using UnityEngine;

public class SessionManager
{
    /// Evento disparado cuando cambia el usuario activo (incluye null al cerrar sesión).
    public event Action<UserProfile> OnUserChanged;

    /// Usuario actualmente autenticado (puede ser null).
    public UserProfile ActiveUser { get; private set; }

    /// (Opcional) Clave en PlayerPrefs para recordar el último username.
    private const string LastUsernameKey = "SESSION_LAST_USERNAME";
    private readonly bool _rememberLastUser;

    public SessionManager(bool rememberLastUser = false)
    {
        _rememberLastUser = rememberLastUser;
    }

    /// Establece el usuario activo y emite evento de cambio.
    public void SetActiveUser(UserProfile user)
    {
        ActiveUser = user;

        if (_rememberLastUser)
        {
            if (user != null && !string.IsNullOrEmpty(user.username))
                PlayerPrefs.SetString(LastUsernameKey, user.username);
            else
                PlayerPrefs.DeleteKey(LastUsernameKey);
            PlayerPrefs.Save();
        }

        try
        {
            OnUserChanged?.Invoke(ActiveUser);
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"[SessionManager] Excepción en OnUserChanged: {ex.Message}");
        }
    }

    /// Limpia la sesión actual (usuario = null)
    public void Clear()
    {
        SetActiveUser(null);
    }
    /// Devuelve el último username recordado (si rememberLastUser=true), o null si no existe.
    public string GetLastUsername()
    {
        if (!_rememberLastUser) return null;
        return PlayerPrefs.GetString(LastUsernameKey, null);
    }
}
