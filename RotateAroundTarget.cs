using UnityEngine;

/// <summary>
/// 指定されたターゲットを中心にオブジェクトを円軌道で回転させるクラス
/// 回転中のオブジェクトは敵やボスとの衝突を検知し、ダメージを与える
/// </summary>
public class RotateAroundTarget : MonoBehaviour
{
    /// <summary>
    /// 接触時に与えるダメージの最小値
    /// </summary>
    private const int DamageMin = 1;

    /// <summary>
    /// 接触時に与えるダメージの最大値
    /// </summary>
    private const int DamageMax = 6;

    /// <summary>
    /// 回転の中心となるターゲット
    /// </summary>
    public Transform target;

    /// <summary>
    /// 回転速度
    /// </summary>
    [SerializeField]
    private float rotationSpeed = 30f;

    /// <summary>
    /// 逆回転するか
    /// </summary>
    [SerializeField]
    private bool reverseRotation;

    /// <summary>
    /// 初期角度オフセット
    /// </summary>
    [SerializeField]
    private float phaseOffset;

    /// <summary>
    /// 回転半径
    /// </summary>
    [SerializeField]
    private float radius = 1f;

    /// <summary>
    /// 現在角度
    /// </summary>
    private float angle;

    /// <summary>
    /// 現在角度を取得する
    /// </summary>
    public float Angle => angle;

    private void Update()
    {
        if (target == null)
        {
            return;
        }

        float direction =
            reverseRotation ? 1f : -1f;

        angle +=
            direction *
            rotationSpeed *
            Time.deltaTime;

        UpdatePosition(angle);
    }

    /// <summary>
    /// 指定角度に基づいて位置を更新する
    /// </summary>
    /// <param name="currentAngle">現在の回転角度</param>
    private void UpdatePosition(float currentAngle)
    {
        float x =
            Mathf.Cos(
                Mathf.Deg2Rad *
                (currentAngle + phaseOffset))
            * radius;

        float y =
            Mathf.Sin(
                Mathf.Deg2Rad *
                (currentAngle + phaseOffset))
            * radius;

        transform.position =
            target.position +
            new Vector3(x, y, 0f);
    }

    /// <summary>
    /// 回転角度とオフセットを設定する
    /// </summary>
    /// <param name="angle">基準となる回転角度</param>
    /// <param name="offset">回転角度のオフセット</param>
    public void SetOffset(
        float angle,
        float offset)
    {
        this.angle = angle;
        phaseOffset = offset;

        UpdatePosition(angle);
    }

    /// <summary>
    /// 敵やボスに接触した際の処理
    /// </summary>
    /// <param name="other">接触したコライダー</param>
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Enemy") &&
            !other.CompareTag("Boss"))
        {
            return;
        }

        int damage =
            Random.Range(
                DamageMin,
                DamageMax + 1);

        if (other.TryGetComponent(out Enemy enemy))
        {
            enemy.OnDamage(damage);
        }
    }
}