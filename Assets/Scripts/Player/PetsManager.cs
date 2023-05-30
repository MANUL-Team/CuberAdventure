using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PetsManager : MonoBehaviour
{
    [SerializeField]private Transform[] pets;
    [SerializeField] private Color use, used;
    private void Start() {
        CheckUsed();
    }
    public void CheckUsed(){
        for(int i = 0; i< pets.Length; i++){
            if(PlayerPrefs.GetString("Language") == "ru_RU"){
                if(PlayerPrefs.GetInt("Pet")== i){
                    pets[i].gameObject.GetComponent<Image>().color = used;
                    pets[i].GetChild(0).GetComponent<Text>().text="Выбрано";
                }
                else{
                    pets[i].GetChild(0).GetComponent<Text>().text="Выбрать";
                    pets[i].gameObject.GetComponent<Image>().color = use;
                }
            }
            else if(PlayerPrefs.GetString("Language") == "en_US"){
                if(PlayerPrefs.GetInt("Pet")== i){
                    pets[i].GetChild(0).GetComponent<Text>().text="Used";
                    pets[i].gameObject.GetComponent<Image>().color = used;
                }
                else{
                    pets[i].GetChild(0).GetComponent<Text>().text="Use";
                    pets[i].gameObject.GetComponent<Image>().color = use;
                }
            }
        }
    }
}
