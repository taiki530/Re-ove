using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Title : MonoBehaviour
{

    public GameObject updateTitle;
    public Button[] menuButtons; // ボタンの配列
    public Sprite normalSprite;
    public Sprite selectedSprite;
    public GameObject[] offButton;
    public GameObject[] onText;
    public GameObject Panel;
    private bool ExitOn = false;
    private int currentIndex = 0; // 現在の選択インデックス
    public string[] stageSceneNames;

    // Start is called before the first frame update
    void Start()
    {
        UpdateButtonSelection();
        for (int i = 0; i < 3; i++)
        {
            onText[i].SetActive(false);
        }
        Panel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (!ExitOn)
        {
            // 左右キーで選択肢を移動
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                currentIndex = (currentIndex + 1 + menuButtons.Length) % menuButtons.Length;
                UpdateButtonSelection();
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                currentIndex = (currentIndex - 1 + menuButtons.Length) % menuButtons.Length;
                UpdateButtonSelection();
            }

            if (Input.GetKeyDown(KeyCode.Return))
            {
                if (currentIndex == 0)
                {
                    // Enterキーで選択を確定
                    Debug.Log("gameStart1");
                    menuButtons[currentIndex].onClick.Invoke();
                    SelectStageByIndex(0);
                }
                if (currentIndex == 1)
                {
                    Debug.Log("gameStart2");
                    menuButtons[currentIndex].onClick.Invoke();
                    SelectStageByIndex(1);

                }
                if (currentIndex == 2)
                {
                    Debug.Log("gameExit");
#if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;//ゲームプレイ終了
#else
    Application.Quit();//ゲームプレイ終了
#endif
                }
                if (currentIndex == 3)
                {
                    ExitOn = true;
                    Panel.SetActive(true);
                    for (int i = 0; i < 3; i++)
                    {
                        onText[i].SetActive(true);
                    }
                    for (currentIndex = 0; currentIndex < 4; currentIndex++)
                    {
                        offButton[currentIndex].SetActive(false);
                    }

                }
            }
        }
        else if (ExitOn)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                Panel.SetActive(false);
                for (int i = 0; i < 3; i++)
                {
                    onText[i].SetActive(false);
                }
                for (currentIndex = 0; currentIndex < 4; currentIndex++)
                {
                    offButton[currentIndex].SetActive(true);
                }
                ExitOn = false;
            }
        }
    }
    void UpdateButtonSelection()
    {
        // 全てのボタンの画像を更新
        for (int i = 0; i < menuButtons.Length; i++)
        {
            // Buttonコンポーネントが持つimageプロパティのspriteを変更する
            if (i == currentIndex)
            {
                // 選択されているボタンの画像を変更
                menuButtons[i].image.sprite = selectedSprite;
            }
            else
            {
                // 選択されていないボタンの画像を通常時のものに変更
                menuButtons[i].image.sprite = normalSprite;
            }
        }
    }

    private void SelectStageByIndex(int index)
    {
        // 配列の範囲内に収まっているかチェック
        if (index >= 0 && index < stageSceneNames.Length)
        {
            // 選択されたステージ名をGameManagerに設定
            TitleManager.nextSceneName = stageSceneNames[index];

            // ロードシーンをロードする
            SceneManager.LoadSceneAsync("LoadingScene");
        }
        else
        {
            // 配列の範囲外が指定された場合はエラーメッセージを出す
            Debug.LogError($"指定されたインデックス {index} は無効です。stageSceneNames配列の設定を確認してください。");
        }
    }

}

