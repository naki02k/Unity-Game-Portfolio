using UnityEngine;

/// <summary>
/// 敵の死亡時にコインをドロップするクラス
/// </summary>
public class CoinDropper : MonoBehaviour
{
    [Header("Drop Settings")]

    /// <summary>
    /// ドロップするコイン数の最小値
    /// </summary>
    [SerializeField]
    private int minCoins = 1;

    /// <summary>
    /// ドロップするコイン数の最大値
    /// </summary>
    [SerializeField]
    private int maxCoins = 5;

    /// <summary>
    /// コインをドロップする範囲
    /// </summary>
    [SerializeField]
    private float dropRadius = 1f;

    /// <summary>
    /// 指定位置にコインをドロップする
    /// </summary>
    public void DropCoins(Vector3 dropPosition)
    {
        if (CoinManager.instance == null)
        {
            Debug.LogError(
                "CoinDropper: CoinManagerが見つかりません。"
            );
            return;
        }

        int minimum = Mathf.Max(0, minCoins);
        int maximum = Mathf.Max(minimum, maxCoins);

        int coinCount = Random.Range(
            minimum,
            maximum + 1
        );

        float radius = Mathf.Max(0f, dropRadius);

        for (int i = 0; i < coinCount; i++)
        {
            Vector3 randomOffset = new Vector3(
                Random.Range(-radius, radius),
                Random.Range(-radius, radius),
                0f
            );

            Vector3 finalPosition =
                dropPosition + randomOffset;

            CoinManager.instance.SpawnCoin(finalPosition);
        }
    }
}