using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 敵の出現管理とWave進行に関係する敵数を管理するクラス
/// </summary>
public class EnemyManager : MonoBehaviour
{
    /// <summary>
    /// EnemyManagerのシングルトンインスタンス
    /// </summary>
    public static EnemyManager Instance { get; private set; }

    [Header("Prefabs")]

    [SerializeField]
    private GameObject[] enemyPrefabs;

    [SerializeField]
    private GameObject bossPrefab;

    [Header("Spawn Settings")]

    [SerializeField]
    private Transform spawnArea;

    [SerializeField]
    private int maxSpawnAttempts = 50;

    [Header("Enemy Count Settings")]

    [SerializeField]
    private int baseEnemyCount = 5;

    [SerializeField]
    private int maxEnemyLimit = 100;

    [Header("Object Pooling Settings")]

    [SerializeField]
    private int initialPoolSizePerType = 30;

    [SerializeField]
    private int initialBossPoolSize = 1;

    // 現在出現している敵
    private readonly HashSet<GameObject> activeEnemies =
        new HashSet<GameObject>();

    // 敵Prefabごとのプール
    private readonly Dictionary<GameObject, Queue<GameObject>> enemyPools =
        new Dictionary<GameObject, Queue<GameObject>>();

    // 敵と元Prefabの対応関係
    private readonly Dictionary<GameObject, GameObject> enemyToPrefabMap =
        new Dictionary<GameObject, GameObject>();

    // 現在出現しているボス
    private GameObject activeBoss;

    // スポーン範囲
    private BoxCollider2D areaCollider;

    // 現在のWaveで倒した通常敵数
    private int defeatedEnemyCount;

    private const float EnemyMinDistance = 8f;
    private const float EnemyMaxDistance = 15f;

    private const float BossMinDistance = 10f;
    private const float BossMaxDistance = 18f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (spawnArea != null)
        {
            areaCollider = spawnArea.GetComponent<BoxCollider2D>();
        }

        InitializePools();
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    /// <summary>
    /// オブジェクトプールを初期化する
    /// </summary>
    private void InitializePools()
    {
        if (enemyPrefabs != null)
        {
            foreach (GameObject prefab in enemyPrefabs)
            {
                if (prefab == null || enemyPools.ContainsKey(prefab))
                {
                    continue;
                }

                CreatePool(prefab, initialPoolSizePerType);
            }
        }

        if (bossPrefab != null)
        {
            if (!enemyPools.ContainsKey(bossPrefab))
            {
                CreatePool(bossPrefab, initialBossPoolSize);
            }
        }
    }

    /// <summary>
    /// 指定Prefabのプールを作成する
    /// </summary>
    private void CreatePool(GameObject prefab, int initialSize)
    {
        Queue<GameObject> pool = new Queue<GameObject>();

        enemyPools.Add(prefab, pool);

        int count = Mathf.Max(0, initialSize);

        for (int i = 0; i < count; i++)
        {
            GameObject obj = Instantiate(prefab, transform);

            obj.SetActive(false);

            pool.Enqueue(obj);

            enemyToPrefabMap.Add(obj, prefab);
        }
    }

    /// <summary>
    /// 敵をプールから取得する
    /// </summary>
    private GameObject GetPooledEnemy(GameObject prefab)
    {
        if (prefab == null)
        {
            return null;
        }

        if (!enemyPools.TryGetValue(prefab, out Queue<GameObject> pool))
        {
            CreatePool(prefab, 0);
            pool = enemyPools[prefab];
        }

        while (pool.Count > 0)
        {
            GameObject obj = pool.Dequeue();

            if (obj != null)
            {
                return obj;
            }
        }

        // プールが空の場合は追加生成
        GameObject newObj = Instantiate(prefab, transform);

        enemyToPrefabMap.Add(newObj, prefab);

        newObj.SetActive(false);

        return newObj;
    }

    /// <summary>
    /// 敵をプールに返却する
    /// </summary>
    private void ReturnEnemyToPool(GameObject enemy)
    {
        if (enemy == null)
        {
            return;
        }

        if (!enemyToPrefabMap.TryGetValue(
            enemy,
            out GameObject originalPrefab))
        {
            Debug.LogWarning(
                "[EnemyManager] 対応するPrefabが見つかりません。");

            Destroy(enemy);
            return;
        }

        if (!enemyPools.TryGetValue(
            originalPrefab,
            out Queue<GameObject> pool))
        {
            Destroy(enemy);
            return;
        }

        enemy.SetActive(false);

        enemy.transform.SetParent(transform);

        pool.Enqueue(enemy);
    }

    /// <summary>
    /// 敵撃破時に呼び出す
    /// </summary>
    public void EnemyDefeated(GameObject enemy)
    {
        if (enemy == null || !activeEnemies.Remove(enemy))
        {
            return;
        }

        // ボスかどうかを判定
        bool isBoss = enemy == activeBoss;

        if (isBoss)
        {
            activeBoss = null;
        }
        else
        {
            defeatedEnemyCount++;
        }

        ReturnEnemyToPool(enemy);

        if (isBoss && WaveManager.Instance != null)
        {
            WaveManager.Instance.HandleBossDefeated();
        }
    }

    /// <summary>
    /// 現在出現している敵数を取得する
    /// </summary>
    private int GetCurrentEnemyCount()
    {
        return activeEnemies.Count;
    }

