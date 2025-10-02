using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testCameraControl : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private GameObject player;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        _camera.transform.position = player.transform.position + new Vector3(0, 3.5f, 4);    }
}
