using TMPro;
using UnityEngine;
using System;

public class RotateGameManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject botonVolver;
    public GameObject botonContinuar;

    [SerializeField]
    private Transform [] imagenes;
    //TextMeshProUGUI
    [SerializeField] 
    private GameObject winText;

    public static bool youWin;

    // Evento estático para notificar que el minijuego terminó
    public static event Action OnMinijuego2Completado;

    void Start()
    {
        winText.SetActive(false);
        youWin = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(imagenes[0].rotation.z == 0 && imagenes[1].rotation.z == 0 && imagenes[2].rotation.z == 0 && imagenes[3].rotation.z == 0 && imagenes[4].rotation.z == 0 && imagenes[5].rotation.z == 0)
        {
            if (youWin ==false)
            {
                OnMinijuego2Completado?.Invoke();
                winText.SetActive(true);
                botonVolver.SetActive(false);
                botonContinuar.SetActive(true);
            }
            youWin = true;
        }
    }
}
