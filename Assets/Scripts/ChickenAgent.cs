using System;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class ChickenAgent : Agent
{
    //public Button button;

    public ChickenSpawner chickenSpawner;
    public TileSetter tileSetter;

    public Material winMaterial, loseMaterial, whiteMaterial;
    public MeshRenderer redLightRender, greenLightRender;

    Rigidbody rb;

    private bool firstEpisode;

    public event EventHandler OnEpisodeBeginEvent;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        firstEpisode = true;
    }

    public override void OnEpisodeBegin()
    {
        float x = UnityEngine.Random.Range(-7.0f, 7.0f);
        float z;
        if ( x < -5.0f || x > 5.0f )
        {
            z = UnityEngine.Random.Range(-5.0f, 5.0f);
        }
        else
        {
            z = UnityEngine.Random.Range(-7.0f, 7.0f);
        }

        transform.localPosition = new Vector3(x, transform.localPosition.y, z);

        if (!firstEpisode)
        {
            if (!tileSetter.IsAllTilesPressed())
            {
                redLightRender.material = loseMaterial;
                greenLightRender.material = whiteMaterial;
            }
        }
        else
        {
            firstEpisode = false;
        }
        OnEpisodeBeginEvent?.Invoke(this, EventArgs.Empty);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        List<Tile> tiles = tileSetter.GetTiles();
        foreach(Tile tile in tiles)
        {
            Vector3 dirToButton = (tile.GetTilePosition() - transform.position).normalized;
            sensor.AddObservation(dirToButton.x);
            sensor.AddObservation(dirToButton.z);
            sensor.AddObservation(tile.GetTileNumber());
            sensor.AddObservation(tile.IsPressed() ? 1 : 0);
        }

        //sensor.AddObservation(chickenSpawner.HasPrefabSpawned() ? 1 : 0);

        //if (chickenSpawner.HasPrefabSpawned())
        //{
        //    Vector3 dirToFood = (chickenSpawner.transform.position - transform.position).normalized;
        //    sensor.AddObservation(dirToFood.x);
        //    sensor.AddObservation(dirToFood.z);
        //}
        //else
        //{
        //    sensor.AddObservation(0);
        //    sensor.AddObservation(0);
        //}

    }


    public override void OnActionReceived(ActionBuffers actions)
    {
        int moveX = actions.DiscreteActions[0];
        int moveZ = actions.DiscreteActions[1];
        bool pressTile = actions.DiscreteActions[2] == 1;
        //int nextIndex = 2;

        //List<bool> tilesPressed = new List<bool>();
        //bool allTilesPressed = false;

        //for(int i = 0; i < tileSetter.GetNumberOfTiles(); i++)
        //{
        //    tilesPressed.Add(actions.DiscreteActions[i + nextIndex] == 1);
        //    allTilesPressed = (i == 0) ? tilesPressed[i] : allTilesPressed && tilesPressed[i];
        //}

        Vector3 force = Vector3.zero;

        force.x = (moveX == 2) ? -1 : moveX;
        force.z = (moveZ == 2) ? -1 : moveZ;

        float strength = 5f;
        //rb.AddForce(force * strength);

        //for easier heuristic
        rb.velocity = force * strength;

        if (pressTile)
        {
            Collider[] colliderArray = Physics.OverlapBox(transform.position, Vector3.one * 0.5f);

            foreach (Collider collider in colliderArray)
            {
                if (collider.TryGetComponent<Tile>(out Tile tile))
                {
                    List<Tile> tiles = tileSetter.GetTiles();
                    for (int t = 0; t < tileSetter.GetNumberOfTiles(); t++)
                    {
                        if (t == 0 && tiles[t].Equals(tile))
                        {
                            if (!tile.IsPressed())
                            {
                                Debug.Log("st +1f");
                                AddReward(+1f);
                                tile.PressedTile();
                            }
                        }
                        else if (t != 0 && tiles[t].Equals(tile) && tiles[t - 1].IsPressed())
                        {
                            if (!tile.IsPressed())
                            {
                                Debug.Log("nd +1f");
                                AddReward(+1f);
                                tile.PressedTile();
                            }
                        }
                        else if (t != 0 && tiles[t].Equals(tile) && !tiles[t - 1].IsPressed())
                        {
                            Debug.Log("-1f");
                            AddReward(-1f);
                        }
                    }
                }
            }
        }
        
        //AddReward(-1f / MaxStep);
        AddReward(-0.0002f);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<int> discreteActions = actionsOut.DiscreteActions;

        int horizontal = Mathf.RoundToInt(Input.GetAxisRaw("Horizontal"));
        int vertical   = Mathf.RoundToInt(Input.GetAxisRaw("Vertical"));
        discreteActions[0] = (horizontal == -1) ? 2 : horizontal;
        discreteActions[1] = (vertical == -1) ? 2 : vertical;

        discreteActions[2] = Input.GetKey(KeyCode.Space) ? 1 : 0;
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.TryGetComponent<Tile>(out _))
        {
            if (tileSetter.IsAllTilesPressed())
            {
                greenLightRender.material = winMaterial;
                redLightRender.material = whiteMaterial;

                EndEpisode();
            }
        }
        
    }
    private void OnCollisionEnter(Collision collision)
    {
        
        //if (collision.gameObject.TryGetComponent<Chicken>(out Chicken chicken))
        //{
        //    greenLightRender.material = winMaterial;
        //    redLightRender.material = whiteMaterial;

        //    AddReward(+1f);
        //    Destroy(chicken.gameObject);
        //    EndEpisode();
        //}

        //if(collision.gameObject.TryGetComponent<Wall>(out Wall wall))
        //{
        //    redLightRender.material = loseMaterial;
        //    greenLightRender.material = whiteMaterial;

        //    AddReward(-1f);
        //    EndEpisode();
        //}
    }
}
