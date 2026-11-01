using UnityEngine;
using UnityEngine.Pool;

/// <summary>
/// 一定間隔で斧を生成するクラス
/// </summary>
public class AxeSpawner : MonoBehaviour
{
    /// <summary>
    /// 生成する斧のプレハブ
    /// </summary>
    [SerializeField]
    private AxeController axePrefab;

    /// <summary>
    /// 斧を生成する間隔
    /// </summary>
    [SerializeField]
    private float spawnInterval = 1f;

    /// <summary>
    /// 斧の投擲先を決める範囲
    /// </summary>
    [SerializeField]
    private float range = 5f;

    /// <summary>
    /// 斧の移動速度
    /// </summary>
    [SerializeField]
    private float axeSpeed = 3f;

    /// <summary>
    /// 次の斧を生成するまでの経過時間
    /// </summary>
    private float spawnTimer;

    /// <summary>
    /// 斧を管理するオブジェクトプール
    /// </summary>
    private IObjectPool<AxeController> axePool;

    private void Awake()
    {
        if (axePrefab == null)
        {
            Debug.LogError("AxePrefab is not assigned.");
            enabled = false;
            return;
        }

        if (spawnInterval <= 0f)
        {
            Debug.LogError("Spawn Interval must be greater than 0.");
            enabled = false;
            return;
        }

        if (axeSpeed <= 0f)
        {
            Debug.LogError("Axe Speed must be greater than 0.");
            enabled = false;
            return;
        }

        axePool = new ObjectPool<AxeController>(
            createFunc: () => Instantiate(axePrefab),
            actionOnGet: axe => axe.gameObject.SetActive(true),
            actionOnRelease: axe => axe.gameObject.SetActive(false),
            actionOnDestroy: axe => Destroy(axe.gameObject),
            collectionCheck: true,
            defaultCapacity: 20,
            maxSize: 50
        );
    }

    private void Update()
    {
        spawnTimer += Time.deltaTime;

        while (spawnTimer >= spawnInterval)
        {
            SpawnAxe();
            spawnTimer -= spawnInterval;
        }
    }

    /// <summary>
    /// ランダムな位置へ向かう斧を生成する
    /// </summary>
    private void SpawnAxe()
    {
        AxeController axe = axePool.Get();

        Vector3 spawnPosition = transform.position;

        Vector3 targetPosition =
            spawnPosition +
            new Vector3(
                Random.Range(-range, range),
                Random.Range(0f, range),
                0f);

        axe.transform.position = spawnPosition;
        axe.transform.rotation = Quaternion.identity;

        axe.Initialize(
            targetPosition,
            axeSpeed,
            axePool);
    }
}