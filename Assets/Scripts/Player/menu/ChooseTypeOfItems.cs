using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChooseTypeOfItems : MonoBehaviour
{
    [SerializeField] private GameObject[] itemsScroll;
    [SerializeField] private int main;

    public void Active(){
        for(int i = 0; i < itemsScroll.Length; i++){
            if(i != main-1){
                itemsScroll[i].SetActive(false);
            }
            else{
                itemsScroll[i].SetActive(true);
            }
        }
    }
}
