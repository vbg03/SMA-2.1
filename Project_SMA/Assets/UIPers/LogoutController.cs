using UnityEngine;
using UnityEngine.SceneManagement;

public class LogoutController : MonoBehaviour
{
    [SerializeField] string loginSceneName = "Login";

    public void OnClickLogout()
    {
        AppServices.Progress?.SaveActiveUser();   // opcional
        AppServices.Auth?.Logout();               // cierra sesión
        // (opcional) si no quieres autologin: PlayerPrefs.DeleteKey("SESSION_LAST_USERNAME"); PlayerPrefs.Save();
        SceneManager.LoadScene(loginSceneName);   // volver a login
    }
}
