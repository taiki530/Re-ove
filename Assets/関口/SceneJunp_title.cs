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
    public Button[] menuButtons; // �{�^���̔z��
    public Sprite normalSprite;
    public Sprite selectedSprite;
    public GameObject[] offButton;
    public GameObject[] onText;
    public GameObject Panel;
    private bool ExitOn = false;
    private int currentIndex = 0; // ���݂̑I���C���f�b�N�X
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
            // ���E�L�[�őI�������ړ�
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
                    // Enter�L�[�őI�����m��
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
                    UnityEditor.EditorApplication.isPlaying = false;//�Q�[���v���C�I��
#else
    Application.Quit();//�Q�[���v���C�I��
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
        // �S�Ẵ{�^���̉摜���X�V
        for (int i = 0; i < menuButtons.Length; i++)
        {
            // Button�R���|�[�l���g������image�v���p�e�B��sprite��ύX����
            if (i == currentIndex)
            {
                // �I������Ă���{�^���̉摜��ύX
                menuButtons[i].image.sprite = selectedSprite;
            }
            else
            {
                // �I������Ă��Ȃ��{�^���̉摜��ʏ펞�̂��̂ɕύX
                menuButtons[i].image.sprite = normalSprite;
            }
        }
    }

    private void SelectStageByIndex(int index)
    {
        // �z��͈͓̔��Ɏ��܂��Ă��邩�`�F�b�N
        if (index >= 0 && index < stageSceneNames.Length)
        {
            // �I�����ꂽ�X�e�[�W����GameManager�ɐݒ�
            TitleManager.nextSceneName = stageSceneNames[index];

            // ���[�h�V�[�������[�h����
            SceneManager.LoadSceneAsync("LoadingScene");
        }
        else
        {
            // �z��͈̔͊O���w�肳�ꂽ�ꍇ�̓G���[���b�Z�[�W���o��
            Debug.LogError($"�w�肳�ꂽ�C���f�b�N�X {index} �͖����ł��BstageSceneNames�z��̐ݒ���m�F���Ă��������B");
        }
    }

}

