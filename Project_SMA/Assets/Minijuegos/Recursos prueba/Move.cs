using UnityEngine;

public class Move : MonoBehaviour
{
    [SerializeField] private float speed;

    // Update is called once per frame
    void Update()
    {
        //Agarramos la entrada Horizontal por defecto (A y D o <- y -> )
        float moveX = Input.GetAxis("Horizontal");

        //Solo se mueve en X
        Vector3 movement = new Vector3(moveX, 0, 0);

        //Aplicamos el movimiento
        transform.Translate(movement*speed*Time.deltaTime,Space.World);

    }
}
