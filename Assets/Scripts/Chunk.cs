using System.Collections.Generic;
using UnityEngine;

public class Chunk
{
    public Block[,,] chunkData;
    public GameObject goChunk;
    
    protected Material material;
    public enum ChunkStatus { DRAW, DONE };
    public ChunkStatus status;


    public Chunk()
    {

    }

    public Chunk(Vector3 pos, Material material)
    {
        goChunk = new GameObject(CreateChunkName(pos));
        goChunk.transform.position = pos;
        this.material = material;

        BuildChunk();

        status = ChunkStatus.DRAW;
    }

    public static string CreateChunkName(Vector3 v)
    {
        return (int)v.x + " " + (int)v.y + " " + (int)v.z;
    }

    public Vector3 GetPosition()
    {
        return goChunk.transform.position;
    }


    public virtual void BuildChunk()
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


                    //Debug.Log(Utils.FBM3D(worldX, worldY, worldZ, 11, 1.5f));
                    //Grapher.Log(Utils.fBM3D(worldX, worldY, worldZ, 1, 0.5f), "fBM3D ", Color.yellow);

                    if (worldY <= stoneHeight)
                    { 
                        // grutas
                        if (Utils.FBM3D(worldX, worldY, worldZ, Utils.caveOctaves, Utils.cavePersistence) < Utils.caveFBMValue) 
                        {
                            //Diamonds
                            //Cria uma orla de diamantes com 3 a 4 blocos de grossura com min e max Offset à volta das grutas,
                            //a uma distancia offset da superficie, com pouca percentagem de aparecer
                            if (worldY < dirtHeight - Utils.diamondHeightOffset &&
                                Utils.FBM3D(worldX, worldY, worldZ, Utils.diamondOctaves, Utils.diamondPersistence) < 0.445f &&
                                Utils.FBM3D(worldX, worldY, worldZ, Utils.diamondOctaves, Utils.diamondPersistence) > Utils.diamondFBMValue - Utils.diamondFBMOffset &&
                                Utils.FBM3D(worldX, worldY, worldZ, Utils.caveOctaves, Utils.cavePersistence) < Utils.caveFBMValue - Utils.minDiamondOffset &&
                                Utils.FBM3D(worldX, worldY, worldZ, Utils.caveOctaves, Utils.cavePersistence) > Utils.caveFBMValue - Utils.maxDiamondOffset)
                            {
                                chunkData[x, y, z] = new Block(Block.BlockType.DIAMOND, pos, this);
                                //chunkData[x, y, z] = new Block(Block.BlockType.AIR, pos, this);
                            }

                            //Rochas de carvao
                            else if (worldY < dirtHeight - Utils.diamondHeightOffset &&
                                    Utils.FBM3D(worldX, worldY, worldZ, Utils.caveOctaves + 2, Utils.cavePersistence + 0.6f) < Utils.coalFBMValue &&
                                    Utils.FBM3D(worldX, worldY, worldZ, Utils.caveOctaves + 2, Utils.cavePersistence + 0.6f) > Utils.coalFBMValue - Utils.coalOffset)
                            {
                                chunkData[x, y, z] = new Block(Block.BlockType.COAL, pos, this);
                                //chunkData[x, y, z] = new Block(Block.BlockType.AIR, pos, this);
                            }
                            else
                            {

                                if (Random.Range(0f, 1f) < Utils.granitePercentage)
                                {
                                    chunkData[x, y, z] = new Block(Block.BlockType.GRANITE, pos, this);
                                    //chunkData[x, y, z] = new Block(Block.BlockType.AIR, pos, this);
                                }
                                else
                                {
                                    chunkData[x, y, z] = new Block(Block.BlockType.STONE, pos, this);
                                    //chunkData[x, y, z] = new Block(Block.BlockType.AIR, pos, this);
                                }
                            }
                        }
                        else
                        {
                            chunkData[x, y, z] = new Block(Block.BlockType.AIR, pos, this);
                        }
                    }
                    else
                    {
                        if (worldY == dirtHeight)
                        {
                            chunkData[x, y, z] = new Block(Block.BlockType.GRASS, pos, this);
                        }
                        else if (worldY < dirtHeight)
                        {
                            chunkData[x, y, z] = new Block(Block.BlockType.DIRT, pos, this);
                        }
                        else
                        {
                            chunkData[x, y, z] = new Block(Block.BlockType.AIR, pos, this);
                        }
                    }


                }
            }
        }
    }

    public virtual void DrawChunk()
    {

        for (int z = 0; z < World.chunkSize; z++)
            for (int y = 0; y < World.chunkSize; y++)
                for (int x = 0; x < World.chunkSize; x++)
                    chunkData[x, y, z].Draw();

        CombineQuads();

        MeshCollider collider = goChunk.AddComponent<MeshCollider>();
        collider.sharedMesh = goChunk.GetComponent<MeshFilter>().mesh;
        

        status = ChunkStatus.DONE;
    }



    protected void CombineQuads()
    {
        //1. combine all children meshes
        MeshFilter[] meshFilters = goChunk.GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];

        int i = 0;
        while (i < meshFilters.Length)
        {

            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;

            i++;
        }

        //2. create a new mesh on the parent object
        MeshFilter mf = goChunk.AddComponent<MeshFilter>();
        mf.mesh = new Mesh();

        //3. add combined meshes on children as the parent's mesh
        mf.mesh.CombineMeshes(combine);

        //4. create a renderer for the parent
        MeshRenderer renderer = goChunk.AddComponent<MeshRenderer>();
        renderer.material = material;

        //5. delete all incombined children
        foreach (Transform quad in goChunk.transform)
        {
            GameObject.Destroy(quad.gameObject);
        }
    }
}
