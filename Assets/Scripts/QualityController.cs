using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QualityController : MonoBehaviour
{
    public int targetFrameRate;
    private void Start() {
        PlayerPrefs.SetInt("FPS", targetFrameRate);
        StartCoroutine(GraphicsUpdate());
    }
    private IEnumerator GraphicsUpdate(){
        while(true){
            targetFrameRate = PlayerPrefs.GetInt("FPS");
            QualitySettings.SetQualityLevel(PlayerPrefs.GetInt("Graphics"), true);
            Application.targetFrameRate = targetFrameRate;
            yield return new WaitForSeconds(0.2f);
        }
    }
}
