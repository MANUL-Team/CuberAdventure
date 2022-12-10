using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] chunkPrefabs;

    [SerializeField] private GameObject FirstChunk;
    [SerializeField] private List<GameObject> SpawnedChunks = new List<GameObject>();


    private void SpawnChunk(){
        GameObject newChunk = Instantiate(chunkPrefabs[Random.Range(0, chunkPrefabs.Length)]);
        newChunk.GetComponent<Transform>().position = new Vector2(-27f, -2.17f);
        SpawnedChunks.Add(newChunk);

        if (SpawnedChunks.Count >= 2) {
            Destroy(SpawnedChunks[0]);
            SpawnedChunks.RemoveAt(0);
        }
    }
    private void Start() {
        SpawnedChunks.Add(FirstChunk);
    }
    private void Update() {
        if (SpawnedChunks[SpawnedChunks.Count - 1].GetComponent<Transform>().position.x > 19f){
            SpawnChunk();
        }
    }
}
