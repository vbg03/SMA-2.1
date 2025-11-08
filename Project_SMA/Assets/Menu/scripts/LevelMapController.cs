using UnityEngine;

public class LevelMapController : MonoBehaviour
{
    [System.Serializable]
    public class LevelEntry
    {
        public string levelId;        // "Nivel1", "Nivel2", ...
        public string displayName;    // "Taller de Tejada"
        public string sceneName;      // nombre exacto de la escena
        public Sprite thumbnail;     // imagen de la tarjeta
        public LevelCard card;        // referencia a la tarjeta en escena
    }

    [Header("Orden de niveles (izq?der)")]
    [SerializeField] private LevelEntry[] levels;

    private void Start()
    {
        RefreshStates();
    }

    public void RefreshStates()
    {
        if (levels == null || levels.Length == 0) return;

        for (int i = 0; i < levels.Length; i++)
        {
            var e = levels[i];
            if (e == null || e.card == null) continue;

            // 1) setear textos/imagen/escena
            e.card.Setup(e.displayName, e.thumbnail, e.sceneName);

            // 2) calcular estado
            bool isCompleted = GetLevelCompleted(e.levelId);
            bool isUnlocked = (i == 0) || GetLevelCompleted(levels[i - 1].levelId);

            LevelState state =
                isCompleted ? LevelState.Completed :
                isUnlocked ? LevelState.Unlocked :
                              LevelState.Locked;

            // 3) aplicar visual y botón
            e.card.SetState(state);
        }
    }

    // === AQUÍ consultamos la persistencia ===
    // Por ahora: Nivel1 usa tu ProgressService; los siguientes puedes ir añadiéndolos igual.
    private bool GetLevelCompleted(string levelId)
    {
        if (string.IsNullOrEmpty(levelId)) return false;

        // Si el id no existe en la persistencia, lo tratamos como NO completado (false).
        switch (levelId)
        {
            case "Nivel1":
                return AppServices.Progress != null && AppServices.Progress.IsNivel1Completado();

            // Ejemplos para cuando definas los demás:
            // case "Nivel2": return AppServices.Progress?.IsLevelCompleted("Nivel2") ?? false;
            // case "Nivel3": return AppServices.Progress?.IsLevelCompleted("Nivel3") ?? false;

            default:
                return false; // id desconocido => no completado
        }
    }
}
