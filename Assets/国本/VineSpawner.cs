using UnityEngine;

public class VineSpawner : MonoBehaviour
{
    public GameObject vinePrefab;
    private GameObject spawnedVine;

    public void SpawnVine()
    {
        if (spawnedVine == null && vinePrefab != null)
        {
            spawnedVine = Instantiate(vinePrefab, transform.position, transform.rotation);
        }
    }

    public bool HasVine() => spawnedVine != null;

    public void DestroyVine()
    {
        if (spawnedVine != null)
        {
            Destroy(spawnedVine);
            spawnedVine = null;
        }
    }

    public GameObject SpawnedVine => spawnedVine;
}

