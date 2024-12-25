using UnityEngine;

public class Food : MonoBehaviour
{
    public float spawnAreaSize = 10f; // Yiyeceğin spawnlanabileceği alanın boyutu
    public float rotationSpeed = 100f; // Z ekseninde dönüş hızı

    private void Start()
    {
        // Başlangıçta rastgele bir pozisyonda spawnlan
        SpawnAtRandomPosition();
    }

    private void Update()
    {
        // Z ekseninde döndür
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Eğer çarpan obje "SnakeHead" tag'ine sahipse
        if (other.CompareTag("SnakeHead"))
        {
            // Skoru artır
            if (ScoreManager.Instance != null)
            {
                ScoreManager.Instance.AddScore(1);
            }

            // Yeni bir pozisyonda spawnlan
            SpawnAtRandomPosition();
        }
    }

    private void SpawnAtRandomPosition()
    {
        // Rastgele bir pozisyon belirle
        float randomX = Random.Range(-spawnAreaSize, spawnAreaSize);
        float randomY = Random.Range(-spawnAreaSize, spawnAreaSize);

        // Pozisyonu uygula
        transform.position = new Vector2(Mathf.Round(randomX), Mathf.Round(randomY));
    }
}
