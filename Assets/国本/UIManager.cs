using UnityEngine;
using UnityEngine.UI; // UI Image���g�p���邽�߂ɕK�v

/// <summary>
/// �Q�[������UI�\�����W���Ǘ�����V���O���g���X�N���v�g�B
/// </summary>
public class UIManager : MonoBehaviour
{
    // �V���O���g���C���X�^���X
    public static UIManager Instance { get; private set; }

    [Header("UI Elements")]
    [SerializeField] private Image interactionPromptImage; // �C���^���N�V�����q���g��\������UI Image
    [SerializeField] private Sprite defaultInteractionSprite; // �f�t�H���g�ŕ\������C���^���N�V�����摜

    void Awake()
    {
        // �V���O���g���̏�����
        if (Instance != null && Instance != this)
        {
            // ���ɃC���X�^���X�����݂���ꍇ�A���̃I�u�W�F�N�g��j�����ďd����h��
            Destroy(gameObject);
        }
        else
        {
            // �C���X�^���X���܂����݂��Ȃ��ꍇ�A���̃I�u�W�F�N�g���V���O���g���C���X�^���X�Ƃ��Đݒ�
            Instance = this;
            // �V�[�����܂����ł��j������Ȃ��悤�ɂ���ꍇ�i�K�v�ɉ����āj
            // DontDestroyOnLoad(gameObject);
        }

        // ������Ԃł�UI�摜���\���ɂ���
        if (interactionPromptImage != null)
        {
            interactionPromptImage.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogWarning("UIManager: Interaction Prompt Image (UI Image) �����蓖�Ă��Ă��܂���B");
        }
    }

    /// <summary>
    /// �C���^���N�V�����q���gUI��\�����܂��B
    /// �I�v�V�����ŕ\������摜��؂�ւ��邱�Ƃ��ł��܂��B
    /// </summary>
    /// <param name="spriteToShow">�\���������摜�iSprite�j�Bnull�̏ꍇ�̓f�t�H���g�摜���g�p�B</param>
    public void ShowInteractionPrompt(Sprite spriteToShow = null)
    {
        if (interactionPromptImage != null)
        {
            // �\������摜���w�肳��Ă���΂�����g�p�A�Ȃ���΃f�t�H���g�摜���g�p
            interactionPromptImage.sprite = (spriteToShow != null) ? spriteToShow : defaultInteractionSprite;

            // �摜�����蓖�Ă��Ă��邱�Ƃ��m�F���Ă���\��
            if (interactionPromptImage.sprite != null)
            {
                interactionPromptImage.gameObject.SetActive(true);
            }
            else
            {
                Debug.LogWarning("UIManager: �\������C���^���N�V�����摜���ݒ肳��Ă��܂���B");
            }
        }
    }

    /// <summary>
    /// �C���^���N�V�����q���gUI���\���ɂ��܂��B
    /// </summary>
    public void HideInteractionPrompt()
    {
        if (interactionPromptImage != null)
        {
            interactionPromptImage.gameObject.SetActive(false);
        }
    }

    // ����A����UI�i��: �|�b�v�A�b�v���b�Z�[�W�A�C���x���g���\���Ȃǁj���Ǘ����邽�߂̃��\�b�h�������ɒǉ��ł��܂��B
}