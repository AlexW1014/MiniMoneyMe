using UnityEngine;
using UnityEngine.UIElements;

public class HairController : MonoBehaviour
{
    public UIDocument uiDocument;

    public Sprite boyStraightFrontBase;
    public Sprite boyCurlyFrontBase;
    public Sprite girlStraightFrontBase;
    public Sprite girlStraightBackBase;
    public Sprite girlCurlyFrontBase;
    public Sprite girlCurlyBackBase;

    public Sprite boyStraightFront_Brown;
    public Sprite boyStraightFront_Black;
    public Sprite boyCurlyFront_Brown;
    public Sprite boyCurlyFront_Black;

    public Sprite girlStraightFront_Brown;
    public Sprite girlStraightBack_Brown;
    public Sprite girlStraightFront_Black;
    public Sprite girlStraightBack_Black;

    public Sprite girlCurlyFront_Brown;
    public Sprite girlCurlyBack_Brown;
    public Sprite girlCurlyFront_Black;
    public Sprite girlCurlyBack_Black;

    private VisualElement frontHair_HairstylePage;
    private VisualElement backHair_HairstylePage;
    private VisualElement frontHair_HaircolorPage;
    private VisualElement backHair_HaircolorPage;

    private bool hasUI = false;

    private void Awake()
    {
        if (uiDocument != null)
        {
            var root = uiDocument.rootVisualElement;
            if (root != null)
            {
                hasUI = true;

                frontHair_HairstylePage = root.Q<VisualElement>("FrontHair_Hairstyle");
                backHair_HairstylePage = root.Q<VisualElement>("BackHair_Hairstyle");
                frontHair_HaircolorPage = root.Q<VisualElement>("FrontHair_Haircolor");
                backHair_HaircolorPage = root.Q<VisualElement>("BackHair_Haircolor");

                Button btnStraightBoy = root.Q<Button>("StraightHair_boy");
                if (btnStraightBoy != null) btnStraightBoy.clicked += () => SelectHairstyle("StraightHair_boy");

                Button btnCurlyBoy = root.Q<Button>("CurlyHair_boy");
                if (btnCurlyBoy != null) btnCurlyBoy.clicked += () => SelectHairstyle("CurlyHair_boy");

                Button btnStraightGirl = root.Q<Button>("StraightHair_girl");
                if (btnStraightGirl != null) btnStraightGirl.clicked += () => SelectHairstyle("StraightHair_girl");

                Button btnCurlyGirl = root.Q<Button>("CurlyHair_girl");
                if (btnCurlyGirl != null) btnCurlyGirl.clicked += () => SelectHairstyle("CurlyHair_girl");

                Button btnBrown = root.Q<Button>("BrownHair_Button");
                if (btnBrown != null) btnBrown.clicked += () => SelectColor("Brown");

                Button btnBlack = root.Q<Button>("BlackHair_Button");
                if (btnBlack != null) btnBlack.clicked += () => SelectColor("Black");
            }
        }

        if (string.IsNullOrEmpty(AvatarData.SelectedHairstyleID))
            SelectHairstyle("StraightHair_boy");
        else
        {
            ApplyHairstyleToBasePreview(AvatarData.SelectedHairstyleID);
            ApplyFinalColoredHairToPreviews();
        }

        if (string.IsNullOrEmpty(AvatarData.SelectedHairColorID))
            SelectColor("Brown");
    }

    private void SelectHairstyle(string id)
    {
        AvatarData.SelectedHairstyleID = id;

        if (string.IsNullOrEmpty(AvatarData.SelectedHairColorID))
            AvatarData.SelectedHairColorID = "Brown";

        ApplyHairstyleToBasePreview(id);
        ApplyFinalColoredHairToPreviews();
    }

    private void ApplyHairstyleToBasePreview(string hairstyleId)
    {
        if (!hasUI) return; // MAIN SCENE FIX

        Sprite frontBase = null;
        Sprite backBase = null;

        switch (hairstyleId)
        {
            case "StraightHair_boy":
                frontBase = boyStraightFrontBase; 
                break;

            case "CurlyHair_boy":
                frontBase = boyCurlyFrontBase; 
                break;

            case "StraightHair_girl":
                frontBase = girlStraightFrontBase;
                backBase = girlStraightBackBase;
                break;

            case "CurlyHair_girl":
                frontBase = girlCurlyFrontBase;
                backBase = girlCurlyBackBase;
                break;
        }

        frontHair_HairstylePage.style.backgroundImage = frontBase != null ? new StyleBackground(frontBase) : null;
        backHair_HairstylePage.style.backgroundImage = backBase != null ? new StyleBackground(backBase) : null;
    }

    private void SelectColor(string colorId)
    {
        AvatarData.SelectedHairColorID = colorId;
        ApplyFinalColoredHairToPreviews();
    }

    private void ApplyFinalColoredHairToPreviews()
    {
        Sprite front = null;
        Sprite back = null;

        string style = AvatarData.SelectedHairstyleID;
        string color = AvatarData.SelectedHairColorID;

        if (string.IsNullOrEmpty(style) || string.IsNullOrEmpty(color))
            return;

        switch (style)
        {
            case "StraightHair_girl":
                front = (color == "Brown") ? girlStraightFront_Brown : girlStraightFront_Black;
                back  = (color == "Brown") ? girlStraightBack_Brown  : girlStraightBack_Black;
                break;

            case "CurlyHair_girl":
                front = (color == "Brown") ? girlCurlyFront_Brown : girlCurlyFront_Black;
                back  = (color == "Brown") ? girlCurlyBack_Brown  : girlCurlyBack_Black;
                break;

            case "StraightHair_boy":
                front = (color == "Brown") ? boyStraightFront_Brown : boyStraightFront_Black;
                break;

            case "CurlyHair_boy":
                front = (color == "Brown") ? boyCurlyFront_Brown : boyCurlyFront_Black;
                break;
        }

        AvatarData.SelectedFrontHairSprite = front;
        AvatarData.SelectedBackHairSprite = back;

        if (!hasUI) return; // MAIN SCENE FIX

        frontHair_HaircolorPage.style.backgroundImage = front != null ? new StyleBackground(front) : null;
        backHair_HaircolorPage.style.backgroundImage = back != null ? new StyleBackground(back) : null;

        frontHair_HairstylePage.style.backgroundImage = front != null ? new StyleBackground(front) : null;
        backHair_HairstylePage.style.backgroundImage = back != null ? new StyleBackground(back) : null;
    }

    public void GenerateHairSpritesFromIDs(string hairstyle, string color)
    {
        Sprite front = null;
        Sprite back = null;

        switch (hairstyle)
        {
            case "StraightHair_girl":
                front = (color == "Brown") ? girlStraightFront_Brown : girlStraightFront_Black;
                back  = (color == "Brown") ? girlStraightBack_Brown  : girlStraightBack_Black;
                break;

            case "CurlyHair_girl":
                front = (color == "Brown") ? girlCurlyFront_Brown : girlCurlyFront_Black;
                back  = (color == "Brown") ? girlCurlyBack_Brown  : girlCurlyBack_Black;
                break;

            case "StraightHair_boy":
                front = (color == "Brown") ? boyStraightFront_Brown : boyStraightFront_Black;
                break;

            case "CurlyHair_boy":
                front = (color == "Brown") ? boyCurlyFront_Brown : boyCurlyFront_Black;
                break;
        }

        AvatarData.SelectedFrontHairSprite = front;
        AvatarData.SelectedBackHairSprite = back;
    }
}
