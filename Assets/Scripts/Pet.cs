using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pet : MonoBehaviour
{

    [SerializeField] private GameObject[] pets;
    public int petIndex;
    private IEnumerator petCheck(){
        yield return new WaitForSeconds(10f);
        if(PlayerPrefs.GetInt("Pet") != 0){
            pets[PlayerPrefs.GetInt("Pet") - 1].SetActive(true);
        }
    }
    private void Start() {
        if(PlayerPrefs.GetInt("Pet") != 0){
            pets[PlayerPrefs.GetInt("Pet") - 1].SetActive(true);
        }
        StartCoroutine("petCheck");
    }
    private void Update() {
        petIndex = PlayerPrefs.GetInt("Pet");
        if(PlayerPrefs.GetInt("Pet") == 2){
            if(PlayerPrefs.GetInt("PlayerRotation") == 1){
                pets[PlayerPrefs.GetInt("Pet") - 1].GetComponent<SpriteRenderer>().flipX = false;
            } else{
                pets[PlayerPrefs.GetInt("Pet") - 1].GetComponent<SpriteRenderer>().flipX = true;
            }
        }
    }
}
