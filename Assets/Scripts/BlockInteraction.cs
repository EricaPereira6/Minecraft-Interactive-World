using System.Collections.Generic;
using UnityEngine;

public class BlockInteraction : MonoBehaviour
{

    public Camera cam;

    public GameObject player;

    public enum InterationType { DESTROY, BUILD, WISTLE, CLAP };
    public static InterationType interationType;

    public static bool soundInteration;

    static int treeHeight;

    Vector3 selectedTreeBlock;
    Vector3 selectedTreeChunk;

    // Start is called before the first frame update
    void Start()
    {
        soundInteration = false;
        treeHeight = 0;

        selectedTreeBlock = new Vector3();
        selectedTreeChunk = new Vector3();
    }

    public static bool MicInteraction()
    {
        return Input.GetKeyDown(KeyCode.E);
    }

    void MouseInteraction()
    {
        bool interation = Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1);
        if (interation)
        {
            interationType = Input.GetMouseButtonDown(0) ? InterationType.DESTROY : InterationType.BUILD;
            Interaction(interationType);
        }
    }

    public static void SetTreeHeight(int height)
    {
        treeHeight = height;
    }

    public static void SetInteractionType(InterationType interaction)
    {
        interationType = interaction;
    }

    public static void StartSoundInteraction()
    {
        soundInteration = true;
    }

    void SoundInteraction()
    {
        if (soundInteration)
        {
            soundInteration = false;
            Interaction(interationType);
        }
    }

    void Interaction(InterationType interationType)
    {
        RaycastHit hit;
        bool collision = Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, 10);

        if (collision)
        {
            float chunkx = hit.collider.gameObject.transform.position.x;
            float chunky = hit.collider.gameObject.transform.position.y;
            float chunkz = hit.collider.gameObject.transform.position.z;

            Vector3 hitBlock;
            if (interationType == InterationType.BUILD || interationType == InterationType.WISTLE)
            {
                hitBlock = hit.point + (hit.normal / 2f);
            }
            else
            {
                hitBlock = hit.point - (hit.normal / 2f);
            }

            Vector3 block = UpdateBlock(hitBlock, chunkx, chunky, chunkz);
            Vector3 chunk = UpdateChunk(hitBlock, chunkx, chunky, chunkz);

            int blockx = (int)block.x;
            int blocky = (int)block.y;
            int blockz = (int)block.z;
            chunkx = chunk.x;
            chunky = chunk.y;
            chunkz = chunk.z;
            
            string chunkName = Interact(hit, block, chunk);
            

            List<string> updateNeighbors = new List<string>();

            updateNeighbors.Add(chunkName);

            if (blockx == 0)
                updateNeighbors.Add(Chunk.CreateChunkName(new Vector3(chunkx - World.chunkSize, chunky, chunkz)));
            if (blockx == World.chunkSize - 1)
                updateNeighbors.Add(Chunk.CreateChunkName(new Vector3(chunkx + World.chunkSize, chunky, chunkz)));
            if (blocky == 0)
                updateNeighbors.Add(Chunk.CreateChunkName(new Vector3(chunkx, chunky - World.chunkSize, chunkz)));
            if (blocky == World.chunkSize - 1)
                updateNeighbors.Add(Chunk.CreateChunkName(new Vector3(chunkx, chunky + World.chunkSize, chunkz)));
            if (blockz == 0)
                updateNeighbors.Add(Chunk.CreateChunkName(new Vector3(chunkx, chunky, chunkz - World.chunkSize)));
            if (blockz == World.chunkSize - 1)
                updateNeighbors.Add(Chunk.CreateChunkName(new Vector3(chunkx, chunky, chunkz + World.chunkSize)));


            UpdateChunks(updateNeighbors);
        }
    }

    Vector3 UpdateBlock(Vector3 hitBlock, float chunkx, float chunky, float chunkz)
    {
        int blockx = (int)(Mathf.Round(hitBlock.x) - chunkx);
        int blocky = (int)(Mathf.Round(hitBlock.y) - chunky);
        int blockz = (int)(Mathf.Round(hitBlock.z) - chunkz);

        if (blockx < 0)
        {
            blockx += World.chunkSize;
        }
        if (blockx >= World.chunkSize)
        {
            blockx -= World.chunkSize;
        }
        if (blocky < 0)
        {
            blocky += World.chunkSize;
        }
        if (blocky >= World.chunkSize)
        {
            blocky -= World.chunkSize;
        }
        if (blockz < 0)
        {
            blockz += World.chunkSize;
        }
        if (blockz >= World.chunkSize)
        {
            blockz -= World.chunkSize;
        }

        return new Vector3(blockx, blocky, blockz);
    }
    Vector3 UpdateChunk(Vector3 hitBlock, float chunkx, float chunky, float chunkz)
    {
        int blockx = (int)(Mathf.Round(hitBlock.x) - chunkx);
        int blocky = (int)(Mathf.Round(hitBlock.y) - chunky);
        int blockz = (int)(Mathf.Round(hitBlock.z) - chunkz);

        if (blockx < 0)
        {
            chunkx -= World.chunkSize;
        }
        if (blockx >= World.chunkSize)
        {
            chunkx += World.chunkSize;
        }
        if (blocky < 0)
        {
            chunky -= World.chunkSize;
        }
        if (blocky >= World.chunkSize)
        {
            chunky += World.chunkSize;
        }
        if (blockz < 0)
        {
            chunkz -= World.chunkSize;
        }
        if (blockz >= World.chunkSize)
        {
            chunkz += World.chunkSize;
        }

        return new Vector3(chunkx, chunky, chunkz);
    }

    string Interact(RaycastHit hit, Vector3 block, Vector3 chunk)
    {
        //Block.BlockType addMyType = Block.BlockType.PINKFLOWER;
        Block.BlockType addMyType = ToolBar.CurrentBlockTypeInSlot;

        string chunkName = hit.collider.gameObject.name;
        string uChunkName = Utils.untouchableString + chunkName;
        int blockx = (int)block.x;
        int blocky = (int)block.y;
        int blockz = (int)block.z;
        float chunkx = chunk.x;
        float chunky = chunk.y;
        float chunkz = chunk.z;

        if (interationType == InterationType.BUILD &&

            (addMyType == Block.BlockType.PINKFLOWER ||
             addMyType == Block.BlockType.WEED ||
             addMyType == Block.BlockType.ROOT ||
             addMyType == Block.BlockType.LEAF))
        {
            //Debug.Log("BUILD IN UNTOUCHABLES");
            if (World.unChunkDict.TryGetValue(uChunkName, out UntouchableChunk uc))
            {
                uc.chunkData[blockx, blocky, blockz].SetBlockType(addMyType);
            }
            return uChunkName;
        }

        if (interationType == InterationType.DESTROY) {
            Vector3 hitBlock = hit.point + (hit.normal / 2f);
            Vector3 ublock = UpdateBlock(hitBlock, chunkx, chunky, chunkz);
            Vector3 uchunk = UpdateChunk(hitBlock, chunkx, chunky, chunkz);

            int ublockx = (int)ublock.x;
            int ublocky = (int)ublock.y;
            int ublockz = (int)ublock.z;
            float uchunkx = uchunk.x;
            float uchunky = uchunk.y;
            float uchunkz = uchunk.z;
            string newUCname = Utils.untouchableString + Chunk.CreateChunkName(new Vector3(uchunkx, uchunky, uchunkz));


            //Debug.Log("dESTROY -> block: " + block + ", ublock: " + ublock);
            //Debug.Log("dESTROY -> chunk: " + chunk + ", uchunk: " + uchunk);

            if (World.unChunkDict.TryGetValue(newUCname, out UntouchableChunk uChunk))
            {
                if (uChunk.chunkData[ublockx, ublocky, ublockz].GetBlockType() != Block.BlockType.AIR)
                {
                    //Debug.Log("DESTROY IN UNTOUCHABLES");
                    uChunk.chunkData[ublockx, ublocky, ublockz].SetBlockType(Block.BlockType.AIR);
                    return newUCname;
                }
            }
        }
        
        if (World.chunkDict.TryGetValue(chunkName, out Chunk c))
        {
            Block.BlockType blockType;
            switch (interationType)
            {
                case InterationType.DESTROY:
                    //Debug.Log("DESTROY IN terrain");
                    c.chunkData[blockx, blocky, blockz].SetBlockType(Block.BlockType.AIR);
                    break;

                case InterationType.BUILD:
                    //Debug.Log("BUILD IN terrain");
                    int playerx = (int)(Mathf.Round(player.transform.position.x) - chunkx);
                    int playery = (int)(Mathf.Round(player.transform.position.y) - chunky);
                    int playerz = (int)(Mathf.Round(player.transform.position.z) - chunkz);

                    if (!(blockx == playerx && blocky == playery && blockz == playerz) &&
                        !(blockx == playerx && blocky == playery + 1 && blockz == playerz))
                    {
                        c.chunkData[blockx, blocky, blockz].SetBlockType(addMyType);
                    }
                    break;

                case InterationType.WISTLE:
                    if (World.unChunkDict.TryGetValue(uChunkName, out UntouchableChunk unChunk))
                    {
                        blockType = unChunk.chunkData[blockx, blocky, blockz].GetBlockType();
                        if (blockType == Block.BlockType.ROOT ||
                        (selectedTreeBlock == block && selectedTreeChunk == chunk))
                        {
                            selectedTreeBlock = block;
                            selectedTreeChunk = chunk;
                            //GROW TREE
                            TextCanvas.AddMessage("Generated tree height: " + treeHeight, Color.green);
                            TreeBuilder.SetTreeParameters(treeHeight, chunk, block);
                        }
                    }
                    break;

                case InterationType.CLAP:
                    int blockTypeIndex = Random.Range(0, Block.numBlockTypes - 1);  // no AIR
                    Block.BlockType newBlockType = (Block.BlockType)blockTypeIndex;

                    blockType = c.chunkData[blockx, blocky, blockz].GetBlockType();
                    if (blockType == newBlockType) newBlockType = Block.BlockType.AIR;

                    //newBlockType = Block.BlockType.ROOT;

                    c.chunkData[blockx, blocky, blockz].SetBlockType(newBlockType);
                    TextCanvas.AddMessage("New block type: " + newBlockType, Color.blue);
                    break;
            }
        }

        return chunkName;
    }

    public static List<string> UpdateChunks(List<string> update)
    {
        foreach (string cname in update)
        {
            if (World.chunkDict.TryGetValue(cname, out Chunk chunk))
            {
                DestroyImmediate(chunk.goChunk.GetComponent<MeshFilter>());
                DestroyImmediate(chunk.goChunk.GetComponent<MeshRenderer>());
                DestroyImmediate(chunk.goChunk.GetComponent<MeshCollider>());
                chunk.DrawChunk();
            }
            else if (World.unChunkDict.TryGetValue(cname, out UntouchableChunk uchunk))
            {
                DestroyImmediate(uchunk.goChunk.GetComponent<MeshFilter>());
                DestroyImmediate(uchunk.goChunk.GetComponent<MeshRenderer>());
                DestroyImmediate(uchunk.goChunk.GetComponent<MeshCollider>());
                uchunk.DrawChunk();
            }
        }

        return new List<string>();
    }

    // Update is called once per frame
    void Update()
    {
        MouseInteraction();
        SoundInteraction();
    }
}
