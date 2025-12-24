using System;
using UnityEngine;

public class HungerManager : MonoBehaviour
{
    public static HungerManager Instance { get; private set; }
    public event Action<bool> OnHungerStateChanged;
    public string playerPrefsKey = "LastAteDate";
    public bool IsHungry { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }
    }

    private void Start() => CheckDailyHunger();

    public void CheckDailyHunger()
    {
        string today = DateTime.Now.ToString("yyyy-MM-dd");
        if (!PlayerPrefs.HasKey(playerPrefsKey))
        {
            SetHungry(true);
            return;
        }

        string last = PlayerPrefs.GetString(playerPrefsKey, "");
        if (string.IsNullOrEmpty(last) || last != today) SetHungry(true);
        else SetHungry(false);
    }

    public void MarkAsFedToday()
    {
        string today = DateTime.Now.ToString("yyyy-MM-dd");
        PlayerPrefs.SetString(playerPrefsKey, today);
        PlayerPrefs.Save();
        SetHungry(false);
    }

    private void SetHungry(bool hungry)
    {
        if (IsHungry == hungry) return;
        IsHungry = hungry;
        OnHungerStateChanged?.Invoke(IsHungry);
    }
}
