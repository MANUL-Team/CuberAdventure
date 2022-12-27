using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobCanvasPos : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float height;

    private void Update() {
        transform.position = new Vector2(target.position.x, target.position.y + height);
    }
}
