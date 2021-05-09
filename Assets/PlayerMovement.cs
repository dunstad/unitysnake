#nullable enable
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;
using UnityEngine.Tilemaps;

public class PlayerMovement : MonoBehaviour
{
    Vector2Int direction;
    public Rigidbody2D rb;
    public Tilemap collidable;
    Queue<Vector2Int> inputs;
    Vector2Int newInput;
    Vector2Int lastInput;

    public List<GameObject> tail;
    public GameObject tailPrefab;

    Camera cam;

    // TODO: tail collision
    // ideas: one extra turn to react, ghost tail?

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        direction.x = 0;
        direction.y = 0;
        lastInput = direction;
        inputs = new Queue<Vector2Int>();
        InvokeRepeating("MoveSnake", .5f, .5f);
    }

    // Update is called once per frame
    void Update()
    {
        // input
        // initialize to -lastInput so it's ignored by default
        newInput = -lastInput;

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
        // tail movement
        // stop iterating one before the end, last piece moves to head position
        for (var i = tail.Count - 1; i >= 1; i--)
        {
            tail[i].transform.SetPositionAndRotation(tail[i-1].transform.position, tail[i-1].transform.rotation);
        }
        tail[0].transform.SetPositionAndRotation(transform.position, transform.rotation);

        // head movement
        if (inputs.Count != 0)
        {
            Debug.Log(inputs.Count);
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
            rb.MovePosition(rb.position + direction);
        }
        else
        {
            Die();
        }
    }

    public void LengthenTail()
    {
        Debug.Log("tail +1");
        GameObject newTail = Instantiate(tailPrefab);
        GameObject tailEnd;
        if (tail.Count > 0) 
        {
            tailEnd = tail[tail.Count - 1];
        }
        else
        {
            tailEnd = gameObject;
        }
        newTail.transform.SetPositionAndRotation(tailEnd.transform.position, tailEnd.transform.rotation);
        tail.Add(newTail);
    }

    void Die()
    {
            Debug.Log("u ded");
            Destroy(gameObject);
            // TODO: stop calling MoveSnake after this
    }
}

