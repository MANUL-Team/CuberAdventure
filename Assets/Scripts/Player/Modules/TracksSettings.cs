using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TracksSettings : MonoBehaviour
{
    public float needEnergy{
        get{
            return _needEnergy;
        }
    }
    [SerializeField] private float _needEnergy;
    public int speed{
        get{
            return _speed;
        }
    }
    [SerializeField] private int _speed;
    public float currentSpeed{
        get{
            return _currentSpeed;
        }
    }
    [SerializeField] private float _currentSpeed;
    [SerializeField] private TracksSettings[] prefabs;
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
            if(PlayerPrefs.GetInt("ChangedGusenici") == prefabs[i].id){
                _speed = prefabs[i].speed;
                _needEnergy = prefabs[i].needEnergy;
                _id = prefabs[i].id;
                _sprite = prefabs[i]._sprite;
                ready = true;
            }
        }
    }
    private IEnumerator CorrectSettings(){
        while(true){
            _currentSpeed = _speed * (haveE/_needEnergy);
            yield return new WaitForSeconds(1f);
        }
    }
    private void Start() {
        CheckModule();
        StartCoroutine(CorrectSettings());
    }
}
