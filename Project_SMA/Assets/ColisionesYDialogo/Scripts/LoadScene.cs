using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    public string escena;
    public void cambiarEscena()
    {
        Debug.Log("Cargando: " +  escena);
        SceneManager.LoadScene(escena);
    }

}
