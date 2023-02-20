using System;
using UnityEngine;

public class ChickenSpawnEnv : MonoBehaviour
{
    TileSetter tileSetter;
    ChickenSpawner chickenSpawner;
    ChickenAgent chickenAgent;

    private void Awake()
    {
        tileSetter     = GetComponentInChildren<TileSetter>();
        chickenSpawner = GetComponentInChildren<ChickenSpawner>();
        chickenAgent   = GetComponentInChildren<ChickenAgent>();
    }

    // Start is called before the first frame update
    void Start()
    {
        chickenAgent.OnEpisodeBeginEvent  += HandleOnEpisodeBegin;
        //tileSetter.OnAllTilesPressedEvent += HandleOnAllTilesPressed;
    }

    //public void HandleOnAllTilesPressed(object sender, EventArgs e)
    //{
    //    //chickenSpawner.Spawn();
    //    //tileSetter.SetRandomTiles();
    //}

    public void HandleOnEpisodeBegin(object sender, EventArgs e)
    {
        tileSetter.ResetTiles();

        //GameObject chicken = chickenSpawner.GetPrefabGameObject();
        //if (chicken != null)
        //{
        //    Destroy(chicken);
        //}

        //chickenSpawner.SetPrefabSpawner(false);
    }

}
