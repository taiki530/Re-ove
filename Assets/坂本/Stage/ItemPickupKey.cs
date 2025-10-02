using UnityEngine;

public class ItemPickupKey : MonoBehaviour
{
    [SerializeField] string itemName; // アイテムの名前
    [SerializeField] float maxPickupDistance = 5f; // アイテムを拾える最大距離
    //[SerializeField] GameObject keyImage; // UIに表示するアイテムの画像
    //[SerializeField] Sprite itemIcon; // アイテムのアイコン（UI用）
    [SerializeField] GameObject itemPrefab; // アイテムの3Dモデル
    [SerializeField] AudioClip ItemPickupSound; // 効果音のためのAudioClip

    public Inventory playerInventory; // プレイヤーのインベントリ
    private AudioSource audioSource;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>(); // AudioSourceを追加
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            // カメラの前方にRayを発射
            Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            RaycastHit hit;

            // Rayがオブジェクトに当たった場合、かつ指定の距離内にある場合
            if (Physics.Raycast(ray, out hit, maxPickupDistance))
            {
                if (hit.collider.gameObject == gameObject) // クリックしたオブジェクトがこのアイテムか確認
                {
                    PickupItem();

                    //keyImage.SetActive(true);
                }
            }
        }
    }

    void PickupItem()
    {
        Debug.Log("Picked up: " + itemName);

        // 新しいアイテムを作成し、インベントリに追加
        Inventory.Item newItem = new Inventory.Item
        {
            itemName = itemName,
            //icon = itemIcon,
            itemPrefab = itemPrefab
        };

        playerInventory.AddItem(newItem); // インベントリにアイテムを追加
        audioSource.PlayOneShot(ItemPickupSound); //効果音を再生
        //Destroy(gameObject); // アイテムオブジェクトをシーンから削除


    }
}
