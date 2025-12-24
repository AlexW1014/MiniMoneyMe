using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class CustomizeCharacterSkinTone : MonoBehaviour
{
      private AudioManager audioManager;
    public static CustomizeCharacterSkinTone Instance { get; private set; }
    private UIDocument _document;
    private string Guardianid;
    private string guardianChild;
    private VisualElement chosenAvatar_HairPage;
    private VisualElement chosenAvatar_HairColorPage;
    private VisualElement skinPanel;
    private VisualElement hairPanel;
    private VisualElement hairColorPanel;
    private Button fairSkinBtn;
    private Button lightSkinBtn;
    private Button blackSkinBtn;
    private Button lanjutHairstyleBtn;
    private Button kembaliSkinBtn;
    private Button lanjutHairColorBtn;
    private Button kembaliHairstyleBtn;
    private Button LanjutHomeSceneBtn;
    public AnimatorOverrideController fairSkinAnimator;
    public Sprite fairSkinPreviewSprite;
    public AnimatorOverrideController lightSkinAnimator;
    public Sprite lightSkinPreviewSprite;
    public AnimatorOverrideController blackSkinAnimator;
    public Sprite blackSkinPreviewSprite;
    public Sprite defaultFrontHair;
    public Sprite defaultBackHair;
    private Sprite currentSkinPreviewSprite;
    private void Awake()
    {  
         audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
        guardianChild = StaticGuardianData.guardianChild;
        Guardianid = StaticGuardianData.guardianId;
        
        if (Instance == null) Instance = this; else { Destroy(gameObject); return; }
        _document = GetComponent<UIDocument>();

        if (_document == null) { Debug.LogError("UIDocument not found on this GameObject.", this); return; }
        var root = _document.rootVisualElement;

        if (root == null) { Debug.LogError("Root VisualElement is null for UIDocument.", this); return; }
        chosenAvatar_HairPage = root.Q<VisualElement>("Chosen_Avatar_Hair");
        chosenAvatar_HairColorPage = root.Q<VisualElement>("Chosen_Avatar_HairColor");
        skinPanel = root.Q<VisualElement>("Customize_SkinColor");
        hairPanel = root.Q<VisualElement>("Customize_HairStyle");
        hairColorPanel = root.Q<VisualElement>("Customize_HairColor");
        fairSkinBtn = root.Q<Button>("FairSkin_btn");
        lightSkinBtn = root.Q<Button>("LightSkin_btn");
        blackSkinBtn = root.Q<Button>("BlackSkin_btn");
        lanjutHairstyleBtn = root.Q<Button>("Lanjut_btn_Hairstyle");
        kembaliSkinBtn = root.Q<Button>("Kembali_btn_Skin");
        lanjutHairColorBtn = root.Q<Button>("Lanjut_btn_HairColor");
        kembaliHairstyleBtn = root.Q<Button>("Kembali_btn_Hairstyle");
        LanjutHomeSceneBtn = root.Q<Button>("Lanjut_btn_HomeScene");

        if (fairSkinBtn != null) fairSkinBtn.clicked += () => OnSkinChosen(fairSkinAnimator, fairSkinPreviewSprite);
        if (lightSkinBtn != null) lightSkinBtn.clicked += () => OnSkinChosen(lightSkinAnimator, lightSkinPreviewSprite);
        if (blackSkinBtn != null) blackSkinBtn.clicked += () => OnSkinChosen(blackSkinAnimator, blackSkinPreviewSprite);
        if (lanjutHairstyleBtn != null) lanjutHairstyleBtn.clicked += () => StartCoroutine(SwitchPanels(skinPanel, hairPanel));
        if (kembaliSkinBtn != null) kembaliSkinBtn.clicked += () => StartCoroutine(SwitchPanels(hairPanel, skinPanel));
        if (lanjutHairColorBtn != null) lanjutHairColorBtn.clicked += () => StartCoroutine(SwitchPanels(hairPanel, hairColorPanel));
        if (kembaliHairstyleBtn != null) kembaliHairstyleBtn.clicked += () => StartCoroutine(SwitchPanels(hairColorPanel, hairPanel));
        if (LanjutHomeSceneBtn != null) LanjutHomeSceneBtn.clicked += () => StartCoroutine(FadeOutAndLoadScene());

        var namaAnak = _document.rootVisualElement.Q<VisualElement>("NamaAnak") as Label;
        if(namaAnak!=null) namaAnak.text= guardianChild;
        var namaAnak2 = _document.rootVisualElement.Q<VisualElement>("NamaAnak2") as Label;
        if(namaAnak2!=null) namaAnak2.text= guardianChild;
        var namaAnak3 = _document.rootVisualElement.Q<VisualElement>("NamaAnak3") as Label;
        if(namaAnak3!=null) namaAnak3.text= guardianChild;

        ShowPanel(skinPanel);
        HidePanel(hairPanel, true);
        HidePanel(hairColorPanel, true);
    }
    private void OnSkinChosen(AnimatorOverrideController newAnimator, Sprite previewSprite)
    {
        SetSkin(newAnimator, previewSprite);
    }
    private void SetSkin(AnimatorOverrideController newAnimator, Sprite previewSprite)
    {
        if (newAnimator == null || previewSprite == null) return;
        currentSkinPreviewSprite = previewSprite;
      
        AvatarData.SelectedSkinAnimator = newAnimator;
        
        if (AvatarData.SelectedFrontHairSprite == null)
        {
            AvatarData.SelectedFrontHairSprite = defaultFrontHair;
            AvatarData.SelectedBackHairSprite = defaultBackHair;
        }
        if (chosenAvatar_HairPage != null) chosenAvatar_HairPage.style.backgroundImage = new StyleBackground(previewSprite.texture);
        if (chosenAvatar_HairColorPage != null) chosenAvatar_HairColorPage.style.backgroundImage = new StyleBackground(previewSprite.texture);
    }
    private IEnumerator SwitchPanels(VisualElement from, VisualElement to)
    {
        yield return FadeOut(from);
        ShowPanel(to);
        if (currentSkinPreviewSprite != null)
        {
            if (chosenAvatar_HairPage != null) chosenAvatar_HairPage.style.backgroundImage = new StyleBackground(currentSkinPreviewSprite.texture);
            if (chosenAvatar_HairColorPage != null) chosenAvatar_HairColorPage.style.backgroundImage = new StyleBackground(currentSkinPreviewSprite.texture);
        }
    }
    private void ShowPanel(VisualElement panel)
    {     audioManager.playSFX(audioManager.click);
        if (panel == null) return;
        panel.style.display = DisplayStyle.Flex;
        panel.style.opacity = 1;
    }
    private void HidePanel(VisualElement panel, bool instant = false)
    {
        if (panel == null) return;
        if (instant) { panel.style.display = DisplayStyle.None; panel.style.opacity = 0; } else { StartCoroutine(FadeOut(panel)); }
    }
    private IEnumerator FadeOut(VisualElement panel)
    {
        if (panel == null) yield break;
        float t = panel.style.opacity.value;
        while (t > 0)
        {
            t -= Time.deltaTime * 2f;
            panel.style.opacity = t;
            yield return null;
        }
        panel.style.display = DisplayStyle.None;
        panel.style.opacity = 0;
    }
    private IEnumerator FadeOutAndLoadScene()
    
    {   
        
        if (hairColorPanel != null) yield return FadeOut(hairColorPanel);
        if (AvatarData.SelectedSkinAnimator != null) PlayerPrefs.SetString("SkinAnimatorID", AvatarData.SelectedSkinAnimator.name);
        if (!string.IsNullOrEmpty(AvatarData.SelectedHairstyleID)) PlayerPrefs.SetString("HairstyleID", AvatarData.SelectedHairstyleID);
        
        if (!string.IsNullOrEmpty(AvatarData.SelectedHairColorID)) PlayerPrefs.SetString("HairColorID", AvatarData.SelectedHairColorID);
        PlayerPrefs.Save();
        Debug.Log(PlayerPrefs.HasKey("SkinAnimatorID"));    
         Debug.Log(PlayerPrefs.HasKey("HairstyleID"));    
            Debug.Log(PlayerPrefs.HasKey("HairColorID"));
         Debug.Log("+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++");    
        Debug.Log(AvatarData.SelectedHairColorID.ToString());    
        SceneManager.LoadScene("Child Tutorial");
        yield break;
    }
}
