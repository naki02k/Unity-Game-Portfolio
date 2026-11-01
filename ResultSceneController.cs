using TMPro;
using UnityEngine;

/// <summary>
/// リザルト画面の表示を管理するクラス
/// </summary>
public class ResultSceneController : MonoBehaviour
{
    /// <summary>
    /// スコア表示
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI scoreText;

    /// <summary>
    /// 敵撃破数表示
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI enemiesText;

    /// <summary>
    /// ボス撃破数表示
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI bossesText;

    /// <summary>
    /// コイン取得数表示
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI coinText;

    /// <summary>
    /// ステージ名表示
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI stageNameText;

    private void Start()
    {
        UpdateResultUI();
    }

    /// <summary>
    /// リザルト情報をUIへ反映する
    /// </summary>
    private void UpdateResultUI()
    {
        ScoreManager scoreManager =
            ScoreManager.instance;

        if (scoreManager != null)
        {
            if (scoreText != null)
            {
                scoreText.text =
                    $"Score : {scoreManager.GetScore()}";
            }

            if (enemiesText != null)
            {
                enemiesText.text =
                    $"Enemies Defeated : " +
                    $"{scoreManager.GetEnemiesDefeated()}";
            }

            if (bossesText != null)
            {
                bossesText.text =
                    $"Bosses Defeated : " +
                    $"{scoreManager.GetBossesDefeated()}";
            }

            if (coinText != null)
            {
                coinText.text =
                    $"Coins Collected : " +
                    $"{scoreManager.GetCoin()}";
            }
        }
        else
        {
            Debug.LogWarning(
                "[ResultSceneController] ScoreManagerが見つかりません。"
            );
        }

        if (stageNameText != null)
        {
            stageNameText.text =
                $"Stage : {ModeSelector.CurrentStage}";
        }
    }
}