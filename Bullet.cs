using UnityEngine;

/// <summary>
/// ’eŠÛ‚ÌˆÚ“®‚ÆÕ“Ë‚ğ§Œä‚·‚éƒNƒ‰ƒX
/// </summary>
public class Bullet : MonoBehaviour
{
    /// <summary>
    /// ’eŠÛ‚ÌˆÚ“®‘¬“x
    /// </summary>
    [SerializeField]
    private float speed = 20f;

    /// <summary>
    /// ’eŠÛ‚ª©“®“I‚ÉÁ–Å‚·‚é‚Ü‚Å‚ÌŠÔ
    /// </summary>
    [SerializeField]
    private float lifeTime = 5f;

    /// <summary>
    /// ’eŠÛ‚ªˆÚ“®‰Â”\‚Èó‘Ô‚©
    /// </summary>
    private bool canMove;

    /// <summary>
    /// ’eŠÛ‚ªˆÚ“®‚·‚é•ûŒü
    /// </summary>
    private Vector3 direction;

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        if (!canMove)
        {
            return;
        }

        transform.Translate(
            direction * speed * Time.deltaTime,
            Space.World);
    }

    /// <summary>
    /// ’eŠÛ‚ğw’è•ûŒü‚Ö”­Ë‚·‚é
    /// </summary>
    /// <param name="target">’eŠÛ‚ªŒü‚©‚¤‘ÎÛ‚ÌˆÊ’u</param>
    public void Shoot(Vector3 target)
    {
        direction =
            (target - transform.position).normalized;

        canMove = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy") ||
            other.CompareTag("Boss"))
        {
            Destroy(gameObject);
        }
    }
}