using UnityEngine;
using UnityEngine.UI;

public class ShopCategoryButton : MonoBehaviour
{
    public ClothingCategory category;  

    private void Awake()
    {
       
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        UIShoppingPage.Instance.SetCategory(category);
    }
}
