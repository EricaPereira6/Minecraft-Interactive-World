using UnityEngine;

public class Block
{

    enum Cubeside { BOTTOM, TOP, LEFT, RIGHT, FRONT, BACK };
    public enum BlockType { GRASS, DIRT, STONE, GRANITE, DIAMOND, COAL, WEED, PINKFLOWER, LEAF, ROOT, WOOD, AIR };

    public static int numBlockTypes = System.Enum.GetValues(typeof(BlockType)).Length;

    public BlockType bType;
    Chunk chunkOwner;   
    Vector3 pos;
    bool isSolid;

    static Vector2 GrassSide_LBC = new Vector2(0f, 14f) / 15; //Left Bottom Coner
    static Vector2 GrassTop_LBC = new Vector2(6f, 4f) / 15;
    static Vector2 Dirt_LBC = new Vector2(11f, 13f) / 15;
    static Vector2 Stone_LBC = new Vector2(2f, 3f) / 15;
    static Vector2 Granite_LBC = new Vector2(2f, 9f) / 15;
    static Vector2 Diamond_LBC = new Vector2(2f, 5f) / 15;
    static Vector2 Coal_LBC = new Vector2(0f, 13f) / 15;
    static Vector2 Weed_LBC = new Vector2(4f, 10f) / 15;
    static Vector2 PinkFlower_LBC = new Vector2(1f, 12f) / 15;
    static Vector2 Leaf_LBC = new Vector2(0f, 2f) / 15;
    static Vector2 Root_LBC = new Vector2(9f, 3f) / 15;
    static Vector2 WoodSide_LBC = new Vector2(5f, 3f) / 15;
    static Vector2 WoodTop_LBC = new Vector2(6f, 3f) / 15;

    static Vector2 Clear_LBC = new Vector2(12f, 2f) / 15;

    Vector2[,] blockUVs =
    {
        /*GRASS TOP*/   {
                            GrassTop_LBC,
                            GrassTop_LBC + new Vector2(1f, 0f) / 16,
                            GrassTop_LBC + new Vector2(0f, 1f) / 16,
                            GrassTop_LBC + new Vector2(1f, 1f) / 16
                        },
        /*GRASS SIDE*/  {
                            GrassSide_LBC,
                            GrassSide_LBC + new Vector2(1f, 0f) / 16,
                            GrassSide_LBC + new Vector2(0f, 1f) / 16,
                            GrassSide_LBC + new Vector2(1f, 1f) / 16
                        },
        /*DIRT*/        {
                            Dirt_LBC,
                            Dirt_LBC + new Vector2(1f, 0f) / 16,
                            Dirt_LBC + new Vector2(0f, 1f) / 16,
                            Dirt_LBC + new Vector2(1f, 1f) / 16
                        },
        /*STONE*/       {
                            Stone_LBC,
                            Stone_LBC + new Vector2(1f, 0f) / 16,
                            Stone_LBC + new Vector2(0f, 1f) / 16,
                            Stone_LBC + new Vector2(1f, 1f) / 16
                        },
        /*GRANITE*/     {
                            Granite_LBC,
                            Granite_LBC + new Vector2(1f, 0f) / 16,
                            Granite_LBC + new Vector2(0f, 1f) / 16,
                            Granite_LBC + new Vector2(1f, 1f) / 16
                        },
        /*DIAMOND*/     {
                            Diamond_LBC,
                            Diamond_LBC + new Vector2(1f, 0f) / 16,
                            Diamond_LBC + new Vector2(0f, 1f) / 16,
                            Diamond_LBC + new Vector2(1f, 1f) / 16
                        },
        /*COAL*/        {
                            Coal_LBC,
                            Coal_LBC + new Vector2(1f, 0f) / 16,
                            Coal_LBC + new Vector2(0f, 1f) / 16,
                            Coal_LBC + new Vector2(1f, 1f) / 16
                        },

        /*WEED*/        {
                            Weed_LBC,
                            Weed_LBC + new Vector2(1f, 0f) / 16,
                            Weed_LBC + new Vector2(0f, 1f) / 16,
                            Weed_LBC + new Vector2(1f, 1f) / 16
                        },
        /*PINKFLOWER*/  {
                            PinkFlower_LBC,
                            PinkFlower_LBC + new Vector2(1f, 0f) / 16,
                            PinkFlower_LBC + new Vector2(0f, 1f) / 16,
                            PinkFlower_LBC + new Vector2(1f, 1f) / 16
                        },
        /*LEAF*/        {
                            Leaf_LBC,
                            Leaf_LBC + new Vector2(1f, 0f) / 16,
                            Leaf_LBC + new Vector2(0f, 1f) / 16,
                            Leaf_LBC + new Vector2(1f, 1f) / 16
                        },
        /*ROOT*/        {
                            Root_LBC,
                            Root_LBC + new Vector2(1f, 0f) / 16,
                            Root_LBC + new Vector2(0f, 1f) / 16,
                            Root_LBC + new Vector2(1f, 1f) / 16
                        },
        /*WOOD SIDE*/   {
                            WoodSide_LBC,
                            WoodSide_LBC + new Vector2(1f, 0f) / 16,
                            WoodSide_LBC + new Vector2(0f, 1f) / 16,
                            WoodSide_LBC + new Vector2(1f, 1f) / 16
                        },
        /*WOOD TOP*/    {
                            WoodTop_LBC,
                            WoodTop_LBC + new Vector2(1f, 0f) / 16,
                            WoodTop_LBC + new Vector2(0f, 1f) / 16,
                            WoodTop_LBC + new Vector2(1f, 1f) / 16
                        },
    };


