using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallClimberVine : MonoBehaviour
{
    public bool isTouchingIvy = false;

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        // ‚Â‚½iIvyj‚ÉG‚ê‚Ä‚¢‚é‚©‚ğ”»’è
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

