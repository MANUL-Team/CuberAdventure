using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillsController : MonoBehaviour
{
    [SerializeField] private SelectSkillForButton[] skills;
    [SerializeField] private Text description, useText, needText;
    [SerializeField] private GameObject buy, use;
    private int currentId;
    private int index;
    public void SelectSkill(int id){
        currentId = id;
        for(int i = 0; i < skills.Length; i++){
            if(id == skills[i].id){
                index = i;
                description.text = skills[i]._description;
                if(PlayerPrefs.GetInt("OpenSkill" + id.ToString()) == 1 || id == -1){
                    needText.text = "";
                    use.SetActive(true);
                    buy.SetActive(false);
                    if(PlayerPrefs.GetInt("Skill") == id){
                        use.GetComponent<Image>().color = new Color(120f/255f, 120f/255f, 120f/255f);
                        if(PlayerPrefs.GetInt("Language") == 0){
                            useText.text = "Used";
                        }
                        else if(PlayerPrefs.GetInt("Language") == 1){
                            useText.text = "Выбрано";
                        }
                    }
                    else{
                        use.GetComponent<Image>().color = new Color(176f/255f, 176f/255f, 176f/255f);
                        if(PlayerPrefs.GetInt("Language") == 0){
                            useText.text = "Use";
                        }
                        else if(PlayerPrefs.GetInt("Language") == 1){
                            useText.text = "Выбрать";
                        }
                    }
                }
                else{
                    buy.SetActive(true);
                    use.SetActive(false);
                    if(PlayerPrefs.GetInt("Language") == 0){
                        needText.text = "You need " + skills[i]._price.ToString() + " skill points!";
                    }
                    else{
                        needText.text = "Вам нужно " + skills[i]._price.ToString() + " очков навыков!";
                    }
                }
                break;
            }
        }
    }
    private void CheckSkills(){
        for(int i = 0; i < skills.Length; i++){
            if(PlayerPrefs.GetInt("OpenSkill" + skills[i].id.ToString()) == 0 && skills[i].id != -1){
                skills[i].gameObject.GetComponent<Image>().color = new Color(190f/255f, 190f/255f, 190f/255f);
            }
            else{
                skills[i].gameObject.GetComponent<Image>().color = new Color(1, 1, 1);
            }
        }
    }
    private void Start() {
        CheckSkills();
    }
    public void UseSkill(){
        PlayerPrefs.SetInt("Skill", currentId);
        SelectSkill(currentId);
    }
    public void BuySkill(){
        if(PlayerPrefs.GetInt("SkillPoints") >= skills[index]._price){
            PlayerPrefs.SetInt("SkillPoints", PlayerPrefs.GetInt("SkillPoints") - skills[index]._price);
            PlayerPrefs.SetInt("OpenSkill" + currentId.ToString(), 1);
            CheckSkills();
        }
    }
}
