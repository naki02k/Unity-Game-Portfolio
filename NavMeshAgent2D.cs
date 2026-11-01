using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Unityの3D NavMeshを2Dゲームで利用するための簡易エージェント
/// </summary>
public class NavMeshAgent2D : MonoBehaviour
{
    /// <summary>
    /// 次のコーナーへ移動する判定距離
    /// </summary>
    private const float CornerReachDistance = 0.05f;

    /// <summary>
    /// 移動速度
    /// </summary>
    [SerializeField]
    private float speed = 1f;

    /// <summary>
    /// 停止距離
    /// </summary>
    [SerializeField]
    private float stoppingDistance = 0.1f;

    /// <summary>
    /// 現在の目的地
    /// </summary>
    private Vector2 destination;

    private void Update()
    {
        MoveToDestination();
    }

    /// <summary>
    /// 目的地を設定する
    /// </summary>
    /// <param name="target">移動先座標</param>
    /// <returns>常にtrue</returns>
    public bool SetDestination(Vector2 target)
    {
        destination = target;
        return true;
    }

    /// <summary>
    /// 現在の目的地へ移動する
    /// </summary>
    private void MoveToDestination()
    {
        Vector2 currentPosition = transform.position;

        if (Vector2.Distance(
                currentPosition,
                destination) <= stoppingDistance)
        {
            return;
        }

        NavMeshPath path = new NavMeshPath();

        bool hasPath =
            NavMesh.CalculatePath(
                currentPosition,
                destination,
                NavMesh.AllAreas,
                path);

        if (!hasPath || path.corners.Length == 0)
        {
            return;
        }

        Vector2 nextCorner = path.corners[0];

        if (path.corners.Length > 1 &&
            Vector2.Distance(
                currentPosition,
                nextCorner) <= CornerReachDistance)
        {
            nextCorner = path.corners[1];
        }

        transform.position =
            Vector2.MoveTowards(
                currentPosition,
                nextCorner,
                speed * Time.deltaTime);
    }
}