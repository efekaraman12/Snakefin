﻿using UnityEngine;
using System.Collections.Generic;

public class Snake : MonoBehaviour
{
    public Transform segmentPrefab;
    public Vector2Int direction = Vector2Int.right;
    public float speed = 20f;
    public float speedMultiplier = 1f;
    public int initialSize = 4;
    public bool moveThroughWalls = false;
    public AudioSource audioSource;
    public AudioClip collectSound;
    public AudioClip moveSound;
    public SpriteRenderer backgroundSprite;


    private readonly List<Transform> segments = new List<Transform>();
    private Vector2Int input;
    private float nextUpdate;
    private Vector2 touchStartPosition;
    private bool isDragging = false;
    private float minSwipeDistance = 50f;
    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
        ResetState();
        ResizeBackground();
    }

    private void ResizeBackground()
    {
        if (backgroundSprite == null || mainCamera == null) return;

        float height = 2f * mainCamera.orthographicSize;
        float width = height * mainCamera.aspect;

        float spriteWidth = backgroundSprite.sprite.bounds.size.x;
        float spriteHeight = backgroundSprite.sprite.bounds.size.y;

        float scaleX = width / spriteWidth;
        float scaleY = height / spriteHeight;

        backgroundSprite.transform.localScale = new Vector3(scaleX, scaleY, 1f);
    }

    private void OnRectTransformDimensionsChange()
    {
        ResizeBackground();
    }

    private void OnValidate()
    {
        if (backgroundSprite != null && Camera.main != null)
        {
            mainCamera = Camera.main;
            ResizeBackground();
        }
    }

    private void Update()
    {
        HandleInput();
    }

    private void HandleInput()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    touchStartPosition = touch.position;
                    isDragging = true;
                    break;

                case TouchPhase.Moved:
                    if (isDragging)
                    {
                        Vector2 swipeDelta = touch.position - touchStartPosition;

                        if (swipeDelta.magnitude >= minSwipeDistance)
                        {
                            UpdateDirectionFromSwipe(swipeDelta);
                            touchStartPosition = touch.position;
                        }
                    }
                    break;

                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    isDragging = false;
                    break;
            }
        }

#if UNITY_EDITOR || UNITY_STANDALONE
        if (Input.GetMouseButtonDown(0))
        {
            touchStartPosition = Input.mousePosition;
            isDragging = true;
        }
        else if (Input.GetMouseButton(0) && isDragging)
        {
            Vector2 swipeDelta = (Vector2)Input.mousePosition - touchStartPosition;

            if (swipeDelta.magnitude >= minSwipeDistance)
            {
                UpdateDirectionFromSwipe(swipeDelta);
                touchStartPosition = Input.mousePosition;
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }
#endif
    }

    private void UpdateDirectionFromSwipe(Vector2 swipeDelta)
    {
        if (Mathf.Abs(swipeDelta.x) > Mathf.Abs(swipeDelta.y))
        {
            if (swipeDelta.x > 0 && direction != Vector2Int.left)
            {
                input = Vector2Int.right;
            }
            else if (swipeDelta.x < 0 && direction != Vector2Int.right)
            {
                input = Vector2Int.left;
            }
        }
        else
        {
            if (swipeDelta.y > 0 && direction != Vector2Int.down)
            {
                input = Vector2Int.up;
            }
            else if (swipeDelta.y < 0 && direction != Vector2Int.up)
            {
                input = Vector2Int.down;
            }
        }
    }

    private void FixedUpdate()
    {
        if (Time.time < nextUpdate) return;

        if (input != Vector2Int.zero)
        {
            direction = input;
            input = Vector2Int.zero;

            if (audioSource != null && moveSound != null)
            {
                audioSource.PlayOneShot(moveSound);
            }
        }

        for (int i = segments.Count - 1; i > 0; i--)
        {
            segments[i].position = segments[i - 1].position;
        }

        int x = Mathf.RoundToInt(transform.position.x) + direction.x;
        int y = Mathf.RoundToInt(transform.position.y) + direction.y;
        transform.position = new Vector2(x, y);

        UpdateHeadRotation();
        CheckSelfCollision();

        nextUpdate = Time.time + (1f / (speed * speedMultiplier));
    }

    private void UpdateHeadRotation()
    {
        float targetAngle = 0f;

        if (direction == Vector2Int.up)
        {
            targetAngle = 90f;
        }
        else if (direction == Vector2Int.down)
        {
            targetAngle = -90f;
        }
        else if (direction == Vector2Int.left)
        {
            targetAngle = 180f;
        }
        else if (direction == Vector2Int.right)
        {
            targetAngle = 0f;
        }

        transform.rotation = Quaternion.Euler(0, 0, targetAngle);
    }

    private void CheckSelfCollision()
    {
        for (int i = 1; i < segments.Count; i++)
        {
            if (segments[i].position == transform.position)
            {
                Debug.Log("Yılan kendine çarptı!");
                ResetState();
                break;
            }
        }
    }

    public void Grow()
    {
        Transform segment = Instantiate(segmentPrefab);
        segment.position = segments[segments.Count - 1].position;
        segments.Add(segment);
    }

    public void ResetState()
    {
        direction = Vector2Int.right;
        transform.position = Vector3.zero;

        ScoreManager.Instance.AddScore(-ScoreManager.Instance.GetScore());

        for (int i = 1; i < segments.Count; i++)
        {
            Destroy(segments[i].gameObject);
        }

        segments.Clear();
        segments.Add(transform);

        for (int i = 0; i < initialSize - 1; i++)
        {
            Grow();
        }
    }

    public bool Occupies(int x, int y)
    {
        foreach (Transform segment in segments)
        {
            if (Mathf.RoundToInt(segment.position.x) == x &&
                Mathf.RoundToInt(segment.position.y) == y)
            {
                return true;
            }
        }
        return false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Food"))
        {
            Grow();
            if (audioSource != null && collectSound != null)
            {
                audioSource.PlayOneShot(collectSound);
            }
        }
        else if (other.gameObject.CompareTag("Obstacle"))
        {
            ResetState();
        }
        else if (other.gameObject.CompareTag("Wall"))
        {
            if (moveThroughWalls)
            {
                Traverse(other.transform);
            }
            else
            {
                ResetState();
            }
        }
    }

    private void Traverse(Transform wall)
    {
        Vector3 position = transform.position;

        if (direction.x != 0f)
        {
            position.x = Mathf.RoundToInt(-wall.position.x + direction.x);
        }
        else if (direction.y != 0f)
        {
            position.y = Mathf.RoundToInt(-wall.position.y + direction.y);
        }

        transform.position = position;
    }
}