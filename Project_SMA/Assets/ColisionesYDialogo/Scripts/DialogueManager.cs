using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Button interacButton;
    [SerializeField] private GameObject textPanel;
    [SerializeField] private TextMeshProUGUI textLabel;
    [SerializeField] private TextMeshProUGUI textName;

    [Header("Audio Settings")]
    [SerializeField, Range(0.8f, 1.2f)] private float minPitch = 0.95f;
    [SerializeField, Range(0.8f, 1.2f)] private float maxPitch = 1.05f;
    [SerializeField] private float minDuration = 0.5f;  // Mínimo tiempo de sonido

    [Header("Fade Settings")]
    [SerializeField, Tooltip("Duración del fundido de entrada en segundos (0 = sin fade).")]
    private float fadeInTime = 0.1f;
    [SerializeField, Tooltip("Duración del fundido de salida en segundos (0 = sin fade).")]
    private float fadeOutTime = 0.2f;
    private Coroutine fadeCoroutine;

    private AudioSource audioSource;
    private List<string> dialogo = new List<string> ();
    private int index = -1;
    private bool inZone = false;
    private DialogueGata currentGata;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource != null)
        {
            audioSource.playOnAwake = false;
        }

        if (textPanel)
            textPanel.SetActive(false);

        if (interacButton)
        {
            interacButton.gameObject.SetActive(false);
            interacButton.onClick.AddListener(ShowNext);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        currentGata = other.GetComponent<DialogueGata>();
        if (currentGata != null && currentGata.dialogos != null && currentGata.dialogos.Count > 0)
        {
            dialogo = currentGata.dialogos;
            index = -1;
            inZone = true;
            interacButton.gameObject.SetActive (true);
            textName.text = currentGata.elementName;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (currentGata != null && other.gameObject == currentGata.gameObject)
        {
            inZone = false;
            currentGata = null;
            dialogo =  null;
            index = -1;

            textPanel.SetActive(false);
            interacButton.gameObject.SetActive (false);
            textLabel.text = string.Empty;
        }
    }

    private void ShowNext()
    {
        if (!inZone || dialogo == null || dialogo.Count == 0)
            return;

        if (textPanel && !textPanel.activeSelf)
            textPanel.SetActive(true);

        index++;

        if (index < dialogo.Count)
        {
            string currentLine = dialogo[index];
            if (textLabel) textLabel.text = currentLine;
            PlayDialogueSound(currentLine);
        }
        else
        {
            if (textPanel) textPanel.SetActive(false);
            if (interacButton) interacButton.gameObject.SetActive(false);
            interacButton.onClick.RemoveListener(ShowNext);
        }
    }

    private void PlayDialogueSound(string line)
    {
        if (currentGata.dialogueSound == null || audioSource == null) return;

        // Ajuste de pitch aleatorio
        audioSource.pitch = Random.Range(minPitch, maxPitch);
        audioSource.clip = currentGata.dialogueSound;

        // Duración calculada según longitud del texto
        float duration = Mathf.Max(minDuration, line.Length * 0.02f);

        // Si hay una corrutina previa de fade, la detenemos
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(PlayWithFade(duration));
    }

    private IEnumerator PlayWithFade(float duration)
    {
        audioSource.volume = 0f;
        audioSource.Play();

        // Fade in
        if (fadeInTime > 0f)
        {
            float timer = 0f;
            while (timer < fadeInTime)
            {
                timer += Time.deltaTime;
                audioSource.volume = Mathf.Lerp(0f, 1f, timer / fadeInTime);
                yield return null;
            }
        }
        else
        {
            audioSource.volume = 1f;
        }

        // Mantener volumen durante el tiempo restante (menos el fade out)
        float holdTime = Mathf.Max(0f, duration - fadeOutTime);
        yield return new WaitForSeconds(holdTime);

        // Fade out
        if (fadeOutTime > 0f)
        {
            float timer = 0f;
            while (timer < fadeOutTime)
            {
                timer += Time.deltaTime;
                audioSource.volume = Mathf.Lerp(1f, 0f, timer / fadeOutTime);
                yield return null;
            }
        }

        audioSource.Stop();
        audioSource.volume = 1f;
    }
}
