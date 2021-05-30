using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    float startingScale;

    // Start is called before the first frame update
    void Start()
    {
        startingScale = transform.localScale.x;
        StartCoroutine(Animate());
    }

    IEnumerator Animate()
    {
        var scale = startingScale;
        while (scale < 5) {
            scale *= 1.2f;
            transform.localScale = new Vector3(scale, scale, scale);
            yield return null;
        }
        Destroy(gameObject);
    }
}
