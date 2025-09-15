using UnityEngine;
using UnityEngine.SceneManagement;

public class RetryHandler : MonoBehaviour
{
    //Se utilizar� el SceneManagmente para cargar de nuevo la escena.
    public void Reinicio()
    {
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);

    }
}
