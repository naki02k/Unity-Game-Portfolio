using UnityEngine;

/// <summary>
/// 一定時間ごとに仲間キャラクターを生成するクラス
/// </summary>
public class FellowGenerator : MonoBehaviour
{
    /// <summary>
    /// 仲間キャラクターのプレハブ
    /// </summary>
    [SerializeField]
    private GameObject fellowPrefab;

    /// <summary>
    /// 仲間を生成する間隔
    /// </summary>
    [SerializeField]
    private float spawnInterval = 3f;

    /// <summary>
    /// 追従対象のプレイヤー
    /// </summary>
    private Player player;

    /// <summary>
    /// 仲間を生成してからの経過時間
    /// </summary>
    private float currentTime;

    /// <summary>
    /// プレイヤー周辺に仲間を生成する範囲
    /// </summary>
    private const float SpawnRange = 3f;

    private void Start()
    {
        player = Player.instance;
    }

    private void Update()
    {
        if (player == null)
        {
            return;
        }

        currentTime += Time.deltaTime;

        if (currentTime >= spawnInterval)
        {
            FellowGenerate(player.transform.position);
            currentTime = 0f;
        }
    }

    /// <summary>
    /// プレイヤー周辺に仲間を生成する
    /// </summary>
    /// <param name="targetPosition">仲間を生成する基準位置</param>
    public void FellowGenerate(Vector2 targetPosition)
    {
        if (fellowPrefab == null)
        {
            return;
        }

        Vector2 randomOffset =
            new Vector2(
                Random.Range(-SpawnRange, SpawnRange),
                Random.Range(-SpawnRange, SpawnRange));

        Vector2 fellowSpawnPosition =
            targetPosition + randomOffset;

        Fellow fellow =
            Instantiate(
                fellowPrefab,
                fellowSpawnPosition,
                Quaternion.identity)
            .GetComponent<Fellow>();

        if (fellow != null)
        {
            fellow.SetPlayerTransform(player.transform);
        }
    }
}