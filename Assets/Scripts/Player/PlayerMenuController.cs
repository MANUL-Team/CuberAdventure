using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMenuController : MonoBehaviour
{
    [SerializeField] private Text lvl, dmg, hp, prot;
    [SerializeField] private PlayerStats ps;
    [SerializeField] private GameObject inventory, weaponMenu, menu, lBut, dBut, cBut, sBut, skillsMenu, bg, xpScale, statsBG;
    private bool statsUpdate;


    public void OpenCloseInventory(){
        inventory.SetActive(!inventory.activeSelf);
        lvl.gameObject.SetActive(!inventory.activeSelf);
        dmg.gameObject.SetActive(!inventory.activeSelf);
        hp.gameObject.SetActive(!inventory.activeSelf);
        prot.gameObject.SetActive(!inventory.activeSelf);
        lBut.gameObject.SetActive(!inventory.activeSelf);
        dBut.gameObject.SetActive(!inventory.activeSelf);
        cBut.gameObject.SetActive(!inventory.activeSelf);
        sBut.gameObject.SetActive(!inventory.activeSelf);
        xpScale.SetActive(!xpScale.activeSelf);
        statsBG.SetActive(!statsBG.activeSelf);
    }
    public void OpenWeaponMenu(){
        weaponMenu.SetActive(!weaponMenu.activeSelf);
        lvl.gameObject.SetActive(!weaponMenu.activeSelf);
        dmg.gameObject.SetActive(!weaponMenu.activeSelf);
        hp.gameObject.SetActive(!weaponMenu.activeSelf);
        prot.gameObject.SetActive(!weaponMenu.activeSelf);
        lBut.gameObject.SetActive(!weaponMenu.activeSelf);
        dBut.gameObject.SetActive(!weaponMenu.activeSelf);
        cBut.gameObject.SetActive(!weaponMenu.activeSelf);
        sBut.gameObject.SetActive(!weaponMenu.activeSelf);
        xpScale.SetActive(!xpScale.activeSelf);
        statsBG.SetActive(!statsBG.activeSelf);
    }
    public void OpenSkillsMenu(){
        skillsMenu.SetActive(!skillsMenu.activeSelf);
        lvl.gameObject.SetActive(!skillsMenu.activeSelf);
        dmg.gameObject.SetActive(!skillsMenu.activeSelf);
        hp.gameObject.SetActive(!skillsMenu.activeSelf);
        prot.gameObject.SetActive(!skillsMenu.activeSelf);
        lBut.gameObject.SetActive(!skillsMenu.activeSelf);
        dBut.gameObject.SetActive(!skillsMenu.activeSelf);
        cBut.gameObject.SetActive(!skillsMenu.activeSelf);
        sBut.gameObject.SetActive(!skillsMenu.activeSelf);
        bg.gameObject.SetActive(!skillsMenu.activeSelf);
        xpScale.SetActive(!xpScale.activeSelf);
        statsBG.SetActive(!statsBG.activeSelf);
    }
    private void FixedUpdate() {
        if(!statsUpdate){
            StartCoroutine(UpdateStats());
            statsUpdate = true;
        }
    }
    private void OnDisable() {
        statsUpdate = false;
    }
    private IEnumerator UpdateStats(){
        while(true){
            if(PlayerPrefs.GetInt("Language") == 0){
                lvl.text = "Level: " + ps.lvl.ToString();
                hp.text = "Hp: " + ps.maxHp.ToString();
                dmg.text = "Damage: " + (ps.dmg + ps.dmgBonus).ToString();
                prot.text = "Protection: " + ps.prot.ToString();
            }
            else if(PlayerPrefs.GetInt("Language") == 1){
                lvl.text = "Уровень: " + ps.lvl.ToString();
                hp.text = "Здоровье: " + ps.maxHp.ToString();
                dmg.text = "Урон: " + (ps.dmg + ps.dmgBonus).ToString();
                prot.text = "Защита: " + ps.prot.ToString();
            }
            yield return new WaitForSeconds(0.5f);
        }
    }

    public void Close(){
        gameObject.SetActive(false);
    }
}
