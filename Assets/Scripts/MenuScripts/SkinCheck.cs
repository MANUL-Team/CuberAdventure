using UnityEngine;
using UnityEngine.UI;

public class SkinCheck : MonoBehaviour
{
    [SerializeField]private Transform[] skins;
    [SerializeField] private Color use, used;
    public void CheckUsed(){
        for(int i = 0; i<skins.Length; i++){
            if(PlayerPrefs.GetString("Language") == "ru_RU"){
                if(PlayerPrefs.GetInt("Player")== i){
                    skins[i].gameObject.GetComponent<Image>().color = used;
                    skins[i].GetChild(0).GetComponent<Text>().text="Выбрано";
                }
                else{
                    skins[i].GetChild(0).GetComponent<Text>().text="Выбрать";
                    skins[i].gameObject.GetComponent<Image>().color = use;
                }
            }
            else if(PlayerPrefs.GetString("Language") == "en_US"){
                if(PlayerPrefs.GetInt("Player")== i){
                    skins[i].GetChild(0).GetComponent<Text>().text="Used";
                    skins[i].gameObject.GetComponent<Image>().color = used;
                }
                else{
                    skins[i].GetChild(0).GetComponent<Text>().text="Use";
                    skins[i].gameObject.GetComponent<Image>().color = use;
                }
            }
        }
    }
}
