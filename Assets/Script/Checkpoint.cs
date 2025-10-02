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
                Debug.Log("チェックポイント通過、録画開始！");
            }
        }
    }
}
