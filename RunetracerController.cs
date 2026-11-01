using UnityEngine;

/// <summary>
/// ルーントレーサー（武器）の動きと衝突時の挙動を制御するクラス
/// オブジェクトの反射とダメージ処理を行う
/// </summary>
public class RunetracerController : MonoBehaviour
{
    /// <summary>
    /// ルーントレーサーの移動速度
    /// </summary>
    [SerializeField]
    private float speed = 15f;

    /// <summary>
    /// 与えるダメージ量
    /// </summary>
    [SerializeField]
    private int damage = 5;

    /// <summary>
    /// ルーントレーサーのRigidbody2D
    /// </summary>
    private Rigidbody2D rigidBody2D;

    /// <summary>
    /// 衝突直前の速度
    /// </summary>
    private Vector2 lastVelocity;

    private void Start()
    {
        rigidBody2D = GetComponent<Rigidbody2D>();

        if (rigidBody2D == null)
        {
            Debug.LogError(
                $"{name}: Rigidbody2D が見つかりません。",
                this);

            enabled = false;
            return;
        }

        // 初期方向をランダムに決定
        float randomAngle =
            Random.Range(0f, 360f);

        Vector2 direction =
            new Vector2(
                Mathf.Cos(randomAngle * Mathf.Deg2Rad),
                Mathf.Sin(randomAngle * Mathf.Deg2Rad));

        rigidBody2D.velocity =
            direction.normalized * speed;
    }

    private void Update()
    {
        // 衝突する直前の速度を毎フレーム保存しておく
        if (rigidBody2D.velocity.sqrMagnitude > 0.1f)
        {
            lastVelocity = rigidBody2D.velocity;
        }
    }

    /// <summary>
    /// 他のオブジェクトと衝突したときの処理
    /// </summary>
    /// <param name="other">衝突情報</param>
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Wall"))
        {
            // 衝突面の法線を取得
            Vector2 normal =
                other.contacts[0].normal;

            // 衝突直前の速度を使って反射ベクトルを計算
            Vector2 reflectDirection =
                Vector2.Reflect(
                    lastVelocity.normalized,
                    normal);

            // 角スタックを防止するために反射方向を少しずらす
            float angleOffset =
                Random.Range(-10f, 10f) *
                Mathf.Deg2Rad;

            float cos =
                Mathf.Cos(angleOffset);

            float sin =
                Mathf.Sin(angleOffset);

            reflectDirection =
                new Vector2(
                    reflectDirection.x * cos -
                    reflectDirection.y * sin,
                    reflectDirection.x * sin +
                    reflectDirection.y * cos);

            // 速度を再設定
            rigidBody2D.velocity =
                reflectDirection.normalized * speed;
        }

        if (other.gameObject.TryGetComponent(
            out Enemy enemy))
        {
            enemy.OnDamage(damage);
        }

        if (other.gameObject.TryGetComponent(
            out Boss boss))
        {
            boss.TakeDamage(damage);
        }
    }

    /// <summary>
    /// 設定されているダメージ量を取得する
    /// </summary>
    /// <returns>設定されているダメージ量</returns>
    public int GetDamage()
    {
        return damage;
    }
}