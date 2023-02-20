using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    public GameObject player;
    public Material material;
    public static int chunkSize = Utils.chunkSize;
    public static int radius = Utils.radius;
    public static ConcurrentDictionary<string, Chunk> chunkDict;
    public static ConcurrentDictionary<string, UntouchableChunk> unChunkDict;
    public static List<string> toRemove;
    Vector3 lastBuildPosition;
    bool drawing;


    void BuildChunkAt(Vector3 chunkPos)
    {
        string name = Chunk.CreateChunkName(chunkPos);
        if (!chunkDict.TryGetValue(name, out _))
        {
            Chunk newChunk = new Chunk(chunkPos, material);
            newChunk.goChunk.transform.parent = this.transform;
            chunkDict.TryAdd(newChunk.goChunk.name, newChunk);
        }

    }

    void BuildUnChunkAt(Vector3 chunkPos)
    {
        string name = Utils.untouchableString + Chunk.CreateChunkName(chunkPos);
        
        if (!unChunkDict.TryGetValue(name, out _))
        {
            UntouchableChunk newUnChunk = new UntouchableChunk(chunkPos, material);
            newUnChunk.goChunk.transform.parent = this.transform;
            unChunkDict.TryAdd(newUnChunk.goChunk.name, newUnChunk);
        }
    }

    IEnumerator BuildRecursiveWorld(Vector3 chunkPos, int rad)
    {

        int x = (int)chunkPos.x;
        int y = (int)chunkPos.y;
        int z = (int)chunkPos.z;

        BuildChunkAt(chunkPos);
        yield return null;
        BuildUnChunkAt(chunkPos);
        yield return null;

        if (--rad < 0) yield break;

        Building(new Vector3(x + chunkSize, y, z), rad);
        yield return null;
        Building(new Vector3(x - chunkSize, y, z), rad);
        yield return null;
        Building(new Vector3(x, y + chunkSize, z), rad);
        yield return null;
        Building(new Vector3(x, y - chunkSize, z), rad);
        yield return null;
        Building(new Vector3(x, y, z + chunkSize), rad);
        yield return null;
        Building(new Vector3(x, y, z - chunkSize), rad);
        yield return null;

    }

    IEnumerator RemoveChunks()
    {
        for (int i = 0; i < toRemove.Count; i++)
        {
            string name = toRemove[i];

            if (chunkDict.TryGetValue(name, out Chunk c))
            {
                Destroy(c.goChunk);
                chunkDict.TryRemove(name, out _);
                yield return null;
            }

            name = Utils.untouchableString + name;

            if (unChunkDict.TryGetValue(name, out UntouchableChunk uc))
            {
                Destroy(uc.goChunk);
                unChunkDict.TryRemove(name, out _);
                yield return null;
            }
        }
        toRemove = new List<string>();
    }

    IEnumerator DrawChunks()
    {
        drawing = true;

        foreach (KeyValuePair<string, UntouchableChunk> uc in unChunkDict)
        {
            if (uc.Value.status == Chunk.ChunkStatus.DRAW)
            {
                uc.Value.DrawChunk();
                yield return null;
            }
        }

        foreach (KeyValuePair<string, Chunk> c in chunkDict)
        {
            if (c.Value.status == Chunk.ChunkStatus.DRAW)
            {
                c.Value.DrawChunk();
                yield return null;
            }

            if (c.Value.goChunk && 
                Vector3.Distance(WhichChunk(player.transform.position),
                c.Value.goChunk.transform.position) > chunkSize * radius)
            {
                toRemove.Add(c.Key);
            }
        }

        Removing();

        drawing = false;
    }

    void Removing()
    {
        StartCoroutine(RemoveChunks());
    }

    void Building(Vector3 chunkPos, int rad)
    {
        StartCoroutine(BuildRecursiveWorld(chunkPos, rad));
    }

    void Drawing()
    {
        StartCoroutine(DrawChunks());
    }

    Vector3 WhichChunk(Vector3 playerPosition)
    {
        // Mathf.Floor garante que se a posicao for negativa, arredonta para baixo
        // -0.5 == -1 , ao inves de -0.5 == 0
        Vector3 chunkPos = new Vector3();
        chunkPos.x = Mathf.Floor(playerPosition.x / chunkSize) * chunkSize;
        chunkPos.y = Mathf.Floor(playerPosition.y / chunkSize) * chunkSize;
        chunkPos.z = Mathf.Floor(playerPosition.z / chunkSize) * chunkSize;
        return chunkPos;
    }


    void Start()
    {
        player.SetActive(false);

        chunkDict = new ConcurrentDictionary<string, Chunk>();
        unChunkDict = new ConcurrentDictionary<string, UntouchableChunk>();
        toRemove = new List<string>();

        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;

        Vector3 playerPos = player.transform.position;
        player.transform.position = new Vector3(playerPos.x, Utils.GenerateHeight(playerPos.x, playerPos.z) + 1, playerPos.z);
        lastBuildPosition = player.transform.position;

        Building(WhichChunk(lastBuildPosition), radius);
        Drawing();

        player.SetActive(true);
    }

    void Update()
    {
        Vector3 movement = player.transform.position - lastBuildPosition;
        if (movement.magnitude > chunkSize)
        {
            lastBuildPosition = player.transform.position;
            Building(WhichChunk(lastBuildPosition), radius);
            Drawing();
        }
        if (!drawing) Drawing();
    }
}
