using UnityEngine;

/// <summary>
/// 遊び方パネルを表示するボタン処理
/// </summary>
public class PushHowToButton : MonoBehaviour
{
    /// <summary>
    /// タイトルパネル
    /// </summary>
    [SerializeField]
    private GameObject titlePanel;

    /// <summary>
    /// 遊び方パネル
    /// </summary>
    [SerializeField]
    private GameObject howToPlayPanel;

    /// <summary>
    /// ボタンクリック時
    /// </summary>
    public void OnClick()
    {
        if (titlePanel != null)
        {
            titlePanel.SetActive(false);
        }

        if (howToPlayPanel != null)
        {
            howToPlayPanel.SetActive(true);
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (titlePanel == null)
        {
            Debug.LogWarning($"{name}: TitlePanel が設定されていません");
        }

        if (howToPlayPanel == null)
        {
            Debug.LogWarning($"{name}: HowToPlayPanel が設定されていません");
        }
    }
#endif
}