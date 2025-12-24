using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using System;



public class LearningMaterial2 : MonoBehaviour
{
    private AudioManager audioManager;
    private FirebaseAuth auth;
    private DatabaseReference dbRef;   
    private DataSnapshot currentFundSnapshot;
    private string guardianId = StaticGuardianData.guardianId;
    private string guardianChild = StaticGuardianData.guardianChild;
    private Label CoinLabel;
    private bool rewardClaimed;


    private async Task InitializeFirebase()
    {
        // var dependencyStatus = await FirebaseApp.CheckAndFixDependenciesAsync();
        // if (dependencyStatus == DependencyStatus.Available)
        // {
            auth = FirebaseAuth.DefaultInstance;
            dbRef = FirebaseDatabase.DefaultInstance.RootReference;
        //     Debug.Log("Firebase initialized successfully!");
        // }
        // else
        // {
        //     Debug.LogError("Firebase dependencies unresolved: " + dependencyStatus);
        // }
    }


    private Button BackButton;
    private Button NextButton;
    private Button TutorialBackButton;
    private Button FinishedNextButton;
    private Button FinishedBackButton;


    private VisualElement root;
    private UIDocument uiDocument;
    private VisualElement s1;
    private VisualElement s2;
    private VisualElement s3;

    private VisualElement FinishedModal;



    async void OnEnable()
    {

        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
        rewardClaimed = PlayerPrefsCheckClaimed.GetBool("Learning2RewardClaimed", false);
        if (!rewardClaimed)
        {
            await InitializeFirebase();
            currentFundSnapshot = await dbRef.Child("users").Child(StaticGuardianData.guardianId).Child("children").Child(StaticGuardianData.guardianChild).Child("wallet").Child("normal_fund").GetValueAsync();

        }


        uiDocument = GetComponent<UIDocument>();
        root = uiDocument.rootVisualElement;
        CoinLabel = root.Q<Label>("CoinLabel");

        s1 = root.Q<VisualElement>("1");
        s2 = root.Q<VisualElement>("2");
        s3 = root.Q<VisualElement>("3");
        FinishedModal = root.Q<VisualElement>("FinishedModal");

        BackButton = root.Q<Button>("BackButton");
        NextButton = root.Q<Button>("NextButton");
        TutorialBackButton = root.Q<Button>("TutorialBackButton");
        FinishedNextButton = root.Q<Button>("FinishedNextButton");
        FinishedBackButton = root.Q<Button>("FinishedBackButton");
        

        BackButton.RegisterCallback<ClickEvent>(ev => OnBackButtonClicked());
        NextButton.RegisterCallback<ClickEvent>(ev => OnNextButtonClicked());
        TutorialBackButton.RegisterCallback<ClickEvent>(ev => OnTutorialBackButtonClicked());
        FinishedNextButton.RegisterCallback<ClickEvent>(ev => OnFinishedNextButtonClicked());
        FinishedBackButton.RegisterCallback<ClickEvent>(ev => OnFinishedBackButtonClicked());

        s1.style.display = DisplayStyle.Flex;
        s2.style.display = DisplayStyle.None;
        s3.style.display = DisplayStyle.None;
        
        if (!rewardClaimed)
        {
            CoinLabel.style.display = DisplayStyle.Flex;
        }
    }

    public static class PlayerPrefsCheckClaimed
    {
        public static void SetBool(string key, bool value)
        {
            PlayerPrefs.SetInt(key, value ? 1 : 0);
            PlayerPrefs.Save();
        }

        public static bool GetBool(string key, bool defaultValue = false)
        {
            return PlayerPrefs.GetInt(key, defaultValue ? 1 : 0) == 1;
        }
    }
    void OnBackButtonClicked()
    {    audioManager.playSFX(audioManager.click);
        if (s3.style.display == DisplayStyle.Flex)
        {
            s2.style.display = DisplayStyle.Flex;
            s3.style.display = DisplayStyle.None;
        }
        else if (s2.style.display == DisplayStyle.Flex)
        {
            s1.style.display = DisplayStyle.Flex;
            s2.style.display = DisplayStyle.None;
        }
    }
    void OnNextButtonClicked()
    {    audioManager.playSFX(audioManager.click);
        if (s1.style.display == DisplayStyle.Flex)
        {
            s1.style.display = DisplayStyle.None;
            s2.style.display = DisplayStyle.Flex;
        }
        else if (s2.style.display == DisplayStyle.Flex)
        {
            s2.style.display = DisplayStyle.None;
            s3.style.display = DisplayStyle.Flex;
        }
        else if(s3.style.display == DisplayStyle.Flex)
        {
            s3.style.display = DisplayStyle.None;
            FinishedModal.style.display = DisplayStyle.Flex;
            BackButton.style.display = DisplayStyle.None;
            NextButton.style.display = DisplayStyle.None;
            TutorialBackButton.style.display = DisplayStyle.None;
        }

    }
    void OnTutorialBackButtonClicked()  
    {   audioManager.playSFX(audioManager.click);
        SceneManager.LoadScene("Learning Material");
    }

    async void OnFinishedNextButtonClicked()
    {
        if (!rewardClaimed)
        {    audioManager.playSFX(audioManager.Buying);
            await dbRef.Child("users").Child(StaticGuardianData.guardianId).Child("children").Child(StaticGuardianData.guardianChild).Child("wallet").Child("normal_fund").SetValueAsync(int.Parse(currentFundSnapshot.Value.ToString()) + 10);
            var HistoryPurchase = new Dictionary<string, object>
            {
                { "description", "Hadiah Materi 2" },
                { "amount", 10 },
                { "date", DateTime.Now.ToString("yyyy-MM-dd") }
            };

            string date = DateTime.Today.ToString("yyyy-MM-dd");
            long timestamp = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds;

            _ = dbRef.Child("users")
                    .Child(guardianId)
                    .Child("children")
                    .Child(guardianChild)
                    .Child("history")
                    .Child(date)
                    .Child(timestamp.ToString())
                    .SetValueAsync(HistoryPurchase);
            
            rewardClaimed = true;
            PlayerPrefsCheckClaimed.SetBool("Learning1RewardClaimed", true);
        }
        
        SceneManager.LoadScene("MoneyMiniMe");
    }
    void OnFinishedBackButtonClicked()
    {
            // s3.style.display = DisplayStyle.Flex;
            audioManager.playSFX(audioManager.click);
            FinishedModal.style.display = DisplayStyle.None;
            BackButton.style.display = DisplayStyle.Flex;
            NextButton.style.display = DisplayStyle.Flex;
            TutorialBackButton.style.display = DisplayStyle.Flex;
 
    }


}
