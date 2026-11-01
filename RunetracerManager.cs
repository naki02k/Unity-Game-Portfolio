using UnityEngine;

/// <summary>
/// ルーントレーサー（武器）の生成を管理するクラス
/// </summary>
public class RunetracerManager : MonoBehaviour
{
    /// <summary>
    /// 生成するルーントレーサーのプレハブ
    /// </summary>
    [SerializeField]
    private GameObject runetracerPrefab;

    /// <summary>
    /// ルーントレーサーを生成する位置
    /// </summary>
    [SerializeField]
    private Transform spawnPoint;

    /// <summary>
    /// ルーントレーサーを生成する
    /// </summary>
    public void SpawnRunetracer()
    {
        if (runetracerPrefab == null ||
            spawnPoint == null)
        {
            return;
        }

        Instantiate(
            runetracerPrefab,
            spawnPoint.position,
            Quaternion.identity);
    }
}