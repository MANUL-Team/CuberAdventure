using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionsCheckForClasses : MonoBehaviour
{
    [SerializeField] private GameObject[] classes;
    [SerializeField] private int[] id, steps;
    private void Start() {
        StartCoroutine(ClassesCheck());
    }
    private IEnumerator ClassesCheck(){
        while(true){
            for(int i = 0; i < classes.Length; i++){
                if(PlayerPrefs.GetInt("Mission " + id[i] + " Step " + steps[i]) == 1){
                    classes[i].SetActive(true);
                }
                else{
                    classes[i].SetActive(false);
                }
            }
            yield return new WaitForSeconds(3f);
        }
    }

}
