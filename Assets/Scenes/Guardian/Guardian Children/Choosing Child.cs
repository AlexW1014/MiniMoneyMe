using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using UnityEngine.SceneManagement;
using System;

public class ChoosingChild : MonoBehaviour
{

    private FirebaseAuth auth;
    private DatabaseReference dbRef;

    VisualTreeAsset ChildrenPrefab;
    VisualElement root;
    VisualElement AddChildrenModalContainer;
    private TextField NameTF;
    private TextField AgeTF;
    private UIDocument _document;

    private Button AddChildrenButton;
    private Button AddChildrenModalCancelButton;
    private Button AddChildrenModalConfirmButton;
    private Button GuardianBackButton;

    private string guardianId;

    public ChoosingChild Instance { get; private set; }

    private async Task InitializeFirebase()
    {

            auth = FirebaseAuth.DefaultInstance;
            dbRef = FirebaseDatabase.DefaultInstance.RootReference;

    }




    private async void Awake()
    {
        await InitializeFirebase();
        guardianId = StaticGuardianData.guardianId;
        // guardianId = "cB2wVFyQULZ1KBQC7T5l7tSxQs42";

        if(!string.IsNullOrEmpty(guardianId))
        {
            PlayerPrefs.SetString("GuardianID", guardianId);
            PlayerPrefs.Save();
        }


        if (!PlayerPrefs.HasKey("GuardianID"))
        {
            Debug.LogError("Guardian ID or Child not found in PlayerPrefs.");
            return;
        }
         guardianId = PlayerPrefs.GetString("GuardianID");
         StaticGuardianData.guardianId = guardianId;
        
        int lastSceneIndex = SceneManager.GetActiveScene().buildIndex;
        PlayerPrefs.SetInt("LastLoggedInScene", lastSceneIndex);
        PlayerPrefs.Save();

        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        _document = GetComponent<UIDocument>();
        AddChildrenModalCancelButton = _document.rootVisualElement.Q("AddChildrenModalCancelButton") as Button;
        AddChildrenModalCancelButton.RegisterCallback<ClickEvent>(OnAddChildrenModalCancelButtonClick);


        NameTF = _document.rootVisualElement.Q<TextField>("Name-TF");
        AgeTF = _document.rootVisualElement.Q<TextField>("Age-TF");

        AddChildrenButton = _document.rootVisualElement.Q("AddChildrenButton") as Button;
        AddChildrenButton.RegisterCallback<ClickEvent>(OnWAddChildrenButtonClick);

        AddChildrenModalConfirmButton = _document.rootVisualElement.Q("AddChildrenModalConfirmButton") as Button;
        AddChildrenModalConfirmButton.RegisterCallback<ClickEvent>(OnAddChildrenModalConfirmButtonClick);

    }


    private async Task<VisualTreeAsset> GetChildrenPrefab()
    {
        VisualTreeAsset ChildrenPrefabAsset = Resources.Load<VisualTreeAsset>("ChildrenDataContainer");

        while (ChildrenPrefabAsset == null)
            await Task.Yield();
        return ChildrenPrefabAsset;
    }

