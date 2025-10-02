using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class result : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("スタートキー検知");
            Invoke(nameof(jump_Scene), 2.0f);
        }

    }

    public void jump_Scene()
    {
        SceneManager.LoadSceneAsync("title");
    }

}
