using UnityEngine;

public class Ball : MonoBehaviour
{
    private SphereCollider SpCol;
    private GameObject MGameManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        MGameManager = GameObject.Find("MGameManager");
        SpCol = GetComponent<SphereCollider>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "Floor")
        {
            MGameManager.GetComponent<ScoreHandler>().getPoints(1);
            Destroy(gameObject);
        }
        if (collision.gameObject.name == "MC")
        {
            MGameManager.GetComponent<ScoreHandler>().losePoints(1);
            Destroy(gameObject);
        }
    }
}
