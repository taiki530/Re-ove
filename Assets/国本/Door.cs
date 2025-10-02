using UnityEngine;
using System.Collections; // Coroutineのために必要

public class Door : MonoBehaviour
{
    [Tooltip("この扉を開けるために押さないといけないボタン")]
    public DoorButton[] requiredButtons;

    // ドアの回転軸となるTransformを設定してください！★
    [Tooltip("ドアの回転軸となる空のGameObjectのTransformをここに設定します。")]
    public Transform doorPivotTransform;

    [Tooltip("ドアが開く角度（度）")]
    public float openAngle = 90f; // 例: 90度開く
    [Tooltip("ドアが開くのにかかる時間（秒）")]
    public float openDuration = 1.0f; // 例: 1秒かけて開く
    [Tooltip("ドアの開閉方向。ドアのローカルY軸（上方向）を軸に回転する場合の方向（1または-1）")]
    public int rotationDirection = 1; // 1で正方向（例: Y軸+方向）、-1で逆方向（例: Y軸-方向）

    private bool isOpened = false;
    private Coroutine currentDoorAnimationCoroutine; // 現在実行中のドアアニメーションコルーチン

    // ドアピボットの初期ローカル回転を保存 (開閉ロジックの削除に伴い、使用されなくなりますが、他のロジックで必要なら残してください)
    private Quaternion initialPivotLocalRotation;

    void Awake()
    {
        if (doorPivotTransform == null)
        {
            Debug.LogError("Door: doorPivotTransformが設定されていません！ドアのアニメーションができません。", this);
            this.enabled = false; // スクリプトを無効化
            return; // エラーが発生した場合はこれ以上処理を進めない
        }

        // doorPivotTransformの初期ローカル回転を保存 (閉じる処理がないため、この変数は直接は使われませんが、削除はしていません)
        initialPivotLocalRotation = doorPivotTransform.localRotation;
    }

    private bool AllButtonsPressed()
    {
        foreach (var button in requiredButtons)
        {
            if (button == null || !button.IsPressed)
                return false;
        }
        return true;
    }

    public void OpenDoor()
    {
        if (isOpened) return; // 既に開いていたら何もしない

        isOpened = true;
        Debug.Log("扉が開いた！");

        // 既にアニメーションが実行中であれば停止
        if (currentDoorAnimationCoroutine != null)
        {
            StopCoroutine(currentDoorAnimationCoroutine);
        }
        // ドアを開くコルーチンを開始
        currentDoorAnimationCoroutine = StartCoroutine(AnimateDoor(true));
    }

    

    // ドアのアニメーションを制御するコルーチン
    private IEnumerator AnimateDoor(bool open)
    {
        if (doorPivotTransform == null)
        {
            Debug.LogError("Door: doorPivotTransformが設定されていません！アニメーションできません。", this);
            // アニメーションができない場合、即座に非表示にする
            gameObject.SetActive(false);
            yield break;
        }

        // doorPivotTransformのローカル回転を基準にする
        Quaternion startRotation = doorPivotTransform.localRotation;
        Quaternion endRotation;

        // open が true の場合のみ処理（閉じる処理は削除されたため）
        if (open)
        {
            // 開く最終角度を計算 (Y軸を中心に回転)
            // initialPivotLocalRotationからopenAngleだけ回転させる
            endRotation = initialPivotLocalRotation * Quaternion.Euler(0, openAngle * rotationDirection, 0);
        }
        else
        {
            // ここはopenがfalseの場合の処理ですが、CloseDoor()が削除されたため、
            // 論理的にはこのelseブロックには到達しません。
            // もし何らかの理由で到達した場合、デフォルトでドアを閉じる（元の位置に戻す）動作をさせたいなら
            // endRotation = initialPivotLocalRotation;
            // としますが、今回は「閉じる処理を消す」ため、到達しないことを想定します。
            Debug.LogWarning("Door: AnimateDoorコルーチンがopen=falseで呼ばれましたが、閉じる処理は削除されています。");
            yield break; // 念のためコルーチンを終了
        }


        float elapsedTime = 0;
        while (elapsedTime < openDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / openDuration);
            doorPivotTransform.localRotation = Quaternion.Slerp(startRotation, endRotation, t);
            yield return null; // 1フレーム待つ
        }

        doorPivotTransform.localRotation = endRotation; // 最終位置にスナップ

        // アニメーション完了後の表示/非表示制御
        if (open) // 開くアニメーションが完了した場合のみ非表示にする
        {
            //gameObject.SetActive(false); // 完全に開いたら扉の見た目を非表示にする
        }
        // else ブロックはopenがfalseの場合の処理ですが、CloseDoor()が削除されたため到達しません。
    }
}