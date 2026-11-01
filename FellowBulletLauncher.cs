using UnityEngine;

/// <summary>
/// 味方キャラクターが敵に向けて弾を発射するクラス
/// </summary>
public class FellowBulletLauncher : MonoBehaviour
{
    /// <summary>
    /// 発射する弾のプレハブ
    /// </summary>
    [SerializeField]
    private FellowBullet fellowBulletPrefab;

    /// <summary>
    /// 弾の発射位置
    /// </summary>
    [SerializeField]
    private Transform muzzlePosition;

    /// <summary>
    /// ターゲット検索用コンポーネント
    /// </summary>
    private EnemyScanner enemyScanner;

    private void Start()
    {
        enemyScanner = GetComponent<EnemyScanner>();

        IntervalTime timer =
            GetComponent<IntervalTime>();

        if (timer != null)
        {
            timer.enabled = true;
        }
    }

    /// <summary>
    /// 最も近い敵へ向けて弾を発射する
    /// </summary>
    public void FireBullet()
    {
        if (enemyScanner == null)
        {
            return;
        }

        if (fellowBulletPrefab == null ||
            muzzlePosition == null)
        {
            return;
        }

        GameObject target =
            enemyScanner.ScanWithFindTag();

        if (target == null)
        {
            return;
        }

        FellowBullet fellowBullet =
            Instantiate(
                fellowBulletPrefab,
                muzzlePosition.position,
                Quaternion.identity);

        fellowBullet.Shot(target.transform.position);
    }
}