using UnityEngine;

[RequireComponent(typeof(GhostReplayer))]
public class koredeyattaraiikannji: MonoBehaviour
{
    private Animator animator;
    private GhostReplayer replayer;

    private Vector3 previousPosition;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        replayer = GetComponent<GhostReplayer>();
        previousPosition = transform.position;
    }

    void Update()
    {
        if (!replayer.IsReplaying) return;

        Vector3 velocity = (transform.position - previousPosition) / Time.deltaTime;
        float speed = new Vector3(velocity.x, 0f, velocity.z).magnitude;
        bool isGrounded = CheckGrounded();

        // アニメーションフラグ設定
        animator.SetBool("isWalk", speed > 0.1f && speed <= 3.0f);
        animator.SetBool("isRun", speed > 3.0f);
        animator.SetBool("isJump", !isGrounded);

        previousPosition = transform.position;
    }

    bool CheckGrounded()
    {
        return Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, 1.5f);
    }
}
