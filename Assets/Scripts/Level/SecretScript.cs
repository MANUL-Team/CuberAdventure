using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecretScript : MonoBehaviour
{

    [SerializeField] private GameObject Visible, Invisible;


    void OnTriggerEnter2D(Collider2D collision){
        if(collision.CompareTag("Player")){
            Visible.SetActive(false);
            Invisible.SetActive(true);
        }
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
}
