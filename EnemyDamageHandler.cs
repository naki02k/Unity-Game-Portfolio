using UnityEngine;

/// <summary>
/// 敵のHP管理とダメージ処理を行うクラス
/// </summary>
public class EnemyDamageHandler : MonoBehaviour
{
    /// <summary>
    /// 敵のHP
    /// </summary>
    [SerializeField]
    private int hp = 10;

    /// <summary>
    /// ダメージを受ける
    /// </summary>
    /// <param name="damage">受けるダメージ量</param>
    public void TakeDamage(int damage)
    {
        hp -= damage;

        if (hp <= 0)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// ルーントレーサーとの接触時にダメージを受ける
    /// </summary>
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Runetracer"))
        {
            return;
        }

        if (other.TryGetComponent(
            out RunetracerController runetracer))
        {
            TakeDamage(runetracer.GetDamage());
        }
    }
}