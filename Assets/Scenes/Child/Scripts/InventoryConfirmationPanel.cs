using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class InventoryConfirmationPanel : MonoBehaviour
{    private AudioManager audioManager;
    public static InventoryConfirmationPanel Instance;

    [Header("UI References")]
    [SerializeField] private GameObject panelGameObject; 
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private Image itemIconImage; 
    [SerializeField] private Button confirmButton; 
    [SerializeField] private Button cancelButton;

    private ItemData currentItemToEat;
    private Action onSuccessCallback; 

    private void Awake()
    { 
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
        if (Instance == null) Instance = this;
        else 
        { 
            Destroy(gameObject); 
            return;
        }

        if (panelGameObject != null) panelGameObject.SetActive(false);
    }

    public void Show(ItemData item, Action successCallback)
    {
        if (item == null) return;

        currentItemToEat = item;
        onSuccessCallback = successCallback;

        if (itemNameText != null) itemNameText.text = $"Apakah kamu ingin memakan {item.itemName}?";
        if (itemIconImage != null) itemIconImage.sprite = item.icon;

        if (panelGameObject != null) 
        {
            panelGameObject.SetActive(true);
            panelGameObject.transform.SetAsLastSibling(); 
        }

        if (confirmButton != null)
        {
            confirmButton.onClick.RemoveAllListeners(); 
            confirmButton.onClick.AddListener(OnConfirmClicked);
        }
        else
        {
            Debug.LogError("InventoryConfirmationPanel: Confirm Button is not assigned in Inspector!");
        }

        if (cancelButton != null)
        {
            cancelButton.onClick.RemoveAllListeners();
            cancelButton.onClick.AddListener(Hide);
        }
    }

    private void OnConfirmClicked()
    {
        Debug.Log("Confirm Button Clicked");

       
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.ConsumeItem(currentItemToEat);
        
         audioManager.StopBG();    
         audioManager.playSFX(audioManager.EattingAudio);
         audioManager.playSFX(audioManager.Happy);
        audioManager.playBG();    
         
        }
        else
        {
            Debug.LogWarning("InventoryManager Instance is missing.");
        }

        if (HungerManager.Instance != null)
        {
            HungerManager.Instance.MarkAsFedToday();
        }

     
        if (PlayerAvatarDisplay.Instance != null)
        {
          
            PlayerAvatarDisplay.Instance.TriggerEatingReaction(); 
        }
        else
        {
            Debug.LogWarning("PlayerAvatarDisplay Instance is missing - cannot trigger animation.");
        }
        
        onSuccessCallback?.Invoke();
        Hide();
    }

    public void Hide()
    {
        if (panelGameObject != null) panelGameObject.SetActive(false);
    }
}