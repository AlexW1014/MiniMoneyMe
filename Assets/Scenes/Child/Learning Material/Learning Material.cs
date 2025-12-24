
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;

public class LearningMaterial : MonoBehaviour
{
    // Start is called before the first frame update
    private AudioManager audioManager;
    private Button BackButton;
    private Button LM0;
    private Button LM1;
    private Button LM2;
    private Button LM3;

    private VisualElement root;
    private UIDocument uiDocument;


    private VisualElement FinishedModal;



    async void OnEnable()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
      
        uiDocument = GetComponent<UIDocument>();
        root = uiDocument.rootVisualElement;
        BackButton = root.Q<Button>("BackButton");
        LM0 = root.Q<Button>("0");
        LM1 = root.Q<Button>("1");
        LM2 = root.Q<Button>("2");
        LM3 = root.Q<Button>("3");
        await Task.Delay(1000);
        LM0.RegisterCallback<ClickEvent>(ev => OnLMClicked(ev));
        LM1.RegisterCallback<ClickEvent>(ev => OnLMClicked(ev));
        LM2.RegisterCallback<ClickEvent>(ev => OnLMClicked(ev));
        LM3.RegisterCallback<ClickEvent>(ev => OnLMClicked(ev));
        


        BackButton.RegisterCallback<ClickEvent>(ev => OnBackButtonClicked());




    }

    void OnBackButtonClicked()
    {
        audioManager.playSFX(audioManager.click);
        SceneManager.LoadScene("MoneyMiniMe");
    }
    void OnLMClicked(ClickEvent evt)
    {
        if(evt.currentTarget == LM0)
        {   audioManager.playSFX(audioManager.click);
            SceneManager.LoadScene("Child Tutorial");}
        else if(evt.currentTarget == LM1)
        {   audioManager.playSFX(audioManager.click);
            SceneManager.LoadScene("Learning Material 1");}
        else if(evt.currentTarget == LM2)
        {   audioManager.playSFX(audioManager.click);
            SceneManager.LoadScene("Learning Material 2");}
        else if(evt.currentTarget == LM3)
        {   audioManager.playSFX(audioManager.click);
            SceneManager.LoadScene("Learning Material 3");}
    }



}
