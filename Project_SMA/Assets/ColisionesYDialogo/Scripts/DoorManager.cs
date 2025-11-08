using UnityEngine;

public class DoorManager : MonoBehaviour
{
    public GameObject closedDoor;
    public GameObject openDoor;
    public GameObject llave;
    private ProgressManager progressManager;
    private MinigameProgressReporter _reporter;

    bool prevPuerta = false;
    bool prevLlave = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _reporter = GetComponent<MinigameProgressReporter>();
        progressManager = FindFirstObjectByType<ProgressManager>();
        if (progressManager != null)
        {
            progressManager.RegisterTask(this);
        }
        if (GameFlagManager.I == null)
        {
            Debug.LogWarning("DoorManager: GameFlagManager no está en la escena. No se podrán leer flags.");
            return;
        }
        GameFlagManager.I.SetFlag("Puerta", false);
        GameFlagManager.I.SetFlag("Llave", false);
        prevPuerta = GameFlagManager.I.GetFlag("Puerta");
        prevLlave = GameFlagManager.I.GetFlag("Llave");

        if (prevPuerta) ApplyPuerta();
        if (prevLlave) ApplyLlave();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameFlagManager.I == null) return;
        bool nowPuerta = GameFlagManager.I.GetFlag("Puerta");
        bool nowLlave = GameFlagManager.I.GetFlag("Llave");

        if (nowPuerta && !prevPuerta) ApplyPuerta();
        if (nowLlave && !prevLlave) ApplyLlave();

        prevPuerta = nowPuerta;
        prevLlave = nowLlave;
    }

    void ApplyPuerta() {
        closedDoor.SetActive(false);
        openDoor.SetActive(true);
    }
    void ApplyLlave()
    {
        llave.SetActive(false);
        if (progressManager != null) {
            progressManager.NotifyTaskCompleted(this);
        }
        if (_reporter != null) _reporter.ReportWin();           // <-- ESTA ES LA CLAVE
        else Debug.LogWarning("No hay MinigameProgressReporter en DragDropManager.");
    }
}
