
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class GuardianLearningMaterial : MonoBehaviour
{
    // Start is called before the first frame update
    private Button BackButton;

    private VisualElement root;
    private UIDocument uiDocument;




    void OnEnable()
    {
      
        uiDocument = GetComponent<UIDocument>();
        root = uiDocument.rootVisualElement;
        BackButton = root.Q<Button>("BackButton");
     
        BackButton.RegisterCallback<ClickEvent>(ev => OnBackButtonClicked());

    }

    void OnBackButtonClicked()
    {
        SceneManager.LoadScene("Guardian Learning");
    }


}
