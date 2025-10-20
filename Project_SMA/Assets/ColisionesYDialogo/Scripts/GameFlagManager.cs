using System.Collections.Generic;
using UnityEngine;

public class GameFlagManager : MonoBehaviour
{

    public static GameFlagManager I { get; private set; }

    // Runtime: banderas del juego (no serializables, no visibles)
    private readonly Dictionary<string, bool> flags = new();

    void Awake()
    {
        if (I != null && I != this) { Destroy(gameObject); return; }
        I = this;
        DontDestroyOnLoad(gameObject);
    }

    // API pública
    public void SetFlag(string key, bool value = true) { flags[key] = value; Debug.Log(key + ":" + flags[key]); }
    public bool GetFlag(string key, bool def = false) => flags.TryGetValue(key, out var v) ? v : def;
}
