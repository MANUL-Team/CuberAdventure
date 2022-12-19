using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuScript : MonoBehaviour
{


    [SerializeField]private GameObject Play, SkinsMenu, MainMenu, ServMenu, Shop, PetsMenu, PromoMenu, Loading, settingsMenu, shopMenu;
    [SerializeField]private Image loadingImg;
    [SerializeField]private PresentTimeReset present;

    public void PlayPressed()
    {
        Play.SetActive(true);
        MainMenu.SetActive(false);
        ServMenu.SetActive(false);
    }

    public void NewGame()
    {
        PlayerPrefs.SetInt("Level", 1);
        PlayerPrefs.SetInt("LastLevel", 0);
        PlayerPrefs.SetInt("MaxHp", 100);
        PlayerPrefs.SetInt("PlayerLevel", 1);
        PlayerPrefs.SetInt("Exp", 0);
        PlayerPrefs.SetInt("Coins", 0);
        PlayerPrefs.SetInt("ChangedWeapon", 0);
        PlayerPrefs.SetFloat("Hp", 100);
        PlayerPrefs.SetInt("ChangedArmor", 0);
        PlayerPrefs.SetInt("ChangedGusenici", 0);
        PlayerPrefs.SetInt("ChangedLaser", 0);
        PlayerPrefs.SetInt("ChangedTurbine", 0);
        PlayerPrefs.SetInt("Skill", 0);
        PlayerPrefs.SetInt("SkillPoints", 0);
        PlayerPrefs.SetInt("NewSpawnTP", 0);
        PlayerPrefs.SetInt("NeedExp", 100);
        for(int i = 0; i < 50; i++){
            PlayerPrefs.SetInt("Item" + "Biologic" + i.ToString(), 0);
        }
        for(int i = 0; i < 50; i++){
            PlayerPrefs.SetInt("Item" + "Mob" + i.ToString(), 0);
        }
        for(int i = 0; i < 50; i++){
            PlayerPrefs.SetInt("CutScene" + i.ToString(), 0);
        }
        for(int i = 1; i < 50; i++){
            PlayerPrefs.SetInt("LevelEnded" + i.ToString(), 0);
        }
        for(int i = 0; i < 50; i++){
            PlayerPrefs.SetInt("OpenSkill" + i.ToString(), 0);
        }
        for(int i = 0; i < 50; i++){
            PlayerPrefs.SetInt("Money" + i.ToString(), 0);
        }
        for(int i = 0; i < 50; i++){
            PlayerPrefs.SetInt("Weapon" + i.ToString(), 0);
        }
        for(int i = 0; i < 50; i++){
            PlayerPrefs.SetInt("Armor" + i.ToString(), 0);
        }
        for(int i = 0; i < 50; i++){
            PlayerPrefs.SetInt("Laser" + i.ToString(), 0);
        }
        for(int i = 0; i < 50; i++){
            PlayerPrefs.SetInt("Turbine" + i.ToString(), 0);
        }
        for(int i = 0; i < 50; i++){
            PlayerPrefs.SetInt("Gusenici" + i.ToString(), 0);
        }
        for(int i = 0; i < 50; i++){
            for(int a = 0; a < 50; a++){
                PlayerPrefs.SetString("CollectTimeItem" + "Biologic" + i + a, "");
            }
        }
        for(int i = 0; i < 50; i++){
            for(int a = 0; a < 50; a++){
                PlayerPrefs.SetString("MobTimeDie" + i + a, "");
            }
        }
        StartCoroutine("AsyncLoadNew");
    }
    public void ContinueGame(){
        StartCoroutine("AsyncLoadLoad");
    }
    public void Online(){
        ServMenu.SetActive(true);
         Play.SetActive(false);
         MainMenu.SetActive(false);
         SkinsMenu.SetActive(false);
    }

    public void Skins(){
        SkinsMenu.SetActive(true);
        MainMenu.SetActive(false);
        ServMenu.SetActive(false);
        PetsMenu.SetActive(false);
        PromoMenu.SetActive(false);
        shopMenu.SetActive(false);
    }

    public void Pets(){
        SkinsMenu.SetActive(false);
        MainMenu.SetActive(false);
        ServMenu.SetActive(false);
        PetsMenu.SetActive(true);
    }

    public void Promo(){
        SkinsMenu.SetActive(false);
        MainMenu.SetActive(false);
        ServMenu.SetActive(false);
        PetsMenu.SetActive(false);
        PromoMenu.SetActive(true);
    }

    public void Menu(){
        MainMenu.SetActive(true);
        SkinsMenu.SetActive(false);
        Play.SetActive(false);
        ServMenu.SetActive(false);
        PetsMenu.SetActive(false);
        settingsMenu.SetActive(false);
        StartCoroutine(present.CheckPresent());
    }

    public void ShopPressed(){
        SkinsMenu.SetActive(false);
        shopMenu.SetActive(true);
    }

    public void ExitPressed()
    {
        Application.Quit();
        Debug.Log("Exit pressed!");
    }

    public void SettingsPressed(){
        settingsMenu.SetActive(true);
        MainMenu.SetActive(false);
    }

    public void SetPlayer(int index){
        PlayerPrefs.SetInt("Player", index);
    }
    public void SetPet(int index){
        PlayerPrefs.SetInt("Pet", index);
    }

    IEnumerator AsyncLoadNew(){
        AsyncOperation operation = SceneManager.LoadSceneAsync("Game");
        while(!operation.isDone){
            loadingImg.fillAmount = operation.progress;
            yield return null;
        }
    }
    IEnumerator AsyncLoadLoad(){
        AsyncOperation operation = SceneManager.LoadSceneAsync(PlayerPrefs.GetInt("Level"));
        while(!operation.isDone){
            loadingImg.fillAmount = operation.progress;
            yield return null;
        }
    }


    void Start()
    {
        Application.targetFrameRate = 60;
    }


    void Update()
    {
    }
}
