using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Dropdown graphicsDropDown, language, backgroundDropdown;
    [SerializeField] private GameObject settingsMenu;
    [SerializeField] private LocalizationManager localizationManager;
    [SerializeField] private MenuScript menu;
    private void Start() {
        musicSlider.value = PlayerPrefs.GetFloat("Volume");
        graphicsDropDown.value = PlayerPrefs.GetInt("Graphics");
        backgroundDropdown.value = PlayerPrefs.GetInt("Background");
        if(PlayerPrefs.GetString("Language") == "ru_RU"){
            language.value = 1;
        }
        if(PlayerPrefs.GetString("Language") == "en_US"){
            language.value = 0;
        }
    }
    private void FixedUpdate() {
        PlayerPrefs.SetFloat("Volume", musicSlider.value);
        PlayerPrefs.SetInt("Graphics", graphicsDropDown.value);
        menu.backgroundID = backgroundDropdown.value;
    }
    public void SetLanguage(){
        if(language.value == 1){
            PlayerPrefs.SetString("Language", "ru_RU");
        }
        else if(language.value == 0){
            PlayerPrefs.SetString("Language", "en_US");
        }
        localizationManager.CurrentLanguage = PlayerPrefs.GetString("Language");
    }
    public void OpenSettings(){
        settingsMenu.SetActive(!settingsMenu.activeSelf);
    }
}