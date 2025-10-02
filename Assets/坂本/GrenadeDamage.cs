using UnityEngine;

public class GrenadeDamage : MonoBehaviour
{
    public float damage = 20f;

    void OnCollisionEnter(Collision collision)
    {
        // �S�[�X�g�ɓ��������炻�̃S�[�X�g�����|�[�Y
        GhostReplayer ghost = collision.collider.GetComponent<GhostReplayer>();

        // �v���C���[�Ƀ_���[�W��^����
        PlayerStatus player = FindObjectOfType<PlayerStatus>();

        if (ghost != null && !ghost.GetGrenadePause())
        {
            ghost.GrenadePause(3.0f);
            if (player != null)
            {
                player.TakeDamage(damage, DeathCause.Grenade);
            }
            Debug.Log("�O���l�[�h���v���C���[�ɖ������A�_���[�W��^����");
        }

        // �O���l�[�h���폜
        Destroy(gameObject);
    }
}