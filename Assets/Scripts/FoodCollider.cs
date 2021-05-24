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
    public ParticleSystem particles;

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
        StartCoroutine(Animate());
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        particles.gameObject.transform.position = transform.position;
        var newLocation = locations[Random.Range(0, locations.Count - 1)];
        transform.SetPositionAndRotation(new Vector3(newLocation.x + .5f, newLocation.y + .5f, -1), transform.rotation);
        if (col.GetComponent<PlayerMovement>())
        {
            col.GetComponent<PlayerMovement>().LengthenTail();
            sound.time = 0.1f;
            sound.Play();
            particles.Play();
        }
    }

    IEnumerator Animate()
    {
        while (true) {
            transform.Rotate(new Vector3(0, 0, Mathf.Sin(Time.time) / 2));
            var newScale = 1 * ((Mathf.Sin(Time.time) / 2) + 1.25f);
            transform.localScale = new Vector3(newScale, newScale, newScale);
            yield return null;
        }
    }
}
