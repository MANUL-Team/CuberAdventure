using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserSettings : MonoBehaviour
{
    public float needEnergy{
        get{
            return _needEnergy;
        }
    }
    [SerializeField] private float _needEnergy;
    public int damage{
        get{
            return _damage;
        }
    }
    [SerializeField] private int _damage;
    public float currentDamage{
        get{
            return _currentDamage;
        }
    }
    [SerializeField] private float _currentDamage;
    [SerializeField] private LaserSettings[] prefabs;
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
            if(PlayerPrefs.GetInt("ChangedLaser") == prefabs[i].id){
                _damage = prefabs[i].damage;
                _needEnergy = prefabs[i].needEnergy;
                _id = prefabs[i].id;
                _sprite = prefabs[i]._sprite;
                ready = true;
            }
        }
    }
    private IEnumerator CorrectSettings(){
        while(true){
            _currentDamage = _damage * (haveE/_needEnergy);
            yield return new WaitForSeconds(1f);
        }
    }
    private void Start() {
        CheckModule();
        StartCoroutine(CorrectSettings());
    }
}
