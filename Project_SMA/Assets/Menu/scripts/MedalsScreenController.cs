using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MedalsScreenController : MonoBehaviour
{
    [System.Serializable]
    public class MedalConfig
    {
        public string minigameId;     // Debe coincidir con MinigameProgressReporter y Bootstrap
        public string displayName;    // Texto bajo el ícono
        public Sprite icon;           // Ícono “ganada”
        public Sprite lockedIcon;     // Ícono/placa “bloqueada”
    }

    [Header("UI")]
    [SerializeField] private GameObject panel;         // Panel contenedor (activar/desactivar)
    [SerializeField] private Transform contentParent;  // Grid/Vertical Layout
    [SerializeField] private MedalItem medalItemPrefab;
    [SerializeField] private TMP_Text countText;       // “Medallas: X/6”

    [Header("Catálogo")]
    [SerializeField] private List<MedalConfig> medals = new List<MedalConfig>();
    [SerializeField] private int totalSlots = 6;       // para mostrar 6 espacios aunque tengas menos medallas definidas

    [Header("Medalla de Nivel (opcional)")]
    [SerializeField] private bool showLevelMedal = false;
    [SerializeField] private string levelMedalDisplayName = "Nivel 1";
    [SerializeField] private Sprite levelIcon;
    [SerializeField] private Sprite levelLockedIcon;

    private readonly List<MedalItem> _spawned = new List<MedalItem>();
    private bool _built;

    void OnEnable()
    {
        BuildOnce();
        Subscribe(true);
        RefreshAll();
    }
    void OnDisable() => Subscribe(false);

    private void Subscribe(bool sub)
    {
        if (sub)
        {
            if (AppServices.Session != null)
                AppServices.Session.OnUserChanged += _ => RefreshAll();
            if (AppServices.Progress != null)
                AppServices.Progress.OnNivel1Completado += _ => RefreshAll();
        }
        else
        {
            if (AppServices.Session != null)
                AppServices.Session.OnUserChanged -= _ => RefreshAll();
            if (AppServices.Progress != null)
                AppServices.Progress.OnNivel1Completado -= _ => RefreshAll();
        }
    }

    private void BuildOnce()
    {
        if (_built) return;
        _built = true;

        if (!contentParent || !medalItemPrefab)
        {
            Debug.LogWarning("[Medals] Falta prefab o parent.");
            return;
        }

        // 1) Crea ítems para cada medalla configurada
        foreach (var cfg in medals)
        {
            var item = Instantiate(medalItemPrefab, contentParent);
            item.Setup(cfg.icon, cfg.displayName, cfg.lockedIcon);
            _spawned.Add(item);
        }

        // 2) Medalla de nivel (opcional)
        if (showLevelMedal)
        {
            var item = Instantiate(medalItemPrefab, contentParent);
            item.Setup(levelIcon, levelMedalDisplayName, levelLockedIcon);
            _spawned.Add(item);
        }

        // 3) Rellena con “slots” bloqueados hasta totalSlots
        while (totalSlots > 0 && _spawned.Count < totalSlots)
        {
            var placeholder = Instantiate(medalItemPrefab, contentParent);
            placeholder.Setup(null, "", null); // sin nombre
            placeholder.SetCompleted(false, forceLockedVisual: true);
            _spawned.Add(placeholder);
        }
    }

    public void Show()
    {
        if (panel) panel.SetActive(true);
        RefreshAll();
    }
    public void Hide()
    {
        if (panel) panel.SetActive(false);
    }

    private bool SafeIsCompleted(string minigameId)
    {
        if (string.IsNullOrWhiteSpace(minigameId)) return false;           // id vacío ? no obtenido
        if (AppServices.Progress == null || AppServices.Session == null)   // sin servicios/usuario ? no obtenido
            return false;

        // ProgressState.IsMinigameCompleted ya retorna false si el ID no existe en el JSON.
        return AppServices.Progress.IsMinigameCompleted(minigameId);
    }


    public void RefreshAll()
    {
        if (!_built) BuildOnce();
        if (AppServices.Progress == null || AppServices.Session == null) return;

        int completedCount = 0;
        int index = 0;

        // 1) Minijuegos
        for (int i = 0; i < medals.Count; i++, index++)
        {
            bool completed = SafeIsCompleted(medals[i].minigameId); // ? usa el helper
            _spawned[index].SetCompleted(completed);
            if (completed) completedCount++;
        }

        // 2) Nivel (opcional)
        if (showLevelMedal)
        {
            bool levelDone = AppServices.Progress.IsNivel1Completado();
            _spawned[index].SetCompleted(levelDone);
            if (levelDone) completedCount++;
            index++;
        }

        // 3) Placeholders restantes ya están bloqueados por defecto

        // 4) Texto “Medallas: X/N”
        int maxCount = (totalSlots > 0) ? totalSlots : _spawned.Count;
        if (countText) countText.text = $"Medallas: {completedCount}/{maxCount}";
    }
}
