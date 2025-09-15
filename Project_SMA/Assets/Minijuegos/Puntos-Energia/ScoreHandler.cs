using JetBrains.Annotations;
using UnityEngine;
using TMPro;
using UnityEngine.UIElements;
using UnityEngine.UI;

public class ScoreHandler : MonoBehaviour
{
    public int score;
    public float energy;
    public TextMeshProUGUI puntaje;
    public UnityEngine.UI.Image energiaUI;
    public bool gameOver = false;
    void Start()
    {
        score = 0;
        energy = 100f;
    }

    // Update is called once per frame
    void Update()
    {
        if (score < 0) {
            score = 0;
        }
        //Cuando la energía se acaba, el juego termina. La energía disminuye cuando una bola toca al cubo.
        if (energy <= 0)
        {
           gameOver = true; 
        }
        puntaje.text = "Puntos: " + score;
        energiaUI.fillAmount = energy/100;
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
