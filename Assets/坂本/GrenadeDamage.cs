using UnityEngine;

public class GrenadeDamage : MonoBehaviour
{
    public float damage = 20f;

    void OnCollisionEnter(Collision collision)
    {
        // ゴーストに当たったらそのゴーストだけポーズ
        GhostReplayer ghost = collision.collider.GetComponent<GhostReplayer>();

        // プレイヤーにダメージを与える
        PlayerStatus player = FindObjectOfType<PlayerStatus>();

        if (ghost != null && !ghost.GetGrenadePause())
        {
            ghost.GrenadePause(3.0f);
            if (player != null)
            {
                player.TakeDamage(damage, DeathCause.Grenade);
            }
            Debug.Log("グレネードがプレイヤーに命中し、ダメージを与えた");
        }

        // グレネードを削除
        Destroy(gameObject);
    }
}