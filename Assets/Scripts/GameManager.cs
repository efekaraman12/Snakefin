using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game Area References")]
    public Transform topWall;
    public Transform bottomWall;
    public Transform leftWall;
    public Transform rightWall;
    public Camera mainCamera;
    public RectTransform scorePanel;

    [Header("Background References")]
    public SpriteRenderer gameBackground;
    public RectTransform uiBackground;
    public Canvas mainCanvas;

    [Header("Game Settings")]
    public float baseWallThickness = 0.5f;
    public float scorePanelTopMargin = 20f;
    public float minGameAreaWidth = 8f;
    public float minGameAreaHeight = 12f;

    private float lastScreenWidth;
    private float lastScreenHeight;
    private float playableWidth;
    private float playableHeight;
    private ScreenOrientation lastOrientation;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeGameScene();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        lastOrientation = Screen.orientation;
        AdjustGameArea();
    }

    private void Update()
    {
        if (Screen.width != lastScreenWidth ||
            Screen.height != lastScreenHeight ||
            Screen.orientation != lastOrientation)
        {
            AdjustGameArea();
            lastScreenWidth = Screen.width;
            lastScreenHeight = Screen.height;
            lastOrientation = Screen.orientation;
        }
    }

    public void AdjustGameArea()
    {
        if (mainCamera == null) return;

        // Ekran oran�n� hesapla
        float screenAspect = (float)Screen.width / Screen.height;
        bool isPortrait = Screen.height > Screen.width;

        // Kamera boyutunu ayarla
        float targetSize;
        if (isPortrait)
        {
            targetSize = minGameAreaHeight / 2f;
        }
        else
        {
            targetSize = minGameAreaWidth / (2f * screenAspect);
        }

        mainCamera.orthographicSize = targetSize;

        // Oyun alan� s�n�rlar�n� hesapla
        float vertExtent = mainCamera.orthographicSize;
        float horzExtent = vertExtent * screenAspect;

        // Duvarlar� ayarla
        AdjustWalls(horzExtent, vertExtent, screenAspect, isPortrait);

        // UI'� ayarla
        AdjustUI(isPortrait);

        // Arka plan� ayarla
        AdjustBackground(horzExtent, vertExtent, screenAspect);

        // Oynanabilir alan� g�ncelle
        playableWidth = (horzExtent - baseWallThickness) * 2f;  // Duvar kal�nl���n� ��kar
        playableHeight = (vertExtent - baseWallThickness) * 2f; // Duvar kal�nl���n� ��kar
    }

    private void AdjustWalls(float horzExtent, float vertExtent, float screenAspect, bool isPortrait)
    {
        // Duvar kal�nl���n� ekran boyutuna g�re ayarla
        float wallThickness = baseWallThickness * (isPortrait ? 1f : screenAspect);

        // Duvar pozisyonlar�
        Vector3 topPos = new Vector3(0, vertExtent, 0);
        Vector3 bottomPos = new Vector3(0, -vertExtent, 0);
        Vector3 leftPos = new Vector3(-horzExtent, 0, 0);
        Vector3 rightPos = new Vector3(horzExtent, 0, 0);

        // Duvar �l�ekleri
        Vector3 horizontalScale = new Vector3(horzExtent * 2f, wallThickness, 1f);
        Vector3 verticalScale = new Vector3(wallThickness, vertExtent * 2f, 1f);

        // Duvarlar� ayarla
        if (topWall != null)
        {
            topWall.position = topPos;
            topWall.localScale = horizontalScale;
        }

        if (bottomWall != null)
        {
            bottomWall.position = bottomPos;
            bottomWall.localScale = horizontalScale;
        }

        if (leftWall != null)
        {
            leftWall.position = leftPos;
            leftWall.localScale = verticalScale;
        }

        if (rightWall != null)
        {
            rightWall.position = rightPos;
            rightWall.localScale = verticalScale;
        }
    }

    private void AdjustUI(bool isPortrait)
    {
        if (scorePanel != null)
        {
            // Score panel'i ekran�n �st�ne sabitle
            scorePanel.anchorMin = new Vector2(0.5f, 1f);
            scorePanel.anchorMax = new Vector2(0.5f, 1f);
            scorePanel.pivot = new Vector2(0.5f, 1f);

            // Margin'i ayarla
            float topMargin = scorePanelTopMargin * (isPortrait ? 1f : 1.5f);
            scorePanel.anchoredPosition = new Vector2(0, -topMargin);
        }
    }

    private void AdjustBackground(float horzExtent, float vertExtent, float screenAspect)
    {
        if (gameBackground != null)
        {
            // Arka plan �l�e�ini hesapla
            float bgScale = Mathf.Max(
                (vertExtent * 2.2f) / gameBackground.sprite.bounds.size.y,
                (horzExtent * 2.2f) / gameBackground.sprite.bounds.size.x
            ); // %10 fazla kaplama i�in 2.2f kullan�yoruz

            gameBackground.transform.localScale = new Vector3(bgScale, bgScale, 1f);
            gameBackground.transform.position = Vector3.zero;
            gameBackground.sortingOrder = -1; // Arka planda kalmas�n� sa�la
        }
    }

    public Vector2 GetPlayableArea()
    {
        return new Vector2(playableWidth / 2f, playableHeight / 2f);
    }

    public void InitializeGameScene()
    {
        mainCamera = Camera.main;
        if (mainCamera == null) return;

        // Referanslar� bul
        GameObject[] walls = GameObject.FindGameObjectsWithTag("Wall");
        foreach (GameObject wall in walls)
        {
            if (wall.name.Contains("Top")) topWall = wall.transform;
            else if (wall.name.Contains("Bottom")) bottomWall = wall.transform;
            else if (wall.name.Contains("Left")) leftWall = wall.transform;
            else if (wall.name.Contains("Right")) rightWall = wall.transform;
        }

        // UI elemanlar�n� bul
        GameObject scoreObj = GameObject.FindGameObjectWithTag("ScorePanel");
        if (scoreObj != null) scorePanel = scoreObj.GetComponent<RectTransform>();

        GameObject bgObj = GameObject.FindGameObjectWithTag("GameBackground");
        if (bgObj != null) gameBackground = bgObj.GetComponent<SpriteRenderer>();

        mainCanvas = FindObjectOfType<Canvas>();

        // �lk ayarlamalar� yap
        AdjustGameArea();
    }

    public void RestartGame()
    {
        AdjustGameArea();
    }

    public void PauseGame()
    {
        Time.timeScale = 0;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
    }
}