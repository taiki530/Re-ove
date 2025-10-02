using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSightChecker : MonoBehaviour
{
    [SerializeField] private float _sightAngle = 90.0f;
    [SerializeField] private float _maxDistance = 10.0f;

    [SerializeField] private float damage = 100f;

    private GameObject target;
    private PlayerStatus player;

    // TimeLoopManagerの参照 (フラッシュエフェクトを呼び出すために必要)
    private TimeLoopManager timeLoopManager;

    [Header("Flash Audio Settings")]
    [SerializeField] private AudioSource sightFlashAudioSource; // このオブジェクトのAudioSource
    [SerializeField] private AudioClip sightFlashSoundClip;   // 再生したいサウンドクリップ

    // === ★追加：フラッシュの待ち時間（FlashEffectのdurationと合わせる）★ ===
    // FlashEffectのflashDurationの半分くらいが良いでしょう。
    // 例: FlashEffectのflashDurationが0.2fなら、0.1fを設定
    [SerializeField] private float waitBeforeReloadDuration = 0.1f;
    // ==============================================================

    private bool hasGivenDamage = false;

    private void Start()
    {
        target = GameObject.Find("PlayerRoot");
        player = FindObjectOfType<PlayerStatus>();
        timeLoopManager = TimeLoopManager.Instance;

        if (timeLoopManager == null)
        {
            Debug.LogError("TimeLoopManager.Instance が見つかりません！シーンにTimeLoopManagerがありません。");
        }

        if (sightFlashAudioSource == null)
        {
            sightFlashAudioSource = GetComponent<AudioSource>();
            if (sightFlashAudioSource == null)
            {
                sightFlashAudioSource = gameObject.AddComponent<AudioSource>();
            }
            sightFlashAudioSource.playOnAwake = false;
        }
    }

    private void Update()
    {
        if (IsVisible())
        {
            if (!hasGivenDamage)
            {
                if (player != null)
                {
                    player.TakeDamage(damage, DeathCause.Found);

                    // ★コルーチンを呼び出して、フラッシュ表示とシーンリロードを同期させる★
                    StartCoroutine(HandleDiscoveryAndReload());
                }
                Debug.Log("GAME OVER");
                hasGivenDamage = true;
            }
        }
        else
        {
            hasGivenDamage = false;
        }
    }

    // === ★新しく追加するコルーチン★ ===
    private IEnumerator HandleDiscoveryAndReload()
    {
        // フラッシュエフェクトの呼び出し
        if (timeLoopManager != null && timeLoopManager.flashEffect != null)
        {
            // FlashEffectのDoFlashコルーチンを開始
            // FlashEffect.csを修正しないため、ここではDoFlashの完了を待たない。
            // DoFlashが内部でどのように終了するかはPlayerSightCheckerからは関与しない。
            StartCoroutine(timeLoopManager.flashEffect.DoFlash());
        }
        else
        {
            Debug.LogWarning("FlashEffectがTimeLoopManagerに設定されていないか、TimeLoopManagerが見つかりません。");
        }

        // サウンドを再生
        if (sightFlashAudioSource != null && sightFlashSoundClip != null)
        {
            sightFlashAudioSource.PlayOneShot(sightFlashSoundClip);
        }
        else
        {
            if (sightFlashAudioSource == null) Debug.LogWarning("PlayerSightChecker: sightFlashAudioSource が設定されていません。");
            if (sightFlashSoundClip == null) Debug.LogWarning("PlayerSightChecker: sightFlashSoundClip が設定されていません.");
        }

        Debug.Log("プレイヤーが発見されました！フラッシュが表示されるまで待機します。");

        // ★ここでフラッシュが画面を覆うまでの時間待機する★
        // waitBeforeReloadDuration の値は、FlashEffectのフェードイン時間に合わせて調整してください。
        yield return new WaitForSeconds(waitBeforeReloadDuration);

        // 待機後、ステージリスタートのロジックを実行
        Debug.Log("フラッシュ後、ステージをリスタートします。");
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }
    // ===================================

    public bool IsVisible()
    {
        if (this.gameObject.CompareTag("TimeLoopPlayer") && target != null)
        {
            Vector3 selfPos = transform.position;
            Vector3 targetPos = target.transform.position;

            Vector3 selfDir = transform.forward;
            Vector3 targetDir = targetPos - selfPos;
            float targetDistance = targetDir.magnitude;

            float cosHalf = Mathf.Cos(_sightAngle * 0.5f * Mathf.Deg2Rad);
            float innerProduct = Vector3.Dot(selfDir, targetDir.normalized);

            if (innerProduct > cosHalf && targetDistance <= _maxDistance)
            {
                if (Physics.Raycast(selfPos, targetDir.normalized, out RaycastHit hit, _maxDistance))
                {
                    if (hit.collider.gameObject == target)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }
}