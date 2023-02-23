using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pet : MonoBehaviour
{

    [SerializeField] private SpriteRenderer[] pets;
    public int petIndex;
    private IEnumerator petCheck(){
        yield return new WaitForSeconds(10f);
        if(PlayerPrefs.GetInt("Pet") != 0){
            pets[PlayerPrefs.GetInt("Pet") - 1].gameObject.SetActive(true);
        }
    }
    private void Start() {
        if(PlayerPrefs.GetInt("Pet") != 0){
            pets[PlayerPrefs.GetInt("Pet") - 1].gameObject.SetActive(true);
        }
        StartCoroutine("petCheck");
    }
    private void FixedUpdate() {
        petIndex = PlayerPrefs.GetInt("Pet");
        if(PlayerPrefs.GetInt("Pet") == 2){
            if(PlayerPrefs.GetInt("PlayerRotation") == 1){
                pets[PlayerPrefs.GetInt("Pet") - 1].flipX = false;
            } else{
                pets[PlayerPrefs.GetInt("Pet") - 1].flipX = true;
            }
        }
    }
}
