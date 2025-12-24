using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemDatabase", menuName = "Shop/Item Database")]
public class ItemDatabase : ScriptableObject
{
    [Header("All Shop Items")]
    [Tooltip("Master list of all items in the game.")]
    public List<ItemData> allItems = new List<ItemData>();

    [Header("Categories (auto-filled)")]
    [SerializeField] private List<ItemData> consumableItems = new List<ItemData>();
    [SerializeField] private List<ItemData> inconsumableItems = new List<ItemData>();

    public static ItemDatabase Instance { get; private set; }

    private Dictionary<string, bool> purchasedStates = new Dictionary<string, bool>();

    private const string PURCHASE_KEY_PREFIX = "Purchased_";

    private void OnEnable()
    {
        Instance = this;
        CategorizeItems();
        LoadPurchaseStates();
    }

    private void OnValidate()
    {
        CategorizeItems();
    }

    private void CategorizeItems()
    {
        consumableItems.Clear();
        inconsumableItems.Clear();

        foreach (ItemData item in allItems)
        {
            if (item == null) continue;

            if (item.type == ItemType.Consumable)
                consumableItems.Add(item);
            else if (item.type == ItemType.Inconsumable)
                inconsumableItems.Add(item);
        }
    }

    public List<ItemData> GetAllItems() => new List<ItemData>(allItems);

    public List<ItemData> GetItemsByType(ItemType type)
    {
        CategorizeItems();

        switch (type)
        {
            case ItemType.Consumable:
                return new List<ItemData>(consumableItems);
            case ItemType.Inconsumable:
                return new List<ItemData>(inconsumableItems);
            default:
                return new List<ItemData>();
        }
    }

    public ItemData GetItemByName(string itemName)
    {
        return allItems.Find(item => item != null && item.itemName == itemName);
    }

    public void MarkAsPurchased(ItemData item)
    {
        if (item == null || item.type != ItemType.Inconsumable) return;

        purchasedStates[item.itemName] = true;
        PlayerPrefs.SetInt(PURCHASE_KEY_PREFIX + item.itemName, 1);
        PlayerPrefs.Save();

        Debug.Log($"[ItemDatabase] Marked {item.itemName} as purchased.");
    }

    public bool IsPurchased(ItemData item)
    {
        if (item == null || item.type != ItemType.Inconsumable)
            return false;

        if (purchasedStates.ContainsKey(item.itemName))
            return purchasedStates[item.itemName];

        bool saved = PlayerPrefs.GetInt(PURCHASE_KEY_PREFIX + item.itemName, 0) == 1;

        if (!saved)
        {
            bool ownedOrEquipped = false;

            if (InventoryManager.Instance != null)
                ownedOrEquipped = InventoryManager.Instance.items.Exists(i => i.itemName == item.itemName);

            if (!ownedOrEquipped && AvatarClothingManager.Instance != null)
            {
                var eq = AvatarClothingManager.Instance;
                if ((eq.equippedData.head && eq.equippedData.head.itemName == item.itemName) ||
                    (eq.equippedData.top && eq.equippedData.top.itemName == item.itemName) ||
                    (eq.equippedData.bottom && eq.equippedData.bottom.itemName == item.itemName) ||
                    (eq.equippedData.shoes && eq.equippedData.shoes.itemName == item.itemName))
                {
                    ownedOrEquipped = true;
                }
            }

            if (ownedOrEquipped)
            {
                saved = true;
                PlayerPrefs.SetInt(PURCHASE_KEY_PREFIX + item.itemName, 1);
                PlayerPrefs.Save();
            }
        }

        purchasedStates[item.itemName] = saved;
        return saved;
    }

    private void LoadPurchaseStates()
    {
        purchasedStates.Clear();

        foreach (var item in inconsumableItems)
        {
            if (item == null) continue;

            bool purchased = PlayerPrefs.GetInt(PURCHASE_KEY_PREFIX + item.itemName, 0) == 1;
            purchasedStates[item.itemName] = purchased;
        }

        Debug.Log("[ItemDatabase] Loaded purchase states from PlayerPrefs.");
    }

    public void ClearAllPurchaseStates()
    {
        foreach (var key in new List<string>(purchasedStates.Keys))
        {
            PlayerPrefs.DeleteKey(PURCHASE_KEY_PREFIX + key);
        }
        purchasedStates.Clear();
        PlayerPrefs.Save();

        Debug.Log("[ItemDatabase] Cleared all purchase states.");
    }
}
 