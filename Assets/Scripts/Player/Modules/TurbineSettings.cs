using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurbineSettings : MonoBehaviour
{
    public float needEnergy{
        get{
            return _needEnergy;
        }
    }
    [SerializeField] private float _needEnergy;
    public int jumpHeight{
        get{
            return _jumpHeight;
        }
    }
    [SerializeField] private int _jumpHeight;
    public float currentJumpHeight{
        get{
            return _currentJumpHeight;
        }
    }
    [SerializeField] private float _currentJumpHeight;
    public int jumpCount{
        get{
            return _jumpCount;
        }
    }
    [SerializeField] private int _jumpCount;
    [SerializeField] private TurbineSettings[] prefabs;
    public int id{
        get{
            return _id;
        }
    }
    [SerializeField] private int _id;
    public Sprite sprite{
        get{
            return _sprite;
        }
    }
    [SerializeField] private Sprite _sprite;
    public float haveE;
    public bool ready;
    public void CheckModule() {
        for(int i = 0; i < prefabs.Length; i++){
            if(PlayerPrefs.GetInt("ChangedTurbine") == prefabs[i].id){
                _jumpHeight = prefabs[i].jumpHeight;
                _needEnergy = prefabs[i].needEnergy;
                _id = prefabs[i].id;
                _jumpCount = prefabs[i].jumpCount;
                _sprite = prefabs[i]._sprite;
                ready = true;
            }
        }
    }
    private IEnumerator CorrectSettings(){
        while(true){
            _currentJumpHeight = _jumpHeight * (haveE/_needEnergy);
            yield return new WaitForSeconds(1f);
        }
    }
    private void Start() {
        CheckModule();
        StartCoroutine(CorrectSettings());
    }
}
