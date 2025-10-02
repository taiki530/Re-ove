using UnityEngine;

public class DisappearOnApproach : MonoBehaviour
{
    public Transform player;
    public float triggerDistance = 10.0f;

    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);
        if (distance < triggerDistance)
        {
            Debug.Log("近づいたのでオブジェクトを消します！");
            gameObject.SetActive(false);
        }
    }
}
