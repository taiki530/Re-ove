using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerAnimetorController : MonoBehaviour
{
    private Animator animator;
    bool is_walk = false;
    private float originalSpeed;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        
    }

    // Update is called once per frame
    void Update()
    {

    }
    public bool GetisWalk()
    {
        return is_walk;
    }
    //�������[�V����
    public void SetisWalk(bool Bool)
    {
        is_walk = Bool;
        animator.SetBool("isWalk", Bool);
    }
    //���胂�[�V����
    public void SetisRun(bool Bool)
    {
        animator.SetBool("isRun", Bool);
    }
    //�W�����v���[�V����
    public void SetisJump(bool Bool)
    {
        animator.SetBool("isJump", Bool);
    }
    //�o�胂�[�V����
    public void SetisNoboru(bool Bool)
    {
        animator.SetBool("isNoboru", Bool);
    }
    public void SetisisThrow(bool Bool)
    {
        animator.SetBool("isThrow", Bool);
    }

    public void Stop()
    {
        if (animator.speed != 0)
        {
            originalSpeed = animator.speed;
        }
        animator.speed = 0;

    }
    public void ReStrat()
    {
        animator.speed = originalSpeed;
    }
}
