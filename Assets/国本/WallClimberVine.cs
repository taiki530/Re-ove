using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallClimberVine : MonoBehaviour
{
    public bool isTouchingIvy = false;

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        // ���iIvy�j�ɐG��Ă��邩�𔻒�
        if (hit.collider.CompareTag("Ivy"))
        {
            isTouchingIvy = true;
        }
        else
        {
            isTouchingIvy = false;
        }
    }
}

