using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class soundScript : MonoBehaviour
{
    public AudioClip audioClip;
    public AudioSource audioSource;
    public bool check;
    MeshRenderer mesh;
    // Start is called before the first frame update
    void Start()
    {

        colorAlphaZero();
        audioSource = gameObject.AddComponent<AudioSource>();
        check = true;

    }

    // Update is called once per frame
    void Update()
    {
        if (check)
        {
            Debug.Log("チェック中");
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("通過判定");

        if (check)
        {
            if (audioSource != null)
            {
                audioSource.PlayOneShot(audioClip);
                check = false;
            }
        }
    }

    private void colorAlphaZero()
    {
        mesh = GetComponent<MeshRenderer>();
        mesh.material.color = mesh.material.color = new Color32(0, 0, 0, 0);

    }
}
