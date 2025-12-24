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
public class ChildrenTask : MonoBehaviour
{


    public ChildrenTask Instance { get; private set; }
     private string guardianChild;
    private string guardianId;

    private string currentEditingTaskDifficulty = "";

    private FirebaseAuth auth;

    private DatabaseReference dbRef;
    private UIDocument _document;
    private VisualElement ChildrenTaskContainer;
    private Button ChildrenBackButton;
    private Label NameLabel;
    private VisualElement EasyTaskContainer;
    private VisualElement MediumTaskContainer;
    private VisualElement HardTaskContainer;
    private VisualElement LoadingCover;


    private Label EasyTaskLabel;
    private Label MediumTaskLabel;
    private Label HardTaskLabel;
    private AudioManager audioManager;



    private VisualElement ChildrenApproveTaskModalContainer;
    private VisualElement ChildrenApproveTaskConfirmationModalContainer;
    private Button ApproveTaskCancelButton;
    private Button ApproveTaskConfirmButton;
    private Button ApproveTaskBackButton;
    private Label SelectedTaskLabel;

    private async void Awake()
    {
        await InitializeFirebase();

        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        _document = GetComponent<UIDocument>();
        guardianChild = StaticGuardianData.guardianChild;
        guardianId = StaticGuardianData.guardianId;
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
        // guardianChild =  "mami";
        // guardianId = "cB2wVFyQULZ1KBQC7T5l7tSxQs42";


        ChildrenTaskContainer = _document.rootVisualElement.Q<VisualElement>("ChildrenTaskContainer");
        ChildrenApproveTaskConfirmationModalContainer = _document.rootVisualElement.Q<VisualElement>("ChildrenApproveTaskConfirmationModalContainer");
        ApproveTaskBackButton = _document.rootVisualElement.Q<Button>("ApproveTaskBackButton");


        ChildrenBackButton = _document.rootVisualElement.Q<Button>("ChildrenBackButton");
        NameLabel = _document.rootVisualElement.Q<Label>("NameLabel");
        EasyTaskContainer = _document.rootVisualElement.Q<VisualElement>("EasyTaskContainer");
        MediumTaskContainer = _document.rootVisualElement.Q<VisualElement>("MediumTaskContainer");
        HardTaskContainer = _document.rootVisualElement.Q<VisualElement>("HardTaskContainer");

        EasyTaskLabel = _document.rootVisualElement.Q<Label>("EasyTaskLabel");
        MediumTaskLabel = _document.rootVisualElement.Q<Label>("MediumTaskLabel");
        HardTaskLabel = _document.rootVisualElement.Q<Label>("HardTaskLabel");
        LoadingCover = _document.rootVisualElement.Q<VisualElement>("LoadingCover");

        ChildrenBackButton.RegisterCallback<ClickEvent>(evt => OnChildrenBackButtonClick());
        NameLabel.text = guardianChild + "'s Tasks";
        EasyTaskContainer.RegisterCallback<ClickEvent>(async evt => await OnTaskContainerCLick(evt));
        MediumTaskContainer.RegisterCallback<ClickEvent>(async evt => await OnTaskContainerCLick(evt));
        HardTaskContainer.RegisterCallback<ClickEvent>(async evt => await OnTaskContainerCLick(evt));
        ApproveTaskBackButton.RegisterCallback<ClickEvent>(evt => OnApproveTaskBackButtonClick());



        ChildrenApproveTaskModalContainer = _document.rootVisualElement.Q<VisualElement>("ChildrenApproveTaskModalContainer");
        ApproveTaskCancelButton = _document.rootVisualElement.Q<Button>("ApproveTaskCancelButton");
        ApproveTaskConfirmButton = _document.rootVisualElement.Q<Button>("ApproveTaskConfirmButton");
        SelectedTaskLabel = _document.rootVisualElement.Q<Label>("SelectedTaskLabel");

        ApproveTaskCancelButton.RegisterCallback<ClickEvent>(evt => OnApproveTaskCancelButtonClick());
        ApproveTaskConfirmButton.RegisterCallback<ClickEvent>(evt => OnApproveTaskConfirmButtonClick());

    }

