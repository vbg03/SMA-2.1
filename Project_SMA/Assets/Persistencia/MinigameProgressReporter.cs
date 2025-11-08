using UnityEngine;
using UnityEngine.Events;

public class MinigameProgressReporter : MonoBehaviour
{
    [Header("Identificador del minijuego (debe coincidir con el configurado en ProgressService)")]
    [SerializeField] private string minigameId = "Minijuego1";

    [Header("Eventos opcionales")]
    public UnityEvent onReported;  // Se invoca tras reportar el win

    /// <summary>
    /// Llama esto cuando el minijuego se complete (desde tu Manager o un UnityEvent).
    /// </summary>
    public void ReportWin()
    {
        if (string.IsNullOrWhiteSpace(minigameId))
        {
            Debug.LogWarning("[MinigameProgressReporter] minigameId vacío.");
            return;
        }

        if (AppServices.Progress == null)
        {
            Debug.LogWarning("[MinigameProgressReporter] ProgressService no está inicializado (AppServices.Progress == null).");
            return;
        }

        AppServices.Progress.MarkMinigameCompleted(minigameId, completed: true, autoSave: true);
        onReported?.Invoke();
    }

    /// <summary>
    /// (Opcional) Si quieres revertir/depurar estados.
    /// </summary>
    public void ReportUnset()
    {
        if (string.IsNullOrWhiteSpace(minigameId)) return;
        if (AppServices.Progress == null) return;

        AppServices.Progress.MarkMinigameCompleted(minigameId, completed: false, autoSave: true);
        onReported?.Invoke();
    }
}

/// <summary>
/// Contenedor estático minimalista para exponer servicios a los reporteros.
/// Debe inicializarse en el arranque del juego (ver integración).
/// </summary>
public static class AppServices
{
    public static ProgressService Progress;
    public static SessionManager Session;
    public static LocalUserRepository UsersRepo;
    public static AuthService Auth;
}
