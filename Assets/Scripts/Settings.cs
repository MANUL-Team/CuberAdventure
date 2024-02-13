using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
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
        language.value = PlayerPrefs.GetInt("Language");
    }
    public void SetVolume()
    {
        PlayerPrefs.SetFloat("Volume", musicSlider.value);
    }

    public void SetGraphics()
    {
        PlayerPrefs.SetInt("Graphics", graphicsDropDown.value);
    }

    public void SetBackground()
    {
        menu.backgroundID = backgroundDropdown.value;
    }
    public void SetLanguage(){
        if (language.value != PlayerPrefs.GetInt("Language"))
        {
            PlayerPrefs.SetInt("Language", language.value);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
    public void OpenSettings(){
        settingsMenu.SetActive(!settingsMenu.activeSelf);
    }
}