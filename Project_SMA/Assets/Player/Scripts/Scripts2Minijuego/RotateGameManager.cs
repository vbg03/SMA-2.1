using TMPro;
using UnityEngine;
using System;

public class RotateGameManager : MonoBehaviour
{
    private MinigameProgressReporter _reporter;
    private bool _winReportSent = false; // ADD
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

    public GameObject MedallaPerfil;
    public static bool youWin;

    private ProgressManager progressManager;


    // Evento estático para notificar que el minijuego terminó
    public static event Action OnMinijuego2Completado;

    void Start()
    {
        _reporter = GetComponent<MinigameProgressReporter>();
        winText.SetActive(false);
        youWin = false;
        progressManager = FindFirstObjectByType<ProgressManager>();
        Debug.Log(progressManager.ToString());
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
        if (!_winReportSent)
        {
            _winReportSent = true;
            if (_reporter != null) _reporter.ReportWin();
            else Debug.LogWarning("No hay MinigameProgressReporter en RotateGameManager.");
        }
        if (progressManager != null)
        {
            progressManager.NotifyTaskCompleted(this);
            Debug.Log("Notificó");
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
        MedallaPerfil.SetActive(true);
        youWin = true;
    }
}
