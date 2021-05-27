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
    }
}
