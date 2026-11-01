using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// ボタンクリックなどのイベントでタイトルシーンへ遷移するクラス
/// </summary>
public class PushTitleScene : MonoBehaviour
{
    /// <summary>
    /// 遷移先のタイトルシーン名
    /// </summary>
    [SerializeField]
    private string titleSceneName = "Title";

    /// <summary>
    /// タイトルシーンをロードする
    /// </summary>
    public void LoadTitleScene()
    {
        SceneManager.LoadScene(titleSceneName);
    }
}