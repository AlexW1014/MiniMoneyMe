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
public class GuardianTask : MonoBehaviour
{

    public GuardianTask Instance { get; private set; }

    private string currentEditingTaskDifficulty = "";

    private FirebaseAuth auth;
    private string guardianId;

    private DatabaseReference dbRef;
    private UIDocument _document;
    private string guardianChild;
    private VisualElement GuardianTaskContainer;
    private Button GuardianBackButton;
    private Label NameLabel;
    private VisualElement EasyTaskContainer;
    private VisualElement MediumTaskContainer;
    private VisualElement HardTaskContainer;
    private VisualElement EasyTaskEdit;
    private VisualElement MediumTaskEdit;
    private VisualElement HardTaskEdit;
    private Label EasyTaskLabel;
    private Label MediumTaskLabel;
    private Label HardTaskLabel;


    private VisualElement GuardianEditTaskModalContainer;
    private Button EditTaskCancelButton;
    private Button EditTaskConfirmButton;
    private TextField NewTaskTextField;

    private VisualElement GuardianApproveTaskModalContainer;
    private Button ApproveTaskCancelButton;
    private Button ApproveTaskConfirmButton;
    private Label SelectedTaskLabel;



    private async void Start()
    {


        guardianChild = StaticGuardianData.guardianChild;
        guardianId = StaticGuardianData.guardianId;
        Debug.Log("Fetching tasks for guardian: " + guardianId + ", child: " + guardianChild);

         _document = GetComponent<UIDocument>();
        guardianChild = StaticGuardianData.guardianChild;
        guardianId = StaticGuardianData.guardianId;
        GuardianTaskContainer = _document.rootVisualElement.Q<VisualElement>("GuardianTaskContainer");
        GuardianBackButton = _document.rootVisualElement.Q<Button>("GuardianBackButton");
        NameLabel = _document.rootVisualElement.Q<Label>("NameLabel");
        EasyTaskContainer = _document.rootVisualElement.Q<VisualElement>("EasyTaskContainer");
        MediumTaskContainer = _document.rootVisualElement.Q<VisualElement>("MediumTaskContainer");
        HardTaskContainer = _document.rootVisualElement.Q<VisualElement>("HardTaskContainer");
        EasyTaskEdit = _document.rootVisualElement.Q<VisualElement>("EasyTaskEdit");
        MediumTaskEdit = _document.rootVisualElement.Q<VisualElement>("MediumTaskEdit");
        HardTaskEdit = _document.rootVisualElement.Q<VisualElement>("HardTaskEdit");
        EasyTaskLabel = _document.rootVisualElement.Q<Label>("EasyTaskLabel");
        MediumTaskLabel = _document.rootVisualElement.Q<Label>("MediumTaskLabel");
        HardTaskLabel = _document.rootVisualElement.Q<Label>("HardTaskLabel");

        GuardianBackButton.RegisterCallback<ClickEvent>(evt => OnGuardianBackButtonClick());
        NameLabel.text = guardianChild + "'s Tasks";
        EasyTaskContainer.RegisterCallback<ClickEvent>(async evt => await OnTaskContainerCLick(evt));
        MediumTaskContainer.RegisterCallback<ClickEvent>(async evt => await OnTaskContainerCLick(evt));
        HardTaskContainer.RegisterCallback<ClickEvent>(async evt => await OnTaskContainerCLick(evt));
        EasyTaskEdit.RegisterCallback<ClickEvent>(evt => OnTaskEditButtonClick(evt));
        MediumTaskEdit.RegisterCallback<ClickEvent>(evt => OnTaskEditButtonClick(evt));
        HardTaskEdit.RegisterCallback<ClickEvent>(evt => OnTaskEditButtonClick(evt));

        


        GuardianEditTaskModalContainer = _document.rootVisualElement.Q<VisualElement>("GuardianEditTaskModalContainer");
        EditTaskCancelButton = _document.rootVisualElement.Q<Button>("EditTaskCancelButton");
        EditTaskConfirmButton = _document.rootVisualElement.Q<Button>("EditTaskConfirmButton");
        NewTaskTextField = _document.rootVisualElement.Q<TextField>("NewTaskTextField");
        EditTaskCancelButton.RegisterCallback<ClickEvent>(evt => GuardianEditTaskModalContainer.style.display = DisplayStyle.None);
        EditTaskConfirmButton.RegisterCallback<ClickEvent>(async evt => await OnEditTaskConfirmButtonClick());

        GuardianApproveTaskModalContainer = _document.rootVisualElement.Q<VisualElement>("GuardianApproveTaskModalContainer");
        ApproveTaskCancelButton = _document.rootVisualElement.Q<Button>("ApproveTaskCancelButton");
        ApproveTaskConfirmButton = _document.rootVisualElement.Q<Button>("ApproveTaskConfirmButton");
        SelectedTaskLabel = _document.rootVisualElement.Q<Label>("SelectedTaskLabel");

        ApproveTaskCancelButton.RegisterCallback<ClickEvent>(evt => OnApproveTaskCancelButtonClick());
        ApproveTaskConfirmButton.RegisterCallback<ClickEvent>(evt => OnApproveTaskConfirmButtonClick());


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
                    EasyTaskLabel.text = "(Pending)" + EasyTaskLabel.text;
                }
                if (taskSnapshot.Child("medium_task").Child("status").Value.ToString() == "pending")
                {
                    MediumTaskLabel.text = "(Pending)" + MediumTaskLabel.text;
                }
                if (taskSnapshot.Child("hard_task").Child("status").Value.ToString() == "pending")
                {
                    HardTaskLabel.text = "(Pending)" + HardTaskLabel.text;
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

    private void OnGuardianBackButtonClick()
    {
        Debug.Log("Guardian Back Button Clicked");
        // Add functionality here
        SceneManager.LoadScene("Guardian Landing Scene");
    }

    private void OnTaskEditButtonClick(ClickEvent evt)
    {
        Debug.Log("Task Edit Button Clicked");
        
        VisualElement clickedButton = evt.currentTarget as VisualElement;
        
        if (clickedButton == EasyTaskEdit)
        {
            currentEditingTaskDifficulty = "easy_task";
        }
        else if (clickedButton == MediumTaskEdit)
        {
            currentEditingTaskDifficulty = "medium_task";
        }
        else if (clickedButton == HardTaskEdit)
        {
            currentEditingTaskDifficulty = "hard_task";
        }
        
        // Fetch current task based on difficulty
        NewTaskTextField.value = "Masukan Tugas Baru";
        GuardianEditTaskModalContainer.style.display = DisplayStyle.Flex;
        GuardianApproveTaskModalContainer.style.display = DisplayStyle.None;
    }

    private async Task OnEditTaskConfirmButtonClick()
    {
        string newTask = NewTaskTextField.value;

        var status = await dbRef.Child("users").Child(guardianId).Child("children").Child(guardianChild).Child("task").Child(currentEditingTaskDifficulty).Child("status").GetValueAsync();

        if (!string.IsNullOrEmpty(currentEditingTaskDifficulty))
        {
            Debug.Log(currentEditingTaskDifficulty + " task updated to: " + newTask);
            await dbRef.Child("users").Child(guardianId).Child("children").Child(guardianChild).Child("task").Child(currentEditingTaskDifficulty).Child("task").SetValueAsync(newTask);
        }
        if(status.Value.ToString() == "approved")
        {
            newTask = "<s>" + newTask + "</s>";
        } 
        else if(status.Value.ToString() == "pending")
        {
            newTask = "(Pending)" + newTask;
        }

        if(currentEditingTaskDifficulty == "easy_task")
        {
            EasyTaskLabel.text = newTask;
        }
        else if(currentEditingTaskDifficulty == "medium_task")
        {
            MediumTaskLabel.text = newTask;
        }
        else if(currentEditingTaskDifficulty == "hard_task")
        {
            HardTaskLabel.text = newTask;
        }
        GuardianEditTaskModalContainer.style.display = DisplayStyle.None;
    }
    private async Task OnTaskContainerCLick(ClickEvent evt)
    {

        VisualElement clickedContainer = evt.currentTarget as VisualElement;


        if (clickedContainer == EasyTaskContainer)
        {
            currentEditingTaskDifficulty = "easy_task";
            SelectedTaskLabel.text = $"{EasyTaskLabel.text}";

        }
        else if (clickedContainer == MediumTaskContainer)
        {
            currentEditingTaskDifficulty = "medium_task";
            SelectedTaskLabel.text = $"{MediumTaskLabel.text}";
        }
        else if (clickedContainer == HardTaskContainer)
        {
            currentEditingTaskDifficulty = "hard_task";
            SelectedTaskLabel.text = $"{HardTaskLabel.text}";
        }

        var status = await dbRef.Child("users").Child(guardianId).Child("children").Child(guardianChild).Child("task").Child(currentEditingTaskDifficulty).Child("status").GetValueAsync();
        Debug.Log("Task status: " + status);
        if (status.Value.ToString() == "approved")
        {
            Debug.Log("Task already approved.");
            return;
        }


        // Fetch task data from Firebase
            // dbRef.Child("users").Child(guardianId).Child("children").Child(guardianChild).Child("task").Child(currentEditingTaskDifficulty).GetValueAsync().ContinueWith(task =>

        GuardianApproveTaskModalContainer.style.display = DisplayStyle.Flex;
    }

    private async void OnApproveTaskConfirmButtonClick()
    {
        Debug.Log("Approve Task Confirm Button Clicked");
        // Add functionality here
        var taskSnapshot = await dbRef.Child("users").Child(guardianId).Child("children").Child(guardianChild).Child("task").Child(currentEditingTaskDifficulty).GetValueAsync();
        if (currentEditingTaskDifficulty == "easy_task")
        {
            await dbRef.Child("users").Child(guardianId).Child("children").Child(guardianChild).Child("task").Child("easy_task").Child("status").SetValueAsync("approved");
            var walletValue = await dbRef.Child("users").Child(guardianId).Child("children").Child(guardianChild).Child("wallet").Child("normal_fund").GetValueAsync();
            await dbRef.Child("users").Child(guardianId).Child("children").Child(guardianChild).Child("wallet").Child("normal_fund").SetValueAsync(int.Parse(walletValue.Value.ToString()) + 3);
            EasyTaskLabel.text = "<s>" + taskSnapshot.Child("task").Value.ToString() + "</s>";


            var HistoryPurchase = new Dictionary<string, object>
            {
                { "description", "Hadiah Tugas Mudah" },
                { "amount", 3 },
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
 
        }
        else if (currentEditingTaskDifficulty == "medium_task")
        {
            await dbRef.Child("users").Child(guardianId).Child("children").Child(guardianChild).Child("task").Child("medium_task").Child("status").SetValueAsync("approved");
            var walletValue = await dbRef.Child("users").Child(guardianId).Child("children").Child(guardianChild).Child("wallet").Child("normal_fund").GetValueAsync();
            await dbRef.Child("users").Child(guardianId).Child("children").Child(guardianChild).Child("wallet").Child("normal_fund").SetValueAsync(int.Parse(walletValue.Value.ToString()) + 5);
            MediumTaskLabel.text = "<s>" + taskSnapshot.Child("task").Value.ToString() + "</s>";


            var HistoryPurchase = new Dictionary<string, object>
            {
                { "description", "Hadiah Tugas Sedang" },
                { "amount", 5 },
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
 

            
        }
        else if (currentEditingTaskDifficulty == "hard_task")
        {
            await dbRef.Child("users").Child(guardianId).Child("children").Child(guardianChild).Child("task").Child("hard_task").Child("status").SetValueAsync("approved");
            var walletValue = await dbRef.Child("users").Child(guardianId).Child("children").Child(guardianChild).Child("wallet").Child("normal_fund").GetValueAsync();
            await dbRef.Child("users").Child(guardianId).Child("children").Child(guardianChild).Child("wallet").Child("normal_fund").SetValueAsync(int.Parse(walletValue.Value.ToString()) + 7);
            HardTaskLabel.text = "<s>" + taskSnapshot.Child("task").Value.ToString() + "</s>";

            var HistoryPurchase = new Dictionary<string, object>
            {
                { "description", "Hadiah Tugas Sulit" },
                { "amount", 7 },
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

        }
        GuardianApproveTaskModalContainer.style.display = DisplayStyle.None;
    }
    
    private void OnApproveTaskCancelButtonClick()
    {
        Debug.Log("Approve Task Cancel Button Clicked");
        // Add functionality here
        GuardianApproveTaskModalContainer.style.display = DisplayStyle.None;
    }

}
