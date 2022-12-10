using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndLevel : MonoBehaviour
{
    [SerializeField]private Transform spawnPos;
    [SerializeField]private Image loadingImg;
    [SerializeField]private GameObject loadMenu;
    [SerializeField]private PlayerStats ps;

    public void OnTriggerEnter2D(Collider2D collision){
        if(collision.CompareTag("Player")){
            PlayerPrefs.SetInt("Level", PlayerPrefs.GetInt("Level") + 1);
            PlayerPrefs.SetInt("LastLevel", PlayerPrefs.GetInt("Level") - 1);
            PlayerPrefs.SetInt("Saving", 1);
            loadMenu.SetActive(true);
            PlayerPrefs.SetFloat("Hp", ps.hp);
            PlayerPrefs.SetInt("NewSpawnTP", 0);
            StartCoroutine("AsyncLoad");
        }
    }
    public void LoadScene(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    IEnumerator AsyncLoad(){
        AsyncOperation operation = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
        while(!operation.isDone){
            loadingImg.fillAmount = operation.progress;
            yield return null;
        }
    }

    private void Awake() {
        
    }

    private void Start() {
        ps = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();
    }

    private void Update() {
        }
}