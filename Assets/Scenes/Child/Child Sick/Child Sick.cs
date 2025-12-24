using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using UnityEngine.Video;
using Firebase;
using Firebase.Auth;
using Firebase.Database;


public class ChildSick : MonoBehaviour
{


    private Button NextButton;
    private Button FinishedButton;
    private FirebaseAuth auth;
    private DatabaseReference dbRef;    


    [SerializeField] VideoPlayer videoPlayer;
    private VisualElement root;
    private UIDocument uiDocument;
    private VisualElement s1;
    private VisualElement s2;
    private VisualElement s3;




    async void Start()
    {

        await InitializeFirebase();

        uiDocument = GetComponent<UIDocument>();
        root = uiDocument.rootVisualElement;

        s1 = root.Q<VisualElement>("1");
        s2 = root.Q<VisualElement>("2");
        s3 = root.Q<VisualElement>("3");

        NextButton = root.Q<Button>("NextButton");
        FinishedButton = root.Q<Button>("FinishedButton");
// lex ini aku giniin ya soalnya aku ga bisa masuk unitynya
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        NextButton.RegisterCallback<ClickEvent>(ev => OnNextButtonClicked());
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        FinishedButton.RegisterCallback<ClickEvent>(ev => OnFinishedButtonClicked());

        s1.style.display = DisplayStyle.Flex;
        s2.style.display = DisplayStyle.None;
        s3.style.display = DisplayStyle.None;

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


    async Task OnNextButtonClicked()
    {
        if (s1.style.display == DisplayStyle.Flex)
        {
            s1.style.display = DisplayStyle.None;
            s2.style.display = DisplayStyle.Flex;
            
            videoPlayer.Play();
            await Task.Delay(3000);

            s2.style.display = DisplayStyle.None;
            s3.style.display = DisplayStyle.Flex;

        }
    }


    async void OnFinishedButtonClicked()
    {
        await dbRef.Child("users").Child(StaticGuardianData.guardianId).Child("children").Child(StaticGuardianData.guardianChild).Child("sick_date").SetValueAsync("");
        await dbRef.Child("users").Child(StaticGuardianData.guardianId).Child("children").Child(StaticGuardianData.guardianChild).Child("wallet").Child("emergency_fund").SetValueAsync(0);
        SceneManager.LoadScene("MoneyMiniMe");
    }
    


}
