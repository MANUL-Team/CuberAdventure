using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyMove : MonoBehaviour
{
    [SerializeField] private GameObject sky;
    [SerializeField] private float speed = 0.1f;

    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        sky.GetComponent<Transform>().Translate(new Vector2(-speed, 0) * Time.deltaTime);
    }
}
