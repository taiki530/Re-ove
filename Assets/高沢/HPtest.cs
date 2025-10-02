using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // UI����ɕK�v

public class PlayerHP : MonoBehaviour
{
    // �v���C���[�̍ő�HP�ƌ��݂�HP
    public int maxHp = 10;
    private int currentHp;

    // HP�o�[�Ƃ��Ďg�p����Image�R���|�[�l���g�ւ̎Q��
    public Image HpImage;

    void Start()
    {
        // �����ݒ�
        currentHp = maxHp; // HP���ő�l�ɐݒ�

        // hpImage.fillAmount��0����1�̊Ԃ̒l����邽�߁A�����l�����݂�HP�̊����Őݒ肵�܂��B
        // ����ɂ��AHP�o�[���������������ꂽ��ԂŊJ�n���܂��B
        HpImage.fillAmount = (float)currentHp / maxHp;
    }

    private void Update()
    {
        // �}�E�X�̍��N���b�N (0�Ԗڂ̃{�^��) �������ꂽ��
        if (Input.GetMouseButtonDown(0))
        {
            TakeDamage(1); // 1�_���[�W��^����
        }
    }

    public void TakeDamage(int damage)
    {
        // HP�����炷����
        currentHp -= damage;
        if (currentHp < 0) currentHp = 0; // HP��0�����ɂȂ�Ȃ��悤�ɂ���

        // HP�o�[�Ɍ��݂�HP�𔽉f
        // fillAmount��0.0����1.0�͈̔͂Őݒ肷��K�v�����邽�߁A
        // ���݂�HP���ő�HP�Ŋ����Ċ������v�Z���܂��B
        HpImage.fillAmount = (float)currentHp / maxHp;

        // HP��0�ɂȂ����Ƃ��̏���
        if (currentHp == 0)
        {
            Debug.Log("�Q�[���I�[�o�[�I");
            // �����ɃQ�[���I�[�o�[�̏�����ǉ��ł��܂��B
            // ��: Destroy(gameObject); // �v���C���[�I�u�W�F�N�g���폜
            // ��: Time.timeScale = 0f; // �Q�[�����ꎞ��~
            // ��: SceneManager.LoadScene("GameOverScene"); // �Q�[���I�[�o�[�V�[���ֈڍs
        }
    }

    // �K�v�ɉ�����HP���񕜂��郁�\�b�h���ǉ��ł��܂�
    public void Heal(int amount)
    {
        currentHp += amount;
        if (currentHp > maxHp) currentHp = maxHp; // �ő�HP�𒴂��Ȃ��悤�ɂ���

        HpImage.fillAmount = (float)currentHp / maxHp; // HP�o�[���X�V
    }
}