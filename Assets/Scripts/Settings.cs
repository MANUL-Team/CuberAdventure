using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Dropdown graphicsDropDown, language;
    [SerializeField] private GameObject settingsMenu;
    private void Start() {
        musicSlider.value = PlayerPrefs.GetFloat("Volume");
        graphicsDropDown.value = PlayerPrefs.GetInt("Graphics");
        language.value = PlayerPrefs.GetInt("Language");
    }
    private void Update() {
        PlayerPrefs.SetFloat("Volume", musicSlider.value);
        PlayerPrefs.SetInt("Graphics", graphicsDropDown.value);
        PlayerPrefs.SetInt("Language", language.value);
    }
    public void OpenSettings(){
        settingsMenu.SetActive(!settingsMenu.activeSelf);
    }
}