using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class GameMenuServer : MonoBehaviour
{
    [SerializeField] private GameObject GMenu, RD, RU, LD, LU;
    [SerializeField] private Image menu;
    [SerializeField] private Text menuText;
    private Animator anim;

    private void Awake() {
        anim = GMenu.GetComponent<Animator>();
        GMenu.SetActive(false);
    }

    public void MenuPressed(){
        if(PlayerPrefs.GetInt("Dead") == 0){
            GMenu.SetActive(true);
            anim.SetBool("Close", false);
            RD.SetActive(false);
            RU.SetActive(false);
            LD.SetActive(false);
            LU.SetActive(false);
            menu.color = new Color(255, 255, 255, 0);
            menuText.color = new Color(0, 0, 0, 0);
            Time.timeScale = 0;
        }
    }
    public void Resume(){
        anim.SetBool("Close", true);
        RD.SetActive(true);
        RU.SetActive(true);
        LD.SetActive(true);
        LU.SetActive(true);
        menu.color = new Color(255, 255, 255, 255);
        menuText.color = new Color(0, 0, 0, 255);
        Time.timeScale = 1;
    }
    public void MainMenu(){
        SceneManager.LoadScene("Menu");
        Time.timeScale = 1;
    }
}
