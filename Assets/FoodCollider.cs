#nullable enable
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

// TODO: rebuild locations every move
public class FoodCollider : MonoBehaviour
{
    public Tilemap tileMap;
    List<Vector2> locations;

    void Start()
    {
        locations = new List<Vector2>();
        for (int i = tileMap.origin.x; i < tileMap.origin.x + tileMap.size.x; i++)
        {
            for (int j = tileMap.origin.y; j < tileMap.origin.y + tileMap.size.y; j++)
            {
                Debug.Log("i: " + i + ", j: " + j);
                Sprite? sprite = tileMap.GetSprite(new Vector3Int (i, j, 0));
                if (!(sprite is null) && sprite.name == "colored_tilemap_15")
                {
                    Debug.Log(sprite.name);
                    locations.Add(new Vector2(i, j));
                }
            }
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        var newLocation = locations[Random.Range(0, locations.Count - 1)];
        transform.SetPositionAndRotation(new Vector3(newLocation.x + .5f, newLocation.y + .5f, 0), transform.rotation);
    }
}
