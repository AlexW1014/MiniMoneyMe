using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement; 
public class MenuController : MonoBehaviour
{   
    private AudioManager audioManager;

    [Header("Inventory")]
    [SerializeField] private UIInventoryPage inventoryUI;
    [SerializeField] private Button inventoryButton;

    [Header("Shopping")]
    [SerializeField] private UIShoppingPage shoppingUI;
    [SerializeField] private Button shopButton;

    [Header("Task")]
    [SerializeField] private Button taskButton;
    
    [Header("Wallet")]
    [SerializeField] private Button walletButton;

    [Header("Player Settings")]
    [SerializeField] private Transform player;
    [SerializeField] private Vector3 minimizedScale = new Vector3(0.7f, 0.7f, 1f); 

    private Vector3 originalScale;
    private Vector3 originalPosition;
    [SerializeField] private PlayerAvatarDisplay playerAvatarDisplay;


    private void Awake()
    {   
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
       
        if (player != null)
        {
            originalScale = player.localScale;
            originalPosition = player.localPosition;
            playerAvatarDisplay = player.GetComponent<PlayerAvatarDisplay>();
        }

        if (inventoryButton != null)
            inventoryButton.onClick.AddListener(ToggleInventory);

        if (shopButton != null)
            shopButton.onClick.AddListener(ToggleShopping);

        if (taskButton != null)
            taskButton.onClick.AddListener(Load_TaskScene);

        if (walletButton != null)
            walletButton.onClick.AddListener(Load_WalletScene);
    }

    
    private void Start()
    {
        
        int lastSceneIndex = SceneManager.GetActiveScene().buildIndex;
        PlayerPrefs.SetInt("LastLoggedInScene", lastSceneIndex);
        PlayerPrefs.Save();
        inventoryUI?.InitializeInventoryUI();
        shoppingUI?.InitializeShop();

        if (inventoryUI != null) inventoryUI.Hide();
        if (shoppingUI != null) shoppingUI.Hide();
        
        AdjustPlayerForUI(false);

        if (AvatarClothingManager.Instance != null)
        {
            AvatarClothingManager.Instance.LoadEquippedClothing();
            AvatarClothingManager.Instance.RefreshEquippedClothing();
        }
    }
    private void Update()
    {
        if (!IsAnyUIOpen) return;

        Vector2 inputPosition = Vector2.zero;
        bool inputDetected = false;

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            inputDetected = true;
            inputPosition = Input.GetTouch(0).position;
        }
        else if (Input.GetMouseButtonDown(0))
        {
            inputDetected = true;
            inputPosition = Input.mousePosition;
        }

        if (inputDetected && !IsInputOverUI(inputPosition))
        {
            CloseAllUI();
        }
    }

    public void Load_TaskScene()
    {   
        
        Debug.Log("Loading Task Scene...");
        SceneManager.LoadScene("Children Task Scene");
    }
        public void Load_WalletScene()
    {   audioManager.playSFX(audioManager.click);
        Debug.Log("Loading Wallet Scene...");
        SceneManager.LoadScene("ChildWallet");
    }

     public void ToggleInventory()
    {   
        if(audioManager != null) audioManager.playSFX(audioManager.click);
        if (inventoryUI == null) return;

        if (!inventoryUI.isActiveAndEnabled)
        {
            CloseAllUI(); 
            inventoryUI.Show();
            AdjustPlayerForUI(true);
        }
        else
        {
            inventoryUI.Hide();
            AdjustPlayerForUI(false);
        }
    }

    public void ToggleShopping()
    {   
        if(audioManager != null) audioManager.playSFX(audioManager.click);
        if (shoppingUI == null) return;

        if (!shoppingUI.isActiveAndEnabled)
        {
            CloseAllUI(); 
            shoppingUI.Show();
            AdjustPlayerForUI(true);
        }
        else
        {
            shoppingUI.Hide();
            AdjustPlayerForUI(false);
        }
    }

    private void CloseAllUI()
    {
        inventoryUI?.Hide();
        shoppingUI?.Hide();
        AdjustPlayerForUI(false);

        if (AvatarClothingManager.Instance != null)
        {
            AvatarClothingManager.Instance.LoadEquippedClothing();
            AvatarClothingManager.Instance.RefreshEquippedClothing();
        }
    }

    private bool IsInputOverUI(Vector2 inputPosition)
    {
        PointerEventData inputData = new PointerEventData(EventSystem.current) { position = inputPosition };
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(inputData, results);

        foreach (var hit in results)
        {
            if (inventoryUI != null && (hit.gameObject.transform.IsChildOf(inventoryUI.transform) ||
                                        (inventoryButton != null && hit.gameObject == inventoryButton.gameObject)))
                return true;

            if (shoppingUI != null && (hit.gameObject.transform.IsChildOf(shoppingUI.transform) ||
                                       (shopButton != null && hit.gameObject == shopButton.gameObject)))
                return true;
        }
        return false;
    }

    private void AdjustPlayerForUI(bool uiOpen)
    {
        if (player == null) return;

        if (uiOpen)
        {
            player.localScale = minimizedScale;
            Vector3 newPos = originalPosition;
            newPos.y = 140;
            player.localPosition = newPos;
            if (playerAvatarDisplay != null)
            {
                playerAvatarDisplay.ApplyAvatarCustomization();
            }
            player.SetAsLastSibling();
        }
        else
        {
            player.localScale = originalScale;
            player.localPosition = originalPosition;
            if (playerAvatarDisplay != null)
            {
                playerAvatarDisplay.ApplyAvatarCustomization();
            }
        }
        
        AvatarClothingManager.Instance?.RefreshEquippedClothing();
    }
    
    public bool IsInventoryOpen => inventoryUI != null && inventoryUI.isActiveAndEnabled;
    public bool IsShoppingOpen => shoppingUI != null && shoppingUI.isActiveAndEnabled;
    public bool IsAnyUIOpen => IsInventoryOpen || IsShoppingOpen;
}