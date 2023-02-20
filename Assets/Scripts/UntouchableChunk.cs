using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UntouchableChunk : Chunk
{
    public UntouchableChunk(Vector3 pos, Material material) : base()
    {
        goChunk = new GameObject(Utils.untouchableString + CreateChunkName(pos));
        goChunk.transform.position = pos;
        this.material = material;

        BuildChunk();

        status = ChunkStatus.DRAW;
    }

    public override void BuildChunk()
    {

        chunkData = new Block[World.chunkSize, World.chunkSize, World.chunkSize];

        for (int z = 0; z < World.chunkSize; z++)
        {
            for (int y = 0; y < World.chunkSize; y++)
            {
                for (int x = 0; x < World.chunkSize; x++)
                {
                    Vector3 pos = new Vector3(x, y, z);

                    // coordenadas do mundo  -->  a coord. global do chunk + coord. local
                    int worldX = (int)goChunk.transform.position.x + x;
                    int worldY = (int)goChunk.transform.position.y + y;
                    int worldZ = (int)goChunk.transform.position.z + z;

                    int dirtHeight = Utils.GenerateHeight(worldX, worldZ);
                    int stoneHeight = Utils.GenerateStoneHeight(worldX, worldZ);

                    //Debug.Log(Utils.FBM3D(worldX, worldY, worldZ, 10, 1.2f));
                    //Grapher.Log(Utils.FBM(worldX, worldZ, 1, 0.7f), "fBM ", Color.yellow);

                    if (worldY == dirtHeight + 1 && 
                        worldY > stoneHeight + 1 && 
                        Utils.FBM3D(worldX, worldY, worldZ, Utils.grassOctaves, Utils.grassPersistence) < Utils.grassFBMValue &&
                        Utils.FBM3D(worldX, worldY, worldZ, Utils.grassOctaves, Utils.grassPersistence) > Utils.grassFBMValue - Utils.grassFBMOffset)
                    {
                        if (Utils.FBM3D(worldX, worldY, worldZ, Utils.grassOctaves, Utils.grassPersistence) < Utils.grassFBMValue &&
                        Utils.FBM3D(worldX, worldY, worldZ, Utils.grassOctaves, Utils.grassPersistence) > Utils.grassFBMValue - Utils.flowerFBMOffset)
                        {
                            chunkData[x, y, z] = new Block(Block.BlockType.PINKFLOWER, pos, this);
                        }
                        else
                        {
                            chunkData[x, y, z] = new Block(Block.BlockType.WEED, pos, this);
                        }

                    }
                    else
                    {
                        chunkData[x, y, z] = new Block(Block.BlockType.AIR, pos, this);
                    }

                }
            }
        }
    }

    public override void DrawChunk()
    {

        for (int z = 0; z < World.chunkSize; z++)
            for (int y = 0; y < World.chunkSize; y++)
                for (int x = 0; x < World.chunkSize; x++)
                    chunkData[x, y, z].Draw();

        CombineQuads();

        //MeshCollider collider = goChunk.AddComponent<MeshCollider>();
        //collider.sharedMesh = goChunk.GetComponent<MeshFilter>().mesh;
        //collider.convex = true;
        //collider.isTrigger = true;

        status = ChunkStatus.DONE;
    }

}