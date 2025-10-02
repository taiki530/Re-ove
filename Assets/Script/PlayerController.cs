using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpHeight = 2f;
    public float gravity = -20f;
    public float mouseSensitivity = 2f;

    public float airControl = 0.2f;
    public float groundControl = 10f;

    public Transform cameraHolder;
    public Transform playerRoot;

    private CharacterController controller;
    private Vector3 currentVelocity;
    private float pitch = 0f;
    private WallClimber climber;
    private PlayerStatus playerStatus;
    private float verticalVelocity;

    //�A�j���[�V�����p
    private PlayerAnimetorController animetor;
    [SerializeField] private GameObject playerSD;



    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        climber = GetComponent<WallClimber>();
        playerStatus = GetComponent<PlayerStatus>();
        animetor = playerSD.GetComponent<PlayerAnimetorController>();
    }

        void Update()
    {
        if (PauseManager.isPaused) return;

        HandleMouseLook();
        HandleMovement();
        HandleJumpAndGravity();
    }

    void HandleMouseLook()
    {
        // �}�E�X����
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        // �R���g���[���[�E�X�e�B�b�N����
        float stickX = Input.GetAxis("RightStick X");
        float stickY = Input.GetAxis("RightStick Y");

        // �������p�ł���悤�ɍ��Z
        float lookX = (mouseX + stickX) * mouseSensitivity;
        float lookY = (mouseY + stickY) * mouseSensitivity;

        // ��]����
        playerRoot.Rotate(Vector3.up * lookX);

        pitch -= lookY;
        pitch = Mathf.Clamp(pitch, -80f, 80f);
        cameraHolder.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }


    void HandleMovement()
    {
        float h = Input.GetAxis("Horizontal"); // �L�[�{�[�h & ���X�e�B�b�N
        float v = Input.GetAxis("Vertical");

        Vector3 inputDir = (playerRoot.right * h + playerRoot.forward * v).normalized;

        // Shift�L�[ or R�{�^���iFire3�j�Ń_�b�V��
        bool isRunning = (Input.GetKey(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.JoystickButton5)) &&
                         playerStatus.HasEnoughStamina(0.1f);

        float speed = isRunning ? moveSpeed * 1.8f : moveSpeed;

        if (isRunning && inputDir != Vector3.zero)
        {
            playerStatus.UseStamina(playerStatus.staminaConsumptionRate * Time.deltaTime);

            //�A�j���[�V����(����)
            animetor.SetisRun(true);
            animetor.SetisWalk(true);
        }
        else if(inputDir != Vector3.zero) 
        {
            //�A�j���[�V����(����)
            animetor.SetisRun(false);
            animetor.SetisWalk(true);
        }
        else
        { //�ړ��n�A�j���[�V�����t���O��܂�
            animetor.SetisRun(false);
            animetor.SetisWalk(false);
        }

        Vector3 targetVelocity = inputDir * speed;
        float controlFactor = IsGrounded() ? groundControl : airControl;
        currentVelocity = Vector3.Lerp(currentVelocity, targetVelocity, controlFactor * Time.deltaTime);

        //�W�����v���[�V�����p�`�F�b�N(�󒆂ɂ��邩)
        if (IsGrounded())
        {
            animetor.SetisJump(false);
        }
        else 
        {
            animetor.SetisJump(true);
        }
    }

    void HandleJumpAndGravity()
    {
        bool grounded = IsGrounded();
        bool climbing = climber != null && climber.isTouchingIvy;

        if (climbing)
        {
            float climbInput = Input.GetAxis("Vertical");
            Vector3 climbMove = new Vector3(0, climbInput * moveSpeed, 0);
            controller.Move(climbMove * Time.deltaTime);
            verticalVelocity = 0f;
            animetor.SetisNoboru(true); //�o��A�j���[�V����
            return;
        }
        else
        {
            animetor.SetisNoboru(false);
        }

        if (grounded && verticalVelocity < 0)
        {
            verticalVelocity = -2f;
        }

        if (grounded && Input.GetButtonDown("Jump")) // Space�L�[ or B�{�^��
        {
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        verticalVelocity += gravity * Time.deltaTime;

        Vector3 finalMove = currentVelocity * Time.deltaTime;
        finalMove.y = verticalVelocity * Time.deltaTime;

        controller.Move(finalMove);
    }

    bool IsGrounded()
    {
        float rayStartOffset = 0.1f;
        float rayLength = 1.5f;
        return Physics.Raycast(transform.position + Vector3.up * rayStartOffset, Vector3.down, rayLength);
    }

    public void IsThrowing(int sec)
    {
        int count = sec;

        animetor.SetisisThrow(true);

        while(count < 0)
        {
            count--;
        }

        animetor.SetisisThrow(false);
    }
}
