using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateWalls : MonoBehaviour
{

    [SerializeField] private Transform triggerobj;
    [SerializeField] private Chunk[] chunkPrefabs;

    [SerializeField] private GameObject Sky;

    [SerializeField] private Chunk FirstChunk;

    [SerializeField] private List<Chunk> SpawnedChunks = new List<Chunk>();

    private void SpawnChunk(){
        Chunk newChunk = Instantiate(chunkPrefabs[Random.Range(0, chunkPrefabs.Length)]);
        newChunk.transform.position = SpawnedChunks[SpawnedChunks.Count - 1].End.position - newChunk.Begin.localPosition;
        SpawnedChunks.Add(newChunk);

        if (SpawnedChunks.Count >= 3) {
            Destroy(SpawnedChunks[0].gameObject);
            SpawnedChunks.RemoveAt(0);
        }
    }

    private void Start() {
        SpawnedChunks.Add(FirstChunk);
    }
    private void Update() {
        if (triggerobj.position.x > SpawnedChunks[SpawnedChunks.Count - 1].End.position.x){
            SpawnChunk();
        }
    }
}
