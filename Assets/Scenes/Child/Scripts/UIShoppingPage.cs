using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UIShoppingPage : MonoBehaviour
{
    public static UIShoppingPage Instance;

    [Header("References")]
    [SerializeField] private GameObject shopItemButtonPrefab;
    [SerializeField] private RectTransform content;
    [SerializeField] private ItemDatabase itemDatabase;

    [Header("Filtering")]
    [SerializeField] private ClothingCategory currentCategory = ClothingCategory.All;
    [SerializeField] private ItemType filterByType = ItemType.Consumable;
    [SerializeField] private bool showAllTypes = true;

    private List<GameObject> spawnedItemButtons = new List<GameObject>();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        if (itemDatabase == null)
            itemDatabase = ItemDatabase.Instance;

        InitializeShop();
        StartCoroutine(EnableContentSizeFitterAfterFrame());
    }

    public void InitializeShop()
    {
        ClearExistingItems();
        CreateItemButtons();
    }

    private void ClearExistingItems()
    {
        foreach (GameObject item in spawnedItemButtons)
            if (item != null)
                Destroy(item);

        spawnedItemButtons.Clear();
    }

    private void CreateItemButtons()
    {
        if (itemDatabase == null || shopItemButtonPrefab == null) return;

        List<ItemData> itemsToShow = GetItemsToDisplay();

        foreach (ItemData item in itemsToShow)
        {
            if (item == null) continue;

            GameObject buttonObj = Instantiate(shopItemButtonPrefab, content);
            buttonObj.transform.localScale = Vector3.one;

            ShopItemButton itemButton = buttonObj.GetComponent<ShopItemButton>();
            if (itemButton != null)
            {
                itemButton.SetItemData(item);

                // Mark purchased items
                if (itemDatabase.IsPurchased(item))
                    itemButton.MarkAsPurchased(1);
            }

            spawnedItemButtons.Add(buttonObj);
        }
    }

    public void SetCategory(ClothingCategory category)
    {
        currentCategory = category;
        RefreshShopUI();
    }
    private List<ItemData> GetItemsToDisplay()
    {
        List<ItemData> allItems = itemDatabase.GetAllItems();

        if (currentCategory != ClothingCategory.All)
            allItems = allItems.FindAll(item => item.category == currentCategory);

        if (!showAllTypes)
            allItems = allItems.FindAll(item => item.type == filterByType);

        return allItems;
    }

    private void CreateItemButton(ItemData itemData)
    {
        GameObject buttonObj = Instantiate(shopItemButtonPrefab, content);
        buttonObj.transform.localScale = Vector3.one;

        ShopItemButton itemButton = buttonObj.GetComponent<ShopItemButton>();
        if (itemButton != null)
            itemButton.SetItemData(itemData);

        spawnedItemButtons.Add(buttonObj);
    }

    public void Show()
    {
        gameObject.SetActive(true);
        RefreshShopUI();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void RefreshShopUI()
    {
        InitializeShop();
    }

    IEnumerator EnableContentSizeFitterAfterFrame()
    {
        var fitter = content.GetComponent<ContentSizeFitter>();
        if (fitter != null)
        {
            fitter.enabled = false;
            yield return null;
            fitter.enabled = true;
        }
    }
}
