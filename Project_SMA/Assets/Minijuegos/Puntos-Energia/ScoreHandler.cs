using JetBrains.Annotations;
using UnityEngine;
using TMPro;

public class ScoreHandler : MonoBehaviour
{
    public int score;
    public int energy;
    public TextMeshProUGUI puntaje;
    void Start()
    {
        score = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (score < 0) {
            score = 0;
        }
        puntaje.text = "Puntos: " + score;
    }

    /*Cuando se deba manejar el puntaje/energia, se llamarán estas funciones, el script se encarga
    de actualizar la Interfaz. Existen dos sets de funciones en caso de que la mecánica del juego requiera 
    ambos recursos.*/
    public void getPoints(int points)
    {
        score += points;
    }

    public void losePoints(int points)
    {
        score -= points;
    }

    public void getEnergy(int percentage)
    {
        energy += percentage;
    }
    public void loseEnergy(int percentage){
        energy -= percentage;
    }
}
