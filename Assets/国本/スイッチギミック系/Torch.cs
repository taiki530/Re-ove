using UnityEngine;

// 松明がシーンに存在するときの振る舞いを定義
public class Torch : MonoBehaviour
{
    // ★追加箇所★ 火のエフェクトを生成する位置のTransform
    [Tooltip("火のエフェクトを生成する位置のTransformをここに設定します。")]
    public Transform flameSpawnPoint;

    // 火のエフェクトなど、松明が燃えている状態を示すGameObject
    public GameObject flameEffectPrefab; // ★修正箇所★ Prefabに名前を変更

    // ★追加箇所★ 生成されたエフェクトのインスタンスを保持
    private GameObject currentFlameEffect;

    public bool IsLit { get; private set; } = false; // 火が点いているか

    // 初期化時、火は消えている状態にする
    void Awake()
    {
        // ★修正箇所★ Awake()での初期化ロジックを変更
        if (flameEffectPrefab != null && flameSpawnPoint != null)
        {
            // flameSpawnPointの位置にエフェクトを生成し、子として設定
            Vector3 targetDirection = Vector3.up; // 上方向に向ける例
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
            if (flameSpawnPoint != null)
            {
                currentFlameEffect = Instantiate(flameEffectPrefab, flameSpawnPoint.position, targetRotation, flameSpawnPoint);
            }
            else
            {
                currentFlameEffect = Instantiate(flameEffectPrefab, transform.position, targetRotation, transform);
            }
            //currentFlameEffect = Instantiate(flameEffectPrefab, flameSpawnPoint.position, Quaternion.identity, flameSpawnPoint);
            currentFlameEffect.SetActive(true); // 初期状態は非表示
        }
        else
        {
            Debug.LogWarning("Torch: 火のエフェクトのPrefabまたは生成ポイントが設定されていません。", this);
        }
    }

    // 松明に火を点けるメソッド
    public void LightTorch()
    {
        if (!IsLit)
        {
            IsLit = true;
            if (currentFlameEffect != null)
            {
                currentFlameEffect.SetActive(true); // 火のエフェクトを有効化
            }
            Debug.Log($"{name} に火が点きました！");
        }
    }

    // 松明の火を消すメソッド（リセット用）
    public void ExtinguishTorch()
    {
        if (IsLit)
        {
            IsLit = false;
            if (currentFlameEffect != null)
            {
                currentFlameEffect.SetActive(false); // 火のエフェクトを無効化
            }
            Debug.Log($"{name} の火が消えました。");
        }
    }

    // タイムループのリセットに対応するためのインターフェース（オプション）
    // SeedlingのResetVinesのような仕組みに合わせる場合
    public void ResetState()
    {
        ExtinguishTorch();
    }
}

