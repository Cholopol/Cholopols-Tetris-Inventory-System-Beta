using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.Settings;

public class LocaleSelector : MonoBehaviour
{
    private bool active = false;
    [SerializeField] private Dropdown dropDown;

    private void Start()
    {
        int ID =PlayerPrefs.GetInt("LocaleKey", 0);
        dropDown.value = ID;
        ChangeLocale(ID);
    }

    public void ChangeLocale(int _localID)
    {
        if (active) return;
        StartCoroutine(SetLocale(_localID));
    }

    IEnumerator SetLocale(int _localID)
    {
        active = true;
        yield return LocalizationSettings.InitializationOperation;
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[_localID];
        PlayerPrefs.SetInt("LocaleKey", _localID);
        active = false;
    }
}
