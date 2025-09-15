using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Rain : MonoBehaviour
{
    [SerializeField] private GameObject spawn;
    [SerializeField] private GameObject prefab;
    [SerializeField] private float interval;
    private Transform spawnPoint;
    [SerializeField] private GameObject botonReinicio;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        botonReinicio.SetActive(false);
        spawnPoint = spawn.transform;
        StartCoroutine(BallRain());
        //la lógica del minijuego se encargará de hacer aparecer el boton de reinicio cuando sea necesario.
    }
    //El minijuego de miniprueba es de esquivar objetos que caen
    IEnumerator BallRain()
    {
        for(int i = 0; i<20; i++)
        {
            yield return new WaitForSeconds(interval);
            Instantiate(prefab, spawnPoint.position, Quaternion.identity);
        }
        yield return new WaitForSeconds(1.5f);
        if (gameObject.GetComponent<ScoreHandler>().score < 10)
        {
            gameObject.GetComponent<ScoreHandler>().puntaje.text = "Perdiste :(";
            botonReinicio.SetActive(true);
        }

    }
    

}
