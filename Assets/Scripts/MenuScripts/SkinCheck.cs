using UnityEngine;
using UnityEngine.UI;

public class SkinCheck : MonoBehaviour
{
    [SerializeField]private Transform[] skins;
    private void FixedUpdate(){
        for(int i = 0; i<skins.Length; i++){
            if(PlayerPrefs.GetInt("Language") == 1){
                if(PlayerPrefs.GetInt("Player")== i){
                    skins[i].GetChild(0).GetComponent<Text>().text="Выбрано";
                }
                else{
                    skins[i].GetChild(0).GetComponent<Text>().text="Выбрать";
                }
            }
            else if(PlayerPrefs.GetInt("Language") == 0){
                if(PlayerPrefs.GetInt("Player")== i){
                    skins[i].GetChild(0).GetComponent<Text>().text="Used";
                }
                else{
                    skins[i].GetChild(0).GetComponent<Text>().text="Use";
                }
            }
        }
    }
}
