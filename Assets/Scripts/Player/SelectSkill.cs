using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectSkill : MonoBehaviour
{
    [SerializeField] public int selectedSkill = -1;
    [SerializeField] public GameObject[] texts;
    [SerializeField] private SwitchSkills ss;
    [SerializeField] private GameObject use, used;
    [SerializeField] private BuySkills bs;

    public void SelectNewSkill(int skillId){
        selectedSkill = skillId;
        bs.text.gameObject.SetActive(false);
        bs.gameObject.SetActive(false);
    }
    public void UseSkill(){
        PlayerPrefs.SetInt("Skill", selectedSkill);
        ss.SkillSwitch();
    }
    private void FixedUpdate() {
        if(selectedSkill != -1){
            for(int i = 0; i < texts.Length; i++){
                if(i == selectedSkill){
                    texts[i].SetActive(true);
                }
                else{
                    texts[i].SetActive(false);
                }
            }
        }
        if(selectedSkill == PlayerPrefs.GetInt("Skill")){
            use.SetActive(false);
            used.SetActive(true);
        } else{
            use.SetActive(true);
            used.SetActive(false);
        }
    }
}