    /// <summary>
    /// Waveごとの最大敵数を取得する
    /// </summary>
    private int GetMaxEnemyCount()
    {
        if (WaveManager.Instance == null)
        {
            return Mathf.Max(0, baseEnemyCount);
        }

        int wave = Mathf.Max(1, WaveManager.Instance.CurrentWaveCount);

        // 敵数が指数的に増えすぎないよう上限を設定
        int maxCount = Mathf.Max(0, maxEnemyLimit);
        int baseCount = Mathf.Max(0, baseEnemyCount);

        int additionalCount = 0;

        for (int i = 1; i < wave && baseCount + additionalCount < maxCount; i++)
        {
            additionalCount = Mathf.Min(
                maxCount,
                additionalCount * 2 + 1);
        }

        return Mathf.Min(
            baseCount + additionalCount,
            maxCount);
    }

    /// <summary>
    /// さらに敵を生成できるか判定する
    /// </summary>
    public bool CanSpawnMoreEnemies()
    {
        return GetCurrentEnemyCount() < GetMaxEnemyCount();
    }

    /// <summary>
    /// 通常敵を出現させる
    /// </summary>
    public void SpawnNormalEnemy()
    {
        if (enemyPrefabs == null || enemyPrefabs.Length == 0)
        {
            Debug.LogWarning(
                "[EnemyManager] enemyPrefabsが設定されていません。");
            return;
        }

        if (!CanSpawnMoreEnemies())
        {
            return;
        }

        Vector2 playerPosition = GetPlayerPosition();

        if (!TryGetSafeRandomPosition(
            playerPosition,
            EnemyMinDistance,
            EnemyMaxDistance,
            out Vector2 spawnPosition))
        {
            Debug.LogWarning(
                "[EnemyManager] 安全なスポーン位置が見つかりませんでした。");
            return;
        }

        GameObject selectedPrefab = null;

        // nullのPrefabを選ばないようにする
        for (int i = 0; i < enemyPrefabs.Length; i++)
        {
            GameObject candidate =
                enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];

            if (candidate != null)
            {
                selectedPrefab = candidate;
                break;
            }
        }

        if (selectedPrefab == null)
        {
            Debug.LogWarning(
                "[EnemyManager] 有効な敵Prefabがありません。");
            return;
        }

        GameObject enemy = GetPooledEnemy(selectedPrefab);

        if (enemy == null)
        {
            return;
        }

        enemy.transform.SetParent(null);
        enemy.transform.position = spawnPosition;
        enemy.transform.rotation = Quaternion.identity;

        // 有効化前に管理リストへ登録
        activeEnemies.Add(enemy);

        enemy.SetActive(true);
    }

    /// <summary>
    /// ボス敵を出現させる
    /// </summary>
    public void SpawnBossEnemy()
    {
        if (bossPrefab == null || activeBoss != null)
        {
            return;
        }

        defeatedEnemyCount = 0;

        if (!TryGetSafeRandomPosition(
            GetPlayerPosition(),
            BossMinDistance,
            BossMaxDistance,
            out Vector2 spawnPosition))
        {
            Debug.LogWarning(
                "[EnemyManager] ボスのスポーン位置が見つかりませんでした。");
            return;
        }

        GameObject boss = GetPooledEnemy(bossPrefab);

        if (boss == null)
        {
            return;
        }

        boss.transform.SetParent(null);
        boss.transform.position = spawnPosition;
        boss.transform.rotation = Quaternion.identity;

        activeBoss = boss;

        activeEnemies.Add(boss);

        boss.SetActive(true);
    }

    /// <summary>
    /// プレイヤーの位置を取得する
    /// </summary>
    private Vector2 GetPlayerPosition()
    {
        if (Player.instance != null)
        {
            return Player.instance.transform.position;
        }

        if (areaCollider != null)
        {
            return areaCollider.bounds.center;
        }

        return Vector2.zero;
    }

    /// <summary>
    /// 安全なスポーン位置を取得する
    /// </summary>
    private bool TryGetSafeRandomPosition(
        Vector2 targetPosition,
        float minDistance,
        float maxDistance,
        out Vector2 result)
    {
        result = default;

        if (areaCollider == null)
        {
            return false;
        }

        Bounds bounds = areaCollider.bounds;

        float minDistanceSquared = minDistance * minDistance;
        float maxDistanceSquared = maxDistance * maxDistance;

        for (int i = 0; i < maxSpawnAttempts; i++)
        {
            Vector2 candidate = new Vector2(
                Random.Range(bounds.min.x, bounds.max.x),
                Random.Range(bounds.min.y, bounds.max.y));

            float distanceSquared =
                (candidate - targetPosition).sqrMagnitude;

            if (distanceSquared >= minDistanceSquared &&
                distanceSquared <= maxDistanceSquared)
            {
                result = candidate;
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// 出現中の敵全てオブジェクトプールへ渡す
    /// </summary>
    public void ClearAllEnemies()
    {
        // HashSetを直接変更しながらforeachしないようにコピーする
        List<GameObject> enemies = new List<GameObject>(activeEnemies);

        foreach(GameObject enemy in enemies)
        {
            if (enemy == null) continue;

            activeEnemies.Remove(enemy);

            ReturnEnemyToPool(enemy);
        }

        // ボスの参照もリセット
        activeBoss = null;
    }
}