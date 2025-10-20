using System;
using System.Collections;
using UnityEngine;

public class AnabellaManager : MonoBehaviour
{
    // Objeto cuya visibilidad controlarás (su MeshRenderer se activará/desactivará)
    public MeshRenderer targetRenderer;
    public MeshRenderer targetRenderer2;
    public BoxCollider targetCollider;
    public BoxCollider targetCollider2;

    // Prefab que se debe instanciar cuando "ApariciónAnabela" sea true
    public GameObject anabellaPrefab;

    // Punto de aparición del prefab (posición/rotación tomadas de aquí)
    public Transform spawnPoint;

    // Cache de estado para detectar cambios (evita hacer trabajo cada frame)
    bool prevMinijuego1Terminado = false;
    bool prevAparicionAnabella = false;
    bool prevInscripcion = false;

    private GameObject anabella;

    void Start()
    {
        // Asegurar que el gestor de flags exista
        if (GameFlagManager.I == null)
        {
            Debug.LogWarning("AnabellaManager: GameFlagManager no está en la escena. No se podrán leer flags.");
            return;
        }

        // Estado inicial según flags actuales (por si ya estaban activadas antes de cargar esta escena)
        GameFlagManager.I.SetFlag("Minijuego1Terminado", false);
        GameFlagManager.I.SetFlag("ApariciónAnabella", false);
        GameFlagManager.I.SetFlag("Inscripcion", false);

        prevMinijuego1Terminado = GameFlagManager.I.GetFlag("Minijuego1Terminado");
        prevAparicionAnabella = GameFlagManager.I.GetFlag("ApariciónAnabella");
        prevInscripcion = GameFlagManager.I.GetFlag("Inscripcion");

        // Aplica estado inicial coherente
        ApplyMinijuego1Terminado(prevMinijuego1Terminado);
        if (prevAparicionAnabella) ApplyAparicionAnabella();
        ApplyInscripcion(prevInscripcion);
    }

    void Update()
    {


        if (GameFlagManager.I == null) return;

        // Lee flags actuales
        bool nowMinijuego1Terminado = GameFlagManager.I.GetFlag("Minijuego1Terminado");
        bool nowAparicionAnabella = GameFlagManager.I.GetFlag("ApariciónAnabella");
        bool nowInscripcion = GameFlagManager.I.GetFlag("Inscripcion");

        // Detecta cambios y aplica

        if (nowMinijuego1Terminado && !prevMinijuego1Terminado) ApplyMinijuego1Terminado(true);

        // Si aparece Anabela, ejecuta transición (oculta renderer e instancia prefab)
        if (nowAparicionAnabella && !prevAparicionAnabella) ApplyAparicionAnabella();

        if (nowInscripcion && !prevInscripcion) ApplyInscripcion(true);

        // Actualiza caché
        prevMinijuego1Terminado = nowMinijuego1Terminado;
        prevAparicionAnabella = nowAparicionAnabella;
        prevInscripcion = nowInscripcion;
    }

    // Activa el MeshRenderer cuando el minijuego termina
    void ApplyMinijuego1Terminado(bool isTrue)
    {
        if (targetRenderer != null) targetRenderer.enabled = isTrue;
        if (targetCollider != null) targetCollider.enabled = isTrue;
    }

    // Oculta el MeshRenderer e instancia el prefab en el spawnPoint
    void ApplyAparicionAnabella()
    {
        if (targetRenderer != null) targetRenderer.enabled = false;
        if (targetCollider != null) targetCollider.enabled = false;

        if (anabellaPrefab == null)
        {
            Debug.LogWarning("AnabellaManager: anabellaPrefab no asignado.");
            return;
        }

        Vector3 pos = transform.position;
        Quaternion rot = transform.rotation;

        if (spawnPoint != null)
        {
            pos = spawnPoint.position;
            rot = spawnPoint.rotation;
        }

        anabella = Instantiate(anabellaPrefab, pos, rot);
    }


    void ApplyInscripcion(bool isTrue)
    {
        Destroy(anabella);
        if (targetRenderer2 != null) targetRenderer2.enabled = isTrue;
        if (targetCollider2 != null) targetCollider2.enabled = isTrue;
    }
}
