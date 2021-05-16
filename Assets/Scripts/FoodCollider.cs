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
    public AudioSource sound;

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
        transform.SetPositionAndRotation(new Vector3(newLocation.x + .5f, newLocation.y + .5f, -1), transform.rotation);
        if (col.GetComponent<PlayerMovement>())
        {
            col.GetComponent<PlayerMovement>().LengthenTail();
            sound.time = 0.1f;
            sound.Play();
        }
    }
}
