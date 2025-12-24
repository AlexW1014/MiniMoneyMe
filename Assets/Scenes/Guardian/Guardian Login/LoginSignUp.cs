using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class LoginSignUp : MonoBehaviour
{
    public static LoginSignUp Instance { get; private set; }

    private UIDocument _document;
    private VisualElement LoginMenu, SignUpMenu;
    private Button LoginButton, SignUpButton;
    private Label LoginText, SignUpText, LoginErrorLabel, SignUpErrorLabel;
    private TextField SignUpNameTF, SignUpEmailTF, SignUpPasswordTF, SignUpPassword2TF, LoginEmailTF, LoginPasswordTF;

    private FirebaseAuth auth;
    private DatabaseReference dbRef;

    async void Start() {
     await InitializeFirebase();

        int lastSceneIndex = PlayerPrefs.GetInt("LastLoggedInScene", 0); // Default to 0 if not set
        if (lastSceneIndex != 0) {
            SceneManager.LoadScene(lastSceneIndex);
        } else {
            // If no saved scene, load the main menu (index 0)
            // SceneManager.LoadScene(0);
        }
        Debug.Log("Loaded last scene index: " + lastSceneIndex);
    }


    private async void Awake()
    {
       

        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        _document = GetComponent<UIDocument>();

        LoginMenu = _document.rootVisualElement.Q("LoginMenu");
        SignUpMenu = _document.rootVisualElement.Q("SignUpMenu");

        LoginButton = _document.rootVisualElement.Q<Button>("login_button");
        SignUpButton = _document.rootVisualElement.Q<Button>("sign_up_button");
        LoginText = _document.rootVisualElement.Q<Label>("login_text");
        SignUpText = _document.rootVisualElement.Q<Label>("sign_up_text");

        LoginErrorLabel = _document.rootVisualElement.Q<Label>("login_error_label");
        SignUpErrorLabel = _document.rootVisualElement.Q<Label>("sign_up_error_label");

        SignUpNameTF = _document.rootVisualElement.Q<TextField>("sign_up_name_tf");
        SignUpEmailTF = _document.rootVisualElement.Q<TextField>("sign_up_email_tf");
        SignUpPasswordTF = _document.rootVisualElement.Q<TextField>("sign_up_password_tf");
        SignUpPassword2TF = _document.rootVisualElement.Q<TextField>("sign_up_password2_tf");
        LoginEmailTF = _document.rootVisualElement.Q<TextField>("login_email_tf");
        LoginPasswordTF = _document.rootVisualElement.Q<TextField>("login_password_tf");

        LoginButton.RegisterCallback<ClickEvent>(OnLoginButtonClick);
        SignUpButton.RegisterCallback<ClickEvent>(OnSignUpButtonClick);
        LoginText.RegisterCallback<ClickEvent>(OnLoginTextClick);
        SignUpText.RegisterCallback<ClickEvent>(OnSignUpTextClick);


        ClearErrorLabels();
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

    private void ClearErrorLabels()
    {
        LoginErrorLabel.text = "";
        SignUpErrorLabel.text = "";
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

        try
        {
            var result = await auth.SignInWithEmailAndPasswordAsync(email, password);
            LoginErrorLabel.text = $"✅ Selamat datang, {result.User.DisplayName}!";
            LoginErrorLabel.text = $"✅ Selamat datang, {result.User.Email}!";

            PlayerPrefs.SetString("email", email);
            PlayerPrefs.SetString("password", password);
            PlayerPrefs.Save();
            // Simpan data guardian secara statis
            // Debug.Log(result.User.UserId);
            StaticGuardianData.guardianId = result.User.UserId;
            StaticGuardianData.guardianName = result.User.DisplayName;
            SceneManager.LoadScene("Guardian Children");
        }
        catch (Exception e)
        {
            LoginErrorLabel.text = "❌ Login gagal! Periksa email dan password.";
            // LoginErrorLabel.text = e.Message;
            Debug.LogError($"Login gagal: {e.Message}");
        }
    }

    private async void OnSignUpButtonClick(ClickEvent evt)
    {
        ClearErrorLabels();
        string name = SignUpNameTF.value.Trim();
        string email = SignUpEmailTF.value.Trim();
        string password = SignUpPasswordTF.value.Trim();
        string confirmPassword = SignUpPassword2TF.value.Trim();

        if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            SignUpErrorLabel.text = "❗ Semua kolom harus diisi!";
            return;
        }

        if (password != confirmPassword)
        {
            SignUpErrorLabel.text = "❗ Password tidak cocok!";
            return;
        }

        try
        {
            var result = await auth.CreateUserWithEmailAndPasswordAsync(email, password);
            string userId = result.User.UserId;

            // Buat email aman untuk key database
            string emailSafe = email.Replace(".", "_").Replace("@", "_at_");

            // Data user
            var userData = new Dictionary<string, object>
            {
                { "name", name },
                { "email", email },
                { "uid", userId },
                { "createdAt", DateTime.UtcNow.ToString() },
                { "updatedAt", DateTime.UtcNow.ToString() },
                { "user_id", userId },
            };

            // Simpan di dua tempat: berdasarkan UID dan email
            await dbRef.Child("users").Child(userId).SetValueAsync(userData);

            SignUpErrorLabel.text = "✅ Akun berhasil dibuat! Silakan login.";
            OnLoginTextClick(null);
        }
        catch (Exception e)
        {
            SignUpErrorLabel.text = "❌ Gagal membuat akun. Email mungkin sudah digunakan.";
            Debug.LogError($"Signup gagal: {e.Message}");
        }
    }

    private void OnLoginTextClick(ClickEvent evt)
    {
        ClearErrorLabels();
        LoginMenu.style.display = DisplayStyle.Flex;
        SignUpMenu.style.display = DisplayStyle.None;
    }

    private void OnSignUpTextClick(ClickEvent evt)
    {
        ClearErrorLabels();
        SignUpMenu.style.display = DisplayStyle.Flex;
        LoginMenu.style.display = DisplayStyle.None;
    }
}
