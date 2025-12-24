using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AvatarClothingManager : MonoBehaviour
{
    public static AvatarClothingManager Instance;
     private AudioManager audioManager;
    public SpriteRenderer headSlotRenderer;
    public SpriteRenderer topSlotRenderer;
    public SpriteRenderer bottomSlotRenderer;
    public SpriteRenderer shoesSlotRenderer;

    public ItemDatabase itemDatabase;

    private ItemData equippedHead;
    private ItemData equippedTop;
    private ItemData equippedBottom;
    private ItemData equippedShoes;

    [System.Serializable]
    public class EquippedData
    {
        public ItemData head;
        public ItemData top;
        public ItemData bottom;
        public ItemData shoes;
    }

    public EquippedData equippedData = new EquippedData();

    private void Awake()
    {   audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
        
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        StartCoroutine(DelayedInit());
    }

    private IEnumerator DelayedInit()
    {
        yield return new WaitForSeconds(0.2f); 
        if (itemDatabase == null)
            itemDatabase = ItemDatabase.Instance;

        LoadEquippedClothing();
        RefreshEquippedClothing();
    }


    private void InitializeClothing()
    {
        LoadEquippedClothing();
        RefreshEquippedClothing();
    }


    public void EquipClothing(ItemData clothingItem)
    {
        if (clothingItem == null || clothingItem.type != ItemType.Inconsumable)
        {
            Debug.LogWarning("[AvatarClothingManager] Invalid clothing item passed.");
            return;
        }

        switch (clothingItem.slot)
        {
            case ClothingSlot.Head:
                HandleEquip(ref equippedHead, headSlotRenderer, clothingItem, "Head");
                audioManager.playSFX(audioManager.OutfitChangedAudio);
                break;
            case ClothingSlot.Top:
                HandleEquip(ref equippedTop, topSlotRenderer, clothingItem, "Top");
                audioManager.playSFX(audioManager.OutfitChangedAudio);
                break;
            case ClothingSlot.Bottom:
                HandleEquip(ref equippedBottom, bottomSlotRenderer, clothingItem, "Bottom");
                audioManager.playSFX(audioManager.OutfitChangedAudio);
                break;
            case ClothingSlot.Shoes:
                HandleEquip(ref equippedShoes, shoesSlotRenderer, clothingItem, "Shoes");
                audioManager.playSFX(audioManager.OutfitChangedAudio);
                break;
        }
    }

    private void HandleEquip(ref ItemData equippedItem, SpriteRenderer slotRenderer, ItemData newItem, string slotName)
    {
        if (slotRenderer == null) return;

        if (equippedItem == newItem)
        {
            equippedItem = null;
            slotRenderer.sprite = null;
            slotRenderer.color = new Color(1, 1, 1, 0);
            Debug.Log($"Unequipped {newItem.itemName}");
        }
        else
        {
            equippedItem = newItem;
            slotRenderer.sprite = newItem.clothingSprite != null ? newItem.clothingSprite : newItem.icon;
            slotRenderer.color = Color.white;
            slotRenderer.sortingLayerName = "Clothing";
            slotRenderer.sortingOrder = GetSortingOrder(slotName);
            Debug.Log($"Equipped {newItem.itemName}");
        }

        equippedData.head = equippedHead;
        equippedData.top = equippedTop;
        equippedData.bottom = equippedBottom;
        equippedData.shoes = equippedShoes;

        SaveEquippedClothing();
    }

    private int GetSortingOrder(string slotName)
    {
        switch (slotName)
        {
            case "Head": return 3;
            case "Top": return 2;
            case "Bottom": return 1;
            case "Shoes": return 0;
            default: return 0;
        }
    }

    public void SaveEquippedClothing()
    {
        if (equippedHead)
            PlayerPrefs.SetString("Equipped_Head", equippedHead.itemName);
        else
            PlayerPrefs.DeleteKey("Equipped_Head");

        if (equippedTop)
            PlayerPrefs.SetString("Equipped_Top", equippedTop.itemName);
        else
            PlayerPrefs.DeleteKey("Equipped_Top");

        if (equippedBottom)
            PlayerPrefs.SetString("Equipped_Bottom", equippedBottom.itemName);
        else
            PlayerPrefs.DeleteKey("Equipped_Bottom");

        if (equippedShoes)
            PlayerPrefs.SetString("Equipped_Shoes", equippedShoes.itemName);
        else
            PlayerPrefs.DeleteKey("Equipped_Shoes");

        PlayerPrefs.Save();
        Debug.Log("Saved equipped clothing!");
    }

    public void LoadEquippedClothing()
    {
        if (itemDatabase == null)
        {
            Debug.LogWarning("[AvatarClothingManager] ItemDatabase not assigned!");
            return;
        }

        string headName = PlayerPrefs.GetString("Equipped_Head", "");
        string topName = PlayerPrefs.GetString("Equipped_Top", "");
        string bottomName = PlayerPrefs.GetString("Equipped_Bottom", "");
        string shoesName = PlayerPrefs.GetString("Equipped_Shoes", "");

        equippedHead = !string.IsNullOrEmpty(headName) ? itemDatabase.GetItemByName(headName) : null;
        equippedTop = !string.IsNullOrEmpty(topName) ? itemDatabase.GetItemByName(topName) : null;
        equippedBottom = !string.IsNullOrEmpty(bottomName) ? itemDatabase.GetItemByName(bottomName) : null;
        equippedShoes = !string.IsNullOrEmpty(shoesName) ? itemDatabase.GetItemByName(shoesName) : null;

        equippedData.head = equippedHead;
        equippedData.top = equippedTop;
        equippedData.bottom = equippedBottom;
        equippedData.shoes = equippedShoes;

    Debug.Log($"[Avatar] Loaded items: Head={headName}, Top={topName}, Bottom={bottomName}, Shoes={shoesName}");

        Debug.Log("Loaded  clothing from PlayerPrefs.");
    }


    public void RefreshEquippedClothing()
    {
        ApplyEquippedSprites();
    }

    private void ApplyEquippedSprites()
    {
        ApplyClothingSprite(headSlotRenderer, equippedHead, "Head");
        ApplyClothingSprite(topSlotRenderer, equippedTop, "Top");
        ApplyClothingSprite(bottomSlotRenderer, equippedBottom, "Bottom");
        ApplyClothingSprite(shoesSlotRenderer, equippedShoes, "Shoes");
    }

    private void ApplyClothingSprite(SpriteRenderer slotRenderer, ItemData item, string slotName)
    {
        if (slotRenderer == null) return;

        if (item != null)
        {
            slotRenderer.sprite = item.clothingSprite != null ? item.clothingSprite : item.icon;
            slotRenderer.color = Color.white;
            slotRenderer.sortingLayerName = "Clothing";
            slotRenderer.sortingOrder = GetSortingOrder(slotName);
        }
        else
        {
            slotRenderer.sprite = null;
            slotRenderer.color = new Color(1, 1, 1, 0);
        }
    }


    public void ClearAll()
    {
        equippedHead = equippedTop = equippedBottom = equippedShoes = null;
        PlayerPrefs.DeleteKey("Equipped_Head");
        PlayerPrefs.DeleteKey("Equipped_Top");
        PlayerPrefs.DeleteKey("Equipped_Bottom");
        PlayerPrefs.DeleteKey("Equipped_Shoes");
        PlayerPrefs.Save();

        ApplyEquippedSprites();

    }
}
