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
    [SerializeField] private bool needToUpdate;
    public int currentId;

    private void Start() {
        ps = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();
        if(levelId != -1 && needToUpdate){
            levelId = currentId;
            if(PlayerPrefs.GetInt("LevelEnded" + levelId) == 1){
                gameObject.SetActive(true);
            } else{
                gameObject.SetActive(false);
            }
        }
        else{
            gameObject.SetActive(true);
        }
    }
    private void FixedUpdate() {
        if(levelId != -1 && needToUpdate){
            levelId = currentId;
        }
        else{
            gameObject.SetActive(true);
        }
    }
    public void Teleport(){
        PlayerPrefs.SetInt("LastLevel", PlayerPrefs.GetInt("Level"));
        PlayerPrefs.SetInt("Level", currentId);
        loadMenu.SetActive(true);
        PlayerPrefs.SetFloat("Hp", ps.hp);
        PlayerPrefs.SetInt("NewSpawnTP", 0);
        Invoke("StartAsync", 1f);
    }

    public void DoTeleport(int index)
    {
        currentId = index;
        Teleport();
    }

    private void StartAsync()
    {
        StartCoroutine(AsyncLoad(currentId));
    }
    IEnumerator AsyncLoad(int id){
        AsyncOperation operation = SceneManager.LoadSceneAsync(id);
        while(!operation.isDone){
            loadingImg.fillAmount = operation.progress;
            yield return null;
        }
    }
}
