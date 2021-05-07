using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// TODO: collide with walls
// TODO: queue movement
public class PlayerMovement : MonoBehaviour
{
    public Vector2 direction;
    public Rigidbody2D rb;
    Camera cam;

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        direction.x = 0;
        direction.y = 0;
        InvokeRepeating("MoveSnake", .5f, .5f);
    }

    // Update is called once per frame
    void Update()
    {
        // input

        // handle keyboard/gamepad input
        float axisX = Input.GetAxisRaw("Horizontal");
        float axisY = Input.GetAxisRaw("Vertical");
        if ((axisX != 0) || (axisY != 0))
        {
            // up
            if (axisY >= 0 && (Math.Abs(axisY) > Math.Abs(axisX)))
            {
                direction.x = 0;
                direction.y = 1;
            }
            // down
            else if (axisY < 0 && (Math.Abs(axisY) > Math.Abs(axisX)))
            {
                direction.x = 0;
                direction.y = -1;
            }
            // right
            else if (axisX >= 0 && (Math.Abs(axisX) > Math.Abs(axisY)))
            {
                direction.x = 1;
                direction.y = 0;
            }
            // left
            else
            {
                direction.x = -1;
                direction.y = 0;
            }
        }

        // Handle screen touches.
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (Input.touchCount == 1)
            {

                if (touch.phase == TouchPhase.Began)
                {
                    Vector3 touchPosition = touch.position;
                    Vector3 touchWorldPos = cam.ScreenToWorldPoint(touchPosition);

                    int relativeX = (int) (touchWorldPos.x - rb.position.x);
                    int relativeY = (int) (touchWorldPos.y - rb.position.y);

                    // Debug.Log("rbX " + rb.position.x + " - rbY " + rb.position.y);

                    // up
                    if (relativeY >= 0 && (Math.Abs(relativeY) > Math.Abs(relativeX)))
                    {
                        direction.x = 0;
                        direction.y = 1;
                    }
                    // down
                    else if (relativeY < 0 && (Math.Abs(relativeY) > Math.Abs(relativeX)))
                    {
                        direction.x = 0;
                        direction.y = -1;
                    }
                    // right
                    else if (relativeX >= 0 && (Math.Abs(relativeX) > Math.Abs(relativeY)))
                    {
                        direction.x = 1;
                        direction.y = 0;
                    }
                    // left
                    else
                    {
                        direction.x = -1;
                        direction.y = 0;
                    }
                }
            }
        }
    }

    void FixedUpdate()
    {
        // movement
        // rb.MovePosition(rb.position + (direction * 0.1f));
    }

    void MoveSnake()
    {
        rb.MovePosition(rb.position + direction);
    }
}

