using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Firebase;
using Firebase.Database;
using Firebase.Auth;
using System.Threading.Tasks;
using Unity.VisualScripting;
using System;
using UnityEngine.SceneManagement;



public class ChildWallet : MonoBehaviour
{
    // Start is called before the first frame update
    public ChildWallet Instance { get; private set; }
    private string guardianChild;
    private string guardianId;
    

    private DatabaseReference dbRef;
    private FirebaseAuth auth;

    private UIDocument _document;
    private VisualElement riwayat_button;
    private VisualElement WalletHistoryBackButton;
    private VisualElement SemuaButton;
    private VisualElement PengeluaranButton;
    private VisualElement PemasukanButton;
    private VisualElement DompetUtamaContainer;
    private VisualElement DompetDaruratContainer;
    private Button _historyButton;
    private Button HistoryButton2;
    private Button back_button;
    private Button back_button2;
    private VisualElement TabungButton;
    private Button TabungKembaliButton;
    private Button TabungConfirmButton;
    private Button ChildWalletMainBackButton;
    private TextField TabungTF;
    private Label NormalFundLabel1;
    private Label NormalFundLabel2;
    private Label NormalFundLabe3;
    private Label EmergencyFundLabel2;
    private Label AllFundLabel1;
    private Label WalletNameLabel;
    private Label TabungLabel;
    private AudioManager audioManager;


    

    VisualElement root;
    VisualElement ChildWalletContainer;
    VisualElement MainChildWalletContainer;
    VisualElement EmergencyChildWalletContainer;
    VisualElement historyContainerPemasukan;
    VisualElement historyContainerPengeluaran;
    VisualElement TabungModalContainer;
    VisualElement WalletHistory;
    VisualElement ChildWalletBackButton;





    //walet history
    VisualElement historyContainerSemua;

    VisualTreeAsset historyContainerPrefabAsset;
    VisualTreeAsset historyContainerPemasukanPrefabAsset;
    VisualTreeAsset historyContainerPengeluaranPrefabAsset;
    VisualTreeAsset historyItemPrefabAsset;





    private void OnChildWalletMainBackButtonClick(ClickEvent evt)
    {
        SceneManager.LoadScene("MoneyMiniMe");

    }

    private void OnWalletHistoryBackButtonClick(ClickEvent evt)
    {
        WalletHistory.style.display = DisplayStyle.None;
    }
    private void OnSemuaButtonClick(ClickEvent evt)
    {
        historyContainerSemua.style.display = DisplayStyle.Flex;
        historyContainerPengeluaran.style.display = DisplayStyle.None;
        historyContainerPemasukan.style.display = DisplayStyle.None;
    }
    private void OnPemasukanButtonClick(ClickEvent evt)
    {
        historyContainerSemua.style.display = DisplayStyle.None;
        historyContainerPengeluaran.style.display = DisplayStyle.None;
        historyContainerPemasukan.style.display = DisplayStyle.Flex;
    }
    private void OnPengeluaranButtonClick(ClickEvent evt)
    {
        historyContainerSemua.style.display = DisplayStyle.None;
        historyContainerPengeluaran.style.display = DisplayStyle.Flex;
        historyContainerPemasukan.style.display = DisplayStyle.None;
    }

    private void OnHistoryButtonClick(ClickEvent evt)
    {
        WalletHistory.style.display = DisplayStyle.Flex;

    }

