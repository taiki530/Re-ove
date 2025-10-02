using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // UI操作に必要

//死因
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

    public Image hpImage; // Inspectorで割り当てるImageコンポーネント

    public Image staminaImage;

    void Start()
    {
        currentHP = maxHP;
        currentStamina = maxStamina;


        // nullチェックをし、エラーメッセージを出す
        // nullチェックを追加し、エラーメッセージを出す

        if (hpImage == null)
        {
            Debug.LogError("HP ImageがInspectorで割り当てられていません！PlayerStatusスクリプトがアタッチされているオブジェクトを確認してください。", this);
        }
        else
        {
            hpImage.fillAmount = currentHP / maxHP; // ここはStartで一度更新
        }

        if (staminaImage == null)
        {
            Debug.LogError("staminaImageがInspectorで割り当てられていません！PlayerStatusスクリプトがアタッチされているオブジェクトを確認してください。", this);
        }
        else
        {
            staminaImage.fillAmount = currentStamina / maxStamina; // ここはStartで一度更新
        }

    }

    void Update()
    {
        RecoverStamina();
        //UI_01のブランチのマージが現実的では無かったので変更みながらコメントアウトしてます問題あったら戻して(小澤)

        /*        // UpdateでもHPバーを更新する必要がある場合はここに記述（通常はダメージや回復時に更新）
                // hpImage.fillAmount = currentHP / maxHP; // 例：常にHPバーを最新にしたい場合
                if (Input.GetMouseButtonDown(0))
                {
                    TakeDamageD(10); // 1ダメージを与える
                }
                //Debug.Log($"現在のスタミナ: {currentStamina}");
            }

            //デバッグ用ダメージ処理
            public void TakeDamageD(int damage)
            {
                // HPを減らす処理
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

        // HPバーを更新
        if (hpImage != null) // nullチェック
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

        // HPバーを更新
        if (hpImage != null) // nullチェック
        {
            hpImage.fillAmount = currentHP / maxHP;
        }
    }

    public bool UseStamina(float amount)
    {
        if (currentStamina >= amount)
        {
            currentStamina -= amount;
            // スタミナバー更新
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

            // スタミナバー更新
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
                FindObjectOfType<GameManager>().FoundGameOver(); // デフォルト
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