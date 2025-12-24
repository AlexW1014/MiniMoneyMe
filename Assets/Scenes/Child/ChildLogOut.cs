using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class ChildLogOut : MonoBehaviour
{
    public static ChildLogOut Instance { get; private set; }

    private UIDocument _document;
    private VisualElement LoginMenu;
    private Button LoginButton, BackButton, LogOutButton;
    private Label LoginErrorLabel;
    private TextField LoginEmailTF, LoginPasswordTF;

    private FirebaseAuth auth;
    private DatabaseReference dbRef;

    async void Start() {
     await InitializeFirebase();
    }


    private async void Awake()
    {
       

        _document = GetComponent<UIDocument>();

        LoginMenu = _document.rootVisualElement.Q("LoginMenu");

        LoginButton = _document.rootVisualElement.Q<Button>("login_button");
        BackButton = _document.rootVisualElement.Q<Button>("BackButton");
        LogOutButton = _document.rootVisualElement.Q<Button>("LogOutButton");
 
        LoginErrorLabel = _document.rootVisualElement.Q<Label>("login_error_label");

        LoginEmailTF = _document.rootVisualElement.Q<TextField>("login_email_tf");
        LoginPasswordTF = _document.rootVisualElement.Q<TextField>("login_password_tf");

        LoginButton.RegisterCallback<ClickEvent>(OnLoginButtonClick);
        BackButton.RegisterCallback<ClickEvent>(OnBackButtonClick);
        LogOutButton.RegisterCallback<ClickEvent>(OnLogOutButtonClick);
    


        ClearErrorLabels();
    }

    private async Task InitializeFirebase()
    {

        auth = FirebaseAuth.DefaultInstance;
        dbRef = FirebaseDatabase.DefaultInstance.RootReference;

    }

    private void ClearErrorLabels()
    {
        LoginErrorLabel.text = "";
    }

    private async void OnLoginButtonClick(ClickEvent evt)
    {

        // await InitializeFirebase();
        ClearErrorLabels();
        string email = LoginEmailTF.value.Trim();
        string password = LoginPasswordTF.value.Trim();

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            LoginErrorLabel.text = "❗ Isi semua kolom terlebih dahulu!";
            return;
        }

        else if (email == PlayerPrefs.GetString("email") && password == PlayerPrefs.GetString("password"))
        {
            SceneManager.LoadScene("Guardian Landing Scene");
        }
        else 
        {
            LoginErrorLabel.text = "❌ Log Out gagal! Periksa email dan password.";
            // LoginErrorLabel.text = e.Message;
        }
    }

    private void OnBackButtonClick(ClickEvent evt)
    {
        LoginMenu.style.display = DisplayStyle.None;
    }
    private void OnLogOutButtonClick(ClickEvent evt)
    {
        LoginMenu.style.display = DisplayStyle.Flex;
    }


}
