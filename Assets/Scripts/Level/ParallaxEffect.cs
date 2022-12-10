using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxEffect : MonoBehaviour
{

    [SerializeField] Transform target;
    [SerializeField, Range(0f, 1f)] float parallax;

    [SerializeField] bool disableVerticalParallax;

    Vector3 targetPrevPosition;

    private void Start() {
        if(!target){
            target = Camera.main.transform;
        }
        targetPrevPosition = target.position;
    }
    private void Update() {
        var delta = target.position - targetPrevPosition;

        if(disableVerticalParallax){
            delta.y = 0;
        }
        targetPrevPosition = target.position;
        transform.position += delta * parallax;
    }


}
