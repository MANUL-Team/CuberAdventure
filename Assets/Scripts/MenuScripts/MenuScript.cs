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
    [SerializeField]private SkinCheck skincheck;
    [SerializeField]private PetsManager petcheck;
    [SerializeField] private Animator menuAnimator;

    public void PlayPressed()
    {
        AllAnimationsFalse();
        menuAnimator.SetBool("PlayOpened", true);
    }
    private void AllAnimationsFalse(){
        menuAnimator.SetBool("PlayOpened", false);
        menuAnimator.SetBool("SkinsOpened", false);
        menuAnimator.SetBool("PetsOpened", false);
        menuAnimator.SetBool("PromoOpened", false);
        menuAnimator.SetBool("SettingsOpened", false);
        menuAnimator.SetBool("MainMenuOpened", false);
        menuAnimator.SetBool("ShopOpened", false);
    }

    public void NewGame()
    {
        ReloadDataPart1();
    }
    public void ReloadDataPart1(){
        PlayerPrefs.SetInt("Dead", 0);
        PlayerPrefs.SetInt("Time", 800);
        PlayerPrefs.SetInt("ToDay", 1);
        PlayerPrefs.SetInt("CanLoseAir", 1);
        PlayerPrefs.SetFloat("MaxAir", 100);
        PlayerPrefs.SetInt("HealMnojitel", 1);
        PlayerPrefs.SetInt("Level", 1);
        PlayerPrefs.SetInt("LastLevel", 0);
        ReloadDataPart2();
    }
    public void ReloadDataPart2(){
        PlayerPrefs.SetInt("MaxHp", 100);
        PlayerPrefs.SetInt("PlayerLevel", 1);
        PlayerPrefs.SetInt("Exp", 0);
        PlayerPrefs.SetInt("Coins", 0);
        PlayerPrefs.SetInt("Core", 0);
        PlayerPrefs.SetFloat("Hp", 100);
        PlayerPrefs.SetInt("Armor", 0);
        PlayerPrefs.SetInt("Tracks", 0);
        PlayerPrefs.SetInt("Laser", 0);
        PlayerPrefs.SetInt("Turbine", 0);
        PlayerPrefs.SetInt("UWS", 0);
        ReloadDataPart3();
    }
    public void ReloadDataPart3(){
        PlayerPrefs.SetInt("Skill", -1);
        PlayerPrefs.SetInt("SkillPoints", 0);
        PlayerPrefs.SetInt("NewSpawnTP", 0);
        PlayerPrefs.SetInt("NeedExp", 100);
        ReloadDataPart4();
    }
    public void ReloadDataPart4(){
        for(int i = 0; i < 50; i++){
            PlayerPrefs.SetInt("Item" + " " + i.ToString(), 0);
            PlayerPrefs.SetInt("CutScene" + i.ToString(), 0);
            PlayerPrefs.SetInt("LevelEnded" + i.ToString(), 0);
            PlayerPrefs.SetInt("OpenSkill" + i.ToString(), 0);
            PlayerPrefs.SetInt("MissionTutorial", 0);
            PlayerPrefs.SetInt("MissionTopOfTheFoodChain", 0);
            PlayerPrefs.SetInt("Tutorial " +i, 0);
            for(int a = 0; a < 10; a++){
                PlayerPrefs.SetInt("Money" + " " + i.ToString() + " " + a.ToString(), 0);
                PlayerPrefs.SetInt("Mission " + i + " Step " + a, 0);
            }
            PlayerPrefs.SetInt("Core" + i.ToString(), 0);
            PlayerPrefs.SetInt("Armor" + i.ToString(), 0);
            PlayerPrefs.SetInt("Laser" + i.ToString(), 0);
            PlayerPrefs.SetInt("Turbine" + i.ToString(), 0);
            PlayerPrefs.SetInt("Tracks" + i.ToString(), 0);
            PlayerPrefs.SetInt("UWS" + i.ToString(), 0);
        }
        StartNewGame();
    }
    public void StartNewGame(){
        Loading.SetActive(true);
        StartCoroutine("AsyncLoadNew");
    }
    public void ContinueGame(){
        if(PlayerPrefs.GetInt("Level") != 0){
            Loading.SetActive(true);
            StartCoroutine("AsyncLoadLoad");
        }
    }
    public void Online(){
        ServMenu.SetActive(true);
        Play.SetActive(false);
        MainMenu.SetActive(false);
        SkinsMenu.SetActive(false);
    }
    public void Skins(){
        AllAnimationsFalse();
        menuAnimator.SetBool("SkinsOpened", true);
        skincheck.CheckUsed();
        Invoke("CheckUsed", 0.1f);
    }
    private void CheckUsed(){
        skincheck.CheckUsed();
    }

    public void Pets(){
        AllAnimationsFalse();
        menuAnimator.SetBool("PetsOpened", true);
    }

    public void Promo(){
        AllAnimationsFalse();
        menuAnimator.SetBool("PromoOpened", true);
    }

    public void Menu(){
        AllAnimationsFalse();
        menuAnimator.SetBool("MainMenuOpened", true);
        StartCoroutine(present.CheckPresent());
    }

    public void ShopPressed(){
        AllAnimationsFalse();
        menuAnimator.SetBool("ShopOpened", true);
    }

    public void ExitPressed()
    {
        Application.Quit();
        Debug.Log("Exit pressed!");
    }

    public void SettingsPressed(){
        AllAnimationsFalse();
        menuAnimator.SetBool("SettingsOpened", true);
    }

    public void SetPlayer(int index){
        PlayerPrefs.SetInt("Player", index);
        skincheck.CheckUsed();
    }
    public void SetPet(int index){
        PlayerPrefs.SetInt("Pet", index);
        petcheck.CheckUsed();
    }

    IEnumerator AsyncLoadNew(){
        AsyncOperation operation = SceneManager.LoadSceneAsync(1);
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
        Application.targetFrameRate = 120;
    }
}
