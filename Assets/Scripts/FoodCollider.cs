#nullable enable
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

// TODO: rebuild locations every move
public class FoodCollider : MonoBehaviour
{
    public Tilemap? walkable;
    List<Vector2> locations;
    public AudioSource sound;
    public ParticleSystem particles;
    IEnumerator animate;
    bool hasCollided;
    float originalScale;
    public GameObject explosionPrefab;
    bool explosions;

    void Start()
    {
        explosions = PlayerPrefs.GetInt("explosions", 1) == 0 ? false : true;
        originalScale = transform.localScale.x;
        hasCollided = false;
        if (walkable != null)
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
        animate = Animate();
        StartCoroutine(animate);
        StartCoroutine(Appear());
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        // probably needed because we're cloning this gameObject
        if (!hasCollided)
        {
            hasCollided = true;
            particles.gameObject.transform.position = transform.position;
            var newLocation = locations[Random.Range(0, locations.Count - 1)];
            var newPos = new Vector3(newLocation.x + .5f, newLocation.y + .5f, -1);
            transform.localScale = new Vector3(originalScale, originalScale, originalScale);
            Instantiate(gameObject, newPos, transform.rotation);
            var playerMovement = col.GetComponent<PlayerMovement>();
            if (playerMovement)
            {
                playerMovement.LengthenTail();
                sound.time = 0.1f;
                sound.Play();
                particles.Play();
                StartCoroutine(Die(playerMovement.tickSeconds));
            }
            // for when the food spawns on top of the tail
            else
            {
                Destroy(gameObject);
            }
        }
    }

    IEnumerator Animate()
    {
        while (true) {
            transform.Rotate(new Vector3(0, 0, Mathf.Sin(Time.time) / 2));
            var newScale = originalScale * ((Mathf.Sin(Time.time) / 2) + 1.25f);
            transform.localScale = new Vector3(newScale, newScale, newScale);
            yield return null;
        }
    }

    IEnumerator Appear()
    {
        var alpha = 0f;
        while (alpha < 1f) {
            alpha += .01f;
            var newColor = new Color(1, 1, 1, alpha);
            gameObject.GetComponent<Renderer>().material.color = newColor;
            transform.GetChild(0).GetComponent<Renderer>().material.color = newColor;
            yield return null;
        }
    }

    IEnumerator Die(float tickSeconds)
    {
        if (explosions)
        {
            var explosion = Instantiate(explosionPrefab, transform.position, transform.rotation);
            StartCoroutine(explosion.GetComponent<Explosion>().Explode(tickSeconds));
        }
        gameObject.GetComponent<BoxCollider2D>().enabled = false;
        // so the new star's light doesn't overwrite the current one
        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - .1f);
        StopCoroutine(animate);
        var alpha = 1f;
        // thanks to Animate, current scale varies
        // this shrinks from any scale in the same amount of time
        var scale = transform.localScale.x;
        var scaleStep = scale / 50;
        while (scale > 0) {
            if (alpha > 0)
            {
                alpha -= .02f;
                var newColor = new Color(1, 1, 1, alpha);
                transform.GetChild(0).GetComponent<Renderer>().material.color = newColor;
            }
            
            scale -= scaleStep;
            transform.localScale = new Vector3(scale, scale, scale);
            yield return null;
        }
        Destroy(gameObject);
    }
}
