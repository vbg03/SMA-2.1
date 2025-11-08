using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MedalItem : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private Image lockedOverlay;      // opcional: candado/sombra

    private Sprite _iconWhenCompleted;
    private Sprite _iconWhenLocked;

    public void Setup(Sprite completedIcon, string displayName, Sprite lockedIcon = null)
    {
        _iconWhenCompleted = completedIcon;
        _iconWhenLocked = lockedIcon;
        SetCompleted(false, forceLockedVisual: _iconWhenCompleted == null); // arranca bloqueada si no hay icono
    }

    public void SetCompleted(bool completed, bool forceLockedVisual = false)
    {
        if (icon)
        {
            if (completed && _iconWhenCompleted) icon.sprite = _iconWhenCompleted;
            else if (_iconWhenLocked) icon.sprite = _iconWhenLocked;

            var a = 1f;
            icon.color = new Color(icon.color.r, icon.color.g, icon.color.b, a);
        }
        if (lockedOverlay) lockedOverlay.gameObject.SetActive(!completed || forceLockedVisual);
    }
}
