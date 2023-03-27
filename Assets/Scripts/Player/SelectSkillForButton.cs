using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectSkillForButton : MonoBehaviour
{
    [SerializeField] private int skillId;
    public int id{
        get{
            return skillId;
        }
    }
    [TextArea] [SerializeField] private string descriptionEng, descriptionRu;
    public string _description{
        get{
            if(PlayerPrefs.GetInt("Language") == 0){
                return descriptionEng;
            }
            else{
                return descriptionRu;
            }
        }
    }
    [SerializeField] private SkillsController controller;
    [SerializeField] private int price;
    public int _price{
        get{
            return price;
        }
    }
    public void Select(){
        controller.SelectSkill(skillId);
    }
}
