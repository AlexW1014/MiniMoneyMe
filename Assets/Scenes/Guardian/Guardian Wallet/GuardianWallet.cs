using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;
using Firebase;
using Firebase.Database;
using Firebase.Auth;
using UnityEngine.SceneManagement;


public class GuardianWallet : MonoBehaviour
{

    private Button GuardianWalletBackButton;
    private FirebaseAuth auth;
    private DatabaseReference dbRef;
    private string guardianId;
    private string guardianChild;
    public Label WalletNameLabel;
    public Label WalletNameLabel2;
    public Label NormalFundLabel;
    public Label NormalFundLabel2;
    public Label EmergencyFundLabel;
    public Label AllFundLabel;
    public GuardianWallet Instance { get; private set; }
    private UIDocument _document;
    private VisualElement WalletHistory;
    private VisualElement WalletHistoryBackButton;
    private VisualElement SemuaButton;
    private VisualElement PengeluaranButton;
    private VisualElement PemasukanButton;
    private Button _historyButton;



    //walet history
    VisualElement root;
    VisualElement historyContainerSemua;
    VisualElement historyContainerPemasukan;
    VisualElement historyContainerPengeluaran;

    VisualTreeAsset historyContainerPrefabAsset;

    VisualTreeAsset historyItemPrefabAsset;
    private AudioManager audioManager;



    private async void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
        await InitializeFirebase();

        // Debug.Log("lol");
        _document = GetComponent<UIDocument>();

        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        historyContainerPrefabAsset = await GethistoryContainerSemuaPrefab();
        historyItemPrefabAsset = await GetHistoryItemPrefab();

        NormalFundLabel = _document.rootVisualElement.Q<Label>("NormalFundLabel");
        EmergencyFundLabel = _document.rootVisualElement.Q<Label>("EmergencyFundLabel");
        WalletNameLabel2 = _document.rootVisualElement.Q<Label>("WalletNameLabel2");
        NormalFundLabel2 = _document.rootVisualElement.Q<Label>("NormalFundLabel2");
        AllFundLabel = _document.rootVisualElement.Q<Label>("AllFundLabel");
        WalletNameLabel = _document.rootVisualElement.Q<Label>("WalletNameLabel");
        WalletNameLabel2 = _document.rootVisualElement.Q<Label>("WalletNameLabel2");





