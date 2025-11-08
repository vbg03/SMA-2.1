using UnityEngine;

public class GameBootstrap : MonoBehaviour
{
    [Header("Repositorio (archivo JSON)")]
    [Tooltip("Nombre del archivo JSON en Application.persistentDataPath")]
    public string usersFileName = LocalUserRepository.DefaultFileName;
    [Tooltip("Versión inicial del esquema del JSON si hay que crearlo")]
    public int initialDbVersion = 1;

    [Header("Sesión")]
    [Tooltip("Recordar último username en PlayerPrefs")]
    public bool rememberLastUser = true;

    [Header("Progreso")]
    [Tooltip("IDs de minijuegos requeridos para completar el Nivel 1")]
    public string[] requiredMinigameIds = new[] { "Minijuego1", "Minijuego2" };

    [Header("Ciclo de vida")]
    [Tooltip("Mantener este objeto entre escenas")]
    public bool persistAcrossScenes = true;

    // Instancias internas (también accesibles vía AppServices.*)
    private LocalUserRepository _repo;
    private SessionManager _session;
    private AuthService _auth;
    private ProgressService _progress;

    private void Awake()
    {
        if (persistAcrossScenes)
            DontDestroyOnLoad(gameObject);

        // 1) Repositorio y DB
        _repo = new LocalUserRepository(string.IsNullOrWhiteSpace(usersFileName)
            ? LocalUserRepository.DefaultFileName
            : usersFileName);

        _repo.LoadOrCreate(initialDbVersion);

        // 2) Sesión
        _session = new SessionManager(rememberLastUser);

        // 3) Servicios de dominio
        _auth = new AuthService(_repo, _session, requiredMinigameIds);
        _progress = new ProgressService(_repo, _session, requiredMinigameIds);

        // 4) Publicar en contenedor estático
        AppServices.UsersRepo = _repo;
        AppServices.Session = _session;
        AppServices.Auth = _auth;
        AppServices.Progress = _progress;

        Debug.Log($"[GameBootstrap] Listo. JSON: {_repo.FilePath}");
        Debug.Log(AppServices.Auth);
    }
}
