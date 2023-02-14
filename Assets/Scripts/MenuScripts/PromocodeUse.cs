using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PromocodeUse : MonoBehaviour
{

    [SerializeField] private Text promoInput;
    [SerializeField] private GameObject success, fail;

    public void UsePromo(){
        if(promoInput.text == "MySun"){
            if(PlayerPrefs.GetInt("Pet0") == 0){
                PlayerPrefs.SetInt("Pet0", 1);
                success.SetActive(true);
                fail.SetActive(false);
            }else{
                success.SetActive(false);
                fail.SetActive(true);
            }
        }
        else if(promoInput.text == "Diamonds"){
            PlayerPrefs.SetInt("Diamonds", PlayerPrefs.GetInt("Diamonds") + 100);
            success.SetActive(true);
            fail.SetActive(false);
        }
        else if(promoInput.text == "Naruto"){
            if(PlayerPrefs.GetInt("Pet1") == 0){
                PlayerPrefs.SetInt("Pet1", 1);
                success.SetActive(true);
                fail.SetActive(false);
            }else{
                success.SetActive(false);
                fail.SetActive(true);
            }
        }
        else if(promoInput.text == "ILoveYou"){
            if(PlayerPrefs.GetInt("Pet2") == 0){
                PlayerPrefs.SetInt("Pet2", 1);
                success.SetActive(true);
                fail.SetActive(false);
            }else{
                success.SetActive(false);
                fail.SetActive(true);
            }
        }
        else if(promoInput.text == "ULockAConsole"){
            if(PlayerPrefs.GetInt("ConsoleUnlocked") == 1){
                PlayerPrefs.SetInt("ConsoleUnlocked", 0);
            }
            else{
                PlayerPrefs.SetInt("ConsoleUnlocked", 1);
            }
            success.SetActive(true);
            fail.SetActive(false);
        }
        else{
            success.SetActive(false);
            fail.SetActive(true);
        }
        
    }

}
