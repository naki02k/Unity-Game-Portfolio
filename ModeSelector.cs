using UnityEngine;

/// <summary>
/// ゲームの難易度選択を管理するクラス
/// </summary>
public class ModeSelector : MonoBehaviour
{
    /// <summary>
    /// 選択中のゲームモード
    /// </summary>
    public static string SelectMode { get; private set; } = "Normal";

    /// <summary>
    /// 現在のステージ名
    /// </summary>
    public static string CurrentStage { get; set; } = string.Empty;

    /// <summary>
    /// ノーマルモードを選択
    /// </summary>
    public void SelectNormalMode()
    {
        SelectMode = "Normal";
        Debug.Log("ノーマルモードが選択されました");
    }

    /// <summary>
    /// ハードモードを選択
    /// </summary>
    public void SelectHardMode()
    {
        SelectMode = "Hard";
        Debug.Log("ハードモードが選択されました");
    }
}