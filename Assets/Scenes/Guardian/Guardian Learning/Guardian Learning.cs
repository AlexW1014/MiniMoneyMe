
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;

public class GuardianLearning : MonoBehaviour
{
    // Start is called before the first frame update
    private Button BackButton;
    private Button LM0;
    private Button LM1;
    private Button LM2;
    private Button LM3;

    private VisualElement root;
    private VisualElement LoadingCover;
    private UIDocument uiDocument;


    private VisualElement FinishedModal;



    async void OnEnable()
    {
      
        uiDocument = GetComponent<UIDocument>();
        root = uiDocument.rootVisualElement;
        BackButton = root.Q<Button>("BackButton");
        LM0 = root.Q<Button>("0");
        LM1 = root.Q<Button>("1");
        LM2 = root.Q<Button>("2");
        LoadingCover = root.Q<VisualElement>("LoadingCover");
        LM0.RegisterCallback<ClickEvent>(ev => OnLMClicked(ev));
        LM1.RegisterCallback<ClickEvent>(ev => OnLMClicked(ev));
        LM2.RegisterCallback<ClickEvent>(ev => OnLMClicked(ev));
        BackButton.RegisterCallback<ClickEvent>(ev => OnBackButtonClicked());

        await Task.Delay(1000);
        LoadingCover.style.display = DisplayStyle.None;
    }

    void OnBackButtonClicked()
    {
        SceneManager.LoadScene("Guardian Landing Scene");
    }
    void OnLMClicked(ClickEvent evt)
    {
        if(evt.currentTarget == LM0)
        {  
            SceneManager.LoadScene("Guardian Learning 0");}
        else if(evt.currentTarget == LM1)
        {   
            SceneManager.LoadScene("Guardian Learning 1");}
        else if(evt.currentTarget == LM2)
        {   
            SceneManager.LoadScene("Guardian Learning 2");}
        else if(evt.currentTarget == LM3)
        {   
            SceneManager.LoadScene("Guardian Learning 3");}
    }



}
