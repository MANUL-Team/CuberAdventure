using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuySkillForSkill : MonoBehaviour
{
    [SerializeField] private int needPoints, id;
    [SerializeField] private GameObject bsObj;
    private BuySkills bs;

    public void SelectSkill(){
        bsObj.SetActive(true);
        bs.SelectSkill(needPoints, id);
    }
    private void Start() {
        bs = bsObj.GetComponent<BuySkills>();
        if(PlayerPrefs.GetInt("OpenSkill" + id.ToString()) == 1){
            bs.closed[id].SetActive(false);
            bs.opened[id].SetActive(true);
        }
    }
}
