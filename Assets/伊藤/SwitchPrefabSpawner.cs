using UnityEngine;

public class SwitchPrefabSpawner : MonoBehaviour
{
    [SerializeField] private GameObject prefabToSpawn; // 生成するプレハブ
    [SerializeField] private Transform spawnPoint;     // 生成位置指定（空のGameObjectなど）

    private GameObject spawnedObject; // 生成されたオブジェクトの参照

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (spawnedObject == null)
            {
                spawnedObject = Instantiate(prefabToSpawn, spawnPoint.position, spawnPoint.rotation);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (spawnedObject != null)
            {
                Destroy(spawnedObject);
                spawnedObject = null;
            }
        }
    }
}