    private async void OnEnable()
    {


        ChildrenPrefab = await GetChildrenPrefab();
        root = GetComponent<UIDocument>().rootVisualElement;
        _document = GetComponent<UIDocument>();

        // AddChildrenModalBackground 
        AddChildrenModalContainer = root.Q<VisualElement>("AddChildrenModalContainer");

        VisualElement ContainerChildren = root.Q<ScrollView>("ContainerChildren");

        AddChildrenButton = _document.rootVisualElement.Q("AddChildrenButton") as Button;
        AddChildrenModalCancelButton = _document.rootVisualElement.Q("AddChildrenModalCancelButton") as Button;
        AddChildrenModalConfirmButton = _document.rootVisualElement.Q("AddChildrenModalConfirmButton") as Button;
        GuardianBackButton = _document.rootVisualElement.Q("GuardianBackButton") as Button;
        GuardianBackButton.RegisterCallback<ClickEvent>(OnGuardianBackButtonClick);



        // Load children from Firebase and populate UI
        if (dbRef == null)
            await InitializeFirebase();
        try
        {
            var childrenSnapshot = await dbRef.Child("users").Child(guardianId).Child("children").GetValueAsync();
            if (childrenSnapshot.Exists)
            {
            foreach (var childSnapshot in childrenSnapshot.Children)
            {
                // read fields
                string childName = childSnapshot.Child("name").Value?.ToString() ?? childSnapshot.Key;
                string childAge = childSnapshot.Child("age").Value?.ToString() ?? "";

                // instantiate UXML
                var instance = ChildrenPrefab.CloneTree();
                VisualElement childElement = instance.Q<VisualElement>("ChildrenDataContainer") ?? instance;

                var nameLabel = childElement.Q<Label>("name-label");
                if (nameLabel != null) nameLabel.text = childName;

                var ageLabel = childElement.Q<Label>("age-label");
                    if (ageLabel != null) ageLabel.text = childAge;

                instance.RegisterCallback<ClickEvent>(OnChildrenClick);
                ContainerChildren.Add(instance);
            }
            }
            else
            {
            Debug.Log("No children found for guardian: " + guardianId);
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Failed to load children: " + ex);
        }






    }

    private void OnChildrenClick(ClickEvent evt)
    {
        var childElement = evt.target as VisualElement;
        var nameLabel = childElement?.Q<Label>("name-label");
        Debug.Log("Child clicked! Name: " + nameLabel?.text);
        StaticGuardianData.guardianChild = nameLabel?.text;
        SceneManager.LoadScene("Guardian Landing Scene");
        // Here you can add logic to transition to the child's detailed view or perform other actions
    }
    private void OnGuardianBackButtonClick(ClickEvent evt)
    {
        PlayerPrefs.SetInt("LastLoggedInScene", 0);
        PlayerPrefs.Save();
        SceneManager.LoadScene("Guardian Login Sign Up");
    }
    private void OnAddChildrenModalCancelButtonClick(ClickEvent evt)
    {
        AddChildrenModalContainer.style.display = DisplayStyle.None;
    }
    private void OnWAddChildrenButtonClick(ClickEvent evt)
    {
        AddChildrenModalContainer.style.display = DisplayStyle.Flex;
    }
    private async void OnAddChildrenModalConfirmButtonClick(ClickEvent evt)
    {
        string name = NameTF.text?.Trim();
        string age = AgeTF.text?.Trim();

        // Require both fields
        if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(age))
        {
            Debug.LogWarning("Both name and age must be provided before confirming.");
            return;
        }

        try
        {
            // Ensure Firebase initialized
            if (dbRef == null)
                await InitializeFirebase();

            // Check for existing child with the same name (case-insensitive check)
            var childrenRef = dbRef.Child("users").Child(guardianId).Child("children");
            var existingSnapshot = await childrenRef.Child(name).GetValueAsync();
            if (existingSnapshot.Exists)
            {
                Debug.LogWarning($"A child with the name '{name}' already exists. Please choose a different name.");
                return;
            }

            Debug.Log("Adding child for guardian: " + guardianId);
            Debug.Log("Name: " + name + ", Age: " + age);

            var childData = new Dictionary<string, object>
            {
                { "name", name },
                { "age", age },
                { "task", new Dictionary<string, object>()
                    {
                        { "easy_task", new Dictionary<string, object> { { "task", "masukan tugas" }, { "status", "waiting" } } },
                        { "medium_task", new Dictionary<string, object> { { "task", "masukan tugas" }, { "status", "waiting" } } },
                        { "hard_task", new Dictionary<string, object> { { "task", "masukan tugas" }, { "status", "waiting" } } }
                    }
                },
                {"wallet", new Dictionary<string, object>()
                    {
                        {"emergency_fund", 0 },
                        {"normal_fund", 15 }
                    }
                },
                {
                    "history", new Dictionary<string, object>()
                    {
                        {
                            DateTime.Today.ToString("yyyy-MM-dd"), new Dictionary<string, object>()
                            {
                                {"first_transaction", new Dictionary<string, object>()
                                    {
                                        { "date", System.DateTime.Now.ToString("yyyy-MM-dd") },
                                        { "description", "Initial Fund" },
                                        { "amount", 15 }
                                    }
                                }
                            }
                        }
                    }
                },
                {"last_login_date", System.DateTime.Now.ToString("yyyy-MM-dd") },
                {"sick_date", ""},
                


            };

            // Save to Firebase under the child's name (keeps original behavior)
            await childrenRef.Child(name).SetValueAsync(childData);

            // Ensure prefab is loaded
            if (ChildrenPrefab == null)
                ChildrenPrefab = await GetChildrenPrefab();

            // Instantiate UI for the new child
            var instance = ChildrenPrefab.CloneTree();
            VisualElement childElement = instance.Q<VisualElement>("ChildrenDataContainer") ?? instance;
            instance.RegisterCallback<ClickEvent>(OnChildrenClick);

            var nameLabel = childElement.Q<Label>("name-label");
            if (nameLabel != null) nameLabel.text = name;

            var ageLabel = childElement.Q<Label>("age-label");
            if (ageLabel != null) ageLabel.text = age;

            // Find the container and add the new child element
            VisualElement container = root?.Q<ScrollView>("ContainerChildren");
            if (container == null)
            {
                // Fallback to querying UIDocument root if root wasn't set
                container = _document?.rootVisualElement.Q<ScrollView>("ContainerChildren");
            }

            if (container != null)
            {
                container.Add(instance);
            }
            else
            {
                Debug.LogWarning("ContainerChildren not found. UI not updated.");
            }

            // Clear form and hide modal
            NameTF.value = "";
            AgeTF.value = "";
            AddChildrenModalContainer.style.display = DisplayStyle.None;
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Failed to add child: " + ex);
        }
    }




}