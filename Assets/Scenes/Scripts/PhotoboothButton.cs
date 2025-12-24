using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PhotoboothButton : MonoBehaviour
{
    [Header("UI Elements to Hide")]
    public List<GameObject> uiObjectsToToggle;

    public void ToggleVisibility()
    {
        Debug.Log("1. Button Clicked!");

        if (uiObjectsToToggle == null || uiObjectsToToggle.Count == 0)
        {
            Debug.LogError("ERROR");
            return;
        }

 
        bool isCurrentlyActive = uiObjectsToToggle[0].activeSelf;
        bool newState = !isCurrentlyActive; 

        Debug.Log($" First object is currently: {isCurrentlyActive}. Switching all to: {newState}");

        int count = 0;
        foreach (GameObject obj in uiObjectsToToggle)
        {
            if (obj != null)
            {
                obj.SetActive(newState);
                count++;
            }
            else
            {
                Debug.LogWarning("empty");
            }
        }

        Debug.Log($"Toggled {count} objects.");
    }
}