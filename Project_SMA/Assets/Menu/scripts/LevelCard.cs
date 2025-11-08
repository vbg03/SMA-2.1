using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum LevelState { Locked, Unlocked, Completed }


public class LevelCard : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private Image thumbnail;
    [SerializeField] private Button playButton;

    [Header("Estados Visuales")]
    [SerializeField] private GameObject lockOverlay;     // candado/filtro gris
    [SerializeField] private GameObject completedMark;   // check
    [SerializeField] private GameObject activeOutline;   // borde cian (opcional)

    // datos
    private string _sceneName;

    public void Setup(string displayName, Sprite thumb, string sceneName)
    {
        _sceneName = sceneName;
        if (titleText) titleText.text = displayName ?? "";
        if (thumbnail) thumbnail.sprite = thumb;
    }

    public void SetState(LevelState state)
    {
        bool locked = (state == LevelState.Locked);
        bool completed = (state == LevelState.Completed);
        bool unlocked = (state == LevelState.Unlocked);

        if (playButton) playButton.interactable = !locked;
        if (lockOverlay) lockOverlay.SetActive(locked);
        if (completedMark) completedMark.SetActive(completed);
        if (activeOutline) activeOutline.SetActive(unlocked); // solo el activo usa borde

        // accesibilidad visual mínima
        if (thumbnail) thumbnail.color = new Color(1, 1, 1, locked ? 0.5f : 1f);
    }

    public void OnClickPlay()
    {
        if (!string.IsNullOrEmpty(_sceneName))
            SceneManager.LoadScene(_sceneName);
    }
}
