using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyMoveMenu : MonoBehaviour
{
    [SerializeField] private float speed;

    private void Update() {
        GetComponent<Transform>().Translate(new Vector2(1, 0) * speed * Time.deltaTime);
    }
}
