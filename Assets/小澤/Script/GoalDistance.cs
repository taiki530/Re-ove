using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
    [SerializeField] GameObject target;
    [SerializeField] GameObject player;
    MeshRenderer mesh;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //ゴール座標
        Vector3 targetPos = target.transform.position;
        //プレイヤー座標
        Vector3 playerPos = player.transform.position;
        //距離チェック
        float dis = Vector3.Distance(targetPos, playerPos);

        Debug.Log(dis);
        if (dis < 0)
        {

        }
        else if (dis < 15)
        {
            mesh= GetComponent<MeshRenderer>();
            dis -= 5;
            float alfa= dis / 10;
            byte[] bytes;
            bytes = BitConverter.GetBytes(alfa);
            byte byte1 = bytes[0];
            

            mesh.material.color = new Color32(0, 0, 0, byte1);

        }

    }
}
