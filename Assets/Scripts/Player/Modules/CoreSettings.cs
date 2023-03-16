using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoreSettings : MonoBehaviour
{
    public int countEnegry{
        get{
            return _countEnergy;
        }
    }
    [SerializeField] private int _countEnergy;
    public int countLastEnegry;
    [SerializeField] private CoreSettings[] prefabs;
    public int id{
        get{
            return _id;
        }
    }
    [SerializeField] private int _id;
    public int haveE;
    public bool ready;
    public void CheckModule() {
        for(int i = 0; i < prefabs.Length; i++){
            if(PlayerPrefs.GetInt("Core") == prefabs[i].id){
                _countEnergy = prefabs[i].countEnegry;
                _id = prefabs[i].id;
                ready = true;
            }
        }
        countLastEnegry = countEnegry;
    }
    private void Start() {
        CheckModule();
    }
}
