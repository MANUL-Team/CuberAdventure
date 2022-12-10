using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapBack : MonoBehaviour
{
    private RectTransform rt;
    
    private void Start() {
        rt = GetComponent<RectTransform>();
    }
    private void Update() {
        if(rt.position.x > 1000){
            rt.position = new Vector2(1000, rt.position.y);
        }
        if(rt.position.x < -2000){
            rt.position = new Vector2(-2000, rt.position.y);
        }
        if(rt.position.y > 1500){
            rt.position = new Vector2(rt.position.x, 1500);
        }
        if(rt.position.y < 700){
            rt.position = new Vector2(rt.position.x, 700);
        }
    }
    public void BackMap(){
        rt.position = new Vector2(0, 1000);
    }
}
