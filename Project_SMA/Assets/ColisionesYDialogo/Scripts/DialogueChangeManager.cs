using System;
using System.Collections.Generic;
using UnityEngine;

public class DialogueChangeManager : MonoBehaviour
{
    // Lista de posibles diálogos (puedes arrastrar varios DialogueGata del mismo objeto o hijos)
    public List<DialogueGata> posiblesDialogos = new();

    // Reglas para decidir qué diálogo usar (por prioridad)
    public List<DialogueRule> reglas = new();

    // Secuencia lineal de respaldo (si no aplica ninguna regla)
    public bool usarSecuenciaLineal = true;
    private int indiceLineal = 0;

    private DialogueRule _ultimaReglaUsada;

    // Llama esto desde el jugador al interactuar; devuelve el DialogueGata seleccionado.
    // Si hay reglas que se cumplen, usa la de mayor prioridad. Si no, avanza linealmente.
    public DialogueGata GetCurrentDialogueAndAdvance()
    {
        _ultimaReglaUsada = null; // resetea
        // 1) Reglas (ordenadas por prioridad desc)
        if (reglas != null && reglas.Count > 0)
        {
            reglas.Sort((a, b) => b.prioridad.CompareTo(a.prioridad));
            foreach (var r in reglas)
            {
                if (r == null || r.dialogo == null) continue;
                if (r.usado && r.soloUnaVez) continue;
                if (CondicionesCumplidas(r.condiciones))
                {
                    if (r.soloUnaVez) r.usado = true;
                    //if (!string.IsNullOrEmpty(r.flagActivar)) GameFlagManager.I.SetFlag(r.flagActivar, r.flagValor);
                    _ultimaReglaUsada = r;
                    return r.dialogo;
                }
            }
        }

        // 2) Modo lineal de respaldo
        if (usarSecuenciaLineal && posiblesDialogos != null && posiblesDialogos.Count > 0)
        {
            int idx = Mathf.Clamp(indiceLineal, 0, posiblesDialogos.Count - 1);
            var elegido = posiblesDialogos[idx];
            if (indiceLineal < posiblesDialogos.Count - 1) indiceLineal++;
            return elegido;
        }

        return null;
    }

    // Reincia la secuencia lineal (por si quieres que vuelva al primer diálogo)
    public void ResetLinea() => indiceLineal = 0;

    // ----------------- internas -----------------

    private bool CondicionesCumplidas(List<DialogueCondition> condiciones)
    {
        if (condiciones == null || condiciones.Count == 0) return true;
        foreach (var c in condiciones)
            if (!Evaluar(c)) return false;
        return true;
    }

    private bool Evaluar(DialogueCondition c)
    {
        switch (c.tipo)
        {
            case ConditionType.Siempre: return true;
            case ConditionType.FlagActiva: return GameFlagManager.I.GetFlag(c.clave);
            case ConditionType.FlagInactiva: return !GameFlagManager.I.GetFlag(c.clave);
            default: return false;
        }
    }
    public void OnDialogueFinished(DialogueGata terminado)
    {
        if (_ultimaReglaUsada == null) return;
        Debug.Log(_ultimaReglaUsada.flagActivar);

        // Efectos “post-diálogo”: activar banderas y consumir la regla aquí (no al empezar)
        if (!string.IsNullOrEmpty(_ultimaReglaUsada.flagActivar)) GameFlagManager.I.SetFlag(_ultimaReglaUsada.flagActivar, _ultimaReglaUsada.flagValor);

        if (_ultimaReglaUsada.soloUnaVez) _ultimaReglaUsada.usado = true;

        // Limpia la referencia para evitar repetir
        _ultimaReglaUsada = null;
    }
}

// Datos de una regla: qué diálogo usar y bajo qué condiciones
[Serializable]
public class DialogueRule
{
    public int prioridad = 0;
    public DialogueGata dialogo;
    public List<DialogueCondition> condiciones = new();
    public bool soloUnaVez = false;      // si true, se consume al usarse
    [HideInInspector] public bool usado = false;

    public string flagActivar;           // opcional: activa un flag al usarse
    public bool flagValor = true;
}

// Condición atómica para reglas
[Serializable]
public class DialogueCondition
{
    public ConditionType tipo = ConditionType.Siempre;
    public string clave;
}

public enum ConditionType
{
    Siempre,
    FlagActiva,
    FlagInactiva
}
