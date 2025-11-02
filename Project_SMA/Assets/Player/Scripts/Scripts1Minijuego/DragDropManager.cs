using TMPro;
using UnityEngine;
using System;
using UnityEngine.Audio;

public class DragDropManager : MonoBehaviour
{
    public GameObject botonVolver;
    public GameObject botonContinuar;
    public GameObject PanelMinijuego;
    public GameObject PanelLogro;
    [Header("Configuración del nivel")]
    public int totalFiguras = 3; // Numero total de figuras requeridas
    public AudioSource audioSource;   // Donde se reproduce el sonido
    public AudioClip sonidoFigureRotate;     // El sonido de sonidoFigureRotate
    public AudioClip sonidoLogro;     // El sonido de sonidoFigureRotate

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
        // Inicializar el texto
        ActualizarTexto();
    }

    private void FiguraBloqueadaHandler()
    {
        figurasBloqueadas++;
        ActualizarTexto();

        if (figurasBloqueadas >= totalFiguras)
        {
            NivelCompletado();
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
    private void NivelCompletado()
    {
        //Debug.Log("¡Nivel Completado!");
        completado = true;
        botonContinuar.SetActive(true);
        botonVolver.SetActive(false);
        OnMinijuegoCompletado?.Invoke();
        GameFlagManager.I.SetFlag("Minijuego1Terminado", true);
    }
}
