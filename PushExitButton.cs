using UnityEngine;

/// <summary>
/// ゲーム終了ボタンの処理を行うクラス
/// </summary>
public class PushExitButton : MonoBehaviour
{
    /// <summary>
    /// 終了ボタン押下時に呼び出される
    /// </summary>
    public void OnButton()
    {
#if UNITY_EDITOR
        // Unityエディタ上で実行中の場合は再生を停止
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // ビルド版ではアプリケーションを終了
        Application.Quit();
#endif
    }
}