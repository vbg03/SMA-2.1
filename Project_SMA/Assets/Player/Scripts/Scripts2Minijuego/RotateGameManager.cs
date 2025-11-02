using TMPro;
using UnityEngine;
using System;

public class RotateGameManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject botonVolver;
    public GameObject botonContinuar;
    public GameObject PanelMinijuego;
    public GameObject PanelLogro;
    public GameObject TextoInstrucciones;
    [SerializeField]
    private Transform [] imagenes;
    //TextMeshProUGUI
    [SerializeField] 
    private GameObject winText;
    public AudioSource audioSource;   // Donde se reproduce el sonido
    public AudioClip sonidoLogro;     // El sonido de sonidoFigureRotate

    public static bool youWin;

    private ProgressManager progressManager;


    // Evento estático para notificar que el minijuego terminó
    public static event Action OnMinijuego2Completado;

    void Start()
    {
        winText.SetActive(false);
        youWin = false;
        progressManager = FindFirstObjectByType<ProgressManager>();
        if (progressManager != null)
        {
            progressManager.RegisterTask(this);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(imagenes[0].rotation.z == 0 && imagenes[1].rotation.z == 0 && imagenes[2].rotation.z == 0 && imagenes[3].rotation.z == 0 && imagenes[4].rotation.z == 0 && imagenes[5].rotation.z == 0)
        {
            if (youWin == false) Nivel2Completado();
        }
    }

    public void Nivel2Completado()
    {
        if (progressManager != null)
        {
            progressManager.NotifyTaskCompleted(this);
        }
        OnMinijuego2Completado?.Invoke();
            winText.SetActive(true);
            botonVolver.SetActive(false);
            botonContinuar.SetActive(true);
            audioSource.PlayOneShot(sonidoLogro);
            GameFlagManager.I.SetFlag("Minijuego2Terminado", true);
            PanelMinijuego.SetActive(false);
            PanelLogro.SetActive(true);
            TextoInstrucciones.SetActive(false);
        
        youWin = true;
    }
}
