using UnityEngine;

[RequireComponent(typeof(Collider))] // Obliga a que el objeto tenga un Collider
public class Activar1Minijuego : MonoBehaviour
{
    [Header("Referencias")]
    public GameObject panelMinijuego1;           // El panel a mostrar
    public DragDropManager dragDropManager;  // Arrastra aquí el objeto con el script DragDropManager

    private void Reset()
    {
        // Se asegura de que el collider sea trigger
        GetComponent<Collider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Solo reacciona al jugador
        {
            if (dragDropManager != null && !dragDropManager.completado)
            {
                panelMinijuego1.SetActive(true);
            }
        }
    }
}
