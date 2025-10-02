using UnityEngine;
using UnityEngine.UI; // UI Imageを使用するために必要

/// <summary>
/// ゲーム内のUI表示を集中管理するシングルトンスクリプト。
/// </summary>
public class UIManager : MonoBehaviour
{
    // シングルトンインスタンス
    public static UIManager Instance { get; private set; }

    [Header("UI Elements")]
    [SerializeField] private Image interactionPromptImage; // インタラクションヒントを表示するUI Image
    [SerializeField] private Sprite defaultInteractionSprite; // デフォルトで表示するインタラクション画像

    void Awake()
    {
        // シングルトンの初期化
        if (Instance != null && Instance != this)
        {
            // 既にインスタンスが存在する場合、このオブジェクトを破棄して重複を防ぐ
            Destroy(gameObject);
        }
        else
        {
            // インスタンスがまだ存在しない場合、このオブジェクトをシングルトンインスタンスとして設定
            Instance = this;
            // シーンをまたいでも破棄されないようにする場合（必要に応じて）
            // DontDestroyOnLoad(gameObject);
        }

        // 初期状態ではUI画像を非表示にする
        if (interactionPromptImage != null)
        {
            interactionPromptImage.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogWarning("UIManager: Interaction Prompt Image (UI Image) が割り当てられていません。");
        }
    }

    /// <summary>
    /// インタラクションヒントUIを表示します。
    /// オプションで表示する画像を切り替えることができます。
    /// </summary>
    /// <param name="spriteToShow">表示したい画像（Sprite）。nullの場合はデフォルト画像を使用。</param>
    public void ShowInteractionPrompt(Sprite spriteToShow = null)
    {
        if (interactionPromptImage != null)
        {
            // 表示する画像が指定されていればそれを使用、なければデフォルト画像を使用
            interactionPromptImage.sprite = (spriteToShow != null) ? spriteToShow : defaultInteractionSprite;

            // 画像が割り当てられていることを確認してから表示
            if (interactionPromptImage.sprite != null)
            {
                interactionPromptImage.gameObject.SetActive(true);
            }
            else
            {
                Debug.LogWarning("UIManager: 表示するインタラクション画像が設定されていません。");
            }
        }
    }

    /// <summary>
    /// インタラクションヒントUIを非表示にします。
    /// </summary>
    public void HideInteractionPrompt()
    {
        if (interactionPromptImage != null)
        {
            interactionPromptImage.gameObject.SetActive(false);
        }
    }

    // 今後、他のUI（例: ポップアップメッセージ、インベントリ表示など）を管理するためのメソッドをここに追加できます。
}