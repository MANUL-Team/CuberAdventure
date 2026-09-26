using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    public float hp, maxHp, dmg, exp, hpBonus, dmgBonus, prot;
    public int lvl, armor;
    [SerializeField] private Image healthBar;
    [SerializeField] private Text hpText;
    public int needExp;
    private DeathScript death;
    [SerializeField] private GameObject soundDmgObj;
    [SerializeField] private GameObject levelText;
    [SerializeField] private ModulesController modules;
    private UnderWater uw;
    float shownHp = -1f;
    static readonly Color HealthFull = new Color(0.84f, 0.31f, 0.22f, 1f);
    static readonly Color HealthLow = new Color(0.62f, 0.18f, 0.16f, 1f);
    const float HealthWidth = 200f;
    const float HealthPad = 5f;

    private IEnumerator health(){
        yield return new WaitForSeconds(1f);
        hp = Mathf.Clamp(hp + 3 * PlayerPrefs.GetInt("HealMnojitel"), 0, maxHp);
        StartCoroutine("health");
    }
    public void Damage(float damage){
        if(!uw.loseAir){
            hp -= Mathf.Round(damage - damage * (prot/100));
        }
        else if(uw.loseAir){
            hp -= damage;
        }
        if(Mathf.Round(damage - damage * (prot/100)) >= 1){
            GameObject soundDmg = Instantiate(soundDmgObj, transform.position, Quaternion.identity);
            Destroy(soundDmg, 1f);
        }
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
        int current = Mathf.RoundToInt(Mathf.Max(0f, hp));
        int max = Mathf.RoundToInt(maxHp);
        hpText.text = current + " / " + max;
        float ratio = maxHp > 0f ? Mathf.Clamp01(hp / maxHp) : 0f;
        if (shownHp < 0f)
            shownHp = ratio;
        else
            shownHp = Mathf.Lerp(shownHp, ratio, 1f - Mathf.Exp(-10f * Time.deltaTime));
        RectTransform fill = healthBar.rectTransform;
        float inner = (HealthWidth - HealthPad * 2f) * shownHp;
        fill.offsetMin = new Vector2(HealthPad, HealthPad);
        fill.offsetMax = new Vector2(HealthPad + inner, -HealthPad);
        healthBar.color = Color.Lerp(HealthLow, HealthFull, Mathf.InverseLerp(0.15f, 0.45f, shownHp));
        healthBar.enabled = inner > 1f;
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
        dmg = Mathf.Round(modules._laser.currentDamage);
        armor = PlayerPrefs.GetInt("ChangedArmor");
        needExp = PlayerPrefs.GetInt("NeedExp");
        prot = Mathf.Round(modules._armor.currentProtection);
    }
    public void LevelUp(){
        if(PlayerPrefs.GetInt("Exp") >= needExp){
            PlayerPrefs.SetInt("PlayerLevel", PlayerPrefs.GetInt("PlayerLevel") + 1);
            PlayerPrefs.SetInt("Exp", PlayerPrefs.GetInt("Exp") - needExp);
            PlayerPrefs.SetInt("NeedExp", needExp + needExp/3);
            LoadStats();
            levelText.SetActive(true);
        }
    }
}