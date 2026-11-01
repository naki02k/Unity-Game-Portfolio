using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// ゲーム内のコインとオブジェクトプールを管理するクラス
/// </summary>
public class CoinManager : MonoBehaviour
{
    public static CoinManager instance { get; private set; }

    [Header("Coin Settings")]

    /// <summary>
    /// コインのPrefab
    /// </summary>
    [SerializeField]
    private GameObject coinPrefab;

    /// <summary>
    /// 最初に生成するコイン数
    /// </summary>
    [SerializeField]
    private int initialPoolSize = 30;

    [Header("UI Settings")]

    /// <summary>
    /// コイン数を表示するテキスト
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI coinText;

    /// <summary>
    /// 現在の所持コイン数
    /// </summary>
    private int coinCount;

    /// <summary>
    /// コインのプール
    /// </summary>
    private Queue<GameObject> coinPool =
        new Queue<GameObject>();

    /// <summary>
    /// 現在プールに入っているコイン
    /// </summary>
    private HashSet<GameObject> pooledCoins =
        new HashSet<GameObject>();

    /// <summary>
    /// 現在出現しているコイン
    /// </summary>
    private HashSet<GameObject> activeCoins =
        new HashSet<GameObject>();

    public int CoinCount => coinCount;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        InitializePool();
    }

    private void Start()
    {
        UpdateCoinUI();
    }

    /// <summary>
    /// コインプールを初期化する
    /// </summary>
    private void InitializePool()
    {
        if (coinPrefab == null)
        {
            Debug.LogError(
                "CoinManager: Coin Prefabが設定されていません。"
            );
            return;
        }

        int poolSize = Mathf.Max(0, initialPoolSize);

        for (int i = 0; i < poolSize; i++)
        {
            CreateCoin();
        }

        Debug.Log(
            $"[CoinManager] コインプール初期化完了：{poolSize}体"
        );
    }

    /// <summary>
    /// コインを1つ生成してプールへ格納する
    /// </summary>
    private void CreateCoin()
    {
        GameObject coin = Instantiate(
            coinPrefab,
            transform
        );

        coin.SetActive(false);

        coinPool.Enqueue(coin);
        pooledCoins.Add(coin);
    }

    /// <summary>
    /// コインを生成位置に出現させる
    /// </summary>
    public void SpawnCoin(Vector3 position)
    {
        if (coinPrefab == null)
        {
            Debug.LogError(
                "CoinManager: Coin Prefabが設定されていません。"
            );
            return;
        }

        GameObject coin = GetPooledCoin();

        if (coin == null)
        {
            return;
        }

        coin.transform.SetParent(null);
        coin.transform.position = position;
        coin.transform.rotation = Quaternion.identity;

        activeCoins.Add(coin);

        coin.SetActive(true);
    }

    /// <summary>
    /// プールからコインを取得する
    /// </summary>
    private GameObject GetPooledCoin()
    {
        while (coinPool.Count > 0)
        {
            GameObject coin = coinPool.Dequeue();

            if (coin == null)
            {
                continue;
            }

            pooledCoins.Remove(coin);

            if (coin.activeSelf)
            {
                continue;
            }

            return coin;
        }

        // プールが空なら追加生成
        Debug.Log(
            "[CoinManager] プールが空のためコインを追加生成します。"
        );

        GameObject newCoin = Instantiate(
            coinPrefab,
            transform
        );

        newCoin.SetActive(false);

        return newCoin;
    }

    /// <summary>
    /// コインをプールへ戻す
    /// </summary>
    public void ReturnCoin(GameObject coin)
    {
        if (coin == null)
        {
            return;
        }

        // 二重返却を防止
        if (!activeCoins.Remove(coin))
        {
            return;
        }

        coin.SetActive(false);
        coin.transform.SetParent(transform);

        if (pooledCoins.Add(coin))
        {
            coinPool.Enqueue(coin);
        }
    }

    /// <summary>
    /// 出現中のコインをすべてプールへ戻す
    /// </summary>
    public void ClearAllCoins()
    {
        List<GameObject> coins =
            new List<GameObject>(activeCoins);

        foreach (GameObject coin in coins)
        {
            ReturnCoin(coin);
        }

        Debug.Log(
            $"[CoinManager] コイン返却完了：{coins.Count}枚"
        );
    }

    /// <summary>
    /// コインを1枚追加する
    /// </summary>
    public void AddCoin()
    {
        coinCount++;

        UpdateCoinUI();
    }

    /// <summary>
    /// 指定した枚数のコインを消費する
    /// </summary>
    public bool SpendCoins(int amount)
    {
        if (amount <= 0 || coinCount < amount)
        {
            return false;
        }

        coinCount -= amount;

        UpdateCoinUI();

        return true;
    }

    /// <summary>
    /// コイン数の表示を更新する
    /// </summary>
    private void UpdateCoinUI()
    {
        if (coinText != null)
        {
            coinText.text = $"Coins: {coinCount}";
        }
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }
}