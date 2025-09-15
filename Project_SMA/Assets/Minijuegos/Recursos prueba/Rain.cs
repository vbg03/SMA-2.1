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

    void Start()
    {
        botonReinicio.SetActive(false);
        spawnPoint = spawn.transform;
        StartCoroutine(BallRain());
        //la l�gica del minijuego se encargar� de hacer aparecer el boton de reinicio cuando sea necesario.
    }

    private void Update()
    {
        if (GetComponent<ScoreHandler>().gameOver)
        {
            StopAllCoroutines();
            botonReinicio.SetActive(true);
        }
    }

    //El minijuego de prueba es de esquivar objetos que caen
    IEnumerator BallRain()
    {
        for(int i = 0; i<20; i++)
        {
            yield return new WaitForSeconds(interval);
            Instantiate(prefab, spawnPoint.position, Quaternion.identity);
        }
        yield return new WaitForSeconds(1.5f);
    }
    

}