    public Block(BlockType bType, Vector3 pos, Chunk chunkOwner)
    {
        this.pos = pos;
        this.chunkOwner = chunkOwner;

        SetBlockType(bType);
    }

    public BlockType GetBlockType()
    {
        return bType;
    }

    public void SetBlockType(BlockType blockType)
    {
        bType = blockType;
        isSolid = bType != BlockType.AIR && 
            bType != BlockType.PINKFLOWER && 
            bType != BlockType.WEED && 
            bType != BlockType.ROOT &&
            bType != BlockType.LEAF;
    }


    void CreateQuad(Cubeside side)
    {

        Mesh mesh = new Mesh();

        Vector3 v0 = new Vector3(-0.5f, -0.5f, 0.5f);
        Vector3 v1 = new Vector3(0.5f, -0.5f, 0.5f);
        Vector3 v2 = new Vector3(0.5f, -0.5f, -0.5f);
        Vector3 v3 = new Vector3(-0.5f, -0.5f, -0.5f);
        Vector3 v4 = new Vector3(-0.5f, 0.5f, 0.5f);
        Vector3 v5 = new Vector3(0.5f, 0.5f, 0.5f);
        Vector3 v6 = new Vector3(0.5f, 0.5f, -0.5f);
        Vector3 v7 = new Vector3(-0.5f, 0.5f, -0.5f);


        // Untouchable plants and flowers
        // front
        Vector3 v8 = new Vector3(-0.5f, 0.25f, 0f);
        Vector3 v9 = new Vector3(0.5f, 0.25f, 0f);
        Vector3 v10 = new Vector3(0.5f, -0.5f, 0f);
        Vector3 v11 = new Vector3(-0.5f, -0.5f, 0f);

        // back
        Vector3 v12 = new Vector3(0.5f, 0.25f, -0f);
        Vector3 v13 = new Vector3(-0.5f, 0.25f, -0f);
        Vector3 v14 = new Vector3(-0.5f, -0.5f, -0f);
        Vector3 v15 = new Vector3(0.5f, -0.5f, -0f);

        // left
        Vector3 v16 = new Vector3(-0f, 0.25f, -0.5f);
        Vector3 v17 = new Vector3(-0f, 0.25f, 0.5f);
        Vector3 v18 = new Vector3(-0f, -0.5f, 0.5f);
        Vector3 v19 = new Vector3(-0f, -0.5f, -0.5f);

        // right
        Vector3 v20 = new Vector3(0f, 0.25f, 0.5f);
        Vector3 v21 = new Vector3(0f, 0.25f, -0.5f);
        Vector3 v22 = new Vector3(0f, -0.5f, -0.5f);
        Vector3 v23 = new Vector3(0f, -0.5f, 0.5f);


        Vector2 uv00;
        Vector2 uv01;
        Vector2 uv10;
        Vector2 uv11;


        if (bType == BlockType.GRASS && side == Cubeside.TOP)
        {
            uv00 = blockUVs[0, 0];
            uv10 = blockUVs[0, 1];
            uv01 = blockUVs[0, 2];
            uv11 = blockUVs[0, 3];
        }
        else if (bType == BlockType.GRASS && side == Cubeside.BOTTOM)
        {
            uv00 = blockUVs[2, 0];
            uv10 = blockUVs[2, 1];
            uv01 = blockUVs[2, 2];
            uv11 = blockUVs[2, 3];
        }
        else if (bType == BlockType.WOOD && (side == Cubeside.TOP || side == Cubeside.BOTTOM))
        {
            uv00 = blockUVs[(int)(bType + 2), 0];
            uv10 = blockUVs[(int)(bType + 2), 1];
            uv01 = blockUVs[(int)(bType + 2), 2];
            uv11 = blockUVs[(int)(bType + 2), 3];
        }
        else
        {
            uv00 = blockUVs[(int)(bType + 1), 0];
            uv10 = blockUVs[(int)(bType + 1), 1];
            uv01 = blockUVs[(int)(bType + 1), 2];
            uv11 = blockUVs[(int)(bType + 1), 3];
        }


        Vector3[] vertices = new Vector3[4];
        Vector3[] normals = new Vector3[4];

        int[] triangles = new int[] { 3, 1, 0, 3, 2, 1 }; ;
        Vector2[] uv = new Vector2[] { uv11, uv01, uv00, uv10 };


        if (bType == BlockType.PINKFLOWER || bType == BlockType.WEED || bType == BlockType.ROOT)
        {
            switch (side)
            {
                case Cubeside.FRONT:
                    vertices = new Vector3[] { v8, v9, v10, v11 };
                    normals = new Vector3[] { Vector3.forward, Vector3.forward,
                                                Vector3.forward, Vector3.forward };
                    break;

                case Cubeside.BACK:
                    vertices = new Vector3[] { v12, v13, v14, v15 };
                    normals = new Vector3[] { Vector3.back, Vector3.back,
                                                Vector3.back, Vector3.back };
                    break;

                case Cubeside.LEFT:
                    vertices = new Vector3[] { v16, v17, v18, v19 };
                    normals = new Vector3[] { Vector3.left, Vector3.left,
                                                Vector3.left, Vector3.left };
                    break;

                case Cubeside.RIGHT:
                    vertices = new Vector3[] { v20, v21, v22, v23 };
                    normals = new Vector3[] { Vector3.right, Vector3.right,
                                                Vector3.right, Vector3.right };
                    break;
            }
        }
        else
        {
            switch (side)
            {
                case Cubeside.FRONT:
                    vertices = new Vector3[] { v4, v5, v1, v0 };
                    normals = new Vector3[] { Vector3.forward, Vector3.forward,
                                                Vector3.forward, Vector3.forward };
                    break;

                case Cubeside.BACK:
                    vertices = new Vector3[] { v6, v7, v3, v2 };
                    normals = new Vector3[] { Vector3.back, Vector3.back,
                                                Vector3.back, Vector3.back };
                    break;

                case Cubeside.BOTTOM:
                    vertices = new Vector3[] { v0, v1, v2, v3 };
                    normals = new Vector3[] { Vector3.down, Vector3.down,
                                                Vector3.down, Vector3.down };
                    break;

                case Cubeside.TOP:
                    vertices = new Vector3[] { v7, v6, v5, v4 };
                    normals = new Vector3[] { Vector3.up, Vector3.up,
                                                Vector3.up, Vector3.up };
                    break;

                case Cubeside.LEFT:
                    vertices = new Vector3[] { v7, v4, v0, v3 };
                    normals = new Vector3[] { Vector3.left, Vector3.left,
                                                Vector3.left, Vector3.left };
                    break;

                case Cubeside.RIGHT:
                    vertices = new Vector3[] { v5, v6, v2, v1 };
                    normals = new Vector3[] { Vector3.right, Vector3.right,
                                                Vector3.right, Vector3.right };
                    break;
            }
        }


        mesh.vertices = vertices;
        mesh.normals = normals;
        mesh.triangles = triangles;
        mesh.uv = uv;

        mesh.RecalculateBounds(); //para optimizacao, smp q se mexe numa mesh

        GameObject quad = new GameObject("quad");
        quad.transform.position = this.pos;
        quad.transform.parent = this.chunkOwner.goChunk.transform;

        MeshFilter mf = quad.gameObject.AddComponent<MeshFilter>();
        mf.mesh = mesh;

        //MeshRenderer mr = quad.gameObject.AddComponent<MeshRenderer>();
        //mr.material = material;

    }

