using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;


public class ChildTutorial : MonoBehaviour
{
    private AudioManager audioManager;

    private Button BackButton;
    private Button NextButton;
    private Button TutorialBackButton;
    private Button FinishedNextButton;
    private Button FinishedBackButton;


    private VisualElement root;
    private UIDocument uiDocument;
    private VisualElement s1;
    private VisualElement s2;
    private VisualElement s3;
    private VisualElement s4;
    private VisualElement s5;
    private VisualElement s6;
    private VisualElement s7;
    private VisualElement s8;
    private VisualElement s9;


    private VisualElement FinishedModal;



    void OnEnable()
    {
        
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
        uiDocument = GetComponent<UIDocument>();
        root = uiDocument.rootVisualElement;

        s1 = root.Q<VisualElement>("1");
        s2 = root.Q<VisualElement>("2");
        s3 = root.Q<VisualElement>("3");
        s4 = root.Q<VisualElement>("4");
        s5 = root.Q<VisualElement>("5");
        s6 = root.Q<VisualElement>("6");
        s7 = root.Q<VisualElement>("7");
        s8 = root.Q<VisualElement>("8");
        s9 = root.Q<VisualElement>("9");

        FinishedModal = root.Q<VisualElement>("FinishedModal");

        BackButton = root.Q<Button>("BackButton");
        NextButton = root.Q<Button>("NextButton");
        TutorialBackButton = root.Q<Button>("TutorialBackButton");
        FinishedNextButton = root.Q<Button>("FinishedNextButton");
        FinishedBackButton = root.Q<Button>("FinishedBackButton");
        

        BackButton.RegisterCallback<ClickEvent>(ev => OnBackButtonClicked());
        NextButton.RegisterCallback<ClickEvent>(ev => OnNextButtonClicked());
        TutorialBackButton.RegisterCallback<ClickEvent>(ev => OnTutorialBackButtonClicked());
        FinishedNextButton.RegisterCallback<ClickEvent>(ev => OnFinishedNextButtonClicked());
        FinishedBackButton.RegisterCallback<ClickEvent>(ev => OnFinishedBackButtonClicked());

        s1.style.display = DisplayStyle.Flex;
        s2.style.display = DisplayStyle.None;
        s3.style.display = DisplayStyle.None;

    }

    void OnBackButtonClicked()
    {    audioManager.playSFX(audioManager.click);
        if (s3.style.display == DisplayStyle.Flex)
        {
            s2.style.display = DisplayStyle.Flex;
            s3.style.display = DisplayStyle.None;
        }
        else if (s2.style.display == DisplayStyle.Flex)
        {
            s1.style.display = DisplayStyle.Flex;
            s2.style.display = DisplayStyle.None;
        }
    }
    void OnNextButtonClicked()
    {    audioManager.playSFX(audioManager.click);
        if (s1.style.display == DisplayStyle.Flex)
        {
            s1.style.display = DisplayStyle.None;
            s2.style.display = DisplayStyle.Flex;
        }
        else if (s2.style.display == DisplayStyle.Flex)
        {
            s2.style.display = DisplayStyle.None;
            s3.style.display = DisplayStyle.Flex;
        }
        else if (s3.style.display == DisplayStyle.Flex)
        {
            s3.style.display = DisplayStyle.None;
            s4.style.display = DisplayStyle.Flex;
        }
        else if (s4.style.display == DisplayStyle.Flex)
        {
            s4.style.display = DisplayStyle.None;
            s5.style.display = DisplayStyle.Flex;
        }
        else if (s5.style.display == DisplayStyle.Flex)
        {
            s5.style.display = DisplayStyle.None;
            s6.style.display = DisplayStyle.Flex;
        }
        else if (s6.style.display == DisplayStyle.Flex)
        {
            s6.style.display = DisplayStyle.None;
            s7.style.display = DisplayStyle.Flex;
        }
        else if (s7.style.display == DisplayStyle.Flex)
        {
            s7.style.display = DisplayStyle.None;
            s8.style.display = DisplayStyle.Flex;
        }
        else if (s8.style.display == DisplayStyle.Flex)
        {
            s8.style.display = DisplayStyle.None;
            s9.style.display = DisplayStyle.Flex;
        }
        else if (s9.style.display == DisplayStyle.Flex)
        {
            FinishedModal.style.display = DisplayStyle.Flex;
            BackButton.style.display = DisplayStyle.None;
            NextButton.style.display = DisplayStyle.None;
            TutorialBackButton.style.display = DisplayStyle.None;
        }


    }
    void OnTutorialBackButtonClicked()
    {   audioManager.playSFX(audioManager.click);
        SceneManager.LoadScene("Learning Material");
    }

    void OnFinishedNextButtonClicked()
    {   audioManager.playSFX(audioManager.click);
        SceneManager.LoadScene("MoneyMiniMe");
    }
    void OnFinishedBackButtonClicked()
    {       audioManager.playSFX(audioManager.click);
            s9.style.display = DisplayStyle.Flex;
            FinishedModal.style.display = DisplayStyle.None;
            BackButton.style.display = DisplayStyle.Flex;
            NextButton.style.display = DisplayStyle.Flex;
            TutorialBackButton.style.display = DisplayStyle.Flex;

    }


}
