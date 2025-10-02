using UnityEngine;

public class RotateY : MonoBehaviour
{
    public float rotationSpeed = 60f; // 回転速度（度/秒）

    void Update()
    {
        transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f);
    }
}
