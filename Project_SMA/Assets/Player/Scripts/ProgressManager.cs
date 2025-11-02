using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressManager : MonoBehaviour
{


    [Header("UI")]
    [SerializeField] private Slider progressBar;
    [SerializeField] private GameObject textoSiguienteNivel;
    [SerializeField] private GameObject textoBarraProgreso;
    [SerializeField] private GameObject ColliderObstacle;
    [SerializeField] private GameObject ColliderTriggerResumen; 
    [SerializeField] private GameObject LuzSalida;

    private int totalTasks = 0;
    private int completedTasks = 0;

    // Lista opcional para evitar registrar dos veces el mismo
    //private HashSet<object> registeredTasks = new HashSet<object>();

    void Start()
    {
        if (progressBar != null)
            progressBar.value = 0f;
    }

    /// <summary>
    /// Registra una tarea o minijuego en el progreso global.
    /// </summary>
    public void RegisterTask(object task)
    {
        //if (registeredTasks.Add(task))
        //{
            totalTasks++;
            UpdateProgress();
        //}
    }

    /// <summary>
    /// Llama este método cuando un minijuego o diálogo se complete.
    /// </summary>
    public void NotifyTaskCompleted(object task)
    {
        //if (registeredTasks.Contains(task))
        //{
            completedTasks++;
            UpdateProgress();

            if (completedTasks >= totalTasks)
            {
                Debug.Log("Nivel completado!");
                textoSiguienteNivel.SetActive(true);
                textoBarraProgreso.SetActive(false);
                ColliderObstacle.SetActive(false);
                ColliderTriggerResumen.SetActive(true);
                LuzSalida.SetActive(true);
            }
        //}
    }

    private void UpdateProgress()
    {
        if (progressBar == null || totalTasks == 0) return;

        float progress = (float)completedTasks / totalTasks;
        progressBar.value = progress;
    }
}
