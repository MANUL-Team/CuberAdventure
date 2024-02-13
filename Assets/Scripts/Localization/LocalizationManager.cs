using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class LocalizationManager : MonoBehaviour
{
    private static string currentLanguage;
    private static Dictionary<string, string> localizedText;
    public static bool isReady = false;

	public delegate void ChangeLangText();
    public static event ChangeLangText OnLanguageChanged;

    void Awake()
    {
        if (!PlayerPrefs.HasKey("Language"))
        {
            PlayerPrefs.SetString("Language", "ru_RU");
            //if (Application.systemLanguage == SystemLanguage.Russian || Application.systemLanguage == SystemLanguage.Ukrainian || Application.systemLanguage == SystemLanguage.Belarusian)
            //{
            //    PlayerPrefs.SetString("Language", "ru_RU");
            //}   
            //else
            //{
            //    PlayerPrefs.SetString("Language", "en_US");
            //}
        }
        currentLanguage = PlayerPrefs.GetString("Language");
        LoadLocalizedText(currentLanguage);
    }

    public static void LoadLocalizedText(string langName)
    {
        string path = Path.Combine(Application.streamingAssetsPath + "/Languages/", "ru_RU" + ".json");

        string dataAsJson;

        if (Application.platform == RuntimePlatform.Android)
        {
            UnityWebRequest www = UnityWebRequest.Get(path);
            www.SendWebRequest();
            while (!www.isDone){}
            dataAsJson = www.downloadHandler.text;
        }
        else
        {
            dataAsJson = File.ReadAllText(path);
        }

        LocalizationData loadedData = JsonUtility.FromJson<LocalizationData>(dataAsJson);

        localizedText = new Dictionary<string, string>();
        for (int i = 0; i < loadedData.items.Length; i++)
        {
            localizedText.Add(loadedData.items[i].key, loadedData.items[i].value);
        }
        isReady = true;

        OnLanguageChanged?.Invoke();
    }

    public static string GetLocalizedValue(string key)
    {
        if (key != "PlayerName"){
            if (localizedText.ContainsKey(key))
            {
                return localizedText[key];
            }
            else
            {
                return "Localized text with key \"" + key + "\" not found";
            }
        }
        else{
            return PlayerPrefs.GetString("PlayerName");
        }
    }

    public string CurrentLanguage
    {
        get 
        {
            return currentLanguage;
        }
        set
        {
			LoadLocalizedText(value);			
        }
    }
    public bool IsReady
    {
        get
        {
            return isReady;
        }
    }
}