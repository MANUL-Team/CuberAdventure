using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkinsPages : MonoBehaviour
{

    [SerializeField] private GameObject page1, page2, page3, page4, page5, page6;



    public void FirstPage(){
        page1.SetActive(true);
        page2.SetActive(false);
        page3.SetActive(false);
        page4.SetActive(false);
        page5.SetActive(false);
        page6.SetActive(false);
    }

    public void SecondPage(){
        page1.SetActive(false);
        page2.SetActive(true);
        page3.SetActive(false);
        page4.SetActive(false);
        page5.SetActive(false);
        page6.SetActive(false);
    }
    public void ThirdPage(){
        page1.SetActive(false);
        page2.SetActive(false);
        page3.SetActive(true);
        page4.SetActive(false);
        page5.SetActive(false);
        page6.SetActive(false);
    }
    public void FourthPage(){
        page1.SetActive(false);
        page2.SetActive(false);
        page3.SetActive(false);
        page4.SetActive(true);
        page5.SetActive(false);
        page6.SetActive(false);
    }
    public void FifthPage(){
        page1.SetActive(false);
        page2.SetActive(false);
        page3.SetActive(false);
        page4.SetActive(false);
        page5.SetActive(true);
        page6.SetActive(false);
    }
    public void SixthPage(){
        page1.SetActive(false);
        page2.SetActive(false);
        page3.SetActive(false);
        page4.SetActive(false);
        page5.SetActive(false);
        page6.SetActive(true);
    }
}
