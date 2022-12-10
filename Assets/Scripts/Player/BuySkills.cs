using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuySkills : MonoBehaviour
{
    private int needPoints, id;
    [SerializeField] public GameObject[] closed, opened;
    [SerializeField] public Text text;
    [SerializeField] private SelectSkill ss;
    public void Buy(){
        if(PlayerPrefs.GetInt("SkillPoints") >= needPoints){
            PlayerPrefs.SetInt("SkillPoints", PlayerPrefs.GetInt("SkillPoints") - needPoints);
            PlayerPrefs.SetInt("OpenSkill" + id.ToString(), 1);
            closed[id].SetActive(false);
            opened[id].SetActive(true);
            text.gameObject.SetActive(false);
            gameObject.SetActive(false);
        }
    }
    public void SelectSkill(int needPointsBSFS, int idBSFS){
        id = idBSFS;
        needPoints = needPointsBSFS;
        text.gameObject.SetActive(true);
        ss.selectedSkill = -1;
        for(int i = 0; i < ss.texts.Length; i++){
            ss.texts[i].SetActive(false);
        }
        ss.gameObject.SetActive(false);
        if(PlayerPrefs.GetInt("Language") == 0){
            text.text = "You need: " + needPoints.ToString() + " skill points.";
        }
        else if(PlayerPrefs.GetInt("Language") == 1){
            text.text = "Вам нужно: " + needPoints.ToString() + " очков навыков.";
        }
    }
}
