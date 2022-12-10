using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyGenerate : MonoBehaviour
{


    [SerializeField] private GameObject[] chunkPrefabs;
    [SerializeField] private GameObject Sky;

    [SerializeField] private GameObject FirstChunk;
    [SerializeField] private List<GameObject> SpawnedChunks = new List<GameObject>();


    private void SpawnChunk(){
        GameObject newChunk = Instantiate(chunkPrefabs[Random.Range(0, chunkPrefabs.Length)]);
        newChunk.GetComponent<Transform>().position = new Vector2(51.65f, -11.5f);
        SpawnedChunks.Add(newChunk);

        if (SpawnedChunks.Count >= 3) {
            Destroy(SpawnedChunks[0]);
            SpawnedChunks.RemoveAt(0);
        }
    }
    private void Start() {
        Application.targetFrameRate = 120;
        SpawnedChunks.Add(FirstChunk);
    }
    private void Update() {
        if (SpawnedChunks[SpawnedChunks.Count - 1].GetComponent<Transform>().position.x < -5.7f){
            SpawnChunk();
        }
    }
}
