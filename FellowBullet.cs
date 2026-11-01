using UnityEngine;

/// <summary>
/// 味方キャラクターが発射する弾の挙動を管理するクラス
/// </summary>
public class FellowBullet : MonoBehaviour
{
    /// <summary>
    /// 弾の移動速度
    /// </summary>
    [SerializeField]
    private float speed = 20f;

    /// <summary>
    /// 弾が移動可能な状態か
    /// </summary>
    private bool isEnabled;

    /// <summary>
    /// 弾の進行方向
    /// </summary>
    private Vector3 direction;

    private void FixedUpdate()
    {
        if (!isEnabled)
        {
            return;
        }

        transform.Translate(
            direction.normalized *
            speed *
            Time.fixedDeltaTime);
    }

    /// <summary>
    /// 発射方向を設定して移動を開始する
    /// </summary>
    /// <param name="target">ターゲットのワールド座標</param>
    public void Shot(Vector3 target)
    {
        direction =
            target - transform.position;

        isEnabled = true;
    }

    /// <summary>
    /// 敵またはボスとの接触時に弾を消滅させる
    /// </summary>
    /// <param name="other">接触したコライダー</param>
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy") ||
            other.CompareTag("Boss"))
        {
            Destroy(gameObject);
        }
    }
}