using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIInventoryItem : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private TextMeshProUGUI amountText;
    [SerializeField] private Button equipButton; 
    private ItemData currentItem;

    public void SetData(ItemData item)
    {
        currentItem = item;

       
        if (iconImage != null)
            iconImage.sprite = item.icon;

        
        if (itemNameText != null)
            itemNameText.text = item.itemName;

     
        if (amountText != null)
            amountText.text = item.type == ItemType.Consumable ? item.amount.ToString() : "";

        if (equipButton != null)
        {
            equipButton.interactable = true;
            equipButton.onClick.RemoveAllListeners();
            equipButton.onClick.AddListener(OnClickEquip);
        }
    }

    private void OnClickEquip()
    {
        if (currentItem == null) return;

        if (currentItem.type == ItemType.Inconsumable)
        {
            if (AvatarClothingManager.Instance != null)
            {
                AvatarClothingManager.Instance.EquipClothing(currentItem);
            }
        }
        else if (currentItem.type == ItemType.Consumable)
        {
            if (InventoryConfirmationPanel.Instance != null)
            {
                InventoryConfirmationPanel.Instance.Show(currentItem, () =>
                {
                    RefreshAmount();
                });
            }
            else
            {
                Debug.LogError("InventoryConfirmationPanel Instance not found in scene!");
            }
        }
    }

    private void RefreshAmount()
    {
        if (currentItem == null) return;

        if (currentItem.amount <= 0)
        {
            Clear();
        }
        else
        {
            if (amountText != null)
                amountText.text = currentItem.amount.ToString();
        }
    }

    public void Clear()
    {
        currentItem = null;

        if (iconImage != null) iconImage.sprite = null;
        if (itemNameText != null) itemNameText.text = string.Empty;
        if (amountText != null) amountText.text = string.Empty;
        
        if (equipButton != null) equipButton.interactable = false;
    }
}