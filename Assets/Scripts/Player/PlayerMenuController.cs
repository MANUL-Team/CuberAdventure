using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMenuController : MonoBehaviour
{
    [SerializeField] private Text lvl, dmg, hp, prot;
    [SerializeField] private PlayerStats ps;
    [SerializeField] private GameObject inventory, weaponMenu, menu, lBut, dBut, cBut, sBut, skillsMenu, bg, xpScale, statsBG;
    private Animator animator;
    private bool statsUpdate;
    private void Start() {
        animator = GetComponent<Animator>();
        MenuAppearance.Apply(inventory, skillsMenu);
    }
    public void AnimInventory(){
        animator.SetBool("Inventory", true);
    }
    public void AnimInventoryBack(){
        animator.SetBool("Inventory", false);
    }
    public void OpenCloseInventory(){
        inventory.SetActive(!inventory.activeSelf);
        menu.gameObject.SetActive(!inventory.activeSelf);
    }
    public void OpenWeaponMenu(){
        animator.SetBool("Modules", true);
    }
    public void CloseWeaponMenu(){
        animator.SetBool("Modules", false);
    }
    public void OpenSkillsMenu(){
        animator.SetBool("Skills", true);
    }
    public void CloseSkillsMenu(){
        animator.SetBool("Skills", false);
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
