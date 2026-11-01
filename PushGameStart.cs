using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// ゲーム開始ボタン処理
/// </summary>
public class PushGameStart : MonoBehaviour
{
    [Header("Scene Names")]
    [SerializeField]
    private string normalSceneName = "NormalScene";

    [SerializeField]
    private string hardSceneName = "HardStage";

    /// <summary>
    /// ゲーム開始
    /// </summary>
    public void StartGame()
    {
        switch (ModeSelector.SelectMode)
        {
            case "Normal":
                ModeSelector.CurrentStage = "Normal";
                SceneManager.LoadScene(normalSceneName);
                break;

            case "Hard":
                ModeSelector.CurrentStage = "Hard";
                SceneManager.LoadScene(hardSceneName);
                break;

            default:
                Debug.LogWarning(
                    $"未定義のモードです: {ModeSelector.SelectMode}");

                ModeSelector.CurrentStage = "Normal";
                SceneManager.LoadScene(normalSceneName);
                break;
        }
    }
}