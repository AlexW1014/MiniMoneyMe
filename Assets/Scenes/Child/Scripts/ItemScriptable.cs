using UnityEngine;

public enum ItemType
{
    Consumable,
    Inconsumable
}

public enum ClothingSlot
{
    None,
    Head,
    Top,
    Bottom,
    Shoes
}

public enum ClothingCategory
{
    All,
    Top,
    Bottom,
    Head,
    Shoes,
    Consumable 
}

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    [Header("Basic Info")]
    public string itemName;
    public Sprite icon;
    public ItemType type;
    public int amount = 1;       
    public int price = 0;    

    [Header("Clothing (if Inconsumable)")]
    public ClothingSlot slot = ClothingSlot.None;  
    public Sprite clothingSprite;                   

    [Header("Shop Category")]
    public ClothingCategory category;

}
