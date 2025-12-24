using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using System.Threading.Tasks;


public class GuardianLandingUI : MonoBehaviour
{

    private FirebaseAuth auth;
    private DatabaseReference dbRef;
    private String guardianId;
    private String guardianChild;
    private Button GuardianWalletButton;
    private Button GuardianChildLoginButton;
    private Button GuardianChildTaskButton;
    private Button GuardianBackButton;
    private Button GuardianLearningButton;
    private UIDocument _document;
    private AudioManager audioManager;
    private VisualElement LoadingScreen;


    private async void Start()
    {
        guardianId = StaticGuardianData.guardianId;
        guardianChild = StaticGuardianData.guardianChild;
        // guardianChild =  "apin";
        // guardianId = "cB2wVFyQULZ1KBQC7T5l7tSxQs42";
        StaticGuardianData.guardianChild = guardianChild;
        StaticGuardianData.guardianId = guardianId;


        if (auth == null || dbRef == null)
        {
            await InitializeFirebase();        
        }
        // var Today = DateTime.Now.ToString("yyyy-MM-dd");
        // var LastLogin = await dbRef.Child("users").Child(guardianId).Child("children").Child(guardianChild).Child("last_login_date").GetValueAsync();
        // if(LastLogin.Value.ToString() != Today)
        // {
        //     Debug.Log("Updating last login date and resetting task status for new day.");
        //     await dbRef.Child("users").Child(guardianId).Child("children").Child(guardianChild).Child("last_login_date").SetValueAsync(Today);
        //     await dbRef.Child("users").Child(guardianId).Child("children").Child(guardianChild).Child("task").Child("easy_task").Child("status").SetValueAsync("waiting");
        //     await dbRef.Child("users").Child(guardianId).Child("children").Child(guardianChild).Child("task").Child("medium_task").Child("status").SetValueAsync("waiting");
        //     await dbRef.Child("users").Child(guardianId).Child("children").Child(guardianChild).Child("task").Child("hard_task").Child("status").SetValueAsync("waiting");

        // }



        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
        _document = GetComponent<UIDocument>();
        LoadingScreen = _document.rootVisualElement.Q<VisualElement>("LoadingScreen");
        GuardianWalletButton = _document.rootVisualElement.Q<Button>("GuardianWalletButton");
        GuardianChildLoginButton = _document.rootVisualElement.Q<Button>("GuardianChildLoginButton");
        GuardianChildTaskButton = _document.rootVisualElement.Q<Button>("GuardianTaskButton");
        GuardianBackButton = _document.rootVisualElement.Q<Button>("GuardianBackButton");
        GuardianLearningButton = _document.rootVisualElement.Q<Button>("GuardianLearningButton");
        GuardianWalletButton.RegisterCallback<ClickEvent>(evt => OnGuardianWalletButtonClick());
        GuardianChildLoginButton.RegisterCallback<ClickEvent>(evt => OnGuardianChildLoginButtonClick());
        GuardianChildTaskButton.RegisterCallback<ClickEvent>(evt => OnGuardianChildTaskButtonClick());
        GuardianBackButton.RegisterCallback<ClickEvent>(evt => OnGuardianBackButtonClick());
        GuardianLearningButton.RegisterCallback<ClickEvent>(evt => SceneManager.LoadScene("Guardian Learning"));        
        LoadingScreen.style.display = DisplayStyle.None;


    }

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



    public void OnGuardianWalletButtonClick()
    {
        audioManager.playSFX(audioManager.click);
        Debug.Log("Guardian Wallet Button Clicked");
        SceneManager.LoadScene("Guardian Wallet");
        // Add functionality here
    }

    public void OnGuardianChildLoginButtonClick()
    {
        audioManager.playSFX(audioManager.click);
        Debug.Log("Guardian Child Login Button Clicked");
        SceneManager.LoadScene("MoneyMiniMe");
        // Add functionality here
    }

    public void OnGuardianChildTaskButtonClick()
    {
        audioManager.playSFX(audioManager.click);
        Debug.Log("Guardian Child Task Button Clicked");
        // Add functionality here
        SceneManager.LoadScene("Guardian Task Scene");
    }

    public void OnGuardianBackButtonClick()
    {
        audioManager.playSFX(audioManager.click);
        Debug.Log("Guardian Back Button Clicked");
        // Add functionality here
        SceneManager.LoadScene("Guardian Children");
    }


}
