using UnityEngine;

public class TouchRotate : MonoBehaviour
{

    private void OnMouseDown()
    {
        //!GameControl.youWin
        if (!RotateGameManager.youWin)
        {
            RotateFigure();
        }
    }

    public void RotateFigure() 
    {
        if (!RotateGameManager.youWin) transform.Rotate(0f, 0f, 90f);
    }

}
