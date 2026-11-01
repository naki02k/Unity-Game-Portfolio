using UnityEngine;
using UnityEngine.Pool;

/// <summary>
/// 斧の軌道、速度、ダメージ判定を制御するクラス
/// 放物線を描いて移動し、敵に当たるとダメージを与えて消滅する
/// </summary>
public class AxeController : MonoBehaviour
{
    /// <summary>
    /// 敵に与えるダメージ
    /// </summary>
    [SerializeField]
    private int damage = 10;

    /// <summary>
    /// 斧が描く弧の高さ
    /// </summary>
    [SerializeField]
    private float arcHeight = 2f;

    /// <summary>
    /// 斧の回転速度
    /// </summary>
    [SerializeField]
    private float rotationSpeed = 360f;

    /// <summary>
    /// 斧の移動速度
    /// </summary>
    private float speed;

    /// <summary>
    /// 投擲開始から到着までの時間
    /// </summary>
    private float lifeTime;

    /// <summary>
    /// 投擲開始からの経過時間
    /// </summary>
    private float elapsedTime;

    /// <summary>
    /// 斧の投擲開始位置
    /// </summary>
    private Vector3 startPoint;

    /// <summary>
    /// 斧の投擲先
    /// </summary>
    private Vector3 endPoint;

    /// <summary>
    /// 斧を管理しているオブジェクトプール
    /// </summary>
    private IObjectPool<AxeController> targetPool;

    /// <summary>
    /// 投擲先を設定する
    /// </summary>
    /// <param name="targetPosition">斧が向かう位置</param>
    public void Initialize(
        Vector3 targetPosition,
        float moveSpeed,
        IObjectPool<AxeController> pool)
    {
        if (moveSpeed <= 0f)
        {
            Debug.LogError("Speed must be greater than 0.");
            return;
        }

        startPoint = transform.position;
        endPoint = targetPosition;
        speed = moveSpeed;
        targetPool = pool;
        elapsedTime = 0f;

        lifeTime = Vector3.Distance(startPoint, endPoint) / speed;
    }

    private void Update()
    {
        if (lifeTime <= 0f)
        {
            return;
        }

        elapsedTime += Time.deltaTime;

        // フレーム落ちによる値のオーバーを防ぐ
        float t = Mathf.Clamp01(elapsedTime / lifeTime);

        Vector3 linearPosition =
            Vector3.Lerp(startPoint, endPoint, t);

        // Sin波を利用して放物線軌道を表現
        float height =
            Mathf.Sin(t * Mathf.PI) * arcHeight;

        transform.position = new Vector3(
            linearPosition.x,
            linearPosition.y + height,
            linearPosition.z);

        // 投擲中であることを分かりやすくするため回転
        transform.Rotate(
            0f,
            0f,
            rotationSpeed * Time.deltaTime);

        if (t >= 1f)
        {
            ReleaseToPool();
        }
    }

    /// <summary>
    /// 敵と接触した際にダメージを与える
    /// </summary>
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Enemy"))
        {
            return;
        }

        if (other.TryGetComponent(out Enemy enemy))
        {
            enemy.OnDamage(damage);
        }

        ReleaseToPool();
    }

    /// <summary>
    /// 斧をオブジェクトプールへ戻す
    /// </summary>
    private void ReleaseToPool()
    {
        if (targetPool != null)
        {
            targetPool.Release(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}