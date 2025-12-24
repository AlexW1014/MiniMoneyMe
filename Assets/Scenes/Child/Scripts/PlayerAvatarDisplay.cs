using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerAvatarDisplay : MonoBehaviour
{
    private AudioManager audioManager;
    public static PlayerAvatarDisplay Instance { get; private set; }

    [Header("Body Objects (Toggle these)")]
    public GameObject normalBodyObject;
    public GameObject hungryBodyObject;

    [Header("Face System")]
    public SpriteRenderer faceRenderer;
    public Sprite happyFaceSprite;
    public Sprite sadFaceSprite;
    public Sprite normalFaceSprite;

    [Header("Clothing Renderers (Shared)")]
    public SpriteRenderer topRenderer;
    public SpriteRenderer bottomRenderer;
    public SpriteRenderer shoesRenderer;
    public SpriteRenderer headRenderer;

    [Header("Hair Renderers")]
    public SpriteRenderer backHairRenderer;
    public SpriteRenderer frontHairRenderer;

    [Header("Skin & Body Sprites")]
    public Sprite stillBody_FairSkin;
    public Sprite stillBody_LightSkin;
    public Sprite stillBody_BlackSkin;
    public SpriteRenderer stillBodyRenderer;

    [Header("Skin Animators")]
    public Animator avatarAnimator;
    public AnimatorOverrideController fairSkinAnimator;
    public AnimatorOverrideController lightSkinAnimator;
    public AnimatorOverrideController blackSkinAnimator;

    public string customizeSceneName = "Customize Avatar";

    private Sprite currentStillBodySprite;
    private Coroutine emotionCoroutine;

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        LoadAvatarFromPlayerPrefs();

        if (!IsAvatarCustomized())
        {
            SceneManager.LoadScene(customizeSceneName);
            return;
        }

        ApplyAvatarCustomization();
        EnsureRenderersEnabled();

        if (HungerManager.Instance != null)
        {
            HungerManager.Instance.OnHungerStateChanged += UpdateHungerVisuals;
            UpdateHungerVisuals(HungerManager.Instance.IsHungry);
        }
    }

    private void OnDestroy()
    {
        if (HungerManager.Instance != null)
            HungerManager.Instance.OnHungerStateChanged -= UpdateHungerVisuals;
    }

    public void UpdateClothing(ItemData item)
    {
        if (item == null) return;

        Sprite clothesSprite = item.icon;

        if (item.category == ClothingCategory.Top)
        {
            if (topRenderer) topRenderer.sprite = clothesSprite;
        }
        else if (item.category == ClothingCategory.Bottom)
        {
            if (bottomRenderer) bottomRenderer.sprite = clothesSprite;
        }
        else if (item.category == ClothingCategory.Shoes)
        {
            if (shoesRenderer) shoesRenderer.sprite = clothesSprite;
        }
        else if (item.category == ClothingCategory.Shoes)
        {
            if (headRenderer) headRenderer.sprite = clothesSprite;
        }

        EnsureRenderersEnabled();
    }

    private void EnsureRenderersEnabled()
    {
        if (topRenderer && topRenderer.sprite != null) topRenderer.enabled = true;
        if (bottomRenderer && bottomRenderer.sprite != null) bottomRenderer.enabled = true;
        if (shoesRenderer && shoesRenderer.sprite != null) shoesRenderer.enabled = true;
        if (frontHairRenderer && frontHairRenderer.sprite != null) frontHairRenderer.enabled = true;
        if (backHairRenderer && backHairRenderer.sprite != null) backHairRenderer.enabled = true;
    }

    public void UpdateHungerVisuals(bool isHungry)
    {
        if (emotionCoroutine != null) return;

        if (isHungry)
        {
            if (normalBodyObject) normalBodyObject.SetActive(false);
            if (hungryBodyObject) hungryBodyObject.SetActive(true);

            if (faceRenderer && sadFaceSprite) faceRenderer.sprite = sadFaceSprite;
            if (stillBodyRenderer && currentStillBodySprite) stillBodyRenderer.sprite = currentStillBodySprite;
        }
        else
        {
            if (normalBodyObject) normalBodyObject.SetActive(true);
            if (hungryBodyObject) hungryBodyObject.SetActive(false);

            if (faceRenderer && normalFaceSprite) faceRenderer.sprite = normalFaceSprite;
        }
    }

    public void TriggerEatingReaction()
    {
        if (emotionCoroutine != null) StopCoroutine(emotionCoroutine);
        emotionCoroutine = StartCoroutine(EatRoutine());
    }

    private IEnumerator EatRoutine()
    {
        if (normalBodyObject) normalBodyObject.SetActive(true);
        if (hungryBodyObject) hungryBodyObject.SetActive(false);

        if (faceRenderer && happyFaceSprite) faceRenderer.sprite = happyFaceSprite;

        yield return new WaitForSeconds(3f);

        if (faceRenderer && normalFaceSprite) faceRenderer.sprite = normalFaceSprite;
        emotionCoroutine = null;

        if (HungerManager.Instance != null && HungerManager.Instance.IsHungry)
            UpdateHungerVisuals(true);
    }

    private bool IsAvatarCustomized()
    {
        return AvatarData.SelectedSkinAnimator != null || PlayerPrefs.HasKey("SkinAnimatorID");
    }

    public void ApplyAvatarCustomization()
    {
        ApplySkinAnimator();
        ApplyHair();
        LoadStillBodyForSkin();
    }

    private void ApplySkinAnimator()
    {
        if (AvatarData.SelectedSkinAnimator != null && avatarAnimator != null)
            avatarAnimator.runtimeAnimatorController = AvatarData.SelectedSkinAnimator;
    }

    private void ApplyHair()
    {
        string hairstyleID = PlayerPrefs.GetString("HairstyleID");
        string hairColorID = PlayerPrefs.GetString("HairColorID");

        HairController hair = FindAnyObjectByType<HairController>();

        if (hair == null)
        {
            Debug.LogWarning("HairController not found in scene");
            return;
        }

        hair.GenerateHairSpritesFromIDs(hairstyleID, hairColorID);

        if (AvatarData.SelectedFrontHairSprite != null && frontHairRenderer != null)
        {
            frontHairRenderer.sprite = AvatarData.SelectedFrontHairSprite;
            frontHairRenderer.enabled = true;
        }

        if (AvatarData.SelectedBackHairSprite != null && backHairRenderer != null)
        {
            backHairRenderer.sprite = AvatarData.SelectedBackHairSprite;
            backHairRenderer.enabled = true;
        }
    }

    private void LoadStillBodyForSkin()
    {
        var skin = AvatarData.SelectedSkinAnimator;
        if (skin == fairSkinAnimator) currentStillBodySprite = stillBody_FairSkin;
        else if (skin == lightSkinAnimator) currentStillBodySprite = stillBody_LightSkin;
        else if (skin == blackSkinAnimator) currentStillBodySprite = stillBody_BlackSkin;
        if (stillBodyRenderer != null && currentStillBodySprite != null)
            stillBodyRenderer.sprite = currentStillBodySprite;
    }

    private void LoadAvatarFromPlayerPrefs()
    {
        if (AvatarData.SelectedSkinAnimator != null) return;
        Debug.Log("player prefs check+" + PlayerPrefs.HasKey("SkinAnimatorID").ToString());
        if (!PlayerPrefs.HasKey("SkinAnimatorID")) return;
        Debug.Log("Loading Avatar from PlayerPrefs...");

        string skinID = PlayerPrefs.GetString("SkinAnimatorID");
        string hairstyleID = PlayerPrefs.GetString("HairstyleID");
        string hairColorID = PlayerPrefs.GetString("HairColorID");

        Debug.Log($"Loading Avatar from PlayerPrefs: SkinID={skinID}, HairstyleID={hairstyleID}, HairColorID={hairColorID}");

        AvatarData.SelectedSkinAnimator = FindSkinAnimatorByName(skinID);

        HairController hair = FindAnyObjectByType<HairController>();
        if (hair != null)
        {
            hair.GenerateHairSpritesFromIDs(hairstyleID, hairColorID);

            if (AvatarData.SelectedFrontHairSprite != null && frontHairRenderer != null)
            {
                frontHairRenderer.sprite = AvatarData.SelectedFrontHairSprite;
                frontHairRenderer.enabled = true;
            }

            if (AvatarData.SelectedBackHairSprite != null && backHairRenderer != null)
            {
                backHairRenderer.sprite = AvatarData.SelectedBackHairSprite;
                backHairRenderer.enabled = true;
            }
        }
    }

    private AnimatorOverrideController FindSkinAnimatorByName(string name)
    {
        if (fairSkinAnimator != null && fairSkinAnimator.name == name) return fairSkinAnimator;
        if (lightSkinAnimator != null && lightSkinAnimator.name == name) return lightSkinAnimator;
        if (blackSkinAnimator != null && blackSkinAnimator.name == name) return blackSkinAnimator;
        return null;
    }
}
