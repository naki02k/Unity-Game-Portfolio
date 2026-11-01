using UnityEngine;

/// <summary>
/// ゲームのスコアと統計情報を管理するクラス
/// </summary>
public class ScoreManager : MonoBehaviour
{
    /// <summary>
    /// シングルトンインスタンス
    /// </summary>
    public static ScoreManager instance { get; private set; }

    /// <summary>
    /// 現在のスコア
    /// </summary>
    private int score;

    /// <summary>
    /// 最終スコア
    /// </summary>
    public static int finalScore { get; private set; }

    /// <summary>
    /// 倒した敵の総数
    /// </summary>
    public static int enemiesDefeated { get; private set; }

    /// <summary>
    /// 倒したボスの総数
    /// </summary>
    public static int bossesDefeated { get; private set; }

    /// <summary>
    /// 集めたコインの総数
    /// </summary>
    public static int coinsCollected { get; private set; }

    /// <summary>
    /// 現在のスコアを取得
    /// </summary>
    public int Score => score;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        DontDestroyOnLoad(gameObject);

        InitializeScore();
    }

    /// <summary>
    /// スコアと統計情報を初期化する
    /// </summary>
    public void InitializeScore()
    {
        score = 0;
        finalScore = 0;
        enemiesDefeated = 0;
        bossesDefeated = 0;
        coinsCollected = 0;
    }

    /// <summary>
    /// スコアを加算する
    /// </summary>
    public void AddScore(int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        score += amount;
    }

    /// <summary>
    /// コイン獲得数を加算する
    /// </summary>
    public void AddCoin()
    {
        coinsCollected++;
    }

    /// <summary>
    /// 現在スコアを最終スコアとして保存する
    /// </summary>
    public void SaveFinalScore()
    {
        finalScore = score;

        Debug.Log(
            $"[ScoreManager] 最終スコア保存：{finalScore}"
        );
    }

    /// <summary>
    /// 現在スコアを取得する
    /// </summary>
    public int GetScore()
    {
        return score;
    }

    /// <summary>
    /// 累計コイン獲得数を取得する
    /// </summary>
    public int GetCoin()
    {
        return coinsCollected;
    }

    /// <summary>
    /// 倒した敵の数を加算する
    /// </summary>
    public void IncrementEnemyCount()
    {
        enemiesDefeated++;
    }

    /// <summary>
    /// 倒したボスの数を加算する
    /// </summary>
    public void IncrementBossCount()
    {
        bossesDefeated++;
    }

    /// <summary>
    /// 倒した敵の数を取得する
    /// </summary>
    public int GetEnemiesDefeated()
    {
        return enemiesDefeated;
    }

    /// <summary>
    /// 倒したボスの数を取得する
    /// </summary>
    public int GetBossesDefeated()
    {
        return bossesDefeated;
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }
}