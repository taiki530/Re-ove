using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testplayerMove : MonoBehaviour
{
    [SerializeField] GameObject player;
    Vector3 vector3;
    public static AnimatorController animatorController;
    [SerializeField] GameObject playermodel;
    float playerSpeed = 2.0f;
    // Start is called before the first frame update
    void Start()
    {
        animatorController = playermodel.GetComponent<AnimatorController>();
    }

    // Update is called once per frame
    void Update()
    {
        vector3 = new Vector3(0, 0, 0);
        if (Input.GetKey(KeyCode.W))
        {
            vector3.z = playerSpeed * Time.deltaTime*-1;
        }
        if (Input.GetKey(KeyCode.S)) 
        {
            vector3.z = playerSpeed * Time.deltaTime;
        }

        if(Input.GetKey(KeyCode.D))
        {
            vector3.x = playerSpeed * Time.deltaTime * -1;
        }
        if (Input.GetKey(KeyCode.A))
        {
            vector3.x = playerSpeed * Time.deltaTime;
        }
        //歩行してるかのチェック
        if (vector3.x == 0 && vector3.z == 0) 
        {
            animatorController.SetisWalk(false);
            animatorController.SetisRun(false);
        }
        else 
        {
            animatorController.SetisWalk(true);
            if (Input.GetKey(KeyCode.LeftShift)) 
            {
                vector3.x *= 2;
                vector3.z *= 2;
                animatorController.SetisRun(true);
            }
            else 
            {
                animatorController.SetisRun(false);
            }
        }
        player.transform.position += vector3;
     
    }
}
