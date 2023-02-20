using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TileSetter : MonoBehaviour
{
    public List<Tile> tiles;

    public List<Material> nonPressed, pressed;


    Dictionary<int, Tile> tilesDict;
    List<Tile> sortedTiles;


    private void Start()
    {
        SetRandomTiles();
    }

    public void SetRandomTiles()
    {
        List<int> numbers = new List<int>();
        for(int i = 1; i <= 20; i++)
        {
            numbers.Add(i);
        }

        tilesDict = new Dictionary<int, Tile>();
        foreach (Tile tile in tiles)
        {
            int index = UnityEngine.Random.Range(0, numbers.Count);
            tilesDict.Add(numbers[index], tile);
            //Debug.Log("numbers[index] = " + numbers[index] + " , index = " + index + " , tile: " + tile);
            numbers.RemoveAt(index);
            //TestPrint(numbers);
        }
        int t = 0;
        sortedTiles = new List<Tile>();
        foreach (KeyValuePair<int, Tile> number in tilesDict.OrderBy(key => key.Key))
        {
            sortedTiles.Add(number.Value);
            sortedTiles[t].SetNumber(number.Key, pressed[number.Key - 1], nonPressed[number.Key - 1]);
            //Debug.LogWarning(number.Value.name + " - " + number.Key + " - pressed: " + pressed[number.Key - 1] + " , NON: " + nonPressed[number.Key - 1]);
            t++;
        }
        //TestPrintTiles(sortedTiles);
    }

    public List<Tile> GetTiles()
    {
        return sortedTiles;
    }

    public int GetNumberOfTiles()
    {
        return sortedTiles.Count;
    }

    public bool IsAllTilesPressed()
    {
        foreach(Tile tile in sortedTiles)
        {
            if (!tile.IsPressed())
            {
                return false;
            }
        }

        //OnAllTilesPressedEvent?.Invoke(this, EventArgs.Empty);
        return true;
    }

    public void ResetTiles()
    {
        foreach(Tile tile in sortedTiles)
        {
            tile.ResetTile();
        }
        SetRandomTiles();
    }

    void TestPrintTiles(List<Tile> numbers)
    {
        string str = "sorted tiles: { ";
        for (int n = 0; n < numbers.Count - 1; n++)
        {
            str += $"{numbers[n].GetTileNumber()} : {numbers[n]} , ";
        }
        str += $"{numbers[numbers.Count - 1].GetTileNumber()} : {numbers[numbers.Count - 1]} ]";
        Debug.Log(str);
    }

    void TestPrint(List<int> numbers)
    {
        string str = "numbers: [ ";
        for (int n = 0; n < numbers.Count - 1; n++)
        {
            str += numbers[n] + " , ";
        }
        str += numbers[numbers.Count - 1] + " ]";
        Debug.Log(str);
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
