using UnityEngine;
using System;

public class DragDropMinijuego : MonoBehaviour
{
    public GameObject objectToDrag;
    public GameObject objectDragToPos;

    public float dropDistance;
    public bool isLocked;

    Vector2 objectInitPos;

    // Evento estático que notifica cuando una figura se bloquea
    public static event Action OnFiguraBloqueada;

    void Start()
    {
        objectInitPos = objectToDrag.transform.position;
    }

    public void DragObject()
    {
        if (!isLocked)
        {
            objectToDrag.transform.position = Input.mousePosition;
        }
    }

    public void DropObject()
    {
        float Distance = Vector3.Distance(objectToDrag.transform.position, objectDragToPos.transform.position);

        if (Distance < dropDistance)
        {
            isLocked = true;
            objectToDrag.transform.position = objectDragToPos.transform.position;

            // Notificamos al observador que esta figura se bloqueó
            OnFiguraBloqueada?.Invoke();
        }
        else
        {
            objectToDrag.transform.position = objectInitPos;
        }
    }
}
