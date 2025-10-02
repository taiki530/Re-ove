using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUnderCheck : MonoBehaviour
{
    [SerializeField] GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if (player != null) { 
        
            if(player.transform.position.y <= -20)
            {
                Debug.Log("—Ž‰º–hŽ~ˆ—”­“®");
                player.transform.position = new Vector3(0,20, 0);
            }
        }
    }
}
