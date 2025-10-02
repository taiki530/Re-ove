using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // UI����ɕK�v

//����
public enum DeathCause
{
    Unknown,
    Grenade,
    Found
}

public class PlayerStatus : MonoBehaviour
{
    [Header("HP Settings")]
    public float maxHP = 100f;
    public float currentHP;

    [Header("Stamina Settings")]
    public float maxStamina = 100f;
    public float currentStamina;
    public float staminaRecoveryRate = 10f;
    public float staminaConsumptionRate = 20f;

    private bool isDead = false;

    public Image hpImage; // Inspector�Ŋ��蓖�Ă�Image�R���|�[�l���g

    public Image staminaImage;

    void Start()
    {
        currentHP = maxHP;
        currentStamina = maxStamina;


        // null�`�F�b�N�����A�G���[���b�Z�[�W���o��
        // null�`�F�b�N��ǉ����A�G���[���b�Z�[�W���o��

        if (hpImage == null)
        {
            Debug.LogError("HP Image��Inspector�Ŋ��蓖�Ă��Ă��܂���IPlayerStatus�X�N���v�g���A�^�b�`����Ă���I�u�W�F�N�g���m�F���Ă��������B", this);
        }
        else
        {
            hpImage.fillAmount = currentHP / maxHP; // ������Start�ň�x�X�V
        }

        if (staminaImage == null)
        {
            Debug.LogError("staminaImage��Inspector�Ŋ��蓖�Ă��Ă��܂���IPlayerStatus�X�N���v�g���A�^�b�`����Ă���I�u�W�F�N�g���m�F���Ă��������B", this);
        }
        else
        {
            staminaImage.fillAmount = currentStamina / maxStamina; // ������Start�ň�x�X�V
        }

    }

    void Update()
    {
        RecoverStamina();
        //UI_01�̃u�����`�̃}�[�W�������I�ł͖��������̂ŕύX�݂Ȃ���R�����g�A�E�g���Ă܂���肠������߂���(���V)

        /*        // Update�ł�HP�o�[���X�V����K�v������ꍇ�͂����ɋL�q�i�ʏ�̓_���[�W��񕜎��ɍX�V�j
                // hpImage.fillAmount = currentHP / maxHP; // ��F���HP�o�[���ŐV�ɂ������ꍇ
                if (Input.GetMouseButtonDown(0))
                {
                    TakeDamageD(10); // 1�_���[�W��^����
                }
                //Debug.Log($"���݂̃X�^�~�i: {currentStamina}");
            }

            //�f�o�b�O�p�_���[�W����
            public void TakeDamageD(int damage)
            {
                // HP�����炷����
                currentHP -= damage;
                if (currentHP < 0) currentHP = 0;

                hpImage.fillAmount = (float)currentHP / maxHP;
        */
    }

    public void TakeDamage(float amount, DeathCause cause = DeathCause.Unknown)
    {
        if (isDead) return;

        currentHP -= amount;
        currentHP = Mathf.Clamp(currentHP, 0, maxHP);

        // HP�o�[���X�V
        if (hpImage != null) // null�`�F�b�N
        {
            hpImage.fillAmount = currentHP / maxHP;
        }

        if (currentHP <= 0)
        {
            Die(cause);
        }
    }

    public void Heal(float amount)
    {
        currentHP += amount;
        currentHP = Mathf.Clamp(currentHP, 0, maxHP);

        // HP�o�[���X�V
        if (hpImage != null) // null�`�F�b�N
        {
            hpImage.fillAmount = currentHP / maxHP;
        }
    }

    public bool UseStamina(float amount)
    {
        if (currentStamina >= amount)
        {
            currentStamina -= amount;
            // �X�^�~�i�o�[�X�V
            if (staminaImage != null)
            {
                staminaImage.fillAmount = currentStamina / maxStamina;
            }
            return true;
        }
        return false;
    }

    private void RecoverStamina()
    {
        if (currentStamina < maxStamina)
        {
            currentStamina += staminaRecoveryRate * Time.deltaTime;
            currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);

            // �X�^�~�i�o�[�X�V
            if (staminaImage != null)
            {
                staminaImage.fillAmount = currentStamina / maxStamina;
            }
        }
    }

    private void Die(DeathCause cause)
    {
        if (isDead) return;
        isDead = true;

        Debug.Log($"Player died due to: {cause}");

        switch (cause)
        {
            case DeathCause.Grenade:
                FindObjectOfType<GameManager>().GrenadeGameOver();
                break;
            case DeathCause.Found:
                FindObjectOfType<GameManager>().FoundGameOver();
                break;
            default:
                FindObjectOfType<GameManager>().FoundGameOver(); // �f�t�H���g
                break;
        }
    }

    public bool IsDead()
    {
        return isDead;
    }

    public bool HasEnoughStamina(float amount)
    {
        return currentStamina >= amount;
    }
}