using UnityEngine;

/// <summary>
/// プレイヤーの被ダメージ判定を管理するクラス
/// </summary>
public class PlayerHitBox : MonoBehaviour
{
    /// <summary>
    /// 敵から受けるダメージの最小値
    /// </summary>
    private const int EnemyDamageMin = 1;

    /// <summary>
    /// 敵から受けるダメージの最大値
    /// </summary>
    private const int EnemyDamageMax = 5;

    /// <summary>
    /// ボスから受けるダメージの最小値
    /// </summary>
    private const int BossDamageMin = 3;

    /// <summary>
    /// ボスから受けるダメージの最大値
    /// </summary>
    private const int BossDamageMax = 8;

    /// <summary>
    /// ダメージエリアから受けるダメージ
    /// </summary>
    private const int DamageAreaDamage = 5;

    /// <summary>
    /// プレイヤーのHPを管理するコンポーネント
    /// </summary>
    [SerializeField]
    private PlayerHPBar playerHPBar;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("接触 : " + collision.name);

        if (collision.CompareTag("Enemy") ||
            collision.CompareTag("EnemyAttack"))
        {
            Debug.Log("Enemyに接触");

            playerHPBar.ApplyDamage(
                Random.Range(
                    EnemyDamageMin,
                    EnemyDamageMax + 1));
        }
        else if (collision.CompareTag("Boss") ||
                 collision.CompareTag("BossAttack"))
        {
            playerHPBar.ApplyDamage(
                Random.Range(
                    BossDamageMin,
                    BossDamageMax + 1));
        }
        else if (collision.CompareTag("DamageArea"))
        {
            playerHPBar.ApplyDamage(DamageAreaDamage);
        }
    }
}