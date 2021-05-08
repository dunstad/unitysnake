#nullable enable
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;
using UnityEngine.Tilemaps;

// TODO: button press instead of getAxisRaw
public class PlayerMovement : MonoBehaviour
{
    Vector2Int direction;
    public Rigidbody2D rb;
    public Tilemap collidable;
    Queue<Vector2Int> inputs;
    Vector2Int lastInput;

    Camera cam;

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        direction.x = 0;
        direction.y = 0;
        inputs = new Queue<Vector2Int>();
        InvokeRepeating("MoveSnake", .5f, .5f);
    }

    // Update is called once per frame
    void Update()
    {
        // input

        // handle keyboard/gamepad input
        // up
        if (Keyboard.current.upArrowKey.wasPressedThisFrame)
        {
            if ((inputs.Count == 0) || (lastInput != Vector2Int.up))
            {
                lastInput = new Vector2Int(0, 1);
                inputs.Enqueue(new Vector2Int(0, 1));
            }
        }
        // down
        else if (Keyboard.current.downArrowKey.wasPressedThisFrame)
        {
            if ((inputs.Count == 0) || (lastInput != Vector2Int.down))
            {
                lastInput = new Vector2Int(0, -1);
                inputs.Enqueue(new Vector2Int(0, -1));
            }
        }
        // right
        else if (Keyboard.current.rightArrowKey.wasPressedThisFrame)
        {
            if ((inputs.Count == 0) || (lastInput != Vector2Int.right))
            {
                lastInput = new Vector2Int(1, 0);
                inputs.Enqueue(new Vector2Int(1, 0));
            }
        }
        // left
        else if (Keyboard.current.leftArrowKey.wasPressedThisFrame)
        {
            if ((inputs.Count == 0) || (lastInput != Vector2Int.left))
            {
                lastInput = new Vector2Int(-1, 0);
                inputs.Enqueue(new Vector2Int(-1, 0));
            }
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
                if ((inputs.Count == 0) || (lastInput != Vector2Int.up))
                {
                    lastInput = new Vector2Int(0, 1);
                    inputs.Enqueue(new Vector2Int(0, 1));
                }
            }
            // down
            else if (relativeY < 0 && (Math.Abs(relativeY) > Math.Abs(relativeX)))
            {
                if ((inputs.Count == 0) || (lastInput != Vector2Int.down))
                {
                    lastInput = new Vector2Int(0, -1);
                    inputs.Enqueue(new Vector2Int(0, -1));
                }
            }
            // right
            else if (relativeX >= 0 && (Math.Abs(relativeX) > Math.Abs(relativeY)))
            {
                if ((inputs.Count == 0) || (lastInput != Vector2Int.right))
                {
                    lastInput = new Vector2Int(1, 0);
                    inputs.Enqueue(new Vector2Int(1, 0));
                }
            }
            // left
            else
            {
                if ((inputs.Count == 0) || (lastInput != Vector2Int.left))
                {
                    lastInput = new Vector2Int(-1, 0);
                    inputs.Enqueue(new Vector2Int(-1, 0));
                }
            }
        }
    }

    void MoveSnake()
    {
        if (inputs.Count != 0)
        {
            Debug.Log(inputs.Count);
            direction = inputs.Dequeue();
        }
        var nextPos = new Vector3Int();
        nextPos.x = (int)rb.position.x + direction.x;
        nextPos.y = (int)rb.position.y + direction.y;
        nextPos.z = 0;
        Sprite? sprite = collidable.GetSprite(nextPos);
        if (sprite is null)
        {
            rb.MovePosition(rb.position + direction);
        }
        else
        {
            Debug.Log("u ded");
            Destroy(gameObject);
        }
    }
}

