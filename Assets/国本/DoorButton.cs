using UnityEngine;

public class DoorButton : MonoBehaviour
{
    public bool IsPressed { get; private set; } = false;

    // プレイヤーがJキーを押してボタンを押す
    void Update()
    {
        if (!IsPressed && Input.GetKeyDown(KeyCode.J))
        {
            float distance = Vector3.Distance(Camera.main.transform.position, transform.position);
            if (distance < 2f)
            {
                IsPressed = true;
                Debug.Log($"{name} が押された");
                // 見た目を変える処理（例：色やアニメーション）を追加してもよい
            }
        }
    }
}

