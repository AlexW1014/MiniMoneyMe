using UnityEngine;

public class AvatarClothes : MonoBehaviour
{
    public SpriteRenderer headRenderer;
    public SpriteRenderer topRenderer;
    public SpriteRenderer bottomRenderer;
    public SpriteRenderer shoesRenderer;

    private void OnEnable()
    {
        if (AvatarClothingManager.Instance == null) return;

      
        AvatarClothingManager.Instance.headSlotRenderer = headRenderer;
        AvatarClothingManager.Instance.topSlotRenderer = topRenderer;
        AvatarClothingManager.Instance.bottomSlotRenderer = bottomRenderer;
        AvatarClothingManager.Instance.shoesSlotRenderer = shoesRenderer;

        AvatarClothingManager.Instance.RefreshEquippedClothing();

    }
}
