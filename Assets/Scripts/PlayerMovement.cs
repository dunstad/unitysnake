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

    List<GameObject> snake;
    public GameObject tailPrefab;

    Vector3 lastTailPos;

    Camera cam;

    float tickSeconds;
    float timeSinceTick;

    bool moving;
    IEnumerator coroutine;
    Vector2[] moveStartPositions;

    public AudioSource deathSound;
    public AudioSource inputSound;
    public ParticleSystem dustParticles;
    int lastTouch;

    // TODO: ignore touches that have ended
    // TODO: start with a short tail
    // TODO: screen shake
    // TODO: pause button for touch (two finger tap to pause?)
    // TODO: add game over overlay
    // TODO: score display
    // TODO: fix music loop

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        direction.x = 0;
        direction.y = 0;
        lastInput = direction;
        lastTailPos = transform.position + new Vector3(0, -1, 0);
        snake = new List<GameObject>();
        snake.Add(gameObject);
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
        if (Touchscreen.current.primaryTouch.startTime.ReadValue() >= Time.time)
        {
            if (Touchscreen.current.primaryTouch.touchId.ReadValue() != lastTouch)
            {
                lastTouch = Touchscreen.current.primaryTouch.touchId.ReadValue();
                var touchPosition = Touchscreen.current.position.ReadValue();
                Vector3 touchWorldPos = cam.ScreenToWorldPoint(touchPosition);

                int relativeX = (int) (touchWorldPos.x - rb.position.x);
                int relativeY = (int) (touchWorldPos.y - rb.position.y);

                // up
                if ((lastInput.y == 0) && (relativeY >= 0))
                {
                    newInput = new Vector2Int(0, 1);
                }
                // down
                else if ((lastInput.y == 0) && (relativeY < 0))
                {
                    newInput = new Vector2Int(0, -1);
                }
                // right
                else if ((lastInput.x == 0) && (relativeX >= 0))
                {
                    newInput = new Vector2Int(1, 0);
                }
                // left
                else
                {
                    newInput = new Vector2Int(-1, 0);
                }
            }
        }
        
        // no need to queue moves in the same direction
        // can't go back into your tail
        if ((newInput != lastInput) && (newInput != -lastInput))
        {
            lastInput = newInput;
            inputs.Enqueue(newInput);
            inputSound.Play();
        }
    }

    void MoveSnake()
    {
        // head movement
        if (inputs.Count != 0)
        {
            direction = inputs.Dequeue();
            var particlePos = transform.position;
            particlePos.z += .005f;
            dustParticles.gameObject.transform.position = particlePos;
            int dustRotation;
            if (direction.y != 0)
            {
                dustRotation = direction.y * 90;
            } else
            {
                dustRotation = direction.x * 90 + 90;
            }
            dustParticles.transform.eulerAngles = new Vector3(dustRotation, 90, 90);
            dustParticles.Play();
        }
        var nextPos = new Vector3Int();
        nextPos.x = (int) Math.Floor(rb.position.x) + direction.x;
        nextPos.y = (int) Math.Floor(rb.position.y) + direction.y;
        nextPos.z = 0;
        
        // wall collision and death
        Sprite? sprite = collidable.GetSprite(nextPos);
        if (sprite is null)
        {
            if (moving)
            {
                StopCoroutine(coroutine);
                MovementCleanup(moveStartPositions);
            }

            // first is future head position, last is previous tail position
            moveStartPositions = new Vector2[snake.Count + 2];
            moveStartPositions[0] = rb.position + (Vector2) direction;
            for (var i = 0; i < snake.Count; i++)
            {
                moveStartPositions[i + 1] = snake[i].GetComponent<Rigidbody2D>().position;
            }
            moveStartPositions[moveStartPositions.Length - 1] = lastTailPos;
            lastTailPos = moveStartPositions[moveStartPositions.Length - 2];

            coroutine = Movement(moveStartPositions);
            timeSinceTick = 0f;
            StartCoroutine(coroutine);
        }
        else
        {
            Die();
        }
    }

    public void LengthenTail()
    {
        lastTailPos = (Vector3) lastTailPos;
        lastTailPos.z += .01f;
        GameObject newTail = Instantiate(tailPrefab, lastTailPos, transform.rotation);
        // this prevents our tail colliding with the head during normal movement
        if (snake.Count == 1)
        {
            newTail.GetComponent<BoxCollider2D>().enabled = false;
        }
        snake.Add(newTail);
        CancelInvoke();
        tickSeconds *= 0.95f;
        // don't actually know why tickSeconds / 2 is right
        // it stops the pausing on food pickup though
        InvokeRepeating("MoveSnake", tickSeconds / 2, tickSeconds);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("tail"))
        {
            Die();
        }
    }

    void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void Die()
    {
        Debug.Log("u ded");
        deathSound.Play();
        CancelInvoke();
        Invoke("Restart", 1);
    }
    IEnumerator Movement(Vector2[] startPositions)
    {
        moving = true;
        float sqrRemainingDistance = (rb.position - startPositions[0]).sqrMagnitude;

        while (sqrRemainingDistance > 0.01) {
            timeSinceTick += Time.deltaTime;
            var progress = timeSinceTick / tickSeconds;
            for (var i = 0; i < startPositions.Length - 2; i++)
            {
                var body = snake[i].GetComponent<Rigidbody2D>();
                var target = startPositions[i];
                Vector2 newPosition = Vector2.MoveTowards(startPositions[i + 1], target, progress);
                body.MovePosition(newPosition);

                body.MoveRotation(CalculateRotation(startPositions[i], startPositions[i + 1], startPositions[i + 2], progress));
            }
            sqrRemainingDistance = (rb.position - startPositions[0]).sqrMagnitude;
            yield return null;
        }
        MovementCleanup(startPositions);
    }

    void MovementCleanup(Vector2[] startPositions)
    {
        for (var i = 0; i < startPositions.Length - 2; i++)
        {
            var snakeRigidbody = snake[i].GetComponent<Rigidbody2D>();
            snakeRigidbody.position = startPositions[i];
            snakeRigidbody.MoveRotation(CalculateRotation(startPositions[i], startPositions[i + 1], startPositions[i + 2], 1));
        }
        moving = false;
    }

    float CalculateRotation(Vector2 frontPos, Vector2 midPos, Vector2 backPos, float progress)
    {
        float bodyRotation;
        var direction = frontPos - midPos;
        float oldRotation;
        var oldDirection = midPos - backPos;
        float result;

        // only when the game starts
        if (direction == new Vector2(0, 0))
        {
            result = 0;
        } else if (oldDirection != direction)
        {
            if (direction.x != 0)
            {
                bodyRotation = direction.x * -90;
            } else
            {
                bodyRotation = direction.y * -90 + 90;
            }
            
            if (oldDirection == Vector2.Perpendicular(direction))
            {
                oldRotation = bodyRotation + 90;
            } else
            {
                oldRotation = bodyRotation - 90;
            }
            result = Mathf.Lerp(oldRotation, bodyRotation, progress);
        } else
        {
            if (direction == Vector2.up)
            {
                result = 0;
            } else if (direction == Vector2.right)
            {
                result = -90;
            } else if (direction == Vector2.down)
            {
                result = 180;
            } else
            {
                result = 90;
            }
        }
        return result;
    }
}

