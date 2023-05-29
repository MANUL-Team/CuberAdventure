using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HaveSkinController : MonoBehaviour
{
    [SerializeField] private GameObject Buy, Use;
    [SerializeField] private int index;
    void Start()
    {
        if(PlayerPrefs.GetInt("Skin" + index.ToString()) > 0){
            Buy.SetActive(false);
            Use.SetActive(true);
        }
        else{
            Buy.SetActive(true);
            Use.SetActive(false);
        }
    }
}
