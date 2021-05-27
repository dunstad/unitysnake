using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeBody : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Appear());
    }
    IEnumerator Appear()
    {
        var scale = 0f;
        while (scale < 1f) {
            scale += .02f;
            transform.localScale = new Vector3(scale, scale, scale);
            yield return null;
        }
        // StartCoroutine(Animate());
    }

    // in order for this to work, each tail piece would need an incrementing offset
    // also, interpolation would need to ignore the swaying
    // possible via localPosition if they had a parent?
    IEnumerator Animate()
    {
        while (true) {
            var change = (Mathf.Sin(Time.time) / 2f) / 1000f;
            gameObject.GetComponent<Rigidbody2D>().position += new Vector(change, 0);
            yield return null;
        }
    }
}
