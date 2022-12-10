using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ToVillage : MonoBehaviour
{
    [SerializeField] private GameObject button;

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.CompareTag("Player")){
            button.SetActive(true);
        }
    }
    private void OnTriggerExit2D(Collider2D other) {
        if(other.gameObject.CompareTag("Player")){
            button.SetActive(false);
        }
    }
    public void Leave(){
        SceneManager.LoadScene(3);
        PlayerPrefs.SetInt("NewSpawnTP", 1);
    }
    public void UnLeave(){
        SceneManager.LoadScene(6);
    }
}