    int ConvertToLocalIndex(int i)
    {
        if (i == -1)
            return World.chunkSize - 1;
        if (i == World.chunkSize)
            return 0;
        return i;
    }


    bool HasSolidNeighbour(int x, int y, int z)
    {
        // x, y, z - coordenadas do bloco a comparar (bloco vizinho)

        Block[,,] chunkData;

        // verifica se o bloco esta dentro do chunk atual ou se esta dentro do seguinte
        if (x < 0 || x >= World.chunkSize || y < 0 || y >= World.chunkSize || z < 0 || z >= World.chunkSize)
        {
            // se estiver fora do chunk, vou buscar as coordenadas do chunk vizinho 
            // pos - coordenadas locais do bloco
            // a conta ou da    ->    (-1 - 0)  ou  (17 - 16)  
            // (-1 ou 1) * 16, subtrai ou soma 16 à posicao do chunk em que estou
            Vector3 neighborChunkPos = chunkOwner.goChunk.transform.position + new Vector3(
                (x - (int)pos.x) * World.chunkSize,
                (y - (int)pos.y) * World.chunkSize,
                (z - (int)pos.z) * World.chunkSize
            );

            string chunkName = Chunk.CreateChunkName(neighborChunkPos);

            x = ConvertToLocalIndex(x);
            y = ConvertToLocalIndex(y);
            z = ConvertToLocalIndex(z);


            if (World.chunkDict.TryGetValue(chunkName, out Chunk neighChunk))
            {
                chunkData = neighChunk.chunkData;
            }
            else
                return false;   // ou seja, é air
        }
        else
            // pertence entao ao mesmo chunk
            chunkData = chunkOwner.chunkData;

        try
        {
            return chunkData[x, y, z].isSolid;
        }
        catch (System.IndexOutOfRangeException ex)
        {
            Debug.Log(ex);
            return false;
        }
    }

    public void Draw()
    {
        if (bType == BlockType.AIR)
        {
            return;
        }
        
        if (!HasSolidNeighbour((int)(pos.x - 1), (int)pos.y, (int)pos.z))
            CreateQuad(Cubeside.LEFT);
        if (!HasSolidNeighbour((int)(pos.x + 1), (int)pos.y, (int)pos.z))
            CreateQuad(Cubeside.RIGHT);

        if (!HasSolidNeighbour((int)pos.x, (int)(pos.y - 1), (int)pos.z))
            CreateQuad(Cubeside.BOTTOM);
        if (!HasSolidNeighbour((int)pos.x, (int)(pos.y + 1), (int)pos.z))
            CreateQuad(Cubeside.TOP);

        if (!HasSolidNeighbour((int)pos.x, (int)pos.y, (int)(pos.z - 1)))
            CreateQuad(Cubeside.BACK);
        if (!HasSolidNeighbour((int)pos.x, (int)pos.y, (int)(pos.z + 1)))
            CreateQuad(Cubeside.FRONT);
    }
}