        _historyButton = _document.rootVisualElement.Q("HistoryButton") as Button;
        WalletHistoryBackButton = _document.rootVisualElement.Q("WalletHistoryBackButton") as Button;
        SemuaButton = _document.rootVisualElement.Q("SemuaButton") as Button;
        PemasukanButton = _document.rootVisualElement.Q("PemasukanButton") as Button;
        PengeluaranButton = _document.rootVisualElement.Q("PengeluaranButton") as Button;
        GuardianWalletBackButton = _document.rootVisualElement.Q("GuardianWalletBackButton") as Button;
        WalletHistory = _document.rootVisualElement.Q("WalletHistory");
        _historyButton.RegisterCallback<ClickEvent>(OnHistoryButtonClick);
        WalletHistoryBackButton.RegisterCallback<ClickEvent>(OnWalletHistoryBackButtonClick);
        SemuaButton.RegisterCallback<ClickEvent>(OnSemuaButtonClick);
        PengeluaranButton.RegisterCallback<ClickEvent>(OnPengeluaranButtonClick);
        PemasukanButton.RegisterCallback<ClickEvent>(OnPemasukanButtonClick);
        GuardianWalletBackButton.RegisterCallback<ClickEvent>(OnGuardianWalletBackButtonClick);
    }

    private async Task<VisualTreeAsset> GetHistoryItemPrefab()
    {
        VisualTreeAsset historyItemPrefabAsset = Resources.Load<VisualTreeAsset>("ContainerHistoryDesc");
        while (historyItemPrefabAsset == null)
            await Task.Yield();
        return historyItemPrefabAsset;
    }


    async Task<VisualTreeAsset> GethistoryContainerSemuaPrefab()
    {
        VisualTreeAsset playerDisplayAsset = Resources.Load<VisualTreeAsset>("ContainerHistory");
        while (playerDisplayAsset == null)
            await Task.Yield();
        Debug.Log(playerDisplayAsset);
        return playerDisplayAsset;
    }

    private async void Start()
    {

        guardianChild = StaticGuardianData.guardianChild;
        guardianId = StaticGuardianData.guardianId;
        // guardianChild =  "apin";
        // guardianId = "cB2wVFyQULZ1KBQC7T5l7tSxQs42";

        Debug.Log("Guardian Wallet Scene: " + guardianChild + " " + guardianId);
        if (dbRef == null)
            await InitializeFirebase();

        root = GetComponent<UIDocument>().rootVisualElement;
        historyContainerSemua = root.Q<ScrollView>("ContentSemua");
        historyContainerPemasukan = root.Q<ScrollView>("ContentPemasukan");
        historyContainerPengeluaran = root.Q<ScrollView>("ContentPengeluaran");

        VisualElement localPlayerElement = root.Q<VisualElement>("ContainerHistory");

        var walletSnapshot = await dbRef.Child("users").Child(guardianId).Child("children").Child(guardianChild).Child("wallet").GetValueAsync();
        Debug.Log(walletSnapshot);

        EmergencyFundLabel.text = walletSnapshot.Child("emergency_fund").Value.ToString();
        NormalFundLabel2.text = walletSnapshot.Child("normal_fund").Value.ToString();
        NormalFundLabel.text = walletSnapshot.Child("normal_fund").Value.ToString();
        WalletNameLabel.text = guardianChild + "'s Wallet";
        WalletNameLabel2.text = guardianChild + "'s Wallet";
        AllFundLabel.text = (Convert.ToInt32(walletSnapshot.Child("normal_fund").Value) + Convert.ToInt32(walletSnapshot.Child("emergency_fund").Value)).ToString();


        var history = await dbRef.Child("users").Child(guardianId).Child("children").Child(guardianChild).Child("history").GetValueAsync();

        if (history.Exists)
        {
            Debug.Log(history.ChildrenCount + " history records found.");
            foreach (var dateSnapshot in history.Children)
            {
                VisualElement newContainerSemua = historyContainerPrefabAsset.CloneTree().Q<VisualElement>("ContainerHistory");
                VisualElement newContainerPengeluaran = historyContainerPrefabAsset.CloneTree().Q<VisualElement>("ContainerHistory");
                VisualElement newContainerPemasukan = historyContainerPrefabAsset.CloneTree().Q<VisualElement>("ContainerHistory");


                newContainerSemua.Q<Label>("date-label").text =dateSnapshot.Key + "";
                historyContainerSemua.Add(newContainerSemua);



            foreach (var itemSnapshot in dateSnapshot.Children)
             {
                Debug.Log(itemSnapshot);
                Debug.Log(itemSnapshot.Key);
                Debug.Log("Item: " + itemSnapshot.Child("description").Value + ", Amount: " + itemSnapshot.Child("amount").Value);
                if(Convert.ToInt32(itemSnapshot.Child("amount").Value) < 0)
                {
                    newContainerPengeluaran.Q<Label>("date-label").text = dateSnapshot.Key + "";
                    historyContainerPengeluaran.Add(newContainerPengeluaran);   
                    VisualElement newItem = historyItemPrefabAsset.CloneTree().Q<VisualElement>("ContainerHistoryDesc");
                    newItem.Q<Label>("spending-label").text = itemSnapshot.Child("description").Value.ToString() + "";
                    newItem.Q<Label>("amount-label").text = itemSnapshot.Child("amount").Value.ToString() + "";
                    newContainerPengeluaran.Q<VisualElement>("ContainerHistory").Add(newItem);
                    
                }
                else if (Convert.ToInt32(itemSnapshot.Child("amount").Value) > 0)
                {
                    newContainerPemasukan.Q<Label>("date-label").text = dateSnapshot.Key + "";
                    historyContainerPemasukan.Add(newContainerPemasukan);
                    var newItem = historyItemPrefabAsset.CloneTree().Q<VisualElement>("ContainerHistoryDesc");
                    newItem.Q<Label>("spending-label").text = itemSnapshot.Child("description").Value.ToString() + "";
                    newItem.Q<Label>("amount-label").text = itemSnapshot.Child("amount").Value.ToString() + "";
                    newContainerPemasukan.Q<VisualElement>("ContainerHistory").Add(newItem);

                }
 

                var newItem2 = historyItemPrefabAsset.CloneTree().Q<VisualElement>("ContainerHistoryDesc");
                newItem2.Q<Label>("spending-label").text = itemSnapshot.Child("description").Value.ToString() + "";
                newItem2.Q<Label>("amount-label").text = itemSnapshot.Child("amount").Value.ToString() + "";
                newContainerSemua.Q<VisualElement>("ContainerHistory").Add(newItem2);

                }
            }
        }
        else
        {
            Debug.Log("No history found, creating sample data...");
        }



    }

    private void Update()
    {

    }





    // Update is called once per frame
    private void OnHistoryButtonClick(ClickEvent evt)
    {
        audioManager.playSFX(audioManager.click);
        WalletHistory.style.display = DisplayStyle.Flex;
        
    }

    private void OnWalletHistoryBackButtonClick(ClickEvent evt)
    {
        audioManager.playSFX(audioManager.click);
        WalletHistory.style.display = DisplayStyle.None;
    }

    private void OnSemuaButtonClick(ClickEvent evt)
    {

        audioManager.playSFX(audioManager.click);
        historyContainerSemua.style.display = DisplayStyle.Flex;
        historyContainerPengeluaran.style.display = DisplayStyle.None;
        historyContainerPemasukan.style.display = DisplayStyle.None;
    }

    private void OnPemasukanButtonClick(ClickEvent evt)
    {
        audioManager.playSFX(audioManager.click);
        historyContainerSemua.style.display = DisplayStyle.None;
        historyContainerPengeluaran.style.display = DisplayStyle.None;
        historyContainerPemasukan.style.display = DisplayStyle.Flex;
    }

    private void OnPengeluaranButtonClick(ClickEvent evt)
    {
        audioManager.playSFX(audioManager.click);
        historyContainerSemua.style.display = DisplayStyle.None;
        historyContainerPengeluaran.style.display = DisplayStyle.Flex;
        historyContainerPemasukan.style.display = DisplayStyle.None;
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

    private async void OnGuardianWalletBackButtonClick(ClickEvent evt)
    {
        audioManager.playSFX(audioManager.click);
        await Task.Delay(100); // Small delay to ensure smooth transition
        SceneManager.LoadScene("Guardian Landing Scene");
    }


}





public class History
{
    public class historyItem
    {
        public int Amount { get; set; }
        public string Description { get; set; }
    }
    //Properties
    public DateTime Date { get; set; }
    public historyItem[] Items { get; set; }

    // Constructor
    public History(DateTime date)
    {
        Date = date;
    }
}


