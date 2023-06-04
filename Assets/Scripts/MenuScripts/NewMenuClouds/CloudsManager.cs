using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudsManager : MonoBehaviour
{
    [SerializeField] private Transform clouds, spawnPoint, endPoint, parent;
    [SerializeField] private List<Transform> cloudsMas = new List<Transform>();
    [SerializeField] private float speed;
    
    private void FixedUpdate(){
        for(int i = 0; i < cloudsMas.Count; i++){
            cloudsMas[i].Translate(new Vector2(speed * Time.deltaTime, 0));
        }
        if(cloudsMas[cloudsMas.Count-1].position.x < endPoint.position.x){
            Transform newClouds = Instantiate(clouds, spawnPoint.position, Quaternion.identity);
            newClouds.SetParent(parent);
            newClouds.localScale = new Vector3(1, 1, 1);
            cloudsMas.Add(newClouds);
        }
        if(cloudsMas.Count >= 4){
            Destroy(cloudsMas[0].gameObject);
            cloudsMas.RemoveAt(0);
        }
    }
}