using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmorSettings : MonoBehaviour
{
    public int protection{
        get{
            return _protection;
        }
    }
    [SerializeField] private int _protection;
    public float currentProtection{
        get{
            return _currentProtection;
        }
    }
    [SerializeField] private float _currentProtection;
    public bool isEnergy{
        get{
            return _isEnergy;
        }
    }
    [SerializeField] private bool _isEnergy;
    public float needEnergy{
        get{
            return _needEnergy;
        }
    }
    [SerializeField] private float _needEnergy;
    public int id{
        get{
            return _id;
        }
    }
    [SerializeField] private int _id;
    public float haveE;
    [SerializeField] private ArmorSettings[] prefabs;
    public bool ready;
    public void CheckModule() {
        for(int i = 0; i < prefabs.Length; i++){
            if(PlayerPrefs.GetInt("ChangedArmor") == prefabs[i].id){
                _protection = prefabs[i].protection;
                _isEnergy = prefabs[i].isEnergy;
                _needEnergy = prefabs[i].needEnergy;
                _id = prefabs[i].id; 
                ready = true;
            }
        }
    }
    private IEnumerator CorrectSettings(){
        while(true){
            if(_isEnergy){
                    _currentProtection = _protection * (haveE/_needEnergy);
                }
                else{
                    _currentProtection = _protection;
                }
            yield return new WaitForSeconds(1f);
        }
    }
    private void Start() {
        CheckModule();
        StartCoroutine(CorrectSettings());
    }
}
