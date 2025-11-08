using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AuthUIController : MonoBehaviour
{
    [Header("Referencias UI (arrástralas)")]
    public TMP_InputField usernameInput;
    public TMP_InputField passwordInput;
    public Button registerButton;
    public TMP_Text feedbackText; // opcional

    [Header("Validación mínima")]
    public int minPasswordLength = 4;

    void Awake()
    {
        // Botón comienza deshabilitado hasta que haya datos válidos
        if (registerButton) registerButton.interactable = false;

        // Suscribir cambios para validar en caliente
        if (usernameInput) usernameInput.onValueChanged.AddListener(_ => ValidateForm());
        if (passwordInput) passwordInput.onValueChanged.AddListener(_ => ValidateForm());
    }

    void OnEnable() => ValidateForm();

    public void OnRegisterClick()
    {
        if (AppServices.Auth == null)
        {
            SetFeedback("Sistema no inicializado (Auth nulo).", true);
            return;
        }

        var user = usernameInput ? usernameInput.text.Trim() : "";
        var pass = passwordInput ? passwordInput.text : "";

        var result = AppServices.Auth.Register(user, pass);
        SetFeedback(result.message, !result.success);

        if (result.success)
        {
            // Limpia campos o navega a otra pantalla, según tu flujo
            if (passwordInput) passwordInput.text = "";
        }
    }

    private void ValidateForm()
    {
        var userOk = usernameInput && !string.IsNullOrWhiteSpace(usernameInput.text);
        var passOk = passwordInput && (passwordInput.text?.Length >= minPasswordLength);

        if (registerButton) registerButton.interactable = (userOk && passOk);
    }

    private void SetFeedback(string msg, bool isError)
    {
        if (!feedbackText) return;
        feedbackText.text = msg ?? "";
        // Si quieres, cambia color según error/éxito
        // feedbackText.color = isError ? Color.red : Color.green;
    }
}
