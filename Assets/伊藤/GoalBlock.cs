using UnityEngine;

public class GoalBlock : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("ゲームクリア！");
            FindObjectOfType<GameManager>().GameClear();
        }
    }
}