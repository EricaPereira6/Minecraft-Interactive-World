using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeBuilder : MonoBehaviour
{
    const int maxLeafRad = 4;
    const int noLeafPercent = 45;

    static int treeHeight;
    static Vector3 chunkPos;
    static Vector3 blockPos;

    static List<string> chunksToUpdate;

    private void Start()
    {
        treeHeight = 0;
        chunkPos = new Vector3();
        blockPos = new Vector3();

        chunksToUpdate = new List<string>();
    }

    public static List<string> GetChunksToUpdate()
    {
        return chunksToUpdate;
    }

    public static void SetTreeParameters(int height, Vector3 chunkPosition, Vector3 blockPosition)
    {
        treeHeight = height;
        chunkPos = chunkPosition;
        blockPos = blockPosition;
    }

    void GrowTree()
    {
        if (treeHeight > 0)
        {
            StartTreeGrowth();
            treeHeight = 0;
        }
    }

    void StartTreeGrowth()
    {
        int numChunks = 0;

        int blockx = (int)blockPos.x;
        int blocky = (int)blockPos.y;
        int blockz = (int)blockPos.z;

        float chunkx = chunkPos.x;
        float chunky = chunkPos.y;
        float chunkz = chunkPos.z;

        string chunkName = Chunk.CreateChunkName(new Vector3(chunkx, chunky, chunkz));

        int rand = Random.Range(2, 6);
        int leafHeight = Mathf.Max(treeHeight / rand, 1);

        for (int y = blocky; y < blocky + 20; y++)
        {
            //block height local position - depends on the current chunk
            int currentBlockHeight = y - (World.chunkSize * numChunks);

            if (World.chunkDict.TryGetValue(chunkName, out Chunk chunk))
            {
                if (y < blocky + treeHeight)
                {
                    chunk.chunkData[blockx, currentBlockHeight, blockz].SetBlockType(Block.BlockType.WOOD);
                }
                else
                {
                    chunk.chunkData[blockx, currentBlockHeight, blockz].SetBlockType(Block.BlockType.AIR);
                }

                int rad = maxLeafRad;
                bool fillWithLeaves = false;

                if (y >= (blocky + leafHeight) && y < blocky + treeHeight)
                {
                    rad = Random.Range(1, maxLeafRad + 1);
                    fillWithLeaves = true;
                }
                // se nao entrar no IF anterior, elimina possiveis folhas fora do novo intervalo de folhas -> fillWithLeaves = false
                UpdatingLeaves(new Vector3(chunkx, chunky, chunkz), new Vector3(blockx, currentBlockHeight, blockz), rad, noLeafPercent, fillWithLeaves);
            }

            if (currentBlockHeight == World.chunkSize - 1)
            {
                chunky += World.chunkSize;
                chunkName = Chunk.CreateChunkName(new Vector3(chunkx, chunky, chunkz));
                if (!chunksToUpdate.Contains(chunkName))
                    chunksToUpdate.Add(chunkName);
                numChunks++;
            }
        }
    }

    void UpdatingLeaves(Vector3 chunkPos, Vector3 blockPos, int rad, float noLeafPercent, bool fillWithLeaves)
    {
        StartCoroutine(UpdateRecursiveLeaves(chunkPos, blockPos, rad, noLeafPercent, fillWithLeaves));
    }

    IEnumerator UpdateRecursiveLeaves(Vector3 chunkPos, Vector3 blockPos, int rad, float noLeafPercent, bool fillWithLeaves)
    {
        int bx = (int)blockPos.x;
        int by = (int)blockPos.y;
        int bz = (int)blockPos.z;

        float cx = chunkPos.x;
        float cy = chunkPos.y;
        float cz = chunkPos.z;

        if (bx == World.chunkSize)
        {
            bx = 0;
            cx += World.chunkSize;
        }
        else if (bx == -1)
        {
            bx = World.chunkSize - 1;
            cx -= World.chunkSize;
        }
        if (bz == World.chunkSize)
        {
            bz = 0;
            cz += World.chunkSize;
        }
        else if (bz == -1)
        {
            bz = World.chunkSize - 1;
            cz -= World.chunkSize;
        }
        blockPos = new Vector3(bx, by, bz);
        chunkPos = new Vector3(cx, cy, cz);
        string chunkName = Chunk.CreateChunkName(chunkPos);



        if (World.chunkDict.TryGetValue(chunkName, out Chunk chunk))
        {
            Block.BlockType currentType = chunk.chunkData[bx, by, bz].GetBlockType();
            Block.BlockType newType = Block.BlockType.AIR;

            if (fillWithLeaves)
            {
                if (currentType == Block.BlockType.AIR || currentType == Block.BlockType.LEAF)
                {
                    int random = Random.Range(0, 100);
                    if (random > noLeafPercent)
                        newType = Block.BlockType.LEAF;

                    UpdateLeaf(chunk, blockPos, newType);
                }
            }
            else if (currentType == Block.BlockType.LEAF)
            {
                UpdateLeaf(chunk, blockPos, newType);
            }
            yield return null;

            if (--rad < 0) yield break;

            UpdatingLeaves(chunkPos, new Vector3(bx + 1, by, bz), rad, noLeafPercent, fillWithLeaves);
            yield return null;
            UpdatingLeaves(chunkPos, new Vector3(bx - 1, by, bz), rad, noLeafPercent, fillWithLeaves);
            yield return null;
            UpdatingLeaves(chunkPos, new Vector3(bx, by, bz + 1), rad, noLeafPercent, fillWithLeaves);
            yield return null;
            UpdatingLeaves(chunkPos, new Vector3(bx, by, bz - 1), rad, noLeafPercent, fillWithLeaves);
            yield return null;
            
        }
        
    }

    void UpdateLeaf(Chunk chunk, Vector3 blockPos, Block.BlockType type)
    {
        string chunkName = Chunk.CreateChunkName(chunk.GetPosition());

        if (World.chunkDict.TryGetValue(chunkName, out Chunk c))
        {
            c.chunkData[(int)blockPos.x, (int)blockPos.y, (int)blockPos.z].SetBlockType(type);
            if (!chunksToUpdate.Contains(chunkName))
                chunksToUpdate.Add(chunkName);
        }


    }

    private void Update()
    {
        GrowTree();
        if (chunksToUpdate.Count > 0)
            chunksToUpdate = BlockInteraction.UpdateChunks(chunksToUpdate);
    }
}
