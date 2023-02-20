using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChickenSpawner : MonoBehaviour
{
    public GameObject givenPrefab;
    bool hasPrefabSpawned;
    GameObject prefab;
    Vector3 movement;
    float speed;

    public GameObject GetPrefabGameObject()
    {
        return prefab;
    }

    public void SetPrefabSpawner(bool spawned)
    {
        hasPrefabSpawned = spawned;
    }

    public bool HasPrefabSpawned()
    {
        return hasPrefabSpawned;
    }

    public Vector3 GetPrefabPosition()
    {
        return prefab.transform.position;
    }

    // Start is called before the first frame update
    void Awake()
    {
        hasPrefabSpawned = false;
    }

    public void Spawn()
    {
        //food = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        //food.name = "Food";
        prefab = Instantiate(givenPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        prefab.name = "chicken";
        prefab.AddComponent<Chicken>();

        prefab.transform.parent = transform;

        prefab.transform.localPosition = new Vector3(0, 2, 0);
        //rb.velocity = new Vector3(Random.Range(-3, 3), 0, Random.Range(-3, 3));
        prefab.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);


        int x = Random.Range(0, 100) < 40 ? -1 : Random.Range(0, 100) > 60 ? 1 : 0;
        int y;
        if (x == 0)
        {
            y = Random.Range(0, 100) < 50 ? -1 : 1;
        }
        else
        {
            y = Random.Range(0, 100) < 40 ? -1 : Random.Range(0, 100) > 60 ? 1 : 0;
        }
        movement = new Vector3(x, 0, y);

        speed = 1.5f;

        SetPrefabSpawner(true);

    }

    public void Update()
    {
        if (hasPrefabSpawned)
        {
            prefab.transform.Translate(movement * speed * Time.deltaTime);
        }
    }
    
}
