using UnityEngine;

public class RotateY : MonoBehaviour
{
    public float rotationSpeed = 60f; // ��]���x�i�x/�b�j

    void Update()
    {
        transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f);
    }
}
