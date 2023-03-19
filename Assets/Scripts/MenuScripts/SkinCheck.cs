using UnityEngine;
using UnityEngine.UI;

public class SkinCheck : MonoBehaviour
{
    [SerializeField]private Transform[] skins;
    [SerializeField] private Sprite use, used;
    public void CheckUsed(){
        for(int i = 0; i<skins.Length; i++){
            if(PlayerPrefs.GetInt("Language") == 1){
                if(PlayerPrefs.GetInt("Player")== i){
                    skins[i].gameObject.GetComponent<Image>().sprite = used;
                    skins[i].GetChild(0).GetComponent<Text>().text="Выбрано";
                }
                else{
                    skins[i].GetChild(0).GetComponent<Text>().text="Выбрать";
                    skins[i].gameObject.GetComponent<Image>().sprite = use;
                }
            }
            else if(PlayerPrefs.GetInt("Language") == 0){
                if(PlayerPrefs.GetInt("Player")== i){
                    skins[i].GetChild(0).GetComponent<Text>().text="Used";
                    skins[i].gameObject.GetComponent<Image>().sprite = used;
                }
                else{
                    skins[i].GetChild(0).GetComponent<Text>().text="Use";
                    skins[i].gameObject.GetComponent<Image>().sprite = use;
                }
            }
        }
    }
}
