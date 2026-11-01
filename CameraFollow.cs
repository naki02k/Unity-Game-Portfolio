using UnityEngine;

/// <summary>
/// プレイヤーを追従するカメラ
/// </summary>
public class CameraFollow : MonoBehaviour
{
    /// <summary>
    /// 追従するプレイヤー
    /// </summary>
    [SerializeField]
    private Transform player;

    private void LateUpdate()
    {
        if (player == null)
        {
            return;
        }

        transform.position = new Vector3(
            player.position.x,
            player.position.y,
            transform.position.z);
    }
}