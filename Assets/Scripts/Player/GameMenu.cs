using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMenu : MonoBehaviour
{

    [SerializeField] private GameObject GMenu;


    public void MenuPressed(){
        if(PlayerPrefs.GetInt("Dead") == 0){
            GMenu.SetActive(true);
            Time.timeScale = 0f;
        }
    }
    public void Resume(){
        GMenu.SetActive(false);
        Time.timeScale = 1f;
    }
    public void MainMenu(){
        PlayerPrefs.SetInt("Dead", 0);
        SceneManager.LoadScene("Menu");
        Time.timeScale = 1f;
    }



}
