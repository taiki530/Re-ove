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

        // フェードイン（白くなっていく）
        for (float t = 0; t < half; t += Time.deltaTime)
        {
            float alpha = t / half;
            flashImage.color = new Color(1, 1, 1, alpha);
            yield return null;
        }

        // 完全に白になった瞬間にコールバックを呼び出す
        flashImage.color = new Color(1, 1, 1, 1);
        yield return new WaitForSeconds(0.3f); // 白い状態キープ
        onWhiteFlash?.Invoke();

        // フェードアウト（元に戻る）
        for (float t = 0; t < half; t += Time.deltaTime)
        {
            float alpha = 1 - (t / half);
            flashImage.color = new Color(1, 1, 1, alpha);
            yield return null;
        }

        flashImage.color = new Color(1, 1, 1, 0);
    }

}
