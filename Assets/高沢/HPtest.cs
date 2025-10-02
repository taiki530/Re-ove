using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // UI操作に必要

public class PlayerHP : MonoBehaviour
{
    // プレイヤーの最大HPと現在のHP
    public int maxHp = 10;
    private int currentHp;

    // HPバーとして使用するImageコンポーネントへの参照
    public Image HpImage;

    void Start()
    {
        // 初期設定
        currentHp = maxHp; // HPを最大値に設定

        // hpImage.fillAmountは0から1の間の値を取るため、初期値を現在のHPの割合で設定します。
        // これにより、HPバーが正しく満たされた状態で開始します。
        HpImage.fillAmount = (float)currentHp / maxHp;
    }

    private void Update()
    {
        // マウスの左クリック (0番目のボタン) が押されたら
        if (Input.GetMouseButtonDown(0))
        {
            TakeDamage(1); // 1ダメージを与える
        }
    }

    public void TakeDamage(int damage)
    {
        // HPを減らす処理
        currentHp -= damage;
        if (currentHp < 0) currentHp = 0; // HPが0未満にならないようにする

        // HPバーに現在のHPを反映
        // fillAmountは0.0から1.0の範囲で設定する必要があるため、
        // 現在のHPを最大HPで割って割合を計算します。
        HpImage.fillAmount = (float)currentHp / maxHp;

        // HPが0になったときの処理
        if (currentHp == 0)
        {
            Debug.Log("ゲームオーバー！");
            // ここにゲームオーバーの処理を追加できます。
            // 例: Destroy(gameObject); // プレイヤーオブジェクトを削除
            // 例: Time.timeScale = 0f; // ゲームを一時停止
            // 例: SceneManager.LoadScene("GameOverScene"); // ゲームオーバーシーンへ移行
        }
    }

    // 必要に応じてHPを回復するメソッドも追加できます
    public void Heal(int amount)
    {
        currentHp += amount;
        if (currentHp > maxHp) currentHp = maxHp; // 最大HPを超えないようにする

        HpImage.fillAmount = (float)currentHp / maxHp; // HPバーを更新
    }
}