using UnityEngine;

// �I�u�W�F�N�g�̃}�e���A���̃e�N�X�`��Y�I�t�Z�b�g�����ԂƋ��ɃA�j���[�V����������X�N���v�g�B
// ��ɃX�N���[������w�i��G�t�F�N�g�ȂǂɎg�p���܂��B
public class MaterialOffsetAnimator : MonoBehaviour
{
    [Header("�ݒ�")]
    [SerializeField] private Renderer targetRenderer; // �}�e���A��������Renderer (���ݒ�Ȃ玩�����o)
    [SerializeField] private float yOffsetSpeed = 1.0f; // Y�I�t�Z�b�g��1�b���Ƃɕω������ (��: 1.0f��1�b���Ƃ�-1)
    [SerializeField] private string texturePropertyName = "_BaseMap"; // �A�j���[�V��������e�N�X�`���̃v���p�e�B��

    private Material materialInstance; // �ύX����}�e���A���̃C���X�^���X

    void Start()
    {
        // Renderer��Inspector�Ŋ��蓖�Ă��Ă��Ȃ���΁A���̃Q�[���I�u�W�F�N�g����擾�����݂�
        if (targetRenderer == null)
        {
            targetRenderer = GetComponent<Renderer>();
            if (targetRenderer == null)
            {
                Debug.LogError("MaterialOffsetAnimator: ���̃Q�[���I�u�W�F�N�g��Renderer��������܂���B�X�N���v�g�𖳌������܂��B", this);
                enabled = false; // Renderer��������Ȃ���΃X�N���v�g�𖳌���
                return;
            }
        }

        // �d�v: targetRenderer.material ���g�p���ă}�e���A���̃C���X�^���X���擾����
        // ����ɂ��A�v���W�F�N�g�A�Z�b�g�̃}�e���A�����̂ł͂Ȃ��A
        // ���̃I�u�W�F�N�g�Ɋ��蓖�Ă�ꂽ�}�e���A���̃R�s�[�𑀍삷�邽�߁A
        // ���̃I�u�W�F�N�g�ɉe����^�����ɌʂɃA�j���[�V�����ł��܂��B
        materialInstance = targetRenderer.material;

        // �w�肳�ꂽ�e�N�X�`���v���p�e�B���}�e���A���ɑ��݂��邩�m�F
        if (!materialInstance.HasProperty(texturePropertyName))
        {
            Debug.LogError($"MaterialOffsetAnimator: �}�e���A�� '{materialInstance.name}' �Ƀe�N�X�`���v���p�e�B '{texturePropertyName}' ��������܂���B�X�N���v�g�𖳌������܂��B", this);
            enabled = false;
            return;
        }
    }

    void Update()
    {
        // �}�e���A���C���X�^���X���L���łȂ���Ώ������Ȃ�
        if (materialInstance == null) return;

        // ���݂�Y�I�t�Z�b�g���擾
        Vector2 currentOffset = materialInstance.GetTextureOffset(texturePropertyName);

        // Y�I�t�Z�b�g�����ԂƋ��Ɍ��������� (yOffsetSpeed��1.0f�Ȃ�1�b���Ƃ�-1)
        currentOffset.y -= yOffsetSpeed * Time.deltaTime;

        // �V�����I�t�Z�b�g���}�e���A���ɐݒ�
        materialInstance.SetTextureOffset(texturePropertyName, currentOffset);
    }

    // �X�N���v�g�����������ꂽ��A�Q�[���I�u�W�F�N�g���j�����ꂽ�肵���ꍇ�ɁA
    // ���I�ɍ쐬���ꂽ�}�e���A���C���X�^���X���N���[���A�b�v���܂��B
    // ����ɂ��A���������[�N��h���܂��B
    void OnDisable()
    {
        if (materialInstance != null)
        {
            // �v���C���[�h���ł����Destroy�A�G�f�B�^���[�h���ł����DestroyImmediate���g�p
            // ����ɂ��A�G�f�B�^��ł̕ύX�����������Z�b�g����܂��B
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