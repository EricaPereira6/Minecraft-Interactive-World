using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Tile : MonoBehaviour
{
    private bool tilePressed;
    Material pressedMaterial;
    MeshRenderer tileMeshRenderer; 

    int tileNumber;
    GameObject goTile;
    Vector3 tilePosition;

    private void Awake()
    {
        goTile = gameObject;
        tileMeshRenderer = GetComponent<MeshRenderer>();
        tilePosition = goTile.transform.position;
        tileNumber = 0;

        tilePressed = false;
    }

    public void SetNumber(int number, Material pressedMaterial, Material nonPressedMaterial)
    {
        tileNumber = number;
        this.pressedMaterial = pressedMaterial;
        tileMeshRenderer.material = nonPressedMaterial;
    }

    public int GetTileNumber()
    {
        return tileNumber;
    }
    
    public Vector3 GetTilePosition()
    {
        return tilePosition;
    }

    public bool IsPressed()
    {
        return tilePressed;
    }

    public void PressedTile()
    {
        if (!tilePressed)
        {
            tilePressed = true;
            tileMeshRenderer.material = pressedMaterial;
        }
    }

    public void ResetTile()
    {
        tilePressed = false;
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
