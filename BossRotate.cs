using UnityEngine;

/// <summary>
/// プレイヤーの位置に応じてボスの向きを変更する
/// </summary>
public class BossRotate : MonoBehaviour
{
    /// <summary>
    /// プレイヤーの位置に応じてボスの向きを変更する
    /// </summary>
    /// <param name="playerPosition">プレイヤーの位置</param>
    public void LookAtPlayer(Vector3 playerPosition)
    {
        float directionX =
            playerPosition.x - transform.position.x;

        if (directionX > 0f)
        {
            transform.eulerAngles =
                new Vector3(0f, 180f, 0f);
        }
        else if (directionX < 0f)
        {
            transform.eulerAngles = Vector3.zero;
        }
    }
}