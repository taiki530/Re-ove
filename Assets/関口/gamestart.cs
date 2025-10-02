using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class gamestart : MonoBehaviour
{
    Title title;
    public Button scenejump;  
    // Start is called before the first frame update
    void Start()
    {
        title = scenejump.GetComponent<Title>();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
