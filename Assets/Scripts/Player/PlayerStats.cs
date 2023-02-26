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
    private UnderWater uw;

    private IEnumerator health(){
        yield return new WaitForSeconds(1f);
        hp = Mathf.Clamp(hp + 3 * PlayerPrefs.GetInt("HealMnojitel"), 0, maxHp);
        StartCoroutine("health");
    }
    public void Damage(float damage){
        if(damage - prot >= 0 && !uw.loseAir){
            hp -= damage - prot;
        }
        else if(uw.loseAir){
            hp -= damage;
        }
        GameObject soundDmg = Instantiate(soundDmgObj, transform.position, Quaternion.identity);
        Destroy(soundDmg, 1f);
    }
    private void Start(){
        uw = GetComponent<UnderWater>();
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
        if(lvl != 1){
            PlayerPrefs.SetInt("DmgBonus", lvl * 4 + (lvl-1) * 2);
            PlayerPrefs.SetInt("HpBonus", lvl * 4 + (lvl-1) * 2);
        }
        else{
            PlayerPrefs.SetInt("DmgBonus", 0);
            PlayerPrefs.SetInt("HpBonus", 0);
        }
    }
    public void LoadStats(){
        lvl = PlayerPrefs.GetInt("PlayerLevel");
        if(lvl != 1){
            PlayerPrefs.SetInt("DmgBonus", lvl * 4 + (lvl-1) * 2);
            PlayerPrefs.SetInt("HpBonus", lvl * 4 + (lvl-1) * 2);
        }
        else{
            PlayerPrefs.SetInt("DmgBonus", 0);
            PlayerPrefs.SetInt("HpBonus", 0);
        }
        maxHp = PlayerPrefs.GetInt("MaxHp") + PlayerPrefs.GetInt("HpBonus");
        exp = PlayerPrefs.GetInt("Exp");
        hpBonus = PlayerPrefs.GetInt("HpBonus");
        dmgBonus = PlayerPrefs.GetInt("DmgBonus");
        dmg = PlayerPrefs.GetInt("PlayerDmg") + PlayerPrefs.GetInt("LaserDmg");
        armor = PlayerPrefs.GetInt("ChangedArmor");
        if(armor == 0){
            prot = 0;
        } else if(armor == 1){
            prot = 10;
        } else if(armor == 2){
            prot = 25;
        } else if(armor == 3){
            prot = 15;
        }
        needExp = PlayerPrefs.GetInt("NeedExp");
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