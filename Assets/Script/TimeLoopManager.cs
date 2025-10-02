using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Linq;

public class TimeLoopManager : MonoBehaviour
{
    public static TimeLoopManager Instance { get; private set; }

    [Header("References")]
    [SerializeField] private Volume postProcessVolume; // �|�X�g�v���Z�X�{�����[��
    public GameObject ghostPrefab; // �S�[�X�g�̃v���n�u
    public PlayerRecorder playerRecorder; // �v���C���[�̍s�����L�^����R���|�[�l���g
    public Transform player; // �v���C���[��Transform
    public FlashEffect flashEffect; // �t���b�V���G�t�F�N�g
    public float loopTime = 15000.0f; // �^�C�����[�v�̎��ԁi���ݎg�p����Ă��Ȃ��\��������܂����ێ��j

    [Header("Audio Settings")]
    [SerializeField] private AudioSource loopAudioSource; // ���[�v�T�E���h�p��AudioSource
    [SerializeField] private AudioClip monochromeLoopClip; // ���m�N����Ԏ��̃��[�v�T�E���h�N���b�v

    private List<List<PlayerRecorder.FrameData>> allRecordings = new(); // �S�Ă̘^��f�[�^��ۑ����郊�X�g
    private List<GameObject> spawnedGhosts = new(); // �������ꂽ�S�[�X�g�̃Q�[���I�u�W�F�N�g���X�g
    public Vector3 checkpointPosition; // �`�F�b�N�|�C���g�̈ʒu
    private Quaternion checkpointRotation; // �`�F�b�N�|�C���g�̉�]
    private bool isPaused = false; // ���݁A�^�C�����[�v���ꎞ��~�i���m�N���j��Ԃ��ǂ���
    private bool isReadyToRecord = false; // �V�����^�C�����[�v�̘^�揀�����ł��Ă��邩�i�`�F�b�N�|�C���g�ʉߌ�j

    // �^�C�����[�v���A�N�e�B�u���ǂ����������t���O
    public bool isTimeLoopActive { get; private set; } = false;

    // ���݂̃^�C�����[�v���J�n������Ύ���
    private float currentLoopStartTime;

