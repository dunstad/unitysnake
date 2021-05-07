#nullable enable
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

// TODO: rebuild locations every move
public class FoodCollider : MonoBehaviour
{
    public Tilemap walkable;
    List<Vector2> locations;

    void Start()
    {
        locations = new List<Vector2>();
        for (int i = walkable.origin.x; i < walkable.origin.x + walkable.size.x; i++)
        {
            for (int j = walkable.origin.y; j < walkable.origin.y + walkable.size.y; j++)
            {
                Sprite? sprite = walkable.GetSprite(new Vector3Int (i, j, 0));
                if (sprite != null)
                {
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
