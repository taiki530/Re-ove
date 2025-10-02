using UnityEngine;

[RequireComponent(typeof(GhostReplayer))]
public class GhostAnimatorController : MonoBehaviour
{
    private Animator animator;
    private GhostReplayer replayer; // GhostReplayerへの参照

    // AnimatorのパラメータIDをキャッシュするとパフォーマンスが向上します
    private static readonly int SpeedParam = Animator.StringToHash("Speed");
    private static readonly int IsGroundedParam = Animator.StringToHash("IsGrounded");
    private static readonly int IsJumpParam = Animator.StringToHash("IsJump");

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        replayer = GetComponent<GhostReplayer>(); // GhostReplayerコンポーネントを取得

        if (animator == null)
        {
            Debug.LogError("Animator not found in children. Please ensure your character has an Animator component.");
        }
        if (replayer == null)
        {
            Debug.LogError("GhostReplayer not found. This script requires a GhostReplayer component.");
        }
    }

    void Update()
    {
        // GhostReplayerが再生中ではない、またはreplayerがnullの場合は処理しない
        if (replayer == null || !replayer.IsReplaying) return;

        // GhostReplayerから直接速度と接地状態を取得
        float speed = replayer.CurrentHorizontalSpeed;
        bool isGrounded = replayer.IsGrounded;

        // アニメーションパラメータ設定
        animator.SetFloat(SpeedParam, speed);
        animator.SetBool(IsGroundedParam, isGrounded);
        // ジャンプは接地していない場合のみtrueとします
        animator.SetBool(IsJumpParam, !isGrounded);
    }

    // CheckGrounded() メソッドはGhostReplayerからisGroundedを取得するため不要になりました。
    // そのため、このメソッドは削除します。
    // bool CheckGrounded()
    // {
    //     // このロジックはGhostReplayer.csのCharacterController.isGroundedが担当します。
    //     return Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, 1.5f);
    // }
}