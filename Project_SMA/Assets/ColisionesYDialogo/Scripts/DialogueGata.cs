using UnityEngine;
using System.Collections.Generic;

public class DialogueGata : MonoBehaviour
{
    public string elementName = "Null";
    public List<string> dialogos = new List<string>();
    public AudioClip dialogueSound;  // Sonido a reproducir por línea

    public ProgressManager progressManager;

    void Start()
    {
        progressManager = FindFirstObjectByType<ProgressManager>();
        if (progressManager != null)
        {
            progressManager.RegisterTask(this);
        }
    }
}
