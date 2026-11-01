using UnityEngine;

/// <summary>
/// Playerシングルトンからプレイヤーオブジェクトを取得するクラス
/// </summary>
public class PlayerScanner : MonoBehaviour
{
    /// <summary>
    /// キャッシュしたプレイヤーオブジェクト
    /// </summary>
    private GameObject player;

    /// <summary>
    /// プレイヤーオブジェクトを取得する
    /// </summary>
    /// <returns>
    /// プレイヤーオブジェクト。
    /// 存在しない場合はnull。
    /// </returns>
    public GameObject ScanPlayer()
    {
        if (player != null)
        {
            return player;
        }

        if (Player.instance != null)
        {
            player =
                Player.instance.gameObject;
        }
        else
        {
            Debug.LogError(
                "Playerシングルトンインスタンスが見つかりません。");
        }

        return player;
    }
}