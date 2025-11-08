using System;
using UnityEngine;

public class AuthService
{
    public const int MinPasswordLength = 4;

    private readonly LocalUserRepository _repo;
    private readonly SessionManager _session; // puede ser null
    private readonly string[] _initialMinigameIds; // opcional: para inicializar progreso

    /// <param name="repo">Repositorio de usuarios (I/O a JSON).</param>
    /// <param name="session">Gestor de sesión actual (puede ser null si aún no lo implementas).</param>
    /// <param name="initialMinigameIds">IDs de minijuegos del nivel 1 para inicializar el progreso (puede ser null).</param>
    public AuthService(LocalUserRepository repo, SessionManager session = null, string[] initialMinigameIds = null)
    {
        _repo = repo ?? throw new ArgumentNullException(nameof(repo));
        _session = session; // opcional
        _initialMinigameIds = initialMinigameIds;
    }

    //API Pública
    /// Registra un nuevo usuario; valida unicidad de username y longitud mínima de password.
    /// Inicializa progreso con minijuegos requeridos (si fueron provistos).
    public AuthResult Register(string username, string password)
    {
        // Validaciones básicas
        if (string.IsNullOrWhiteSpace(username))
            return AuthResult.Fail(AuthError.UsernameEmpty, "El nombre de usuario es obligatorio.");

        if (string.IsNullOrEmpty(password))
            return AuthResult.Fail(AuthError.PasswordEmpty, "La contraseña es obligatoria.");

        if (password.Length < MinPasswordLength)
            return AuthResult.Fail(AuthError.PasswordTooShort, $"La contraseña debe tener al menos {MinPasswordLength} caracteres.");

        try
        {
            // Ya existe?
            var existing = _repo.FindByUsername(username, ignoreCase: true);
            if (existing != null)
                return AuthResult.Fail(AuthError.UsernameExists, "El nombre de usuario ya está registrado.");

            // Crear perfil
            var profile = UserProfile.CreateNew(username.Trim(), password);

            // Inicializar progreso si hay lista de minijuegos
            if (_initialMinigameIds != null && _initialMinigameIds.Length > 0)
            {
                profile.progress.EnsureMinigames(_initialMinigameIds);
                profile.progress.RecalculateNivel1Completado(_initialMinigameIds);
            }

            // Persistir
            var added = _repo.AddUser(profile, ignoreCase: true, autoSave: true);
            if (!added)
                return AuthResult.Fail(AuthError.RepositoryError, "No se pudo guardar el usuario (repositorio).");

            // Marcar sesión activa
            _session?.SetActiveUser(profile);

            return AuthResult.Ok(profile, "Usuario registrado correctamente.");
        }
        catch (Exception ex)
        {
            Debug.LogError($"[AuthService] Error en Register: {ex.Message}");
            return AuthResult.Fail(AuthError.RepositoryError, "Ocurrió un error al registrar el usuario.");
        }
    }

    /// Inicia sesión comparando password en texto plano y actualiza lastLoginAt.
    public AuthResult Login(string username, string password)
    {
        if (string.IsNullOrWhiteSpace(username))
            return AuthResult.Fail(AuthError.UsernameEmpty, "El nombre de usuario es obligatorio.");

        if (string.IsNullOrEmpty(password))
            return AuthResult.Fail(AuthError.PasswordEmpty, "La contraseña es obligatoria.");

        try
        {
            var user = _repo.FindByUsername(username, ignoreCase: true);
            if (user == null)
                return AuthResult.Fail(AuthError.UsernameNotFound, "El usuario no existe.");

            if (!string.Equals(user.password, password, StringComparison.Ordinal))
                return AuthResult.Fail(AuthError.InvalidCredentials, "Credenciales inválidas.");

            user.TouchLastLoginUtcNow();
            _repo.UpdateUser(user, autoSave: true);

            _session?.SetActiveUser(user);

            return AuthResult.Ok(user, "Inicio de sesión exitoso.");
        }
        catch (Exception ex)
        {
            Debug.LogError($"[AuthService] Error en Login: {ex.Message}");
            return AuthResult.Fail(AuthError.RepositoryError, "Ocurrió un error al iniciar sesión.");
        }
    }

    /// Cierra la sesión si hay SessionManager.
    public void Logout()
    {
        _session?.Clear();
    }
}

//Resultados y errores

public enum AuthError
{
    None = 0,
    UsernameEmpty,
    PasswordEmpty,
    PasswordTooShort,
    UsernameExists,
    UsernameNotFound,
    InvalidCredentials,
    RepositoryError
}

[Serializable]
public class AuthResult
{
    public bool success;
    public AuthError error;
    public string message;

    // Usuario autenticado/registrado (cuando success = true)
    public UserProfile user;

    public static AuthResult Ok(UserProfile user, string message = null)
    {
        return new AuthResult
        {
            success = true,
            error = AuthError.None,
            message = message,
            user = user
        };
    }

    public static AuthResult Fail(AuthError error, string message)
    {
        return new AuthResult
        {
            success = false,
            error = error,
            message = message,
            user = null
        };
    }
}
