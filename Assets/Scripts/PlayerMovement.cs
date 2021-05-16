#nullable enable
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    Vector2Int direction;
    Vector2 moveTargetPos;
    public Rigidbody2D rb;
    public Tilemap collidable;
    Queue<Vector2Int> inputs;
    Vector2Int newInput;
    Vector2Int lastInput;

    public List<GameObject> tail;
    public GameObject tailPrefab;

    Vector3 lastTailPos;

    Camera cam;

    float tickSeconds;
    float timeSinceTick;

    bool moving;
    IEnumerator coroutine;
    Vector2[] moveStartPositions;

    // ideas: one extra turn to react, ghost tail?
    // TODO: improve touch
    // TODO: sounds

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        direction.x = 0;
        direction.y = 0;
        lastInput = direction;
        lastTailPos = transform.position;
        tail = new List<GameObject>();
        inputs = new Queue<Vector2Int>();
        tickSeconds = .25f;
        moving = false;
        moveStartPositions = new Vector2[1];
        moveStartPositions[0] = rb.position;
        InvokeRepeating("MoveSnake", .5f, tickSeconds);
    }

    // Update is called once per frame
    void Update()
    {
        // input
        // initialize to -lastInput so it's ignored by default
        newInput = -lastInput;

        // pause
        if (Keyboard.current.pKey.wasPressedThisFrame)
        {
            Time.timeScale = (Time.timeScale + 1) % 2;
        }

        // handle keyboard/gamepad input
        // TODO: ignore inputs that go back into the tail (direction + input = 0)
        // up
        if (Keyboard.current.upArrowKey.wasPressedThisFrame)
        {
            newInput = new Vector2Int(0, 1);
        }
        // down
        else if (Keyboard.current.downArrowKey.wasPressedThisFrame)
        {
            newInput = new Vector2Int(0, -1);
        }
        // right
        else if (Keyboard.current.rightArrowKey.wasPressedThisFrame)
        {
            newInput = new Vector2Int(1, 0);
        }
        // left
        else if (Keyboard.current.leftArrowKey.wasPressedThisFrame)
        {
            newInput = new Vector2Int(-1, 0);
        }

        // Handle screen touches.
        if (Touchscreen.current.primaryTouch.phase.ReadValue() == UnityEngine.InputSystem.TouchPhase.Began)
        {
            var touchPosition = Touchscreen.current.position.ReadValue();
            Vector3 touchWorldPos = cam.ScreenToWorldPoint(touchPosition);

            int relativeX = (int) (touchWorldPos.x - rb.position.x);
            int relativeY = (int) (touchWorldPos.y - rb.position.y);

            // up
            if (relativeY >= 0 && (Math.Abs(relativeY) > Math.Abs(relativeX)))
            {
                newInput = new Vector2Int(0, 1);
            }
            // down
            else if (relativeY < 0 && (Math.Abs(relativeY) > Math.Abs(relativeX)))
            {
                newInput = new Vector2Int(0, -1);
            }
            // right
            else if (relativeX >= 0 && (Math.Abs(relativeX) > Math.Abs(relativeY)))
            {
                newInput = new Vector2Int(1, 0);
            }
            // left
            else
            {
                newInput = new Vector2Int(-1, 0);
            }
        }
        
        // no need to queue moves in the same direction
        // can't go back into your tail
        if ((newInput != lastInput) && (newInput != -lastInput))
        {
            lastInput = newInput;
            inputs.Enqueue(newInput);
        }
    }

    void MoveSnake()
    {
        // head movement
        if (inputs.Count != 0)
        {
            direction = inputs.Dequeue();
        }
        var nextPos = new Vector3Int();
        nextPos.x = (int) Math.Floor(rb.position.x) + direction.x;
        nextPos.y = (int) Math.Floor(rb.position.y) + direction.y;
        nextPos.z = 0;
        
        // wall collision and death
        Sprite? sprite = collidable.GetSprite(nextPos);
        if (sprite is null)
        {
            // this move doesn't happen right away
            // rb.MovePosition(rb.position + direction);
            if (moving)
            {
                StopCoroutine(coroutine);
                rb.position = moveTargetPos;
                if (tail.Count > 0)
                {
                    tail[0].GetComponent<Rigidbody2D>().position = moveStartPositions[0];
                }
                for (var i = 1; i < tail.Count; i++)
                {
                    tail[i].GetComponent<Rigidbody2D>().position = moveStartPositions[i];
                }
                moving = false;
                Debug.Log("coroutine stopped!");
            }
            moveTargetPos = rb.position + (Vector2) direction;

            moveStartPositions = new Vector2[tail.Count + 1];
            moveStartPositions[0] = rb.position;
            for (var i = 1; i <= tail.Count; i++)
            {
                moveStartPositions[i] = tail[i - 1].GetComponent<Rigidbody2D>().position;
            }

            coroutine = Movement(moveStartPositions, moveTargetPos);
            timeSinceTick = 0f;
            StartCoroutine(coroutine);
        }
        else
        {
            Die();
        }

        // tail movement
        // stop iterating one before the end, last piece moves to head position
        if (tail.Count > 1)
        {
            for (var i = tail.Count - 1; i >= 1; i--)
            {
                // tail[i].transform.SetPositionAndRotation(tail[i-1].transform.position, tail[i-1].transform.rotation);
            }
        }
        if (tail.Count > 0)
        {
            // tail[0].transform.SetPositionAndRotation(transform.position, transform.rotation);
            lastTailPos = tail[tail.Count - 1].transform.position;
        }
        else 
        {
            lastTailPos = transform.position;
        }
    }

    public void LengthenTail()
    {
        lastTailPos = (Vector3) lastTailPos;
        lastTailPos.z += .01f;
        GameObject newTail = Instantiate(tailPrefab, lastTailPos, transform.rotation);
        // this prevents our tail colliding with the head during normal movement
        if (tail.Count == 0)
        {
            newTail.GetComponent<BoxCollider2D>().enabled = false;
        }
        tail.Add(newTail);
        CancelInvoke();
        tickSeconds *= 0.95f;
        InvokeRepeating("MoveSnake", tickSeconds, tickSeconds);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("tail"))
        {
            Die();
        }
    }

    void Die()
    {
            Debug.Log("u ded");
            Destroy(gameObject);
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    IEnumerator Movement(Vector2[] startPositions, Vector2 end)
    {
        Debug.Log("coroutine!");
        moving = true;
        float sqrRemainingDistance = (rb.position - end).sqrMagnitude;

        while (sqrRemainingDistance > 0.01) {
            timeSinceTick += Time.deltaTime;
            var progress = timeSinceTick / tickSeconds;
            for (var i = 0; i < startPositions.Length; i++)
            {
                Rigidbody2D body;
                Vector2 target;
                if (i == 0)
                {
                    body = rb;
                    target = end;
                }
                else 
                {
                    body = tail[i-1].GetComponent<Rigidbody2D>();
                    target = startPositions[i - 1];
                }
                Vector2 newPosition = Vector2.MoveTowards(startPositions[i], target, progress);
                body.MovePosition(newPosition);
            }
            sqrRemainingDistance = (rb.position - end).sqrMagnitude;
            yield return null;
        }

        rb.position = end;
        if (tail.Count > 0)
        {
        tail[0].GetComponent<Rigidbody2D>().position = startPositions[0];
        }
        for (var i = 1; i < tail.Count; i++)
        {
            tail[i].GetComponent<Rigidbody2D>().position = startPositions[i];
        }
        moving = false;
        Debug.Log("coroutine finished!");
    }
}

