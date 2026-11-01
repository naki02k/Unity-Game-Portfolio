using UnityEngine;

/// <summary>
/// 敵が弾を生成し、プレイヤーへ発射するクラス
/// </summary>
public class EnemyBulletLauncher : MonoBehaviour
{
    /// <summary>
    /// 発射する弾のプレハブ
    /// </summary>
    [SerializeField]
    private EnemyBullet enemyBulletPrefab;

    /// <summary>
    /// 弾の発射位置
    /// </summary>
    [SerializeField]
    private Transform muzzlePosition;

    /// <summary>
    /// プレイヤーを探索するコンポーネント
    /// </summary>
    private PlayerScanner playerScanner;

    private void Start()
    {
        playerScanner = GetComponent<PlayerScanner>();

        if (playerScanner == null)
        {
            Debug.LogError("PlayerScanner が見つかりません。", this);
        }
    }

    /// <summary>
    /// プレイヤーに向けて弾を発射する
    /// </summary>
    public void FireBullet()
    {
        if (WaveManager.Instance != null &&
            WaveManager.Instance.CurrentWaveCount < 5)
        {
            return;
        }

        if (playerScanner == null)
        {
            return;
        }

        if (enemyBulletPrefab == null ||
            muzzlePosition == null)
        {
            Debug.LogWarning(
                "EnemyBulletPrefab または MuzzlePosition が設定されていません。",
                this);
            return;
        }

        GameObject target =
            playerScanner.ScanPlayer();

        if (target == null)
        {
            return;
        }

        EnemyBullet bullet = Instantiate(
            enemyBulletPrefab,
            muzzlePosition.position,
            Quaternion.identity);

        bullet.Shot(target.transform.position);
    }
}