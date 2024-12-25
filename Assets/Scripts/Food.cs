using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Food : MonoBehaviour
{
    public float rotationSpeed = 100f;
    private Snake snake;

    private void Awake()
    {
        snake = FindObjectOfType<Snake>();
    }

    private void Start()
    {
        RandomizePosition();
    }

    private void Update()
    {
        transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
    }

    public void RandomizePosition()
    {
        if (GameManager.Instance == null) return;

        Vector2 playableArea = GameManager.Instance.GetPlayableArea();
        float wallThickness = GameManager.Instance.baseWallThickness;

        // Duvar kalınlığını hesaba katarak spawn alanını belirle
        float maxX = playableArea.x - wallThickness;
        float maxY = playableArea.y - wallThickness;

        // Tam sayı pozisyonlarda spawn ol
        int x = Mathf.RoundToInt(Random.Range(-maxX, maxX));
        int y = Mathf.RoundToInt(Random.Range(-maxY, maxY));

        // Yılanın üzerinde spawn olmadığından emin ol
        while (snake != null && snake.Occupies(x, y))
        {
            x = Mathf.RoundToInt(Random.Range(-maxX, maxX));
            y = Mathf.RoundToInt(Random.Range(-maxY, maxY));
        }

        transform.position = new Vector2(x, y);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("SnakeHead"))
        {
            RandomizePosition();
            if (ScoreManager.Instance != null)
            {
                ScoreManager.Instance.AddScore(1);
            }
        }
    }
}