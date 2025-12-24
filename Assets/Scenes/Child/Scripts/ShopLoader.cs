using UnityEngine;

public class ShopLoader : MonoBehaviour
{
    [Header("Database & Prefabs")]
    [Tooltip("Reference to the Item Database ScriptableObject")]
    public ItemDatabase itemDatabase; 

    [Tooltip("Prefab for each shop item UI (must have ShopItemButton)")]
    public GameObject shopItemPrefab;

    [Tooltip("Parent container (usually Content under Scroll View)")]
    public Transform shopContentParent; 

    private void Start()
    {
        Debug.Log("[ShopLoader] Start() called — attempting to load shop items...");
        LoadShopItems();
    }

    public void LoadShopItems()
    {
        Debug.Log("[ShopLoader] LoadShopItems() started...");

       

        var allItems = itemDatabase.GetAllItems();
        if (allItems == null)
        {
            Debug.LogError("❌ itemDatabase.GetAllItems() returned NULL!");
            return;
        }

        Debug.Log($"[ShopLoader] Found {allItems.Count} items in the database.");

        foreach (Transform child in shopContentParent)
        {
            Destroy(child.gameObject);
        }

        int spawnCount = 0;
        foreach (ItemData item in allItems)
        {
            if (item == null)
            {
                Debug.LogWarning("[ShopLoader] Encountered a NULL item in the database list!");
                continue;
            }

            Debug.Log($"[ShopLoader] Spawning UI for item: {item.itemName}");

            GameObject shopItemObj = Instantiate(shopItemPrefab, shopContentParent);
            if (shopItemObj == null)
            {
                Debug.LogError($"❌ Failed to instantiate prefab for {item.itemName}");
                continue;
            }

            ShopItemButton button = shopItemObj.GetComponent<ShopItemButton>();
            if (button == null)
            {
                Debug.LogError($"❌ The prefab {shopItemPrefab.name} is missing the ShopItemButton script!");
                continue;
            }

            button.SetItemData(item);
            spawnCount++;

            if (item.type == ItemType.Inconsumable && itemDatabase.IsPurchased(item))
            {
                Debug.Log($"[ShopLoader] Marking '{item.itemName}' as purchased (default qty 1).");
                button.MarkAsPurchased(1);
            }
        }

        Debug.Log($"✅ [ShopLoader] Successfully spawned {spawnCount} shop items into the UI.");
        Debug.Log($"[ShopLoader] ContentParent now has {shopContentParent.childCount} child objects.");
    }
}
