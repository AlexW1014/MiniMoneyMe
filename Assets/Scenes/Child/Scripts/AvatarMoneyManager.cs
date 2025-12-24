using UnityEngine;
using System;
using TMPro; 
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;


public class AvatarMoneyManager : MonoBehaviour
{   private FirebaseAuth auth;
    private DatabaseReference dbRef;
    private string guardianChild;
    private string guardianId;
    

     private async Task InitializeFirebase()
    {
        // var dependencyStatus = await FirebaseApp.CheckAndFixDependenciesAsync();
        // if (dependencyStatus == DependencyStatus.Available)
        // {
            auth = FirebaseAuth.DefaultInstance;
            dbRef = FirebaseDatabase.DefaultInstance.RootReference;
//             Debug.Log("Firebase initialized successfully!");
//         }
//         else
//         {
//             Debug.LogError("Firebase dependencies unresolved: " + dependencyStatus);
// } 
}

    public static AvatarMoneyManager Instance;

    [Header("Money Settings")]
   public int  currentMoney;
     
    [Header("UI (Optional)")]
    [SerializeField] private TMP_Text moneyText; 

    public event Action<float> OnMoneyChanged;

    
    public async void Start()
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

        Debug.Log(guardianChild + " " + guardianId);

        Debug.Log($"----------------------------------------------------------Guardian Child: {guardianChild}, Guardian ID: {guardianId}");
       

        var snapshot = await dbRef.Child("users").Child(guardianId).Child("children").Child(guardianChild).Child("wallet").Child("normal_fund").GetValueAsync();
        Debug.Log("Fetched money from database: " + snapshot.Value);
        currentMoney = snapshot.Value != null ? Convert.ToInt32(snapshot.Value) : 0;

        
  
         UpdateMoneyUI();
    

        var Today = DateTime.Now.ToString("yyyy-MM-dd");

         var LastLogin = await dbRef.Child("users").Child(guardianId).Child("children").Child(guardianChild).Child("last_login_date").GetValueAsync();
        var SickDate = await dbRef.Child("users").Child(guardianId).Child("children").Child(guardianChild).Child("sick_date").GetValueAsync();
        if(SickDate.Value != null && SickDate.Value.ToString() == Today)
        {
            Debug.Log("Child is sick today.");
            SceneManager.LoadScene("Child Sick");
        }
        Debug.Log("Last login date from database: " + LastLogin.Value);


        if( LastLogin.Value == null || LastLogin.Value.ToString() != Today )
        {
            Debug.Log("Updating last login date for new day.");
            await dbRef.Child("users").Child(guardianId).Child("children").Child(guardianChild).Child("last_login_date").SetValueAsync(Today);
        }

        {

            Debug.Log("Updating last login date and resetting task status for new day.");

            await dbRef.Child("users").Child(guardianId).Child("children").Child(guardianChild).Child("task").Child("easy_task").Child("status").SetValueAsync("waiting");

            await dbRef.Child("users").Child(guardianId).Child("children").Child(guardianChild).Child("task").Child("medium_task").Child("status").SetValueAsync("waiting");

            await dbRef.Child("users").Child(guardianId).Child("children").Child(guardianChild).Child("task").Child("hard_task").Child("status").SetValueAsync("waiting");

        }

    }



    private async void Awake()
    {    

        

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
        }
        else
        {
            Destroy(gameObject);
            return;
        }

    }

   

    public int GetMoney() => currentMoney;

    public bool CanAfford(int price) => currentMoney >= price;

    public async Task<bool> SpendMoney(int amount)
    {
        if (CanAfford(amount))
        {
            currentMoney -= amount;
            
            await dbRef.Child("users").Child(guardianId).Child("children").Child(guardianChild).Child("wallet").Child("normal_fund").SetValueAsync(currentMoney);
           
            OnMoneyChanged?.Invoke(currentMoney);
            UpdateMoneyUI();
            Debug.Log($"Money spent: {amount}. Remaining: {currentMoney}");
            return true;
        }
        return false;
    }

  


    

    public void UpdateMoneyUI()
    {
        if (moneyText != null)
            moneyText.text = $"{currentMoney:F0}";
    }

    public void RefreshUI() => UpdateMoneyUI();

  
}
