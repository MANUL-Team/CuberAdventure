using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeatherController : MonoBehaviour
{
    [SerializeField] private int randInt;
    [SerializeField] private GameObject rain;
    private Animator rainAnim;
    private void Start() {
        if(rain != null){
            StartCoroutine("StartRain");
            rainAnim = rain.GetComponent<Animator>();
        }
        StartCoroutine(LowUpdate());
    }
    private IEnumerator LowUpdate(){
        while(true){
            if(rain != null){
                if(PlayerPrefs.GetInt("Rain") == 1){
                    rain.SetActive(true);
                } else if(PlayerPrefs.GetInt("Rain") == 0 && rain.activeSelf == true){
                    rainAnim.SetTrigger("RainExit");
                }
            }
            randInt = PlayerPrefs.GetInt("Rain");
            yield return new WaitForSeconds(0.2f);
        }
    }
    IEnumerator StartRain(){
        yield return new WaitForSeconds(60f);
        PlayerPrefs.SetInt("Rain", Random.Range(0, 2));
        StartCoroutine("StartRain");
    }
    public void RainExit(){
        gameObject.SetActive(false);
    }

}
