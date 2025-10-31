using UnityEngine;

public class SonidosBotones : MonoBehaviour
{
    [Header("Componentes de sonido")]
    public AudioSource audioSource;   // Donde se reproduce el sonido
    public AudioClip sonidoClick;     // El sonido del click
    public AudioClip sonidoFigureRotate;     // El sonido de sonidoFigureRotate
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Método que puede ser asignado al botón
    public void ReproducirSonidoClick()
    {
        if (audioSource != null && sonidoClick != null)
        {
            audioSource.PlayOneShot(sonidoClick);
        }
        else
        {
            Debug.LogWarning("Falta asignar el AudioSource o el AudioClip en " + gameObject.name);
        }
    }

    public void ReproducirSonidoRotarFigura()
    {
        if (audioSource != null && sonidoFigureRotate != null)
        {
            audioSource.PlayOneShot(sonidoFigureRotate);
        }
        else
        {
            Debug.LogWarning("Falta asignar el AudioSource o el AudioClip en " + gameObject.name);
        }
    }
}
