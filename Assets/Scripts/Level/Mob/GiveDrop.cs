using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiveDrop : MonoBehaviour
{
    [SerializeField] private Drop[] drop;
    [SerializeField] private int[] counts;
    private MobController mc;

    private void Start() {
        mc = GetComponent<MobController>();
    }
    public void Drop(){
        for(int i = 0; i < drop.Length; i++){
            PlayerPrefs.SetInt("Item" + drop[i].id, PlayerPrefs.GetInt("Item" + drop[i].id) + counts[i]);
        }
    }


}
