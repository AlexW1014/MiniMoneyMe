using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

public class ShoppingManager : MonoBehaviour
{
    public static ShoppingManager Instance;
    private AudioManager audioManager;

    [Header("References")]
    public ItemDatabase itemDatabase;

    [Header("UI Panels")]
    public GameObject consumableConfirmationPanel;
    public GameObject inconsumableConfirmationPanel;
    public GameObject resultPanel;

    [Header("Confirmation UI Elements")]
    public Image confirmationIcon;
    public TMP_Text confirmationItemName;
    public TMP_Text confirmationPrice;
    public Button confirmPurchaseButton;
    public Button cancelPurchaseButton;

    [Header("Consumable Quantity Controls")]
    public TMP_Text confirmationAmountText;
    public Button plusButton;
    public Button minusButton;
    public TMP_InputField quantityInputField;

    [Header("Result UI Elements")]
    public TMP_Text resultTitleText;
    public TMP_Text resultMessageText;
    public TMP_Text resultBalanceText;
    public Button resultBackButton;

    private ItemData currentItem;
    private int selectedQuantity = 1;

    private FirebaseAuth auth;
    private string guardianChild;
    private string guardianId;

    private DatabaseReference dbRef;

    private bool firebaseInitialized = false;

    private async Task InitializeFirebase()
    {
        if (firebaseInitialized) return;

        var dependencyStatus = await FirebaseApp.CheckAndFixDependenciesAsync();
        if (dependencyStatus == DependencyStatus.Available)
        {
            auth = FirebaseAuth.DefaultInstance;
            dbRef = FirebaseDatabase.DefaultInstance.RootReference;
            firebaseInitialized = true;
            Debug.Log("Firebase initialized successfully!");
        }
        else
        {
            Debug.LogError("Firebase dependencies unresolved: " + dependencyStatus);
        }
    }

    private async void Awake()
    {   audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
        await InitializeFirebase();

        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        if (quantityInputField != null)
        {
            quantityInputField.onEndEdit.AddListener(OnQuantityInputChanged);
        }
    }

    private async void OnEnable()
    {
        if (!firebaseInitialized)
            await InitializeFirebase();

        guardianChild = StaticGuardianData.guardianChild;
        guardianId = StaticGuardianData.guardianId;

        if (string.IsNullOrEmpty(guardianChild))
            guardianChild = "mami";
        if (string.IsNullOrEmpty(guardianId))
            guardianId = "cB2wVFyQULZ1KBQC7T5l7tSxQs42";
    }

    public void ShowItemConfirmation(ItemData item, int quantity)
    {
        if (item == null) return;

        if (item.type == ItemType.Inconsumable && itemDatabase != null && itemDatabase.IsPurchased(item))
        {
            Debug.Log($"Blocked attempt to buy already-owned item: {item.itemName}");
            return;
        }

        currentItem = item;
        selectedQuantity = Mathf.Max(1, quantity);

        HideAllPanels();

        if (item.type == ItemType.Consumable)
        {
            consumableConfirmationPanel?.SetActive(true);
        }
        else
        {
            inconsumableConfirmationPanel?.SetActive(true);
            selectedQuantity = 1;
        }

        bool isConsumable = item.type == ItemType.Consumable;

        if (plusButton) plusButton.gameObject.SetActive(isConsumable);
        if (minusButton) minusButton.gameObject.SetActive(isConsumable);
        if (quantityInputField) quantityInputField.gameObject.SetActive(isConsumable);
        if (confirmationAmountText) confirmationAmountText.gameObject.SetActive(isConsumable);

        if (confirmationIcon) confirmationIcon.sprite = item.icon;
        if (confirmationItemName) confirmationItemName.text = item.itemName;

        UpdateConfirmationUI();

        if (confirmPurchaseButton != null)
        {
            confirmPurchaseButton.onClick.RemoveAllListeners();
            confirmPurchaseButton.onClick.AddListener(ConfirmPurchase);
        }

        if (cancelPurchaseButton != null)
        {
            audioManager.playSFX(audioManager.click);
            cancelPurchaseButton.onClick.RemoveAllListeners();
            cancelPurchaseButton.onClick.AddListener(HideAllPanels);
        }

        if (isConsumable)
        {
            if (plusButton != null && minusButton != null)
            {
                plusButton.onClick.RemoveAllListeners();
                minusButton.onClick.RemoveAllListeners();

                plusButton.onClick.AddListener(() => ChangeQuantity(1));
                minusButton.onClick.AddListener(() => ChangeQuantity(-1));
            }
        }
    }

