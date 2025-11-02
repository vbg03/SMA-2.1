using UnityEngine;

public class Resumen1Manager : MonoBehaviour
{
    [SerializeField] private GameObject playerUI;
    [SerializeField] private GameObject resumenUI;
    public AudioSource audioSource;   // Donde se reproduce el sonido
    public AudioClip sonidoLogro;

    public bool enter = false;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerUI.SetActive(false);
            resumenUI.SetActive(true);

            if (!enter)
            {
                audioSource.PlayOneShot(sonidoLogro);
            }
            enter = true;
        }
    }


}
