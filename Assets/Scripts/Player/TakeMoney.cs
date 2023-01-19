using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TakeMoney : MonoBehaviour
{
    [SerializeField] private GameObject button, particles, soundObj;
    [SerializeField] private int count;
    public int id;
    private int loc;
    private void Start() {
        loc = SceneManager.GetActiveScene().buildIndex;
        if(PlayerPrefs.GetInt("Money" + " " + loc + " " + id.ToString()) == 1){
            gameObject.SetActive(false);
        }
    }
    private void OnTriggerEnter2D(Collider2D other) {
        if(other.CompareTag("Player")){
            button.SetActive(true);
        }
    }
    private void OnTriggerExit2D(Collider2D other) {
        if(other.CompareTag("Player")){
            button.SetActive(false);
        }
    }
    public void TakeMoneyVoid(){
        PlayerPrefs.SetInt("Money" + " " + loc + " " + id.ToString(), 1);
        PlayerPrefs.SetInt("Coins", PlayerPrefs.GetInt("Coins") + count);
        PlayerPrefs.SetInt("Exp", PlayerPrefs.GetInt("Exp") + 5);
        GameObject part = Instantiate(particles, transform.position, Quaternion.identity);
        GameObject sound = Instantiate(soundObj, transform.position, Quaternion.identity);
        gameObject.SetActive(false);
        Destroy(part, 1f);
        Destroy(sound, 1f);
    }
}
