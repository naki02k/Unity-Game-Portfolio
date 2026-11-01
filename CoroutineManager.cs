using System.Collections;
using UnityEngine;

/// <summary>
/// MonoBehaviourを継承していないクラスから
/// コルーチンを実行するための管理クラス
/// </summary>
public class CoroutineManager : MonoBehaviour
{
    /// <summary>
    /// コルーチン管理のインスタンス
    /// </summary>
    public static CoroutineManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// コルーチンを実行する
    /// </summary>
    /// <param name="coroutine">実行するコルーチン</param>
    /// <returns>開始したコルーチン</returns>
    public Coroutine RunCoroutine(IEnumerator coroutine)
    {
        if (coroutine == null)
        {
            Debug.LogWarning("Coroutine is null.");
            return null;
        }

        return StartCoroutine(coroutine);
    }
}