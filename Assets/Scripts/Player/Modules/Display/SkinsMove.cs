using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkinsMove : MonoBehaviour
{
    [SerializeField] private Transform tracks;
    [SerializeField] private float delta;
    private void Update() {
        if(tracks.position.y < transform.position.y - delta){
            transform.position = new Vector2(tracks.position.x, tracks.position.y + delta);
        }
        transform.position = new Vector2(tracks.position.x - 0.2f, transform.position.y);
    }
}
