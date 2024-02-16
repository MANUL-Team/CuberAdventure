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
        int lang = PlayerPrefs.GetInt("Language");
        for(int i = 0; i < skills.Length; i++){
            if(id == skills[i].id){
                index = i;
                description.text = skills[i]._description;
                if(PlayerPrefs.GetInt("OpenSkill" + id) == 1 || id == -1){
                    needText.text = "";
                    use.SetActive(true);
                    buy.SetActive(false);
                    if(PlayerPrefs.GetInt("Skill") == id){
                        use.GetComponent<Image>().color = new Color(120f/255f, 120f/255f, 120f/255f);
                        switch (lang)
                        {
                            case 0:
                                useText.text = "Выбрано";
                                break;
                            default:
                                useText.text = "Used";
                                break;
                        }
                    }
                    else{
                        use.GetComponent<Image>().color = new Color(176f/255f, 176f/255f, 176f/255f);
                        switch (lang)
                        {
                            case 0:
                                useText.text = "Выбрать";
                                break;
                            default:
                                useText.text = "Use";
                                break;
                        }
                    }
                }
                else{
                    buy.SetActive(true);
                    use.SetActive(false);
                    switch (lang)
                    {
                        case 0:
                            needText.text = "Вам нужно " + skills[i]._price.ToString() + " очков навыков!";
                            break;
                        default:
                            needText.text = "You need " + skills[i]._price.ToString() + " skill points!";
                            break;
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