    private async void Start()
    {
        Debug.Log("ChildWallet OnEnable called");
        await InitializeFirebase();
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
        audioManager.playBG();

        guardianChild = StaticGuardianData.guardianChild;
        guardianId = StaticGuardianData.guardianId;
        Debug.Log("Guardian Child: " + guardianChild);



        var walletSnapshot = await dbRef.Child("users").Child(guardianId).Child("children").Child(guardianChild).Child("wallet").GetValueAsync();

         _document = GetComponent<UIDocument>();
        historyContainerPrefabAsset = await GethistoryContainerSemuaPrefab();
        historyItemPrefabAsset = await GetHistoryItemPrefab();

        DompetUtamaContainer = _document.rootVisualElement.Q<VisualElement>("dompet_utama_container-VE");

        WalletHistoryBackButton = _document.rootVisualElement.Q("WalletHistoryBackButton") as Button;
        TabungTF = _document.rootVisualElement.Q<TextField>("Tabung-TF") as TextField;

        TabungButton = _document.rootVisualElement.Q("dompet_darurat_container-VE");
        TabungButton.RegisterCallback<ClickEvent>(OnTabungClick);
        TabungKembaliButton = _document.rootVisualElement.Q("tabung_back-button") as Button;
        TabungKembaliButton.RegisterCallback<ClickEvent>(OnTabungKembaliClick);
        
        TabungConfirmButton =  _document.rootVisualElement.Q("tabung_confirm-button") as Button;
        TabungConfirmButton.RegisterCallback<ClickEvent>(OnTabungConfirmClick);

        ChildWalletMainBackButton = _document.rootVisualElement.Q("ChildWalletMainBackButton") as Button;
        ChildWalletMainBackButton.RegisterCallback<ClickEvent>(OnChildWalletMainBackButtonClick);


        _historyButton = _document.rootVisualElement.Q("HistoryButton") as Button;
        WalletHistoryBackButton = _document.rootVisualElement.Q("WalletHistoryBackButton") as Button;
        SemuaButton = _document.rootVisualElement.Q("SemuaButton") as Button;
        PemasukanButton = _document.rootVisualElement.Q("PemasukanButton") as Button;
        PengeluaranButton = _document.rootVisualElement.Q("PengeluaranButton") as Button;
        WalletHistory = _document.rootVisualElement.Q("child_wallet_history-VE") as VisualElement;
        WalletNameLabel = _document.rootVisualElement.Q<Label>("WalletNameLabel");
        _historyButton.RegisterCallback<ClickEvent>(OnHistoryButtonClick);
        WalletHistoryBackButton.RegisterCallback<ClickEvent>(OnWalletHistoryBackButtonClick);
        SemuaButton.RegisterCallback<ClickEvent>(OnSemuaButtonClick);
        PengeluaranButton.RegisterCallback<ClickEvent>(OnPengeluaranButtonClick);
        PemasukanButton.RegisterCallback<ClickEvent>(OnPemasukanButtonClick);


        NormalFundLabel1 = _document.rootVisualElement.Q<Label>("NormalFundLabel1");
        NormalFundLabe3 = _document.rootVisualElement.Q<Label>("NormalFundLabel3");
        EmergencyFundLabel2 = _document.rootVisualElement.Q<Label>("EmergencyFundLabel2");
        AllFundLabel1 = _document.rootVisualElement.Q<Label>("AllFundLabel1");
        TabungLabel = _document.rootVisualElement.Q<Label>("TabungLabel");
    

        root = GetComponent<UIDocument>().rootVisualElement;
        historyContainerSemua = root.Q<ScrollView>("ContentSemua");
        historyContainerPemasukan = root.Q<ScrollView>("ContentPemasukan");
        historyContainerPengeluaran = root.Q<ScrollView>("ContentPengeluaran");
        ChildWalletContainer = root.Q<VisualElement>("child_wallet_container-VE");
        MainChildWalletContainer = root.Q<VisualElement>("child_main_wallet_container-VE");
        EmergencyChildWalletContainer = root.Q<VisualElement>("child_emergency_wallet_container-VE");
        historyContainerPemasukan = root.Q<ScrollView>("ContentPemasukan");
        historyContainerPengeluaran = root.Q<ScrollView>("ContentPengeluaran");
        TabungModalContainer = root.Q<VisualElement>("tabung_modal_container-VE");


        VisualElement localPlayerElement = root.Q<VisualElement>("ContainerHistory");

        root.Q<Label>("name-label").text = "Hello " + guardianChild + "!";
        WalletNameLabel.text = "Dompet " + guardianChild;

        NormalFundLabel1.text = walletSnapshot.Child("normal_fund").Value + "";
        NormalFundLabe3.text = walletSnapshot.Child("normal_fund").Value + "";
        EmergencyFundLabel2.text = walletSnapshot.Child("emergency_fund").Value + "";
        AllFundLabel1.text = (walletSnapshot.Child("normal_fund").Value.ConvertTo<int>() + walletSnapshot.Child("emergency_fund").Value.ConvertTo<int>()) + "";


        DompetUtamaContainer = root.Q<VisualElement>("dompet_utama_container-VE");


        // DompetDaruratContainer = root.Q<VisualElement>("dompet_darurat_container-VE");
        // DompetDaruratContainer.RegisterCallback<ClickEvent>(OnDompetDaruratClick);




        //main wallet


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





    private void OnDompetDaruratClick(ClickEvent evt)
    {
        ChildWalletContainer.style.display = DisplayStyle.None;

    }

        private void OnTabungClick(ClickEvent evt)
    {
        audioManager.playSFX(audioManager.click);
        TabungModalContainer.style.display = DisplayStyle.Flex;

    }

        private void OnTabungKembaliClick(ClickEvent evt)
    {
        audioManager.playSFX(audioManager.click);
        Debug.Log("Tabung Kembali Clicked");
        TabungModalContainer.style.display = DisplayStyle.None;

    }

    private async void OnTabungConfirmClick(ClickEvent evt)
    {
        audioManager.playSFX(audioManager.click);
        var walletSnapshot = await dbRef.Child("users").Child(guardianId).Child("children").Child(guardianChild).Child("wallet").GetValueAsync();
        Debug.Log("Tabung Confirm Clicked");
        var tabungAmount = int.Parse(TabungTF.text);
        if (tabungAmount <= 0)
        {
            Debug.Log("Invalid amount entered for saving.");
            
            TabungLabel.text = "Masukan Angka Yang Benar";
            TabungLabel.style.display = DisplayStyle.Flex;
            return;
        }
        else if (tabungAmount > walletSnapshot.Child("normal_fund").Value.ConvertTo<int>())
        {
            Debug.Log("Insufficient funds in main wallet.");
            TabungLabel.text = "Koin Tidak Cukup";
            TabungLabel.style.display = DisplayStyle.Flex;

            return;
        }
        else if(tabungAmount + walletSnapshot.Child("emergency_fund").Value.ConvertTo<int>() > 25)
        {
            Debug.Log("Maximum is 25.");
            TabungLabel.text = "Tabungan Darurat Maksimal 25 Koin";
            TabungLabel.style.display = DisplayStyle.Flex;

            return;
        }
            TabungLabel.style.display = DisplayStyle.None;

        // Update Firebase
        await dbRef.Child("users").Child(guardianId).Child("children").Child(guardianChild).Child("wallet").Child("normal_fund").SetValueAsync( walletSnapshot.Child("normal_fund").Value.ConvertTo<int>() - tabungAmount);
        await dbRef.Child("users").Child(guardianId).Child("children").Child(guardianChild).Child("wallet").Child("emergency_fund").SetValueAsync(walletSnapshot.Child("emergency_fund").Value.ConvertTo<int>() + tabungAmount);
        var emergency_fund = walletSnapshot.Child("emergency_fund").Value.ConvertTo<int>() + tabungAmount;

            var HistoryPurchase = new Dictionary<string, object>
            {
            { "description", "Tabung to Emergency Fund" },
            { "amount", -tabungAmount },
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


        walletSnapshot = await dbRef.Child("users").Child(guardianId).Child("children").Child(guardianChild).Child("wallet").GetValueAsync();

        // Update local UI
        NormalFundLabel1.text = walletSnapshot.Child("normal_fund").Value.ConvertTo<int>()  + "";
        NormalFundLabe3.text = walletSnapshot.Child("normal_fund").Value.ConvertTo<int>() + "";
        EmergencyFundLabel2.text = walletSnapshot.Child("emergency_fund").Value.ConvertTo<int>() + "";
        AllFundLabel1.text = walletSnapshot.Child("normal_fund").Value.ConvertTo<int>() + walletSnapshot.Child("emergency_fund").Value.ConvertTo<int>()  + "";   

        

        TabungModalContainer.style.display = DisplayStyle.None;
        var today_date = DateTime.Now.AddDays(2).ToString("yyyy-MM-dd");

        if(emergency_fund == 25)
        {
            await dbRef.Child("users").Child(guardianId).Child("children").Child(guardianChild).Child("sick_date").SetValueAsync(today_date);
        }



    }

    private void OnBackButtonClick(ClickEvent evt)
    {
        audioManager.playSFX(audioManager.click);
        ChildWalletContainer.style.display = DisplayStyle.Flex;
        EmergencyChildWalletContainer.style.display = DisplayStyle.None;

    }
    



}
