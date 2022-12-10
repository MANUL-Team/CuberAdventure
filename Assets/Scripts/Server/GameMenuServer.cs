using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class GameMenuServer : MonoBehaviour
{
    [SerializeField] private GameObject GMenu, joystick, jump, stats, map, health, attack;
    [SerializeField] private Image menu;
    [SerializeField] private Text menuText;

    private void Awake() {
        GMenu.SetActive(false);
    }

    public void MenuPressed(){
        if(PlayerPrefs.GetInt("Dead") == 0){
            GMenu.SetActive(true);
            joystick.SetActive(false);
            jump.SetActive(false);
            stats.SetActive(false);
            map.SetActive(false);
            health.SetActive(false);
            attack.SetActive(false);
            menu.color = new Color(255, 255, 255, 0);
            menuText.color = new Color(0, 0, 0, 0);
            Time.timeScale = 0;
        }
    }
    public void Resume(){
        GMenu.SetActive(false);
        joystick.SetActive(true);
        jump.SetActive(true);
        stats.SetActive(true);
        map.SetActive(true);
        health.SetActive(true);
        attack.SetActive(true);
        menu.color = new Color(255, 255, 255, 255);
        menuText.color = new Color(0, 0, 0, 255);
        Time.timeScale = 1;
    }
    public void MainMenu(){
        SceneManager.LoadScene("Menu");
        Time.timeScale = 1;
    }
}
