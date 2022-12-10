using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScript : MonoBehaviour
{


    [SerializeField] private GameObject loading;
    [SerializeField] private GameObject load;
    [SerializeField] private GameObject load1;
    [SerializeField] private GameObject load2;
    [SerializeField] private GameObject load3;
    [SerializeField] private GameObject load4;

    public void DelLoad(){
        loading.SetActive(false);
    }
    public void loadingF(){
        load.SetActive(true);
    }
    public void loading1(){
        load1.SetActive(true);
    }
    public void loading2(){
        load2.SetActive(true);
    }
    public void loading3(){
        load3.SetActive(true);
    }
    public void loading4(){
        load4.SetActive(true);
    }
    public void loadingDelete(){
        load1.SetActive(false);
        load2.SetActive(false);
        load3.SetActive(false);
        load4.SetActive(false);
    }
    void Start()
    {
        Invoke("loadingF", 0f);
        Invoke("loading1", 0.2f);
        Invoke("loading2", 0.4f);
        Invoke("loading3", 0.6f);
        Invoke("loading4", 0.8f);


        Invoke("DelLoad", 1f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
