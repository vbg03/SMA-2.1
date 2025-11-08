using TMPro;
using UnityEngine;

public class ProfileScreenController : MonoBehaviour
{
    [SerializeField] private TMP_Text titleText; // “Bienvenido [Usuario]”
    [SerializeField] private GameObject panel;   // Panel de perfil

    private void OnEnable()
    {
        if (AppServices.Session != null)
            AppServices.Session.OnUserChanged += HandleUserChanged;
        Refresh();
    }

    private void OnDisable()
    {
        if (AppServices.Session != null)
            AppServices.Session.OnUserChanged -= HandleUserChanged;
    }

    private void HandleUserChanged(UserProfile user) => Refresh();

    public void Show()
    {
        if (panel) panel.SetActive(true);
        Refresh();
    }

    public void Hide()
    {
        if (panel) panel.SetActive(false);
    }

    private void Refresh()
    {
        var user = AppServices.Session != null ? AppServices.Session.ActiveUser : null;
        var username = (user != null && !string.IsNullOrEmpty(user.username)) ? user.username : "Invitado";
        if (titleText) titleText.text = $"Bienvenido {username}";
    }
}
