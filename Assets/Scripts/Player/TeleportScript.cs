using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TeleportScript : MonoBehaviour
{
    [SerializeField]private Image loadingImg;
    [SerializeField]private GameObject loadMenu;
    [SerializeField]private PlayerStats ps;
    [SerializeField]private int levelId;

    private void Start() {
        if(levelId != -1){
            if(PlayerPrefs.GetInt("LevelEnded" + levelId) == 1){
                gameObject.SetActive(true);
            } else{
                gameObject.SetActive(false);
            }
        }
        else{
            gameObject.SetActive(true);
        }
        ps = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();
        
    }
    public void Teleport(int id){
        PlayerPrefs.SetInt("LastLevel", PlayerPrefs.GetInt("Level"));
        PlayerPrefs.SetInt("Level", id);
        loadMenu.SetActive(true);
        PlayerPrefs.SetFloat("Hp", ps.hp);
        PlayerPrefs.SetInt("NewSpawnTP", 0);
        StartCoroutine(AsyncLoad(id));
    }
    IEnumerator AsyncLoad(int id){
        AsyncOperation operation = SceneManager.LoadSceneAsync(id);
        while(!operation.isDone){
            loadingImg.fillAmount = operation.progress;
            yield return null;
        }
    }
}
