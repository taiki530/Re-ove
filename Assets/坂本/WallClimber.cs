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
        // ���a0.5�̋���O���ɏo����Ivy�ɐڐG���Ă��邩�`�F�b�N
        Vector3 origin = transform.position - Vector3.up * 1.0f; // ���̍���
        Vector3 direction = transform.forward;
        float distance = 0.6f;

        Ray ray = new Ray(origin, direction);
        isTouchingIvy = Physics.SphereCast(ray, 0.5f, out RaycastHit hit, distance) && hit.collider.CompareTag("Ivy");
    }
}
