using UnityEngine;

// オブジェクトのマテリアルのテクスチャYオフセットを時間と共にアニメーションさせるスクリプト。
// 主にスクロールする背景やエフェクトなどに使用します。
public class MaterialOffsetAnimator : MonoBehaviour
{
    [Header("設定")]
    [SerializeField] private Renderer targetRenderer; // マテリアルを持つRenderer (未設定なら自動検出)
    [SerializeField] private float yOffsetSpeed = 1.0f; // Yオフセットが1秒ごとに変化する量 (例: 1.0fで1秒ごとに-1)
    [SerializeField] private string texturePropertyName = "_BaseMap"; // アニメーションするテクスチャのプロパティ名

    private Material materialInstance; // 変更するマテリアルのインスタンス

    void Start()
    {
        // RendererがInspectorで割り当てられていなければ、このゲームオブジェクトから取得を試みる
        if (targetRenderer == null)
        {
            targetRenderer = GetComponent<Renderer>();
            if (targetRenderer == null)
            {
                Debug.LogError("MaterialOffsetAnimator: このゲームオブジェクトにRendererが見つかりません。スクリプトを無効化します。", this);
                enabled = false; // Rendererが見つからなければスクリプトを無効化
                return;
            }
        }

        // 重要: targetRenderer.material を使用してマテリアルのインスタンスを取得する
        // これにより、プロジェクトアセットのマテリアル自体ではなく、
        // このオブジェクトに割り当てられたマテリアルのコピーを操作するため、
        // 他のオブジェクトに影響を与えずに個別にアニメーションできます。
        materialInstance = targetRenderer.material;

        // 指定されたテクスチャプロパティがマテリアルに存在するか確認
        if (!materialInstance.HasProperty(texturePropertyName))
        {
            Debug.LogError($"MaterialOffsetAnimator: マテリアル '{materialInstance.name}' にテクスチャプロパティ '{texturePropertyName}' が見つかりません。スクリプトを無効化します。", this);
            enabled = false;
            return;
        }
    }

    void Update()
    {
        // マテリアルインスタンスが有効でなければ処理しない
        if (materialInstance == null) return;

        // 現在のYオフセットを取得
        Vector2 currentOffset = materialInstance.GetTextureOffset(texturePropertyName);

        // Yオフセットを時間と共に減少させる (yOffsetSpeedが1.0fなら1秒ごとに-1)
        currentOffset.y -= yOffsetSpeed * Time.deltaTime;

        // 新しいオフセットをマテリアルに設定
        materialInstance.SetTextureOffset(texturePropertyName, currentOffset);
    }

    // スクリプトが無効化されたり、ゲームオブジェクトが破棄されたりした場合に、
    // 動的に作成されたマテリアルインスタンスをクリーンアップします。
    // これにより、メモリリークを防ぎます。
    void OnDisable()
    {
        if (materialInstance != null)
        {
            // プレイモード中であればDestroy、エディタモード中であればDestroyImmediateを使用
            // これにより、エディタ上での変更も正しくリセットされます。
            if (Application.isPlaying)
            {
                Destroy(materialInstance);
            }
            else
            {
                DestroyImmediate(materialInstance);
            }
        }
    }
}