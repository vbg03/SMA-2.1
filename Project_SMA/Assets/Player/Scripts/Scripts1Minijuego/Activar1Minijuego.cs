using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Collider))] // Obliga a que el objeto tenga un Collider
public class Activar1Minijuego : MonoBehaviour
{
    [Header("Referencias")]
    public GameObject panelMinijuego1;           // El panel del minijuego
    public GameObject signoInteraccion;           // El signo interaccion
    public GameObject panelPlayerUI;           // El panel de los botones del jugador
    public DragDropManager dragDropManager;      // Script que maneja el minijuego
    public Button botonInteractuar;              // Botón en la UI para interactuar

    private bool minijuegoTerminado = false;

    private void Reset()
    {
        // Se asegura de que el collider sea trigger
        GetComponent<Collider>().isTrigger = true;
    }

    private void OnEnable()
    {
        // Nos suscribimos al evento del minijuego
        DragDropManager.OnMinijuegoCompletado += DesactivarBotonDefinitivo;
    }

    private void OnDisable()
    {
        // Nos desuscribimos para evitar errores
        DragDropManager.OnMinijuegoCompletado -= DesactivarBotonDefinitivo;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !minijuegoTerminado)
        {
            if (dragDropManager != null && !dragDropManager.completado)
            {
                botonInteractuar.gameObject.SetActive(true);
                botonInteractuar.onClick.AddListener(MostrarMinijuego);
                signoInteraccion.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && !minijuegoTerminado)
        {
            botonInteractuar.onClick.RemoveListener(MostrarMinijuego);
            botonInteractuar.gameObject.SetActive(false);
            panelPlayerUI.SetActive(true);
            signoInteraccion.SetActive(false);

        }
    }

    private void MostrarMinijuego()
    {
        panelMinijuego1.SetActive(true);
        panelPlayerUI.SetActive(false);
        // Aquí podrías notificar al DragDropManager que el minijuego inició
        // ej: dragDropManager.IniciarMinijuego();
    }

    // Se llama automáticamente cuando el minijuego se completa
    private void DesactivarBotonDefinitivo()
    {
        minijuegoTerminado = true;

        // Oculta y limpia el botón
        botonInteractuar.onClick.RemoveListener(MostrarMinijuego);
        botonInteractuar.gameObject.SetActive(false);
        signoInteraccion.SetActive(false);
        panelPlayerUI.SetActive(false);

        Debug.Log(" El minijuego ya terminó, botón desactivado permanentemente.");
    }
}
