using System;
using UnityEngine;

public class LimitTrigger : MonoBehaviour
{
    [SerializeField] private GameObject limitPrefab;
    private GameObject limit;

    private void OnTriggerEnter(Collider other)
    {
        if (limitPrefab != null && other.CompareTag("Player")){
            Debug.Log("Si");
            limit = Instantiate(limitPrefab, transform.position, transform.rotation);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Destroy(limit);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
