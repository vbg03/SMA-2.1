using UnityEngine;

public class Ball : MonoBehaviour
{
    private SphereCollider SpCol;
    private GameObject MGameManager;
    void Start()
    {
        MGameManager = GameObject.Find("MGameManager");
        SpCol = GetComponent<SphereCollider>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Cuando una bola toca el piso, ganamos un punto.
        if (collision.gameObject.name == "Floor")
        {
            MGameManager.GetComponent<ScoreHandler>().getPoints(1);
            Destroy(gameObject);
        }
        //Cuando una bola toca al cubo, perdemos un punto y 20% de energía.
        if (collision.gameObject.name == "MC")
        {
            MGameManager.GetComponent<ScoreHandler>().losePoints(1);
            MGameManager.GetComponent <ScoreHandler>().loseEnergy(20);
            Destroy(gameObject);
        }
    }
}
