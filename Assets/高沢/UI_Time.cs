using System;
using UnityEngine;
using UnityEngine.UI;

public class DeltaTimeClock : MonoBehaviour
{
    [SerializeField]
    private Transform hourHand;

    [SerializeField]
    private Transform minuteHand;

    [SerializeField]
    private Transform secondHand;

    // SystemTimer スクリプトへの参照
    public SystemTimer systemTimer; // インスペクターからドラッグ＆ドロップでアサイン

    private float elapsedTime; // UI_Time自身の経過時間はもう不要になる

    private void Start()
    {
        // SystemTimer がアサインされていることを確認
        //NULLチェック
       
        if (systemTimer == null)
        {
            Debug.LogError("SystemTimer がアサインされていません！");
            // Fallback: もしアサインされていなければ、自身の経過時間で初期化
            DateTime now = DateTime.Now;
            elapsedTime = now.Hour * 3600 + now.Minute * 60 + now.Second + now.Millisecond / 1000f;
        }
        else
        {
            //初期化

            // SystemTimerがアサインされている場合でも、Start時点での初期値を渡す
            // SystemTimerのelapsedTimeがまだ初期化されていない可能性を考慮し、
            // ここではDateTime.Nowをベースにした値を使うのが安全です。
            DateTime now = DateTime.Now;
            elapsedTime = now.Hour * 3600 + now.Minute * 60 + now.Second + now.Millisecond / 1000f;
        }

        // UpdateClockHands() に初期値を渡して呼び出す
        // ここではelapsedTimeを直接渡します。

        UpdateClockHands(elapsedTime);
    }

    private void Update()
    {
        // SystemTimer の elapsedtime を使用
        if (systemTimer != null)
        {
            elapsedTime = systemTimer.ElapsedTime; // SystemTimer の経過時間を取得
        }
        else
        {
            // Fallback: SystemTimer がなければ自身のelapsedTimeを加算
            elapsedTime += Time.deltaTime;
        }


        // 24時間を超えたらリセット（省略可能、SystemTimerのelapsedtimeはリセットされないので注意）
        // UI_Time側で常にSystemTimerの経過時間を使う場合、このリセットは意味がなくなります。
        // もし時計として24時間周期で表示したいなら、elapsedTimeをモジュロ演算で調整します。
        float displayTime = elapsedTime;
        if (displayTime >= 86400f) // 24 * 60 * 60
        {
            displayTime = displayTime % 86400f; // 24時間でループさせる
        }


        UpdateClockHands(displayTime);
    }

    private void UpdateClockHands(float totalSeconds) // 引数で経過時間を受け取るように変更
    {
        // 時・分・秒を再計算
        int hours = (int)(totalSeconds / 3600f);
        int minutes = (int)((totalSeconds % 3600f) / 60f);
        float seconds = totalSeconds % 60f;
    

        // 各針の角度を設定（Z軸回転）
        if (secondHand != null)
        {
            float secAngle = 360f - (seconds / 60f) * 360f;
            secondHand.localEulerAngles = new Vector3(0, 0, secAngle);
        }

        if (minuteHand != null)
        {
            float minAngle = 360f - ((minutes + seconds / 60f) / 60f) * 360f;
            minuteHand.localEulerAngles = new Vector3(0, 0, minAngle);
        }

        if (hourHand != null)
        {
            float hourAngle = 360f - ((hours % 12 + minutes / 60f) / 12f) * 360f;
            hourHand.localEulerAngles = new Vector3(0, 0, hourAngle);

        }
    }
}