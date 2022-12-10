using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    public float hp, maxHp, dmg, exp, hpBonus, dmgBonus;
    public int lvl, armor, prot;
    [SerializeField] private Image healthBar;
    [SerializeField] private Text hpText;
    public int needExp;
    private DeathScript death;
    [SerializeField] private GameObject soundDmgObj;
    [SerializeField] private GameObject levelText;

    private IEnumerator health(){
        yield return new WaitForSeconds(3f);
        hp = Mathf.Clamp(hp + 5, 0, maxHp);
        StartCoroutine("health");
    }
    public void Damage(float damage){
        if(prot / 7 <= damage){
            hp -= (damage - prot / 7);
        }
        GameObject soundDmg = Instantiate(soundDmgObj, transform.position, Quaternion.identity);
        Destroy(soundDmg, 1f);
    }
    private void Start(){
        hp = PlayerPrefs.GetFloat("Hp");
        if(lvl == 1){
            PlayerPrefs.SetInt("NeedExp", 100);
        }
        LoadStats();
        death = GameObject.FindGameObjectWithTag("Killer").GetComponent<DeathScript>();
        StartCoroutine("health");
    }
    private void Update() {
        hpText.text = hp + "/" + maxHp;
        healthBar.fillAmount = hp/maxHp;
        if(hp <= 0){
            death.Death();
        }
        LoadStats();
    }
    private void FixedUpdate() {
        if(PlayerPrefs.GetInt("Exp") >= needExp){
            LevelUp();
        }
        PlayerPrefs.SetInt("DmgBonus", (lvl-1) * lvl);
        PlayerPrefs.SetInt("HpBonus", (lvl-1) * lvl);
    }
    public void LoadStats(){
        lvl = PlayerPrefs.GetInt("PlayerLevel");
        PlayerPrefs.SetInt("DmgBonus", (lvl-1) * lvl);
        PlayerPrefs.SetInt("HpBonus", (lvl-1) * lvl);
        maxHp = PlayerPrefs.GetInt("MaxHp") + PlayerPrefs.GetInt("HpBonus");
        exp = PlayerPrefs.GetInt("Exp");
        hpBonus = PlayerPrefs.GetInt("HpBonus");
        dmgBonus = PlayerPrefs.GetInt("DmgBonus") / 3;
        dmg = PlayerPrefs.GetInt("PlayerDmg") + PlayerPrefs.GetInt("LaserDmg");
        armor = PlayerPrefs.GetInt("ChangedArmor");
        if(armor == 0){
            prot = 0;
        } else if(armor == 1){
            prot = 7;
        } else if(armor == 2){
            prot = 13;
        } else if(armor == 3){
            prot = 18;
        }
        needExp = PlayerPrefs.GetInt("NeedExp");
    }
    private void SaveStats(){

    }
    public void UpdateLevel(){
        LoadStats();
        needExp = PlayerPrefs.GetInt("NeedExp");
    }
    public void LevelUp(){
        if(PlayerPrefs.GetInt("Exp") >= needExp){
            PlayerPrefs.SetInt("PlayerLevel", PlayerPrefs.GetInt("PlayerLevel") + 1);
            PlayerPrefs.SetInt("Exp", PlayerPrefs.GetInt("Exp") - needExp);
            PlayerPrefs.SetInt("NeedExp", needExp + needExp/3);
            LoadStats();
            levelText.SetActive(true);
            Debug.Log("LevelUp");
        }
    }
}