using UnityEngine;

public class GoalBlock : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("�Q�[���N���A�I");
            FindObjectOfType<GameManager>().GameClear();
        }
    }
}