    private async void Start()
    {
        audioManager.playBG();
        guardianChild = StaticGuardianData.guardianChild;
        guardianId = StaticGuardianData.guardianId;
        Debug.Log("Fetching tasks for guardian: " + guardianId + ", child: " + guardianChild);

        if (dbRef == null)
            await InitializeFirebase();
        try
        {
            var taskSnapshot = await dbRef.Child("users").Child(guardianId).Child("children").Child(guardianChild).Child("task").GetValueAsync();
            Debug.Log(taskSnapshot);
            if (taskSnapshot.Exists)
            {
                EasyTaskLabel.text = taskSnapshot.Child("easy_task").Child("task").Value.ToString();
                MediumTaskLabel.text = taskSnapshot.Child("medium_task").Child("task").Value.ToString();
                HardTaskLabel.text = taskSnapshot.Child("hard_task").Child("task").Value.ToString();

                if (taskSnapshot.Child("easy_task").Child("status").Value.ToString() == "approved")
                {
                    EasyTaskLabel.text = "<s>" + EasyTaskLabel.text + "</s>";
                }

                if (taskSnapshot.Child("medium_task").Child("status").Value.ToString() == "approved")
                {
                    MediumTaskLabel.text = "<s>" + MediumTaskLabel.text + "</s>";
                }

                if (taskSnapshot.Child("hard_task").Child("status").Value.ToString() == "approved")
                {
                    HardTaskLabel.text = "<s>" + HardTaskLabel.text + "</s>";
                }

                if (taskSnapshot.Child("easy_task").Child("status").Value.ToString() == "pending")
                {
                    EasyTaskLabel.text = "(Pending) " + EasyTaskLabel.text;
                }
                if (taskSnapshot.Child("medium_task").Child("status").Value.ToString() == "pending")
                {
                    MediumTaskLabel.text = "(Pending) " + MediumTaskLabel.text;
                }
                if (taskSnapshot.Child("hard_task").Child("status").Value.ToString() == "pending")
                {
                    HardTaskLabel.text = "(Pending) " + HardTaskLabel.text;
                }




            }
            else
            {
                Debug.Log("No children found for guardian: " + guardianId);
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Failed to load tasks: " + ex);
        }


    }

    async void OnEnable()
    {
        await Task.Delay(800); // Small delay to ensure UI is ready
        LoadingCover.style.display = DisplayStyle.None;
                ChildrenBackButton.RegisterCallback<ClickEvent>(evt => OnChildrenBackButtonClick());
                NameLabel.text = guardianChild + "'s Tasks";
                EasyTaskContainer.RegisterCallback<ClickEvent>(async evt => await OnTaskContainerCLick(evt));
                MediumTaskContainer.RegisterCallback<ClickEvent>(async evt => await OnTaskContainerCLick(evt));
                HardTaskContainer.RegisterCallback<ClickEvent>(async evt => await OnTaskContainerCLick(evt));
                ApproveTaskBackButton.RegisterCallback<ClickEvent>(evt => OnApproveTaskBackButtonClick());
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

    private void OnChildrenBackButtonClick()
    {
        SceneManager.LoadScene("MoneyMiniMe");
    }

    private async Task OnTaskContainerCLick(ClickEvent evt)
    {
        audioManager.playSFX(audioManager.click);

        VisualElement clickedContainer = evt.currentTarget as VisualElement;

        var taskText = "";
        if (clickedContainer == EasyTaskContainer)
        {
            currentEditingTaskDifficulty = "easy_task";
            taskText = EasyTaskLabel.text;
        }
        else if (clickedContainer == MediumTaskContainer)
        {
            currentEditingTaskDifficulty = "medium_task";
            taskText = MediumTaskLabel.text;
        }
        else if (clickedContainer == HardTaskContainer)
        {
            currentEditingTaskDifficulty = "hard_task";
            taskText = HardTaskLabel.text;
        }


        var status  = await dbRef.Child("users").Child(guardianId).Child("children").Child(guardianChild).Child("task").Child(currentEditingTaskDifficulty).Child("status").GetValueAsync();
        if (status.Value.ToString() == "pending" || status.Value.ToString() == "approved")
        {
            return;
        }

        // Fetch task data from Firebase
            // dbRef.Child("users").Child(guardianId).Child("children").Child(guardianChild).Child("task").Child(currentEditingTaskDifficulty).GetValueAsync().ContinueWith(task =>

        SelectedTaskLabel.text = $"{taskText}";
        ChildrenApproveTaskModalContainer.style.display = DisplayStyle.Flex;
    }

    private void OnApproveTaskBackButtonClick()
    {
        audioManager.playSFX(audioManager.click);
        ChildrenApproveTaskConfirmationModalContainer.style.display = DisplayStyle.None;
    }


    private void OnApproveTaskConfirmButtonClick()
    {
        audioManager.playSFX(audioManager.click);
        Debug.Log("Approve Task Confirm Button Clicked");
        // Add functionality here
        

        if (currentEditingTaskDifficulty == "easy_task")
        {
            dbRef.Child("users").Child(guardianId).Child("children").Child(guardianChild).Child("task").Child("easy_task").Child("status").SetValueAsync("pending");
            EasyTaskLabel.text = "(Pending) " + EasyTaskLabel.text  ;
        }
        else if (currentEditingTaskDifficulty == "medium_task")
        {
            dbRef.Child("users").Child(guardianId).Child("children").Child(guardianChild).Child("task").Child("medium_task").Child("status").SetValueAsync("pending");
            MediumTaskLabel.text = "(Pending) " + MediumTaskLabel.text  ;
        }
        else if (currentEditingTaskDifficulty == "hard_task")
        {
            dbRef.Child("users").Child(guardianId).Child("children").Child(guardianChild).Child("task").Child("hard_task").Child("status").SetValueAsync("pending");
            HardTaskLabel.text = "(Pending) " + HardTaskLabel.text ;
        }
        ChildrenApproveTaskModalContainer.style.display = DisplayStyle.None;
        ChildrenApproveTaskConfirmationModalContainer.style.display = DisplayStyle.Flex;
    }
    


    private void OnApproveTaskCancelButtonClick()
    {
        audioManager.playSFX(audioManager.click);
        Debug.Log("Approve Task Cancel Button Clicked");
        ChildrenApproveTaskModalContainer.style.display = DisplayStyle.None;
    }



}
