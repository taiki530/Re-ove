using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using static UnityEngine.Rendering.HDROutputUtils;
using UnityEditor.Rendering;


public class LoadingScript : MonoBehaviour
{
    // Start is called before the first frame update

    //非同期動作するAsyncOperation
    private AsyncOperation async;
    //シーンロード中にあらわれるUI
    [SerializeField] private GameObject loadUI;
    [SerializeField] private GameObject Icon;
    private float iconSpeed = 360.0f;
    //読み込み率を表示するスライダー
    //private Slider slider;
    //public Slider progressBar;

    public void NextScene() 
    {
        Icon = Icon.GetComponent<GameObject>();
        //ロード画面をアクティブに
      loadUI.SetActive(true);
        //コルーチンを開始
    }

    IEnumerator LoadData() 
    {
        // GameManagerから次にロードするシーン名を取得
        string sceneToLoad = TitleManager.nextSceneName;

        // シーン名が設定されていない場合はエラーを防ぐため処理を中断
        if (string.IsNullOrEmpty(sceneToLoad))
        {
            Debug.LogError("ロードするシーン名が指定されていません！");
            yield break; // コルーチンを終了
        }

        // シーンの読み込みをする
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneToLoad);
        operation.allowSceneActivation = false;

        while (!operation.isDone)
        {
            Icon.transform.Rotate(0,0,iconSpeed*Time.deltaTime*-1);
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
           // progressBar.value = progress;

            if (progress >= 1f)
            {
                // ここで少し待ったり演出したりできる
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