    // ���݂̃^�C�����[�v���J�n������Ύ������擾���郁�\�b�h (���ԋL�^�͕s�v�ɂȂ�܂������A�݊����̂��ߎc���܂�)
    public float GetCurrentLoopStartTime() => currentLoopStartTime;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // DontDestroyOnLoad(gameObject); // �K�v�ł���΃V�[�����ׂ��ňێ�
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        // Q�L�[ (�܂���JoystickButton3) �������ꂽ�����m�F
        if (Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.JoystickButton3))
        {
            // �������݃��m�N����ԁiisPaused = true�j�Ȃ�A��������
            if (isPaused)
            {
                ResumeTimeLoop(); // ���m�N�������ƃ^�C�����[�v�ĊJ
            }
            // ���m�N����Ԃł͂Ȃ��iisPaused = false�j�Ȃ�A���m�N���������݂�
            else
            {
                if (isReadyToRecord && playerRecorder.HasRecording())
                {
                    StartCoroutine(HandleTimeLoop()); // ���m�N�����ƃ��v���C�J�n
                }
                else if (!isReadyToRecord)
                {
                    Debug.Log("�^�揀�����ł��Ă��܂���F�`�F�b�N�|�C���g��ʉ߂��Ă��������B");
                }
                else // playerRecorder.HasRecording() �� false �̏ꍇ
                {
                    Debug.Log("�^��ł��܂���F�^��f�[�^������܂���B");
                }
            }
        }

        // �f�o�b�O�p: ����̃L�[�ŃS�[�X�g������
        if (Input.GetKeyDown(KeyCode.K)) // ��Ƃ���K�L�[���g�p
        {
            Debug.Log("[TimeLoopManager] �f�o�b�O: K�L�[��������܂����B�S�ẴS�[�X�g���������܂��B");
            DestroyAllGhosts(); // �S�ẴS�[�X�g��j�󂷂郁�\�b�h���Ăяo��
        }
    }

    // �^�C�����[�v���ꎞ��~��Ԃ���������A���m�N�����ʂ𖳌��ɂ���B
    void ResumeTimeLoop()
    {
        SetMonochrome(false); // ���m�N�����ʂ�����
        isPaused = false;     // ��Ԃ�ʏ�ɖ߂�
        isTimeLoopActive = false; // �^�C�����[�v��A�N�e�B�u�ɐݒ�
        RestartTimeLoop();    // ���Ԃ̐i�s���ĊJ
    }

    // �S�[�X�g�𐶐�����i���݂�ReplayAllGhosts����Ă΂��j
    void SpawnGhost()
    {
        GameObject ghost = Instantiate(ghostPrefab);
        ghost.transform.position = Vector3.zero; // �����ʒu�����Z�b�g
    }

    // �`�F�b�N�|�C���g��ʉ߂����Ƃ��ɌĂ΂�鏈���B
    // �v���C���[�̘^����J�n���A�^�C�����[�v�̏����𐮂���B
    public void NotifyCheckpointPassed()
    {
        playerRecorder.StartRecordingFromCheckpoint(); // �`�F�b�N�|�C���g����̘^����J�n
        isReadyToRecord = true; // �^�C�����[�v�̘^�揀������

        // ���݂̃v���C���[�̈ʒu�Ɖ�]���`�F�b�N�|�C���g�Ƃ��ĕۑ�
        Transform playerRoot = playerRecorder.transform;
        checkpointPosition = playerRoot.position;
        checkpointRotation = playerRoot.rotation;

        Debug.Log("�`�F�b�N�|�C���g�ʉ� �� �^�揀������");
    }

    // �S�Ă̘^��f�[�^�Ɋ�Â��ăS�[�X�g�����v���C����B
    void ReplayAllGhosts()
    {
        float sharedStartTime = Time.time; // �S�[�X�g�������Ƀ��v���C���J�n���邽�߂̋��L�J�n����

        foreach (var recording in allRecordings)
        {
            SpawnGhost(recording, sharedStartTime); // �e�^��f�[�^�ŃS�[�X�g�𐶐�
        }
    }

    // ����̘^��f�[�^�ƊJ�n���ԂɊ�Â��ăS�[�X�g�𐶐����A���v���C���J�n����B
    void SpawnGhost(List<PlayerRecorder.FrameData> frames, float sharedStartTime)
    {
        if (frames == null || frames.Count == 0) return;

        GameObject ghost = Instantiate(ghostPrefab); // �S�[�X�g�𐶐�
        var replayer = ghost.GetComponent<GhostReplayer>(); // GhostReplayer�R���|�[�l���g���擾

        if (replayer != null)
        {
            replayer.PlayRecording(frames, sharedStartTime); // �^����Đ�
            spawnedGhosts.Add(ghost); // �������ꂽ�S�[�X�g�̃��X�g�ɒǉ�
        }
        else
        {
            Debug.LogError("GhostReplayer �R���|�[�l���g��������܂���I"); // �G���[���O
            Destroy(ghost); // �R���|�[�l���g���Ȃ���΃S�[�X�g��j��
        }
    }

    // �V�[�����̑S�ẴS�[�X�g��j�󂷂�B (�A�N�Z�X�C���q��public�ɕύX)
    public void DestroyAllGhosts()
    {
        foreach (GameObject ghost in spawnedGhosts)
        {
            if (ghost != null)
                Destroy(ghost); // �S�[�X�g��j��
        }
        spawnedGhosts.Clear(); // ���X�g���N���A
        Debug.Log("[TimeLoopManager] �S�ẴS�[�X�g���j�󂳂�܂����B"); // �ǉ�
    }

    // �^�C�����[�v���ĊJ����i���Ԃ�A���̐����Ȃǁj�B
    public void RestartTimeLoop()
    {
        // VineTimeManager �����݂��邩�m�F���A�^�C�}�[���J�n
        if (VineTimeManager.Instance != null)
        {
            VineTimeManager.Instance.StartTimer();
        }
        else
        {
            Debug.LogWarning("VineTimeManager.Instance ��������܂���B");
        }

        // �V�[�����̑S�Ă�Seedling�i�c�؁j�����Z�b�g���A�������X�P�W���[��
        foreach (var seedling in FindObjectsOfType<Seedling>())
        {
            seedling.ResetVines();
            // VineTimeManager.Instance �����݂���ꍇ�̂݃X�P�W���[�������݂�
            if (VineTimeManager.Instance != null)
            {
                seedling.ScheduleAutoGrow(VineTimeManager.Instance.GetStartTime());
            }
        }
        // �����ɃQ�[���v�f�̃��Z�b�g������ǉ�
        ResetGameElementsForLoop();
    }

    // �^�C�����[�v�̃��C�������i�R���[�`���j�B
    // �v���C���[�̘^��A�S�[�X�g�̃��v���C�A�v���C���[�ʒu�̃��Z�b�g�A���m�N�����Ȃǂ��s���B
    IEnumerator HandleTimeLoop()
    {
        isReadyToRecord = false; // �^�C�����[�v�N�����͐V�����^�揀�����ꎞ��~
        isTimeLoopActive = true; // �^�C�����[�v�A�N�e�B�u�ɐݒ�
        currentLoopStartTime = Time.time; // ���݂̃��[�v�J�n�������L�^
        Debug.Log($"[TimeLoopManager] HandleTimeLoop ���J�n����܂����BcurrentLoopStartTime: {currentLoopStartTime:F2}"); // ���ǉ����O��

        // ���ǉ��_�� �^�C�����[�v�J�n���ɃC���x���g���̏����̐��ʂ�ۑ�
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.SaveInitialTorchQuantity();
        }

        var frames = playerRecorder.GetFramesSinceCheckpoint(); // �`�F�b�N�|�C���g����̃t���[���f�[�^���擾
        if (frames.Count > 0)
        {
            // �^�C���X�^���v���ŏ��̃t���[������̑��Ύ��Ԃɂ���
            float firstTime = frames[0].timeStamp;
            for (int i = 0; i < frames.Count; i++)
                frames[i].timeStamp -= firstTime;

            allRecordings.Add(frames); // �^��f�[�^�����X�g�ɒǉ�
            DestroyAllGhosts();        // �����̃S�[�X�g��S�Ĕj��
            ReplayAllGhosts();         // �S�[�X�g�̃��v���C���J�n

            // �t���b�V���G�t�F�N�g���Đ����A������҂B
            // �t���b�V��������ɁA�v���C���[���Z�b�g�⃂�m�N�������s���B
            yield return flashEffect.DoFlash(() =>
            {
                // �v���C���[���`�F�b�N�|�C���g�ɖ߂�
                Transform root = playerRecorder.transform;
                var controller = root.GetComponent<CharacterController>();
                if (controller != null) controller.enabled = false; // CharacterController���ꎞ�����ɂ��Ĉʒu�ݒ�

                Rigidbody rb = root.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.velocity = Vector3.zero;     // ���x�����Z�b�g
                    rb.angularVelocity = Vector3.zero; // �p���x�����Z�b�g
                    rb.position = checkpointPosition; // �ʒu���`�F�b�N�|�C���g�ɐݒ�
                    rb.rotation = checkpointRotation; // ��]���`�F�b�N�|�C���g�ɐݒ�
                }
                else
                {
                    root.position = checkpointPosition; // Rigidbody���Ȃ��ꍇ��Transform�𒼐ڐݒ�
                    root.rotation = checkpointRotation;
                }

                if (controller != null) controller.enabled = true; // CharacterController���ēx�L���ɂ���

                // �ꎞ��~��Ԃɂ���: ���m�N�����ʓK�p + �v���C���[���͖�����
                SetMonochrome(true); // ���m�N�����ʂ�L�������A�v���C���[���͂𖳌���
                isPaused = true;     // �^�C�����[�v���ꎞ��~��Ԃɐݒ�
            });

            // HandleTimeLoop���������A���m�N����Ԃ̏ꍇ�iQ�ŉ����ҋ@�j�́A
            // isReadyToRecord�͂����ł�true�ɖ߂��܂���B���m�N���������ɍs���܂��B
        }
        else
        {
            // �^��f�[�^���Ȃ��̂ɃR���[�`�����Ă΂�Ă��܂����ꍇ�̃��J�o��
            // (Update()�ł̃`�F�b�N�����邽�ߒʏ�͔������ɂ����ł����O�̂���)
            Debug.Log("�^��f�[�^���Ȃ����߃^�C�����[�v�͎��s����܂���ł����B�^�揀����Ԃɖ߂��܂��B");
            isReadyToRecord = true; // �^�揀����Ԃɖ߂�
            isTimeLoopActive = false; // �^�C�����[�v��A�N�e�B�u�ɐݒ�
        }
    }

    // �V�[���̃��m�N�����ʂ�؂�ւ��A�S�[�X�g��v���C���[�̓���𐧌䂷��B
    // �܂��A���m�N����Ԏ��̃��[�v�T�E���h���Đ�/��~����B
    void SetMonochrome(bool enabled)
    {
        // postProcessVolume���ݒ肳��Ă��邩�m�F
        if (postProcessVolume == null)
        {
            Debug.LogWarning("postProcessVolume ���ݒ肳��Ă��܂���I");
            return;
        }

        // ColorAdjustments�R���|�[�l���g���擾���A�ʓx��ݒ�
        // URP�ł́AColor Grading�R���|�[�l���g����Saturation�ɉe�����܂��B
        if (postProcessVolume.profile.TryGet<ColorAdjustments>(out var colorAdjustments))
        {
            colorAdjustments.saturation.value = enabled ? -100f : 0f; // enabled��true�Ȃ烂�m�N���Afalse�Ȃ�ʏ�
        }

        // �S�Ă�GhostReplayer�̈ꎞ��~/�ĊJ�𐧌�
        foreach (var replayer in FindObjectsOfType<GhostReplayer>())
        {
            if (enabled) replayer.Pause();
            else replayer.Resume();
        }

        // �v���C���[�̓��͂��ꎞ��~/�ĊJ
        var playerController = player.GetComponent<PlayerController>();
        if (playerController != null) playerController.enabled = !enabled;

        // ���m�N�����[�v�T�E���h�̍Đ�/��~
        if (loopAudioSource != null && monochromeLoopClip != null)
        {
            if (enabled) // ���m�N�����L���ȏꍇ
            {
                if (!loopAudioSource.isPlaying) // ���łɍĐ����łȂ��ꍇ�̂ݍĐ�
                {
                    loopAudioSource.clip = monochromeLoopClip;
                    loopAudioSource.loop = true; // ���[�v�Đ����m���ɂ���
                    loopAudioSource.Play();
                }
            }
            else // ���m�N���������ȏꍇ
            {
                if (loopAudioSource.isPlaying) // �Đ����̏ꍇ�̂ݒ�~
                {
                    loopAudioSource.Stop();
                }
            }
        }
        else
        {
            Debug.LogWarning("���[�v�p�I�[�f�B�I�\�[�X�܂��̓��m�N�����[�v�p�I�[�f�B�I�N���b�v���ݒ肳��Ă��܂���B");
        }

        // ���m�N�����������ꂽ�Ƃ��ɁA�V�����^��̏�����������
        if (!enabled)
        {
            isReadyToRecord = true;
        }
    }

    // �Q�[���v�f�����Z�b�g����v���C�x�[�g���\�b�h
    private void ResetGameElementsForLoop()
    {
        Debug.Log("[TimeLoopManager] �Q�[���v�f���^�C�����[�v�̂��߂Ƀ��Z�b�g���܂��B");

        // �V�[�����̑S�Ă�PressurePlate�������ă��Z�b�g
        PressurePlate[] allPressurePlates = FindObjectsOfType<PressurePlate>();
        foreach (var plate in allPressurePlates)
        {
            if (plate != null)
            {
                Debug.Log($"[TimeLoopManager] PressurePlate: {plate.name} �� ResetPlate() ���Ăяo���܂��B");
                plate.ResetPlate();
            }
        }

        // �V�[�����̑S�Ă�BrazierSpawner�������ă��Z�b�g
        BrazierSpawner[] allBrazierSpawners = FindObjectsOfType<BrazierSpawner>();
        foreach (var spawner in allBrazierSpawners)
        {
            if (spawner != null)
            {
                // BrazierSpawner �̋L�^���X�V���郁�\�b�h���Ăяo��
                Debug.Log($"[TimeLoopManager] BrazierSpawner: {spawner.name} �� ResetSpawner() ���Ăяo���܂��B");
                spawner.ResetSpawner(); // ���ꂪ Brazier �̔j���� wasLitByPlayerInPreviousLoop �̍X�V���s��
            }
        }

        // BrazierManager�̃��X�g���N���[���A�b�v�iBrazierSpawner��Brazier���j������邽�߁j
        if (BrazierManager.Instance != null)
        {
            BrazierManager.Instance.braziers.RemoveAll(b => b == null);
            Debug.Log("[BrazierManager] BrazierManager�̃��X�g���N���[���A�b�v���܂����B");
        }

        // ���C���_�� �C���x���g���S�̂̃��Z�b�g�ł͂Ȃ��A�����̐��ʂ𕜌�
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.RestoreTorchesToInitialQuantity();
        }
    }

    // �Q�[���S�̂̋L�^���N���A����i�Ⴆ�΁A�Q�[�����ŏ������蒼���ꍇ�Ȃǁj
    public void ClearAllRecords()
    {
        Debug.Log("[TimeLoopManager] �S�Ă̋L�^���N���A���܂��B");

        // �S�Ă�BrazierSpawner�̋L�^���N���A
        BrazierSpawner[] allBrazierSpawners = FindObjectsOfType<BrazierSpawner>();
        foreach (var spawner in allBrazierSpawners)
        {
            if (spawner != null)
            {
                spawner.ClearRecord();
            }
        }

        // �S�Ă�PressurePlate�̋L�^���N���A
        PressurePlate[] allPressurePlates = FindObjectsOfType<PressurePlate>();
        foreach (var plate in allPressurePlates)
        {
            if (plate != null)
            {
                plate.ClearRecord();
            }
        }

        // ���C���_�� �C���x���g���S�̂̃��Z�b�g�ł͂Ȃ��A�����̐��ʂ𕜌�
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.SaveInitialTorchQuantity(); // �������ʂ�0�Ƃ��ĕۑ�
            InventoryManager.Instance.RestoreTorchesToInitialQuantity(); // 0�ɕ���
        }
    }
}