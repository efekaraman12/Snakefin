using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public Transform topWall;
    public Transform bottomWall;
    public Transform leftWall;
    public Transform rightWall;
    public RectTransform scorePanel;

    public SpriteRenderer background; // Arka plan için SpriteRenderer

    public Camera mainCamera;

    public float baseWallThickness = 0.5f;
    public float scorePanelTopMargin = 20f;

    private ScreenOrientation lastOrientation;

    private AudioSource audioSource; // Arka plan müziği için AudioSource
    public AudioClip backgroundMusic; // Atanacak müzik dosyası

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        // AudioSource bileşeni ekleniyor ve müzik başlatılıyor
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.loop = true;
        audioSource.playOnAwake = true;
        audioSource.volume = 0.5f; // Ses seviyesi ayarlanabilir
        if (backgroundMusic != null)
        {
            audioSource.clip = backgroundMusic;
            audioSource.Play();
        }
    }

    private void Start()
    {
        AdjustGameArea();
        lastOrientation = Screen.orientation;
    }

    private void Update()
    {
        if (Screen.orientation != lastOrientation)
        {
            AdjustGameArea();
            lastOrientation = Screen.orientation;
            Debug.Log("Ekran yönü değişti: " + Screen.orientation);
        }
    }

    public void InitializeGameScene()
    {
        mainCamera = Camera.main;

        GameObject[] walls = GameObject.FindGameObjectsWithTag("Wall");
        foreach (GameObject wall in walls)
        {
            if (wall.name.Contains("Top")) topWall = wall.transform;
            else if (wall.name.Contains("Bottom")) bottomWall = wall.transform;
            else if (wall.name.Contains("Left")) leftWall = wall.transform;
            else if (wall.name.Contains("Right")) rightWall = wall.transform;
        }

        GameObject scoreObj = GameObject.FindGameObjectWithTag("ScorePanel");
        if (scoreObj != null)
        {
            scorePanel = scoreObj.GetComponent<RectTransform>();
        }

        GameObject backgroundObj = GameObject.FindGameObjectWithTag("Background");
        if (backgroundObj != null)
        {
            background = backgroundObj.GetComponent<SpriteRenderer>();
        }
    }

    public void AdjustGameArea()
    {
        if (mainCamera == null) mainCamera = Camera.main;

        float vertExtent = mainCamera.orthographicSize;
        float horzExtent = vertExtent * Screen.width / Screen.height;

        AdjustWalls(horzExtent, vertExtent);
        AdjustUI();
        AdjustBackground(horzExtent, vertExtent); // Arka planı güncelle
    }

    private void AdjustWalls(float horzExtent, float vertExtent)
    {
        Vector3 horizontalScale = new Vector3(horzExtent * 2, baseWallThickness, 1);
        Vector3 verticalScale = new Vector3(baseWallThickness, vertExtent * 2, 1);

        if (topWall != null)
        {
            topWall.position = new Vector3(0, vertExtent, 0);
            topWall.localScale = horizontalScale;
        }

        if (bottomWall != null)
        {
            bottomWall.position = new Vector3(0, -vertExtent, 0);
            bottomWall.localScale = horizontalScale;
        }

        if (leftWall != null)
        {
            leftWall.position = new Vector3(-horzExtent, 0, 0);
            leftWall.localScale = verticalScale;
        }

        if (rightWall != null)
        {
            rightWall.position = new Vector3(horzExtent, 0, 0);
            rightWall.localScale = verticalScale;
        }
    }

    private void AdjustUI()
    {
        if (scorePanel == null) return;

        var safeArea = Screen.safeArea;
        float topMargin = (Screen.height - safeArea.yMax) + scorePanelTopMargin;

        scorePanel.anchorMin = new Vector2(0.5f, 1f);
        scorePanel.anchorMax = new Vector2(0.5f, 1f);
        scorePanel.pivot = new Vector2(0.5f, 1f);
        scorePanel.anchoredPosition = new Vector2(0, -topMargin);
    }

    private void AdjustBackground(float horzExtent, float vertExtent)
    {
        if (background == null) return;

        // Arka planı ekran boyutuna göre ölçekle
        float bgScale = Mathf.Max(
            (vertExtent * 2) / background.sprite.bounds.size.y,
            (horzExtent * 2) / background.sprite.bounds.size.x
        );

        background.transform.localScale = new Vector3(bgScale, bgScale, 1);
        background.transform.position = Vector3.zero; // Ortalanmış pozisyon
    }
}
