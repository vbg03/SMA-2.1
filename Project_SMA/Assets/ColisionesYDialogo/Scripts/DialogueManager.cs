using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] private Button interacButton;
    [SerializeField] private GameObject textPanel;
    [SerializeField] private TextMeshProUGUI textLabel;

    private List<string> dialogo = new List<string> ();
    private int index = -1;
    private bool inZone = false;
    private DialogueGata currentGata;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (textPanel)
        {
            textPanel.SetActive (false);
        }
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
        //  Fix: la guardia debe bloquear cuando NO estás en zona o no hay lista
        if (!inZone || dialogo == null || dialogo.Count == 0)
            return;

        // Mostrar el panel en el primer click si aún estaba oculto
        if (textPanel && !textPanel.activeSelf)
            textPanel.SetActive(true);

        index++;

        if (index < dialogo.Count)
        {
            if (textLabel) textLabel.text = dialogo[index];
        }
        else
        {
            // Fin del diálogo: ocultar UI
            if (textPanel) textPanel.SetActive(false);
            if (interacButton) interacButton.gameObject.SetActive(false);
        }
    }
}
