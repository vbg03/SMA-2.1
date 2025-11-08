using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class DragDropMinijuego : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public RectTransform objectToDrag;
    public RectTransform objectDragToPos;
    public float dropDistance = 50f;
    public bool isLocked = false;

    private Vector2 objectInitPos;
    private Canvas canvas;

    // Evento estático que notifica cuando una figura se bloquea
    public static event Action OnFiguraBloqueada;

    void Start()
    {
        objectInitPos = objectToDrag.anchoredPosition;
        canvas = GetComponentInParent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (isLocked) return;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isLocked) return;

        // Movimiento del objeto basado en el delta del evento
        Vector2 movePos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            eventData.position,
            canvas.worldCamera,
            out movePos);

        objectToDrag.anchoredPosition = movePos;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (isLocked) return;

        float distance = Vector2.Distance(objectToDrag.anchoredPosition, objectDragToPos.anchoredPosition);

        if (distance < dropDistance)
        {
            isLocked = true;
            objectToDrag.anchoredPosition = objectDragToPos.anchoredPosition;

            // Notificamos al observador que esta figura se bloqueó
            OnFiguraBloqueada?.Invoke();
        }
        else
        {
            objectToDrag.anchoredPosition = objectInitPos;
        }
    }
}
