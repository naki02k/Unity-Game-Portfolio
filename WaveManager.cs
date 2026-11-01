using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// ウェーブ進行、タイマー、クリア条件を管理するクラス
/// </summary>
public class WaveManager : MonoBehaviour
{
    private const int BossWaveInterval = 5;
    private const float BossWarningDuration = 2f;
    private const float ResultTransitionDelay = 2f;
    private const float HpGrowthPerWave = 0.2f;
    private const string ResultSceneName = "ResultScene";

    public static WaveManager Instance { get; private set; }

    [Header("Wave Settings")]

    [SerializeField]
    private int maxWaveCount = 10;

    [SerializeField]
    private float initialWaveTime = 30f;

    [SerializeField]
    private float initialSpawnInterval = 1f;

    [SerializeField]
    private float minSpawnInterval = 0.1f;

    [SerializeField]
    private float intervalDecreaseStep = 0.05f;

    [Header("Timings")]

    [SerializeField]
    private float clearTextDuration = 2f;

    [SerializeField]
    private float shopOpenDelay = 1f;

    public int MaxWaveCount => maxWaveCount;

    public int CurrentWaveCount { get; private set; } = 1;

    public float CurrentWaveTime { get; private set; }

    public bool IsBossWave { get; private set; }

    private bool canSpawnEnemies;
    private bool isTimeRunning;
    private bool isWaveTransitioning;
    private bool isGameEnded;

    private float currentSpawnInterval;

    private GameUIManager uiManager;

    private Coroutine spawnRoutine;
    private Coroutine waveCompletionRoutine;
    private Coroutine bossSpawnRoutine;
    private Coroutine shopRoutine;
    private Coroutine resultTransitionRoutine;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        uiManager = GameUIManager.instance;

        maxWaveCount = Mathf.Max(1, maxWaveCount);
        initialWaveTime = Mathf.Max(0.1f, initialWaveTime);
        initialSpawnInterval = Mathf.Max(0.01f, initialSpawnInterval);
        minSpawnInterval = Mathf.Max(0.01f, minSpawnInterval);
        intervalDecreaseStep = Mathf.Max(0f, intervalDecreaseStep);
        clearTextDuration = Mathf.Max(0f, clearTextDuration);
        shopOpenDelay = Mathf.Max(0f, shopOpenDelay);

        currentSpawnInterval = initialSpawnInterval;

        if (uiManager != null)
        {
            uiManager.UpdateWaveText(
                CurrentWaveCount,
                maxWaveCount
            );
        }

        Debug.Log(
            $"<color=cyan>【Wave開始】" +
            $"Wave{CurrentWaveCount}がスタートしました！</color>"
        );

