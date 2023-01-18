using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChooseTypeOfItems : MonoBehaviour
{
    [SerializeField] private GameObject[] itemsScroll;

    public void Active(int id){
        for(int i = 0; i < itemsScroll.Length; i++){
            if(i != id){
                itemsScroll[i].SetActive(false);
            }
            else{
                itemsScroll[i].SetActive(true);
            }
        }
    }
}
