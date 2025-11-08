using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoginUIController : MonoBehaviour
{
    [Header("Referencias UI")]
    public TMP_InputField usernameInput;
    public TMP_InputField passwordInput;
    public Button loginButton;
    public TMP_Text feedbackText; // opcional

    void Awake()
    {
        if (loginButton) loginButton.interactable = false;
        if (usernameInput) usernameInput.onValueChanged.AddListener(_ => ValidateForm());
        if (passwordInput) passwordInput.onValueChanged.AddListener(_ => ValidateForm());
    }

    void OnEnable() => ValidateForm();

    public void OnLoginClick()
    {
        if (AppServices.Auth == null) { SetFeedback("Inicializando servicios…", true); return; }

        var user = usernameInput ? usernameInput.text.Trim() : "";
        var pass = passwordInput ? passwordInput.text : "";

        var result = AppServices.Auth.Login(user, pass);
        SetFeedback(result.message, !result.success);

        if (result.success)
        {
            // Limpia campo sensible o navega a tu siguiente pantalla
            if (passwordInput) passwordInput.text = "";
            // Ejemplo: habilitar panel de juego / deshabilitar panel de login
            // gamePanel.SetActive(true); loginPanel.SetActive(false);
            SceneManager.LoadScene("Perfil");
        }
    }

    private void ValidateForm()
    {
        bool ok = usernameInput && !string.IsNullOrWhiteSpace(usernameInput.text)
               && passwordInput && !string.IsNullOrEmpty(passwordInput.text);
        if (loginButton) loginButton.interactable = ok;
    }

    private void SetFeedback(string msg, bool isError)
    {
        if (!feedbackText) return;
        feedbackText.text = msg ?? "";
        // feedbackText.color = isError ? Color.red : Color.green; // opcional
    }
}
