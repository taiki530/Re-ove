using System.Collections;
using System.Collections.Generic;
using UnityEditor.XR;
using UnityEngine;

public class AnimatorController : MonoBehaviour
{
    private Animator animator;
    bool is_walk = false;

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
    public void SetisWalk(bool Bool) 
    {
        is_walk = Bool;
        animator.SetBool("isWalk",Bool);
    }
    public void SetisRun(bool Bool) 
    {
        animator.SetBool("isRun", Bool);
    }
    public void SetisJump(bool Bool)
    {
        animator.SetBool("isJump", Bool);
    }
}
