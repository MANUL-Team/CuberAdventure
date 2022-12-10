using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkySpawn : MonoBehaviour
{

    [SerializeField] private Transform cam;
    [SerializeField] private GameObject[] chunkPrefabs;

    [SerializeField] private GameObject FirstChunk;
    [SerializeField] public List<GameObject> SpawnedChunks = new List<GameObject>();


    private void SpawnChunk(){
        GameObject newChunk = Instantiate(chunkPrefabs[Random.Range(0, chunkPrefabs.Length)]);
        newChunk.GetComponent<Transform>().position = new Vector3(SpawnedChunks[SpawnedChunks.Count - 1].GetComponent<Transform>().position.x + 252.7f, SpawnedChunks[SpawnedChunks.Count - 1].GetComponent<Transform>().position.y, 51f);
        SpawnedChunks.Add(newChunk);

        if (SpawnedChunks.Count >= 3) {
            Destroy(SpawnedChunks[0]);
            SpawnedChunks.RemoveAt(0);
        }
    }
    private void Start() {
        SpawnedChunks.Add(FirstChunk);
    }
    private void Update() {
        if (SpawnedChunks[SpawnedChunks.Count - 1].GetComponent<Transform>().position.x - GetComponent<Transform>().position.x < 1.53f){
            SpawnChunk();
        }
    }
}
