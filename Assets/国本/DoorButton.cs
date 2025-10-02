using UnityEngine;

public class DoorButton : MonoBehaviour
{
    public bool IsPressed { get; private set; } = false;

    // �v���C���[��J�L�[�������ă{�^��������
    void Update()
    {
        if (!IsPressed && Input.GetKeyDown(KeyCode.J))
        {
            float distance = Vector3.Distance(Camera.main.transform.position, transform.position);
            if (distance < 2f)
            {
                IsPressed = true;
                Debug.Log($"{name} �������ꂽ");
                // �����ڂ�ς��鏈���i��F�F��A�j���[�V�����j��ǉ����Ă��悢
            }
        }
    }
}

