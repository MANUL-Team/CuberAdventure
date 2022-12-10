using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MovePlayerGD : MonoBehaviour
{

    public float speed = 5f;

    public GameObject obj;
    //public GameObject camera;

    public float y, z;

    public float jumpForce = 400f;

    public bool isGrounded;

    void OnCollisionEnter2D(){
        isGrounded = true;
    }
    void OnTriggerEnter2D(){
        SceneManager.LoadScene("Menu");
    }

    private void Update() {
        obj.GetComponent<Transform>().Translate(new Vector2(1, y) * speed * Time.deltaTime);
        //camera.GetComponent<Transform>().Translate(new Vector2(1, 0) * speed * Time.deltaTime);

        if(Input.GetKeyDown(KeyCode.Space) && isGrounded){
            isGrounded = false;
            obj.GetComponent<Rigidbody2D>().AddForce(new Vector2(0, jumpForce));
        }
        if(Input.GetKeyDown(KeyCode.Mouse0) && isGrounded){
            isGrounded = false;
            obj.GetComponent<Rigidbody2D>().AddForce(new Vector2(0, jumpForce));
        }
    }

}
