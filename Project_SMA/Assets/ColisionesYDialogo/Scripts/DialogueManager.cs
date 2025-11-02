using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    public int index = -1;
    private bool inZone = false;
    private DialogueGata currentGata;
    private DialogueChangeManager currentChange;
    private GameObject currentTarget;
    private bool rearming = false;

    //private ProgressManager progressManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Start()
    {
        //progressManager = FindFirstObjectByType<ProgressManager>();
        //if (progressManager != null)
        //{
        //    progressManager.RegisterTask(this);
        //}
    }

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
        }
    }
    private void ForceReevaluateCurrentTarget()
    {
        if (currentTarget == null || rearming) return;
        StartCoroutine(RearmColliderCoroutine(currentTarget));
    }
    private System.Collections.IEnumerator RearmColliderCoroutine(GameObject target)
{
    rearming = true;

    // Soporta 3D y 2D
    var boxCol = target.GetComponent<BoxCollider>();
    var SphCol = target.GetComponent<SphereCollider>();

    // Apaga el collider un frame
    if (boxCol) boxCol.enabled = false;
    if (SphCol) SphCol.enabled = false;

    // Espera un frame (o WaitForFixedUpdate si usas física)
    yield return new WaitForSeconds(1f); // o: yield return new WaitForFixedUpdate();

    // Vuelve a encenderlo
    if (boxCol) boxCol.enabled = true;
    if (SphCol) SphCol.enabled = true;

    rearming = false;
}

    private void OnTriggerEnter(Collider other)
    {
        currentChange = other.GetComponent<DialogueChangeManager>();
        // 1) Si el interactuable tiene un DialogueChangeManager, pídele el DialogueGata correcto
        DialogueGata elegido = null;
        var change = other.GetComponent<DialogueChangeManager>();
        if (change != null)
            elegido = change.GetCurrentDialogueAndAdvance();
        else
            // 2) Si no hay gestor, usa el DialogueGata directo del interactuable (tu flujo actual)
            elegido = other.GetComponent<DialogueGata>();

        // 3) Si no hay diálogo, no armamos UI
        if (elegido == null || elegido.dialogos == null || elegido.dialogos.Count == 0)
            return;

        // 4) Cargamos el diálogo seleccionado como siempre
        currentGata = elegido;
        dialogo = currentGata.dialogos;
        index = -1;
        inZone = true;

        if (interacButton) interacButton.gameObject.SetActive(true);
        if (textName) textName.text = currentGata.elementName;

        if (interacButton != null)
            interacButton.onClick.RemoveAllListeners();
            interacButton.onClick.AddListener(ShowNext);
    }

    private void OnTriggerExit(Collider other)
    {
        // Salimos solo si estamos dejando el mismo objeto con el que estábamos dialogando
        if (currentGata != null && other.gameObject == currentGata.gameObject)
        {
            inZone = false;
            currentGata = null;
            dialogo = null;
            index = -1;
            currentChange = null;

            if (textPanel) textPanel.SetActive(false);
            if (interacButton) interacButton.gameObject.SetActive(false);
            if (textLabel) textLabel.text = string.Empty;

            if (interacButton != null) interacButton.onClick.RemoveListener(ShowNext);
        }
    }

    private void ShowNext()
    {
        if (!inZone || dialogo == null || dialogo.Count == 0) return;

        if (textPanel && !textPanel.activeSelf) textPanel.SetActive(true);

        index++;

        if (index < dialogo.Count)
        {
            string currentLine = dialogo[index];
            if (textLabel) textLabel.text = currentLine;
            PlayDialogueSound(currentLine);
        }
        else
        {
            if (currentChange != null && currentGata != null) 
            {
                currentGata.progressManager?.NotifyTaskCompleted(currentGata);
                currentChange.OnDialogueFinished(currentGata); 
            }
            if (textPanel) textPanel.SetActive(false);
            if (interacButton) interacButton.gameObject.SetActive(false);
            //progressManager?.NotifyTaskCompleted(this);
            currentGata.dialogos.Clear();
            dialogo = null;
            index = -1;
            ForceReevaluateCurrentTarget();
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
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);

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
