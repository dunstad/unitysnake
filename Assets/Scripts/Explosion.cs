using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    float startingScale;
    Camera cam;

    // Start is called before the first frame update
    void Start()
    {
    }

    public IEnumerator Explode(float tickSeconds)
    {
        cam = Camera.main;
        startingScale = transform.localScale.x;
        var tickDifference = .2f - tickSeconds;
        cam.GetComponent<CameraShake>().Shake(.25f + tickDifference, .25f + tickDifference);
        var scale = startingScale;
        while (scale < 5 + (tickDifference * 40)) {
            scale *= 1.2f;
            transform.localScale = new Vector3(scale, scale, scale);
            yield return null;
        }
        Destroy(gameObject);
    }
}
