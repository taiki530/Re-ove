using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;


public class FlashEffect : MonoBehaviour
{
    public Image flashImage;

    public IEnumerator DoFlash(Action onWhiteFlash = null, float duration = 1.0f)
    {
        float half = duration / 2f;

        // �t�F�[�h�C���i�����Ȃ��Ă����j
        for (float t = 0; t < half; t += Time.deltaTime)
        {
            float alpha = t / half;
            flashImage.color = new Color(1, 1, 1, alpha);
            yield return null;
        }

        // ���S�ɔ��ɂȂ����u�ԂɃR�[���o�b�N���Ăяo��
        flashImage.color = new Color(1, 1, 1, 1);
        yield return new WaitForSeconds(0.3f); // ������ԃL�[�v
        onWhiteFlash?.Invoke();

        // �t�F�[�h�A�E�g�i���ɖ߂�j
        for (float t = 0; t < half; t += Time.deltaTime)
        {
            float alpha = 1 - (t / half);
            flashImage.color = new Color(1, 1, 1, alpha);
            yield return null;
        }

        flashImage.color = new Color(1, 1, 1, 0);
    }

}
