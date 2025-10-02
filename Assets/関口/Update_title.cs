using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class Update_title : MonoBehaviour
{
    public Button titleText;
    private Image buttonImage;
    public float textMoveSpeed = 1.0f;
    private bool isFadingOut = true;

    // Start is called before the first frame update
    void Start()
    {
        buttonImage = titleText.GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        if (buttonImage != null)
        {
            Color currentColor = buttonImage.color;

            if (isFadingOut)
            {
                currentColor.a -= textMoveSpeed * Time.deltaTime;
                if (currentColor.a <= 0)
                {
                    currentColor.a = 0;
                    isFadingOut = false; // �t�F�[�h�C���ɐ؂�ւ�
                }
            }
            else
            {
                currentColor.a += textMoveSpeed * Time.deltaTime;
                if (currentColor.a >= 1)
                {
                    currentColor.a = 1;
                    isFadingOut = true; // �t�F�[�h�A�E�g�ɐ؂�ւ�
                }
            }

            //�F�X�V
            buttonImage.color = currentColor;
        }
    }

    //�X�^�[�g���ɓ_�ł�����������֐�
    public void start() 
    {
        Debug.Log("�����֐����s");
        textMoveSpeed = 8;
    }

    public void Delete_PushTo()
    {
        textMoveSpeed = 0;
    }
}
