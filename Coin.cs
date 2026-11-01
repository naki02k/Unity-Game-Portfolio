using UnityEngine;

/// <summary>
/// コインの回収処理を管理するクラス
/// </summary>
public class Coin : MonoBehaviour
{
    /// <summary>
    /// すでに回収されたか
    /// </summary>
    private bool isCollected;

    private void OnEnable()
    {
        // プールから再利用されたときに初期化
        isCollected = false;
    }

    /// <summary>
    /// プレイヤーがコインに接触したときの処理
    /// </summary>
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isCollected)
        {
            return;
        }

        if (!other.CompareTag("Player"))
        {
            return;
        }

        if (CoinManager.instance == null)
        {
            Debug.LogError(
                "Coin: CoinManagerが見つかりません。"
            );
            return;
        }

        // 二重回収を防止
        isCollected = true;

        // 所持コインを加算
        CoinManager.instance.AddCoin();

        // 累計回収コイン数を加算
        if (ScoreManager.instance!=null)
        {
            ScoreManager.instance.AddCoin();
        }

        // プールへ返却
        CoinManager.instance.ReturnCoin(gameObject);
    }
}