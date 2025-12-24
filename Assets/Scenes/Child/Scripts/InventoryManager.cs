using UnityEngine;
using System.Collections.Generic;
using System;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;
    
    public List<ItemData> items = new List<ItemData>();
    
    private const string CONSUMABLE_SAVE_KEY = "Inventory_Consumables_Save";

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        items.Clear();

        LoadConsumables();

        foreach (var item in ItemDatabase.Instance.GetItemsByType(ItemType.Inconsumable))
        {
            if (ItemDatabase.Instance.IsPurchased(item))
            {
                AddItemToInternalList(item, 1);
            }
        }

        RefreshUI();
    }

    public void AddItem(ItemData newItem, int quantity = 1)
    {
        AddItemToInternalList(newItem, quantity);

        if (newItem.type == ItemType.Consumable)
        {
            SaveConsumables();
        }

        RefreshUI();
    }

    private void AddItemToInternalList(ItemData newItem, int quantity)
    {
        ItemData existing = items.Find(i => i.itemName == newItem.itemName && i.type == newItem.type);

        if (existing != null && existing.type == ItemType.Consumable)
        {
            existing.amount += quantity;
        }
        else if (existing == null)
        {
            ItemData clone = ScriptableObject.Instantiate(newItem);
            clone.name = newItem.itemName; 
            clone.amount = (clone.type == ItemType.Consumable ? quantity : 1);
            items.Add(clone);
        }
    }

    public void ConsumeItem(ItemData item)
    {
        if (item == null || item.type != ItemType.Consumable) return;

        item.amount--;

        if (item.amount <= 0)
        {
            items.Remove(item);
        }

        SaveConsumables();
        RefreshUI();
    }

    public void RemoveItem(ItemData item)
    {
        if (item == null) return;

        if (items.Contains(item))
        {
            items.Remove(item);
            if (item.type == ItemType.Consumable) SaveConsumables();
            RefreshUI();
        }
    }

    private void RefreshUI()
    {
        if (UIInventoryPage.Instance != null)
            UIInventoryPage.Instance.RefreshInventoryUI();
    }

    private void SaveConsumables()
    {
        InventorySaveWrapper wrapper = new InventorySaveWrapper();
        
        foreach (var item in items)
        {
            if (item.type == ItemType.Consumable)
            {
                wrapper.savedItems.Add(new InventoryItemSaveData 
                { 
                    itemName = item.itemName, 
                    amount = item.amount 
                });
            }
        }

        string json = JsonUtility.ToJson(wrapper);
        PlayerPrefs.SetString(CONSUMABLE_SAVE_KEY, json);
        PlayerPrefs.Save();
    }

    private void LoadConsumables()
    {
        if (PlayerPrefs.HasKey(CONSUMABLE_SAVE_KEY))
        {
            string json = PlayerPrefs.GetString(CONSUMABLE_SAVE_KEY);
            InventorySaveWrapper wrapper = JsonUtility.FromJson<InventorySaveWrapper>(json);

            if (wrapper != null && wrapper.savedItems != null)
            {
                foreach (var savedData in wrapper.savedItems)
                {
                    ItemData dbItem = ItemDatabase.Instance.GetItemByName(savedData.itemName);
                    if (dbItem != null)
                    {
                        AddItemToInternalList(dbItem, savedData.amount);
                    }
                }
            }
        }
    }
}

[Serializable]
public class InventorySaveWrapper
{
    public List<InventoryItemSaveData> savedItems = new List<InventoryItemSaveData>();
}

[Serializable]
public class InventoryItemSaveData
{
    public string itemName;
    public int amount;
}