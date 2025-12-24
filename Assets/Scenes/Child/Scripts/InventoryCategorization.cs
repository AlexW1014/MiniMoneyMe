using UnityEngine;
using UnityEngine.UI;

public class InventoryCategoryButton : MonoBehaviour
{
    public ClothingCategory category;
    public bool showAll = false;

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        if (UIInventoryPage.Instance != null)
        {
            UIInventoryPage.Instance.SetCategory(category, showAll);
        }
    }
}