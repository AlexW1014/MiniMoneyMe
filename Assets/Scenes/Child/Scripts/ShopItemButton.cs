using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopItemButton : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image itemIcon;
    [SerializeField] private TMP_Text itemNameText;
    [SerializeField] private TMP_Text priceText;
    [SerializeField] private Button buyButton;

    [Header("Overlay (for purchased state)")]
    [SerializeField] private GameObject purchasedOverlay;
    [SerializeField] private TMP_Text ownedQuantityText;

    private ItemData itemData;
    public ItemData ItemData => itemData;

    private bool isPurchased = false;
    private int purchasedQuantity = 0;

    public void SetItemData(ItemData newItemData)
    {
        itemData = newItemData;
        isPurchased = false;
        purchasedQuantity = 0;

        if (itemIcon != null) itemIcon.sprite = itemData.icon;
        if (itemNameText != null) itemNameText.text = itemData.itemName;
        if (priceText != null) priceText.text = $"{itemData.price} Gold";

        if (buyButton != null)
        {
            buyButton.onClick.RemoveAllListeners();
            buyButton.onClick.AddListener(OnItemClicked);
        }

        if (purchasedOverlay != null)
            purchasedOverlay.SetActive(false);

        UpdateOwnedQuantityUI();
        RefreshAffordability();
    }

    public void RefreshAffordability()
    {
        if (buyButton == null || itemData == null) return;

        float currentMoney = AvatarMoneyManager.Instance != null
            ? AvatarMoneyManager.Instance.GetMoney()
            : 0;

        bool canAfford = currentMoney >= itemData.price;

        buyButton.interactable = canAfford && !isPurchased;

        if (priceText != null)
            priceText.color = canAfford ? Color.black : Color.red;
    }

    private void OnItemClicked()
    {
        if (itemData == null || isPurchased) return;

        if (ShoppingManager.Instance == null)
        {
            Debug.LogError("ShoppingManager not found in scene!");
            return;
        }
        
        ShoppingManager.Instance.ShowItemConfirmation(itemData, 1);
    }

    public void MarkAsPurchased(int boughtQuantity)
    {
        isPurchased = true;
        purchasedQuantity += boughtQuantity;

        if (purchasedOverlay != null)
            purchasedOverlay.SetActive(true);

        if (buyButton != null)
            buyButton.interactable = false;
        
        UpdateOwnedQuantityUI();
    }

    private void UpdateOwnedQuantityUI()
    {
        if (ownedQuantityText != null)
        {
            ownedQuantityText.gameObject.SetActive(purchasedQuantity > 0);
            ownedQuantityText.text = $"x{purchasedQuantity} Owned";
        }
    }
}