    private void UpdateConfirmationUI()
    {
        if (currentItem == null) return;

        int totalPrice = (int)(currentItem.price * selectedQuantity);

        if (confirmationPrice) confirmationPrice.text = $"Harga: {totalPrice:F0} Gold";

        if (confirmationAmountText)
            confirmationAmountText.text = $"Jumlah: {selectedQuantity}";

        if (quantityInputField != null)
            quantityInputField.text = selectedQuantity.ToString();

        if (confirmPurchaseButton != null)
        {
            bool canAfford = AvatarMoneyManager.Instance != null && AvatarMoneyManager.Instance.CanAfford(totalPrice);
            confirmPurchaseButton.interactable = canAfford;
            if (confirmationPrice != null)
                confirmationPrice.color = canAfford ? Color.black : Color.red;
        }
    }

    private void OnQuantityInputChanged(string newText)
    {
        if (int.TryParse(newText, out int newQuantity))
        {
            selectedQuantity = Mathf.Max(1, newQuantity);
        }
        else
        {
            selectedQuantity = 1;
        }
        UpdateConfirmationUI();
    }

    private void ChangeQuantity(int delta)
    {
        selectedQuantity = Mathf.Max(1, selectedQuantity + delta);
        UpdateConfirmationUI();
    }

    public void ConfirmPurchase()
    {
        int totalCost = (int)(currentItem.price * selectedQuantity);
        bool success = PurchaseItem(currentItem, selectedQuantity, totalCost);
        ShowResultPanel(success);

        if (success && currentItem != null && currentItem.type == ItemType.Inconsumable)
        {
          UnityEngine.Object.FindFirstObjectByType<UIShoppingPage>()?.RefreshShopUI();

        }
    }

    private bool PurchaseItem(ItemData item, int quantity, int totalCost)
{
    if (audioManager != null) audioManager.playSFX(audioManager.Buying);

    if (item == null || AvatarMoneyManager.Instance == null || !AvatarMoneyManager.Instance.CanAfford(totalCost))
        return false;

    AvatarMoneyManager.Instance.SpendMoney(totalCost);
    InventoryManager.Instance?.AddItem(item, quantity);

    var HistoryPurchase = new Dictionary<string, object>
    {
        { "date", DateTime.Now.ToString("yyyy-MM-dd") },
        { "description", "Beli-" + (currentItem != null ? currentItem.itemName : "item") },
        { "amount", (totalCost * -1) }
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

    if (item.type == ItemType.Inconsumable)
        itemDatabase.MarkAsPurchased(item);

    return true;
}


    private void ShowResultPanel(bool success)
    {
        HideAllPanels();
        if (resultPanel != null) resultPanel.SetActive(true);

        int balance = AvatarMoneyManager.Instance != null ? AvatarMoneyManager.Instance.GetMoney() : 0;

        if (resultTitleText != null && resultMessageText != null)
        {
            if (success)
            {
                resultTitleText.text = "Pembayaran Berhasil";
                resultMessageText.text = $"{(currentItem != null ? currentItem.itemName : "Item")} sudah masuk ke inventory kamu!";
            }
            else
            {
                resultTitleText.text = "Pembayaran Gagal";
                string message = "Saldo kamu tidak cukup untuk membeli item ini.";

                if (currentItem != null && currentItem.type == ItemType.Inconsumable && itemDatabase != null && itemDatabase.IsPurchased(currentItem))
                    message = "Kamu sudah memiliki item ini!";

                resultMessageText.text = message;
            }
        }

        if (resultBalanceText != null)
            resultBalanceText.text = $"Sisa saldo: {balance:F0} Gold";

        if (resultBackButton != null)
        {
            resultBackButton.onClick.RemoveAllListeners();
            resultBackButton.onClick.AddListener(() => resultPanel.SetActive(false));
        }
    }

    private void HideAllPanels()
    {
        consumableConfirmationPanel?.SetActive(false);
        inconsumableConfirmationPanel?.SetActive(false);
        resultPanel?.SetActive(false);
    }
}