        StartWave();
    }

    private void Update()
    {
        UpdateTimer();
    }

    /// <summary>
    /// ウェーブを開始する
    /// </summary>
    private void StartWave()
    {
        if (isGameEnded || CurrentWaveCount > maxWaveCount)
        {
            return;
        }

        isWaveTransitioning = false;
        IsBossWave = false;

        CurrentWaveTime = initialWaveTime;
        isTimeRunning = true;
        canSpawnEnemies = true;

        StopSpawnRoutine();

        spawnRoutine = StartCoroutine(SpawnEnemyRoutine());

        if (uiManager != null)
        {
            uiManager.UpdateTimerText(CurrentWaveTime);
        }

        Debug.Log(
            $"<color=cyan>Wave {CurrentWaveCount} 開始</color>"
        );
    }

    /// <summary>
    /// ウェーブタイマーを更新する
    /// </summary>
    private void UpdateTimer()
    {
        if (!isTimeRunning || isGameEnded)
        {
            return;
        }

        CurrentWaveTime -= Time.deltaTime;
        CurrentWaveTime = Mathf.Max(0f, CurrentWaveTime);

        if (uiManager != null)
        {
            uiManager.UpdateTimerText(CurrentWaveTime);
        }

        if (CurrentWaveTime <= 0f)
        {
            OnWaveTimeEnd();
        }
    }

    /// <summary>
    /// 敵を一定間隔で生成する
    /// </summary>
    private IEnumerator SpawnEnemyRoutine()
    {
        while (canSpawnEnemies && isTimeRunning && !isGameEnded)
        {
            if (EnemyManager.Instance != null)
            {
                EnemyManager.Instance.SpawnNormalEnemy();
            }

            yield return new WaitForSeconds(currentSpawnInterval);
        }

        spawnRoutine = null;
    }

    /// <summary>
    /// 敵生成コルーチンを停止する
    /// </summary>
    private void StopSpawnRoutine()
    {
        if (spawnRoutine != null)
        {
            StopCoroutine(spawnRoutine);
            spawnRoutine = null;
        }
    }

    /// <summary>
    /// ウェーブ終了時の処理
    /// </summary>
    private void OnWaveTimeEnd()
    {
        if (isWaveTransitioning || isGameEnded)
        {
            return;
        }

        isTimeRunning = false;
        canSpawnEnemies = false;

        StopSpawnRoutine();

        ClearStage();

        waveCompletionRoutine =
            StartCoroutine(HandleWaveCompletion());
    }

    /// <summary>
    /// ウェーブクリア処理
    /// </summary>
    public IEnumerator HandleWaveCompletion()
    {
        if (isWaveTransitioning || isGameEnded)
        {
            yield break;
        }

        isWaveTransitioning = true;

        if (uiManager != null)
        {
            yield return StartCoroutine(
                uiManager.ShowWaveCompleteText(
                    "Wave Clear!",
                    clearTextDuration
                )
            );
        }

        // ボスウェーブ判定
        if (CurrentWaveCount % BossWaveInterval == 0 &&
            CurrentWaveCount != maxWaveCount)
        {
            bossSpawnRoutine =
                StartCoroutine(HandleBossSpawn());

            waveCompletionRoutine = null;
            yield break;
        }

        // 最終ウェーブ判定
        if (CurrentWaveCount >= maxWaveCount)
        {
            waveCompletionRoutine = null;
            GameClear();
            yield break;
        }

        PrepareNextWave();

        waveCompletionRoutine = null;

        shopRoutine = StartCoroutine(OpenShopAfterDelay());
    }

    /// <summary>
    /// ボス出現処理
    /// </summary>
    private IEnumerator HandleBossSpawn()
    {
        IsBossWave = true;
        canSpawnEnemies = false;
        isTimeRunning = false;

        if (uiManager != null)
        {
            yield return StartCoroutine(
                uiManager.ShowBossWarning(BossWarningDuration)
            );
        }

        if (isGameEnded)
        {
            bossSpawnRoutine = null;
            yield break;
        }

        if (EnemyManager.Instance != null)
        {
            EnemyManager.Instance.SpawnBossEnemy();
        }
        else
        {
            Debug.LogError("EnemyManagerが見つかりません。");
        }

        bossSpawnRoutine = null;
    }

    /// <summary>
    /// ボス撃破時の処理
    /// </summary>
    public void HandleBossDefeated()
    {
        if (!IsBossWave || !isWaveTransitioning || isGameEnded)
        {
            return;
        }

        IsBossWave = false;
        canSpawnEnemies = false;
        isTimeRunning = false;

        if (bossSpawnRoutine != null)
        {
            StopCoroutine(bossSpawnRoutine);
            bossSpawnRoutine = null;
        }

        PrepareNextWave();

        if (CurrentWaveCount > maxWaveCount)
        {
            GameClear();
            return;
        }

        shopRoutine = StartCoroutine(OpenShopAfterDelay());
    }

    /// <summary>
    /// ショップを開く
    /// </summary>
    private IEnumerator OpenShopAfterDelay()
    {
        yield return new WaitForSeconds(shopOpenDelay);

        if (isGameEnded)
        {
            shopRoutine = null;
            yield break;
        }

        if (ShopManager.instance != null)
        {
            ShopManager.instance.OpenShop();
        }
        else
        {
            Debug.LogWarning(
                "ShopManagerが見つからないため、次のウェーブを開始します。"
            );

            StartWave();
        }

        shopRoutine = null;
    }

    /// <summary>
    /// ショップ終了後にウェーブを再開する
    /// </summary>
    public void ResumeAfterShop()
    {
        if (isGameEnded || !isWaveTransitioning)
        {
            return;
        }

        StartWave();
    }

    /// <summary>
    /// 次のウェーブを準備する
    /// </summary>
    private void PrepareNextWave()
    {
        CurrentWaveCount++;

        if (uiManager != null)
        {
            uiManager.UpdateWaveText(
                CurrentWaveCount,
                maxWaveCount
            );
        }

        currentSpawnInterval = Mathf.Max(
            currentSpawnInterval - intervalDecreaseStep,
            minSpawnInterval
        );

        float nextMultiplier =
            1f + (CurrentWaveCount - 1) * HpGrowthPerWave;

        Debug.Log(
            $"<color=orange>【Waveアップ】" +
            $"次ウェーブは Wave {CurrentWaveCount} です。" +
            $"以降に湧く敵のHP倍率は " +
            $"【{nextMultiplier}倍】になります！</color>"
        );
    }

    /// <summary>
    /// ゲームクリア処理
    /// </summary>
    private void GameClear()
    {
        if (isGameEnded)
        {
            return;
        }

        isGameEnded = true;
        isWaveTransitioning = true;

        canSpawnEnemies = false;
        isTimeRunning = false;
        IsBossWave = false;

        StopSpawnRoutine();

        if (waveCompletionRoutine != null)
        {
            StopCoroutine(waveCompletionRoutine);
            waveCompletionRoutine = null;
        }

        if (bossSpawnRoutine != null)
        {
            StopCoroutine(bossSpawnRoutine);
            bossSpawnRoutine = null;
        }

        if (shopRoutine != null)
        {
            StopCoroutine(shopRoutine);
            shopRoutine = null;
        }

        // 最終スコアを保存
        if (ScoreManager.instance != null)
        {
            ScoreManager.instance.SaveFinalScore();
        }
        else
        {
            Debug.LogWarning(
                "ScoreManagerが見つからないため、最終スコアを保存できません。"
            );
        }

        if (uiManager != null)
        {
            uiManager.ShowGameClearUI(true);
        }

        resultTransitionRoutine =
            StartCoroutine(TransitionToResultScene());
    }

    /// <summary>
    /// リザルト画面へ遷移する
    /// </summary>
    private IEnumerator TransitionToResultScene()
    {
        yield return new WaitForSeconds(ResultTransitionDelay);

        SceneManager.LoadScene(ResultSceneName);
    }

    /// <summary>
    /// ステージ上の敵とコインを整理する
    /// </summary>
    private void ClearStage()
    {
        // 敵はDestroyせず、EnemyManagerのプールへ返却する
        if (EnemyManager.Instance != null)
        {
            EnemyManager.Instance.ClearAllEnemies();
        }

        // コインをプールへ返却
        if (CoinManager.instance != null)
        {
            CoinManager.instance.ClearAllCoins();
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}