using UnityEngine;

public class KeyDoorController : MonoBehaviour
{
    public Inventory inventory; // �C���x���g���̎Q��
    public string keyName; // ���̃h�A�ŕK�v�ȃL�[�̖��O
    public float openAngle = 90f; // �h�A���J���p�x
    public float openSpeed = 2f; // �h�A���J�����x
    public AudioClip openDoorSound; // �h�A���J���Ƃ��̌��ʉ�
    public AudioClip useKeySound; // ���������Ƃ��̌��ʉ�
    public AudioClip notKeyDoorSound; // ���������Ă��Ȃ������ꍇ�̌��ʉ�

    private Quaternion closedRotation;
    private Quaternion openRotation;
    private bool isOpen = false;
    private AudioSource audioSource;

    private bool isOpening = false; // �h�A���J���Ă��邩���Ǘ�
    private float delayTimer = 0f; // �f�B���C�p�̃^�C�}�[
    private float delayDuration = 1f; // �f�B���C����

    void Start()
    {
        closedRotation = transform.rotation;
        openRotation = Quaternion.Euler(transform.eulerAngles + new Vector3(0, openAngle, 0));
        audioSource = gameObject.AddComponent<AudioSource>(); // AudioSource��ǉ�
    }

    void Update()
    {
        // �h�A�̉�]���X���[�Y�ɍs��
        transform.rotation = Quaternion.Lerp(transform.rotation, isOpen ? openRotation : closedRotation, Time.deltaTime * openSpeed);

        if (Input.GetKeyDown(KeyCode.F))
        {
            TryOpenDoor();
        }

        // �h�A���J���܂ł̃f�B���C��ҋ@
        if (isOpening)
        {
            delayTimer += Time.deltaTime;
            if (delayTimer >= delayDuration)
            {
                ToggleDoor();
                audioSource.PlayOneShot(openDoorSound); // �h�A���J���Ƃ��Ɍ��ʉ����Đ�
                isOpening = false; // �h�A�J����������
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
                // �L�[���g�p���ăh�A���J����
                if (inventory.UseKey(keyName))
                {
                    audioSource.PlayOneShot(useKeySound); // �����g�����Ƃ��Ɍ��ʉ����Đ�
                    delayTimer = 0f; // �^�C�}�[�����Z�b�g
                    isOpening = true; // �h�A�J���J�n
                }
                else
                {
                    Debug.Log("�L�[���s�����Ă��܂��B");
                    audioSource.PlayOneShot(notKeyDoorSound); // �����Ȃ��������̌��ʉ����Đ�
                }
            }
        }
    }

    void ToggleDoor()
    {
        isOpen = !isOpen;
    }
}
