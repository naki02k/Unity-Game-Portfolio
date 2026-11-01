using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 敵をスキャンし、最も近いターゲットを取得するクラス
/// </summary>
public class EnemyScanner : MonoBehaviour
{
    /// <summary>
    /// ターゲットを探索する範囲
    /// </summary>
    [SerializeField]
    private float scanRange = 15f;

    /// <summary>
    /// 最も近い敵またはボスを取得する
    /// </summary>
    /// <returns>
    /// 最も近い敵またはボス。
    /// 見つからなかった場合はnull。
    /// </returns>
    public GameObject ScanWithFindTag()
    {
        GameObject[] enemies =
            GameObject.FindGameObjectsWithTag("Enemy");

        GameObject[] bosses =
            GameObject.FindGameObjectsWithTag("Boss");

        List<GameObject> allTargets =
            new List<GameObject>();

        allTargets.AddRange(enemies);
        allTargets.AddRange(bosses);

        GameObject nearestTarget = null;
        float minDistance = float.MaxValue;

        foreach (GameObject target in allTargets)
        {
            float distanceToTarget =
                Vector3.Distance(
                    transform.position,
                    target.transform.position);

            if (distanceToTarget < scanRange &&
                distanceToTarget < minDistance)
            {
                minDistance = distanceToTarget;
                nearestTarget = target;
            }
        }

        return nearestTarget;
    }

    /// <summary>
    /// エディタ上でスキャン範囲を可視化する
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(
            transform.position,
            scanRange);
    }
}