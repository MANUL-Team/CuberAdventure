using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSpecialAttack : MonoBehaviour
{
    [SerializeField] private float startCallDown;
    private float callDown;
    [SerializeField] private WebStop attack;
    private void Start() {
        StartCoroutine(AttackCD());
    }
    private IEnumerator AttackCD(){
        while(true){
            if(callDown<0){
                Attack();
            }
            callDown-=0.1f;
            yield return new WaitForSeconds(0.1f);
        }
    }
    private void Attack(){
        callDown = startCallDown;
        attack.Attack();
    }
}
