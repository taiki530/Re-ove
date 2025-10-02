using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using static UnityEngine.Rendering.HDROutputUtils;
using UnityEditor.Rendering;


public class Stage2LoadingScript : MonoBehaviour
{
    // Start is called before the first frame update

    //�񓯊����삷��AsyncOperation
    private AsyncOperation async;
    //�V�[�����[�h���ɂ������UI
    [SerializeField] private GameObject loadUI;
    [SerializeField] private GameObject Icon;
    private float iconSpeed = 360.0f;
    //�ǂݍ��ݗ���\������X���C�_�[
    //private Slider slider;
    //public Slider progressBar;

    public void NextScene() 
    {
        Icon = Icon.GetComponent<GameObject>();
        //���[�h��ʂ��A�N�e�B�u��
      loadUI.SetActive(true);
        //�R���[�`�����J�n
    }

    IEnumerator LoadData() 
    {
        // �V�[���̓ǂݍ��݂�����
        AsyncOperation operation = SceneManager.LoadSceneAsync("MainScene");
        operation.allowSceneActivation = false;

        while (!operation.isDone)
        {
            Icon.transform.Rotate(0,0,iconSpeed*Time.deltaTime*-1);
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
           // progressBar.value = progress;

            if (progress >= 1f)
            {
                // �����ŏ����҂����艉�o������ł���
                yield return new WaitForSeconds(0.0f);
                operation.allowSceneActivation = true;
            }

            yield return null;
        }
    }

    void Start()
    {
        StartCoroutine(LoadData());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
