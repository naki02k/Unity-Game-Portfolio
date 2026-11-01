using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// プレイヤーの特殊攻撃によって生成される
/// 斬撃エフェクトの処理を担うクラス
/// </summary>
public class SlashEffect : MonoBehaviour
{
    /// <summary>
    /// ダメージ量
    /// </summary>
    private int damageAmount;

    /// <summary>
    /// 敵レイヤー
    /// </summary>
    private LayerMask enemyLayer;

    /// <summary>
    /// 攻撃範囲
    /// </summary>
    private float attackRadius;

    /// <summary>
    /// 攻撃範囲が未設定の場合に使用する半径
    /// </summary>
    [SerializeField]
    private float defaultAttackRadius = 0.5f;

    /// <summary>
    /// 同じ敵への重複ダメージを防ぐためのリスト
    /// </summary>
    private readonly HashSet<Enemy> damagedEnemies =
        new HashSet<Enemy>();

    /// <summary>
    /// 同じボスへの重複ダメージを防ぐためのリスト
    /// </summary>
    private readonly HashSet<Boss> damagedBosses =
        new HashSet<Boss>();

    /// <summary>
    /// ダメージ情報を初期化する
    /// </summary>
    /// <param name="damage">与えるダメージ量</param>
    /// <param name="layer">攻撃対象のレイヤー</param>
    /// <param name="radius">攻撃範囲</param>
    public void Initialize(
        int damage,
        LayerMask layer,
        float radius)
    {
        if (damage <= 0)
        {
            return;
        }

        damageAmount = damage;
        enemyLayer = layer;

        attackRadius =
            radius > 0f
                ? radius
                : defaultAttackRadius;

        damagedEnemies.Clear();
        damagedBosses.Clear();

        ApplyDamage();
    }

    /// <summary>
    /// 範囲内の敵へダメージを与える
    /// </summary>
    private void ApplyDamage()
    {
        Collider2D[] hitEnemies =
            Physics2D.OverlapCircleAll(
                transform.position,
                attackRadius,
                enemyLayer);

        foreach (Collider2D hitEnemy in hitEnemies)
        {
            if (hitEnemy == null)
            {
                continue;
            }

            Boss boss =
                hitEnemy.GetComponentInParent<Boss>();

            if (boss != null)
            {
                if (damagedBosses.Add(boss))
                {
                    boss.TakeDamage(damageAmount);
                }

                continue;
            }

            Enemy enemy =
                hitEnemy.GetComponentInParent<Enemy>();

            if (enemy != null &&
                damagedEnemies.Add(enemy))
            {
                enemy.OnDamage(damageAmount);
            }
        }
    }

    /// <summary>
    /// 攻撃範囲を可視化する
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        float displayRadius =
            attackRadius > 0f
                ? attackRadius
                : defaultAttackRadius;

        Gizmos.DrawWireSphere(
            transform.position,
            displayRadius);
    }
}