using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSet : MonoBehaviour
{
    private AudioSource audio;
    [SerializeField] bool sounds, rainEnd;
    [SerializeField] private float volume = 1;

    private void Start() {
        audio = GetComponent<AudioSource>();
    }
    private void Update() {
        if(!rainEnd){
            if(sounds)
                audio.volume = PlayerPrefs.GetFloat("Volume")*volume/5;
            else{
                audio.volume = PlayerPrefs.GetFloat("Volume")*volume;
            }
        }
        else if(rainEnd && audio.volume > 0){
            audio.volume -= 0.005f;
        }
    }
    public void RainEnd(){
        rainEnd = true;
    }
    public void AntiRainEnd(){
        rainEnd = false;
    }
}
