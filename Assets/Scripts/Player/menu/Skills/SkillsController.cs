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
                        use.GetComponent<Image>().color = new Color(0.33f, 0.48f, 0.46f, 1f);
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
                        use.GetComponent<Image>().color = new Color(0.22f, 0.72f, 0.70f, 1f);
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
        int equipped = PlayerPrefs.GetInt("Skill");
        for(int i = 0; i < skills.Length; i++){
            Image icon = skills[i].GetComponent<Image>();
            bool locked = PlayerPrefs.GetInt("OpenSkill" + skills[i].id.ToString()) == 0 && skills[i].id != -1;
            bool selected = skills[i].id == equipped;
            if(icon != null){
                if(locked)
                    icon.color = new Color(0.62f, 0.62f, 0.66f, 1f);
                else if(selected)
                    icon.color = new Color(0.82f, 1f, 0.96f, 1f);
                else
                    icon.color = Color.white;
            }
            Outline outline = skills[i].GetComponent<Outline>();
            if(outline == null)
                outline = skills[i].gameObject.AddComponent<Outline>();
            outline.effectDistance = new Vector2(3f, -3f);
            if(selected)
                outline.effectColor = new Color(0.20f, 0.85f, 0.82f, 1f);
            else if(locked)
                outline.effectColor = new Color(0.15f, 0.10f, 0.07f, 0.8f);
            else
                outline.effectColor = new Color(0.55f, 0.28f, 0.08f, 0.9f);
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
