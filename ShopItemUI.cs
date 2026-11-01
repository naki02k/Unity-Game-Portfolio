using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// ショップアイテムのUI表示と購入ボタンの設定を行うクラス
/// </summary>
public class ShopItemUI : MonoBehaviour
{
    [Header("UI References")]

    /// <summary>
    /// アイテム名を表示するテキスト
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI itemNameText;

    /// <summary>
    /// アイテム価格を表示するテキスト
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI itemPriceText;

    /// <summary>
    /// アイテムアイコンを表示するImage
    /// </summary>
    [SerializeField]
    private Image itemIconImage;

    /// <summary>
    /// 購入ボタン
    /// </summary>
    [SerializeField]
    private Button buyButton;

    /// <summary>
    /// アイテム情報をUIへ反映し、購入ボタンを設定する
    /// </summary>
    /// <param name="item">表示するアイテムデータ</param>
    /// <param name="onBuyAction">購入時に実行する処理</param>
    public void Setup(
        ShopItem item,
        UnityAction onBuyAction)
    {
        if (item == null)
        {
            Debug.LogWarning(
                "ShopItem が設定されていません。",
                this);

            return;
        }

        if (itemNameText != null)
        {
            itemNameText.text = item.itemName;
        }

        if (itemPriceText != null)
        {
            itemPriceText.text =
                $"{item.price} Coins";
        }

        if (itemIconImage != null)
        {
            itemIconImage.sprite = item.itemIcon;
        }

        if (buyButton != null)
        {
            buyButton.onClick.RemoveAllListeners();

            if (onBuyAction != null)
            {
                buyButton.onClick.AddListener(onBuyAction);
            }
        }
    }
}