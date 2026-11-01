using TMPro;
using UnityEngine;

/// <summary>
/// ダメージ値を画面上にポップアップ表示するクラス
/// </summary>
public class DamagePopup : MonoBehaviour
{
    /// <summary>
    /// ダメージ値を表示するテキスト
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI damageText;

    /// <summary>
    /// テキストの色と透明度
    /// </summary>
    private Color textColor;

    /// <summary>
    /// フェードアウト速度
    /// </summary>
    [SerializeField]
    private float disappearSpeed = 2f;

    /// <summary>
    /// 上昇速度
    /// </summary>
    [SerializeField]
    private float moveSpeed = 1f;

    private void Awake()
    {
        if (damageText == null)
        {
            damageText = GetComponentInChildren<TextMeshProUGUI>();
        }

        if (damageText == null)
        {
            Debug.LogError("DamageText is not assigned.", this);
            enabled = false;
        }
    }

    /// <summary>
    /// ダメージポップアップを初期化する
    /// </summary>
    /// <param name="damage">表示するダメージ値</param>
    public void Setup(int damage)
    {
        if (damageText == null)
        {
            return;
        }

        damageText.text = damage.ToString();
        textColor = damageText.color;

        transform.position += Vector3.up;
    }

    private void Update()
    {
        transform.position +=
            Vector3.up * moveSpeed * Time.deltaTime;

        textColor.a -=
            disappearSpeed * Time.deltaTime;

        damageText.color = textColor;

        if (textColor.a <= 0f)
        {
            Destroy(gameObject);
        }
    }
}