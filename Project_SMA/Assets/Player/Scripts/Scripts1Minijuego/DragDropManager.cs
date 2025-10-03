using TMPro;
using UnityEngine;
using System;

public class DragDropManager : MonoBehaviour
{
    public GameObject botonVolver;
    public GameObject botonContinuar;
    [Header("Configuración del nivel")]
    public int totalFiguras = 3; // Número total de figuras requeridas

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
        if (figurasBloqueadas < totalFiguras)
        {
            textoFiguras.text = $"Figuras: {figurasBloqueadas}/{totalFiguras}";
        }
        else
        {
            textoFiguras.text = "Nivel Completado!";
        }
    }
    private void NivelCompletado()
    {
        //Debug.Log("¡Nivel Completado!");
        completado = true;
        botonContinuar.SetActive(true);
        botonVolver.SetActive(false);
        OnMinijuegoCompletado?.Invoke();
    }
}
