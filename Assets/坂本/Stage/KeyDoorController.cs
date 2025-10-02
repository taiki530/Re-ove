using UnityEngine;

public class KeyDoorController : MonoBehaviour
{
    public Inventory inventory; // インベントリの参照
    public string keyName; // このドアで必要なキーの名前
    public float openAngle = 90f; // ドアが開く角度
    public float openSpeed = 2f; // ドアが開く速度
    public AudioClip openDoorSound; // ドアが開くときの効果音
    public AudioClip useKeySound; // 鍵をつかうときの効果音
    public AudioClip notKeyDoorSound; // 鍵を持っていなかった場合の効果音

    private Quaternion closedRotation;
    private Quaternion openRotation;
    private bool isOpen = false;
    private AudioSource audioSource;

    private bool isOpening = false; // ドアが開いているかを管理
    private float delayTimer = 0f; // ディレイ用のタイマー
    private float delayDuration = 1f; // ディレイ時間

    void Start()
    {
        closedRotation = transform.rotation;
        openRotation = Quaternion.Euler(transform.eulerAngles + new Vector3(0, openAngle, 0));
        audioSource = gameObject.AddComponent<AudioSource>(); // AudioSourceを追加
    }

    void Update()
    {
        // ドアの回転をスムーズに行う
        transform.rotation = Quaternion.Lerp(transform.rotation, isOpen ? openRotation : closedRotation, Time.deltaTime * openSpeed);

        if (Input.GetKeyDown(KeyCode.F))
        {
            TryOpenDoor();
        }

        // ドアが開くまでのディレイを待機
        if (isOpening)
        {
            delayTimer += Time.deltaTime;
            if (delayTimer >= delayDuration)
            {
                ToggleDoor();
                audioSource.PlayOneShot(openDoorSound); // ドアが開くときに効果音を再生
                isOpening = false; // ドア開け処理完了
            }
        }
    }

    void TryOpenDoor()
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.gameObject == gameObject)
            {
                // キーを使用してドアを開ける
                if (inventory.UseKey(keyName))
                {
                    audioSource.PlayOneShot(useKeySound); // 鍵を使ったときに効果音を再生
                    delayTimer = 0f; // タイマーをリセット
                    isOpening = true; // ドア開け開始
                }
                else
                {
                    Debug.Log("キーが不足しています。");
                    audioSource.PlayOneShot(notKeyDoorSound); // 鍵がなかった時の効果音を再生
                }
            }
        }
    }

    void ToggleDoor()
    {
        isOpen = !isOpen;
    }
}
