using UnityEngine;

[RequireComponent(typeof(GhostReplayer))]
public class GhostAnimatorController : MonoBehaviour
{
    private Animator animator;
    private GhostReplayer replayer; // GhostReplayer�ւ̎Q��

    // Animator�̃p�����[�^ID���L���b�V������ƃp�t�H�[�}���X�����サ�܂�
    private static readonly int SpeedParam = Animator.StringToHash("Speed");
    private static readonly int IsGroundedParam = Animator.StringToHash("IsGrounded");
    private static readonly int IsJumpParam = Animator.StringToHash("IsJump");

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        replayer = GetComponent<GhostReplayer>(); // GhostReplayer�R���|�[�l���g���擾

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
        // GhostReplayer���Đ����ł͂Ȃ��A�܂���replayer��null�̏ꍇ�͏������Ȃ�
        if (replayer == null || !replayer.IsReplaying) return;

        // GhostReplayer���璼�ڑ��x�Ɛڒn��Ԃ��擾
        float speed = replayer.CurrentHorizontalSpeed;
        bool isGrounded = replayer.IsGrounded;

        // �A�j���[�V�����p�����[�^�ݒ�
        animator.SetFloat(SpeedParam, speed);
        animator.SetBool(IsGroundedParam, isGrounded);
        // �W�����v�͐ڒn���Ă��Ȃ��ꍇ�̂�true�Ƃ��܂�
        animator.SetBool(IsJumpParam, !isGrounded);
    }

    // CheckGrounded() ���\�b�h��GhostReplayer����isGrounded���擾���邽�ߕs�v�ɂȂ�܂����B
    // ���̂��߁A���̃��\�b�h�͍폜���܂��B
    // bool CheckGrounded()
    // {
    //     // ���̃��W�b�N��GhostReplayer.cs��CharacterController.isGrounded���S�����܂��B
    //     return Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, 1.5f);
    // }
}