using UnityEngine;

public class WallClimber : MonoBehaviour
{
    public bool isTouchingIvy { get; private set; }

    private CharacterController controller;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        // 半径0.5の球を前方に出してIvyに接触しているかチェック
        Vector3 origin = transform.position - Vector3.up * 1.0f; // 胸の高さ
        Vector3 direction = transform.forward;
        float distance = 0.6f;

        Ray ray = new Ray(origin, direction);
        isTouchingIvy = Physics.SphereCast(ray, 0.5f, out RaycastHit hit, distance) && hit.collider.CompareTag("Ivy");
    }
}
