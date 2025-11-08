using UnityEngine;

public class YaraManager : MonoBehaviour
{
    public GameObject yaraPrefab;
    public Transform spawnPoint;
    public MeshRenderer cuadro;

    bool prevMinijuego2Terminado = false;
    bool prevNota = false;

    private GameObject yara;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (GameFlagManager.I == null)
        {
            Debug.LogWarning("YaraManager: GameFlagManager no está en la escena. No se podrán leer flags.");
            return;
        }
        GameFlagManager.I.SetFlag("Minijuego2Terminado", false);
        GameFlagManager.I.SetFlag("Nota", false);

        prevMinijuego2Terminado = GameFlagManager.I.GetFlag("Minijuego2Terminado");
        prevNota = GameFlagManager.I.GetFlag("Nota");

        ApplyMinijuego2Terminado(prevMinijuego2Terminado);
        ApplyNota(prevNota);
    }

    // Update is called once per frame
    void Update()
    {
        if (GameFlagManager.I == null) return;

        // Lee flags actuales
        bool nowMinijuego2Terminado = GameFlagManager.I.GetFlag("Minijuego2Terminado");
        bool nowNota = GameFlagManager.I.GetFlag("Nota");

        // Detecta cambios y aplica

        if (nowMinijuego2Terminado && !prevMinijuego2Terminado) ApplyMinijuego2Terminado(true);


        if (nowNota && !prevNota) ApplyNota(true);

        // Actualiza caché
        prevMinijuego2Terminado = nowMinijuego2Terminado;
        prevNota = nowNota;
    }
    void ApplyMinijuego2Terminado(bool isTrue)
    {
        if (yaraPrefab == null)
        {
            Debug.LogWarning("YaraManager: anabellaPrefab no asignado.");
            return;
        }

        Vector3 pos = transform.position;
        Quaternion rot = transform.rotation;

        if (spawnPoint != null)
        {
            pos = spawnPoint.position;
            rot = spawnPoint.rotation;
        }

        yara = Instantiate(yaraPrefab, pos, rot);
    }
    void ApplyNota(bool isTrue)
    {
        Destroy(yara);
        cuadro.enabled = isTrue;
    }
}
