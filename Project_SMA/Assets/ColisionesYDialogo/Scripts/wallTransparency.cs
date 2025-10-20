using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Editor;

public class wallTransparency : MonoBehaviour
{
    [SerializeField] List<GameObject> paredes  = new List<GameObject>();
    // Update is called once per frame
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            foreach (GameObject pared in paredes)
            {
                pared.GetComponentInChildren<MeshRenderer>().enabled = false;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            foreach (GameObject pared in paredes)
            {
                pared.GetComponentInChildren<MeshRenderer>().enabled = true;
            }
        }

    }
}
