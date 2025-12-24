using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using System.Runtime.CompilerServices;
using System;
using System.Net.WebSockets;
using Unity.Mathematics;
public class ChildUI : MonoBehaviour
{
    [SerializeField] public AvatarMoneyManager avatarMoneyManager;
    public ChildUI Instance { get; private set; }

    private FirebaseAuth auth;
    private VisualElement ChildInterestModalContainer;
    private string guardianId;
    private string guardianChild;

    private DatabaseReference dbRef;
    public UIDocument _document;
    private Button ApproveInterestConfirmButton;
    private Label MoneyLabel;
    private Button BackButton;



    private async void OnEnable()
    {

         await InitializeFirebase();

        guardianChild = StaticGuardianData.guardianChild;
        guardianId = StaticGuardianData.guardianId;


        if(!string.IsNullOrEmpty(guardianId) || !string.IsNullOrEmpty(guardianChild))
        {
            PlayerPrefs.SetString("GuardianID", guardianId);
            PlayerPrefs.SetString("GuardianChild", guardianChild);
            PlayerPrefs.Save();
        }


        if (!PlayerPrefs.HasKey("GuardianID") || !PlayerPrefs.HasKey("GuardianChild"))
        {
            Debug.LogError("Guardian ID or Child not found in PlayerPrefs.");
            return;
        }
         guardianId = PlayerPrefs.GetString("GuardianID");
         guardianChild = PlayerPrefs.GetString("GuardianChild");
         StaticGuardianData.guardianId = guardianId;
         StaticGuardianData.guardianChild = guardianChild;

         _document = GetComponent<UIDocument>();
        guardianChild = StaticGuardianData.guardianChild;
        guardianId = StaticGuardianData.guardianId;
        ChildInterestModalContainer = _document.rootVisualElement.Q<VisualElement>("ChildInterestModalContainer");
       
        ApproveInterestConfirmButton = _document.rootVisualElement.Q<Button>("ApproveInterestConfirmButton");
        MoneyLabel = _document.rootVisualElement.Q<Label>("MoneyLabel");
        ApproveInterestConfirmButton.RegisterCallback<ClickEvent>(evt => OnApproveInterestConfirmButtonClick());
        BackButton = _document.rootVisualElement.Q<Button>("BackButton");
        BackButton.RegisterCallback<ClickEvent>(evt => OnBackButtonClick());
        var moneySnapshot = await dbRef.Child("users").Child(guardianId).Child("children").Child(guardianChild).Child("wallet").Child("normal_fund").GetValueAsync();
        var interest = Mathf.FloorToInt(int.Parse(moneySnapshot.Value.ToString())*10/100);
    
        MoneyLabel.text = moneySnapshot.Value.ToString() + " + " + interest + " = " + (int.Parse(moneySnapshot.Value.ToString()) + interest).ToString();

        try
        {
            var Today = DateTime.Now.ToString("yyyy-MM-dd");
            var last = await dbRef.Child("users").Child(guardianId).Child("children").Child(guardianChild).Child("last_login_date").GetValueAsync();
            Debug.Log(last.Value.ToString());

            if (last.Value.ToString() != Today)
            {
                Debug.Log("Showing interest modal");
                ChildInterestModalContainer.style.display = DisplayStyle.Flex;
            }
            else
            {
                Debug.Log("Hiding interest modal");
                ChildInterestModalContainer.style.display = DisplayStyle.None;
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Failed to load tasks: " + ex);
        }


    }

    private async Task InitializeFirebase()
    {
        var dependencyStatus = await FirebaseApp.CheckAndFixDependenciesAsync();
        if (dependencyStatus == DependencyStatus.Available)
        {
            auth = FirebaseAuth.DefaultInstance;
            dbRef = FirebaseDatabase.DefaultInstance.RootReference;
            Debug.Log("Firebase initialized successfully!");
        }
        else
        {
            Debug.LogError("Firebase dependencies unresolved: " + dependencyStatus);
        }
    }

    private void OnBackButtonClick()
    {
       Debug.Log("Back Button Clicked");
    }
    private async void OnApproveInterestConfirmButtonClick()
    {
        Debug.Log("Approve Task Confirm Button Clicked");
        // Add functionality here
        var moneySnapshot = await dbRef.Child("users").Child(guardianId).Child("children").Child(guardianChild).Child("wallet").Child("normal_fund").GetValueAsync();
        var interest = Mathf.FloorToInt(int.Parse(moneySnapshot.Value.ToString())*10/100);
    
        await dbRef.Child("users").Child(guardianId).Child("children").Child(guardianChild).Child("wallet").Child("normal_fund").SetValueAsync(int.Parse(moneySnapshot.Value.ToString()) + interest);
            var walletValue = await dbRef.Child("users").Child(guardianId).Child("children").Child(guardianChild).Child("wallet").Child("normal_fund").GetValueAsync();
            MoneyLabel.text = moneySnapshot.Value.ToString() + " + " + interest + " = " + walletValue.Value.ToString();
            avatarMoneyManager.currentMoney = int.Parse(walletValue.Value.ToString());
            avatarMoneyManager.UpdateMoneyUI();
            ChildInterestModalContainer.style.display = DisplayStyle.None;
            var Today = DateTime.Now.ToString("yyyy-MM-dd");
            await dbRef.Child("users").Child(guardianId).Child("children").Child(guardianChild).Child("last_login_date").SetValueAsync(Today);

            var HistoryPurchase = new Dictionary<string, object>
            {
                { "description", "Bunga Harian" },
                { "amount", interest },
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
    }
    

}
