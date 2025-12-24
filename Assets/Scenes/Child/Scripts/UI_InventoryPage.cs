using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIInventoryPage : MonoBehaviour
{
    [SerializeField] private GameObject ItemUI_Inventory; 
    [SerializeField] private RectTransform Content;
    
     [Header("Scroll View Reference")]
    [SerializeField] private ScrollRect inventoryScrollRect; 

    public static UIInventoryPage Instance;
    private List<UIInventoryItem> listOfUIItems = new List<UIInventoryItem>();

    private ClothingCategory selectedCategory;
    private bool showAllItems = true;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
     
    private void Start()
    {
        InitializeInventoryUI();
        StartCoroutine(EnableContentSizeFitterAfterFrame());
    }

    public void SetCategory(ClothingCategory category, bool showAll)
    {
        selectedCategory = category;
        showAllItems = showAll;
        InitializeInventoryUI();
    }

    public void InitializeInventoryUI()
    {
        foreach (UIInventoryItem item in listOfUIItems)
        {
            if (item != null)
                Destroy(item.gameObject);
        }
        listOfUIItems.Clear();
        
        if (InventoryManager.Instance == null) return;
        List<ItemData> currentItems = InventoryManager.Instance.items;

        for (int i = 0; i < currentItems.Count; i++)
        {
            ItemData itemData = currentItems[i];

            if (showAllItems || itemData.category == selectedCategory)
            {
                GameObject uiItemObj = Instantiate(ItemUI_Inventory, Content);
                uiItemObj.transform.localScale = Vector3.one;

                UIInventoryItem uiItem = uiItemObj.GetComponent<UIInventoryItem>();
                uiItem.SetData(itemData);

                listOfUIItems.Add(uiItem);
            }
        }
    }

    public void Show()
    {
        gameObject.SetActive(true);
        InitializeInventoryUI();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void RefreshInventoryUI()
    {
        InitializeInventoryUI();
    }

    private IEnumerator EnableContentSizeFitterAfterFrame()
    {
        if (Content.GetComponent<ContentSizeFitter>() != null)
        {
            Content.GetComponent<ContentSizeFitter>().enabled = false;
            yield return null;
            Content.GetComponent<ContentSizeFitter>().enabled = true;
        }
    }
}