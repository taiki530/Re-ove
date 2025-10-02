using UnityEngine;

public class SwitchPrefabSpawner : MonoBehaviour
{
    [SerializeField] private GameObject prefabToSpawn; // ��������v���n�u
    [SerializeField] private Transform spawnPoint;     // �����ʒu�w��i���GameObject�Ȃǁj

    private GameObject spawnedObject; // �������ꂽ�I�u�W�F�N�g�̎Q��

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
