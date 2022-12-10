using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkLoaderGD : MonoBehaviour
{   
    public Transform player;
    public Chunk[] chunkPrefabs;

    public Chunk FirstChunk;

    public List<Chunk> SpawnedChunks = new List<Chunk>();

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
        if (player.position.x > SpawnedChunks[SpawnedChunks.Count - 1].End.position.x - 15){
            SpawnChunk();
        }
    }

}
