using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMenuController : MonoBehaviour
{
    [SerializeField] private Text lvl, dmg, hp, prot;
    [SerializeField] private PlayerStats ps;
    [SerializeField] private GameObject inventory, weaponMenu, menu, lBut, dBut, cBut, sBut, skillsMenu, bg;


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
    }
    private void Update() {
        lvl.text = "Lvl: " + ps.lvl.ToString();
        hp.text = "Hp: " + ps.maxHp.ToString();
        dmg.text = "Dmg: " + (ps.dmg + ps.dmgBonus).ToString();
        prot.text = "Prot: " + ps.prot.ToString();
    }

    public void Close(){
        gameObject.SetActive(false);
    }
}
