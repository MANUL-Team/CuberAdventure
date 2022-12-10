using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RandomSkin : MonoBehaviour
{

    [SerializeField] private int _randomInt;

    [SerializeField] private GameObject BuyButton;

    [SerializeField] private GameObject RandomSkinIcon;

    [SerializeField] private GameObject[] Skins;

    [SerializeField] private Animator animator;

    [SerializeField] private GameObject DropParticles;

    [SerializeField] private GameObject SkinsButton;

    public void RandomBuy(){
        if(PlayerPrefs.GetInt("Diamonds") >= 30){
            PlayerPrefs.SetInt("Diamonds", PlayerPrefs.GetInt("Diamonds") - 30);
            TakeSkin();
        }
    }
    public void TakeSkin(){
            _randomInt = Random.Range(0, Skins.Length);
            if(PlayerPrefs.GetInt("Skin" + (_randomInt + 1).ToString()) == 1){
                TakeSkin();
            }else{
                BuyButton.SetActive(false);
                animator.SetBool("SkinBuyed", true);
                PlayerPrefs.SetInt("Skin" + (_randomInt + 1).ToString(), 1);
        
                Invoke("RandomBuyEnd", 2.7f);
            }
    }

    public void RandomBuyEnd(){
        Instantiate(DropParticles);
        RandomSkinIcon.SetActive(false);
        Skins[_randomInt].SetActive(true);
        Invoke("SkinsButtonAlive", 2f);
    }

    public void SkinsButtonAlive(){
        SkinsButton.SetActive(true);
    }

    public void MainMenuVoid(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    private void Update() {
        if(PlayerPrefs.GetInt("Skin1") == 1 && PlayerPrefs.GetInt("Skin2") == 1 && PlayerPrefs.GetInt("Skin3") == 1 && PlayerPrefs.GetInt("Skin4") == 1 && PlayerPrefs.GetInt("Skin5") == 1 && PlayerPrefs.GetInt("Skin6") == 1 && PlayerPrefs.GetInt("Skin7") == 1 && PlayerPrefs.GetInt("Skin8") == 1 && PlayerPrefs.GetInt("Skin9") == 1 && PlayerPrefs.GetInt("Skin10") == 1 && PlayerPrefs.GetInt("Skin11") == 1 && PlayerPrefs.GetInt("Skin12") == 1 && PlayerPrefs.GetInt("Skin13") == 1 && PlayerPrefs.GetInt("Skin14") == 1 && PlayerPrefs.GetInt("Skin15") == 1 ){
            BuyButton.SetActive(false);
        }
    }
    


}
