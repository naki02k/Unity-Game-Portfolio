using UnityEngine;

/// <summary>
/// 敵を検知して弾を発射するクラス
/// </summary>
public class BulletLauncher : MonoBehaviour
{
    /// <summary>
    /// 発射する弾丸のプレハブ
    /// </summary>
    [SerializeField]
    private Bullet bulletPrefab;

    /// <summary>
    /// 弾丸の発射位置
    /// </summary>
    [SerializeField]
    private Transform muzzlePosition;

    /// <summary>
    /// 敵を検知するコンポーネント
    /// </summary>
    private EnemyScanner enemyScanner;

    private void Start()
    {
        enemyScanner = GetComponent<EnemyScanner>();

        if (enemyScanner == null)
        {
            Debug.LogError("EnemyScanner not found.", this);
        }
    }

    /// <summary>
    /// 最も近い敵へ弾を発射する
    /// </summary>
    public void Fire()
    {
        if (enemyScanner == null)
        {
            return;
        }

        if (bulletPrefab == null ||
            muzzlePosition == null)
        {
            Debug.LogWarning(
                "BulletPrefab または MuzzlePosition が設定されていません。",
                this);
            return;
        }

        GameObject target = enemyScanner.ScanWithFindTag();

        if (target == null)
        {
            return;
        }

        Bullet bullet = Instantiate(
            bulletPrefab,
            muzzlePosition.position,
            Quaternion.identity);

        bullet.Shoot(target.transform.position);
    }
}