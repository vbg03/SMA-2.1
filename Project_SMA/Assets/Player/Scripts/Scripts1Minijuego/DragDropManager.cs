using TMPro;
using UnityEngine;
using System;
using UnityEngine.Audio;

public class DragDropManager : MonoBehaviour
{
    private MinigameProgressReporter _reporter;
    private bool _winReportSent = false;

    public GameObject botonVolver;
    public GameObject botonContinuar;
    public GameObject PanelMinijuego;
    public GameObject PanelLogro;
    [Header("Configuración del nivel")]
    public int totalFiguras = 3; // Numero total de figuras requeridas
    public AudioSource audioSource;   // Donde se reproduce el sonido
    public AudioClip sonidoFigureRotate;     // El sonido de sonidoFigureRotate
    public AudioClip sonidoLogro;     // El sonido de sonidoFigureRotate

    private ProgressManager progressManager;

    public GameObject MedallaPerfil;

    [Header("UI")]
    public TextMeshProUGUI textoFiguras; // Asigna el objeto TMP desde el Canvas en el inspector

    private int figurasBloqueadas = 0;

    public bool completado = false;

    // Evento estático para notificar que el minijuego terminó
    public static event Action OnMinijuegoCompletado;
    void OnEnable()
    {
        // Suscripción al evento
        DragDropMinijuego.OnFiguraBloqueada += FiguraBloqueadaHandler;
    }

    void OnDisable()
    {
        // Desuscripción para evitar errores de referencia
        DragDropMinijuego.OnFiguraBloqueada -= FiguraBloqueadaHandler;
    }

    void Start()
    {
        _reporter = GetComponent<MinigameProgressReporter>();
        // Inicializar el texto

        ActualizarTexto();
        progressManager = FindFirstObjectByType<ProgressManager>();
        if (progressManager != null)
        {
            progressManager.RegisterTask(this);
        }

    }

    private void FiguraBloqueadaHandler()
    {
        figurasBloqueadas++;
        ActualizarTexto();

        if (figurasBloqueadas >= totalFiguras)
        {
            Nivel1Completado();
        }
    }

    private void ActualizarTexto()
    {
        if (audioSource != null && sonidoFigureRotate != null)
        {
            audioSource.PlayOneShot(sonidoFigureRotate);
        }
        else
        {
            Debug.LogWarning("Falta asignar el AudioSource o el AudioClip en " + gameObject.name);
        }

        if (figurasBloqueadas < totalFiguras)
        {
            textoFiguras.text = $"Figuras: {figurasBloqueadas}/{totalFiguras}";
        }
        else
        {
            textoFiguras.text = "Nivel Completado!";
            PanelMinijuego.SetActive(false);
            PanelLogro.SetActive(true);
            audioSource.PlayOneShot(sonidoLogro);

        }
    }
    private void Nivel1Completado()
    {
        //Debug.Log("¡Nivel Completado!");
        if (completado) return;
        completado = true;

        if (!_winReportSent)
        {
            _winReportSent = true;
            if (_reporter != null) _reporter.ReportWin();           // <-- ESTA ES LA CLAVE
            else Debug.LogWarning("No hay MinigameProgressReporter en DragDropManager.");
        }

        if (progressManager != null)
        {
            progressManager.NotifyTaskCompleted(this);
        }
        botonContinuar.SetActive(true);
        botonVolver.SetActive(false);
        OnMinijuegoCompletado?.Invoke();
        GameFlagManager.I.SetFlag("Minijuego1Terminado", true);
        MedallaPerfil.SetActive(true);

    }
}
