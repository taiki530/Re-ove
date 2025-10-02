using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var recorder = other.GetComponent<PlayerRecorder>();
            if (recorder != null)
            {
                FindObjectOfType<TimeLoopManager>().NotifyCheckpointPassed();
                VineTimeManager.Instance.StartTimer();
                Debug.Log("�`�F�b�N�|�C���g�ʉ߁A�^��J�n�I");
            }
        }
    }
}
