using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 魔法書を管理するクラス
/// ターゲットの周囲を回転するオブジェクトの生成と配置を行う
/// </summary>
public class MagicBookManager : MonoBehaviour
{
    /// <summary>
    /// 回転の中心となるターゲット
    /// </summary>
    [SerializeField]
    private Transform target;

    /// <summary>
    /// 回転する魔法書のプレハブ
    /// </summary>
    [SerializeField]
    private RotateAroundTarget rotateAroundPrefab;

    /// <summary>
    /// 生成済みの魔法書一覧
    /// </summary>
    private readonly List<RotateAroundTarget> rotatingBooks =
        new List<RotateAroundTarget>();

    /// <summary>
    /// 魔法書を生成し、均等な角度で再配置する
    /// </summary>
    public void SpawnAndRotate()
    {
        if (target == null)
        {
            Debug.LogWarning(
                "Target が設定されていません。",
                this);
            return;
        }

        if (rotateAroundPrefab == null)
        {
            Debug.LogWarning(
                "RotateAroundPrefab が設定されていません。",
                this);
            return;
        }

        RotateAroundTarget newBook =
            Instantiate(rotateAroundPrefab);

        newBook.target = target;

        rotatingBooks.Add(newBook);

        float baseAngle = rotatingBooks[0].Angle;

        for (int i = 0; i < rotatingBooks.Count; i++)
        {
            float angleOffset =
                360f / rotatingBooks.Count * i;

            rotatingBooks[i].SetOffset(
                baseAngle,
                angleOffset);
        }
    }
}