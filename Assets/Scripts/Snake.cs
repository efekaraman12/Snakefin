using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Snake : MonoBehaviour
{
    public Transform segmentPrefab;
    public Vector2Int direction = Vector2Int.right;
    public float speed = 20f;
    public float speedMultiplier = 1f;
    public int initialSize = 4;
    public bool moveThroughWalls = false;

    private readonly List<Transform> segments = new List<Transform>();
    private Vector2Int input;
    private float nextUpdate;
    private Vector2 lastMousePosition;

    private void Start()
    {
        ResetState();
        lastMousePosition = Input.mousePosition;
    }

    private Vector2 swipeStart; // Tracks the start of a swipe

    private void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                // Capture the starting point of the swipe
                swipeStart = touch.position;
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                // Capture the end point of the swipe
                Vector2 swipeEnd = touch.position;
                Vector2 swipeDelta = swipeEnd - swipeStart;

                // Check the direction of the swipe
                if (Mathf.Abs(swipeDelta.x) > Mathf.Abs(swipeDelta.y))
                {
                    // Horizontal swipe
                    if (swipeDelta.x > 0 && direction != Vector2Int.left)
                    {
                        direction = Vector2Int.right;
                    }
                    else if (swipeDelta.x < 0 && direction != Vector2Int.right)
                    {
                        direction = Vector2Int.left;
                    }
                }
                else
                {
                    // Vertical swipe
                    if (swipeDelta.y > 0 && direction != Vector2Int.down)
                    {
                        direction = Vector2Int.up;
                    }
                    else if (swipeDelta.y < 0 && direction != Vector2Int.up)
                    {
                        direction = Vector2Int.down;
                    }
                }
            }
        }
    }



    private void FixedUpdate()
    {
        // Bekleme süresini kontrol et
        if (Time.time < nextUpdate)
        {
            return;
        }

        // Yeni yönü güncelle
        if (input != Vector2Int.zero)
        {
            direction = input;
        }

        // Segmentlerin pozisyonunu güncelle
        for (int i = segments.Count - 1; i > 0; i--)
        {
            segments[i].position = segments[i - 1].position;
        }

        // Yılanı hareket ettir
        int x = Mathf.RoundToInt(transform.position.x) + direction.x;
        int y = Mathf.RoundToInt(transform.position.y) + direction.y;
        transform.position = new Vector2(x, y);

        // Kafa dönüşünü ayarla
        UpdateHeadRotation();

        // Bir sonraki güncelleme süresini ayarla
        nextUpdate = Time.time + (1f / (speed * speedMultiplier));
    }

    private void UpdateHeadRotation()
    {
        if (direction == Vector2Int.up)
        {
            transform.rotation = Quaternion.Euler(0, 0, 90f);
        }
        else if (direction == Vector2Int.down)
        {
            transform.rotation = Quaternion.Euler(0, 0, -90f);
        }
        else if (direction == Vector2Int.left)
        {
            transform.rotation = Quaternion.Euler(0, 0, 180f);
        }
        else if (direction == Vector2Int.right)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0f);
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

        // Segmentleri temizle
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

