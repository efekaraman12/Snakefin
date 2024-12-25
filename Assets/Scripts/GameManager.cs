using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public Transform topWall;
    public Transform bottomWall;
    public Transform leftWall;
    public Transform rightWall;
    public RectTransform scorePanel;
    public Camera mainCamera;

    public float baseWallThickness = 0.5f;
    public float scorePanelTopMargin = 20f;

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
    }

    private void Start()
    {
        AdjustGameArea();
    }

    public void InitializeGameScene()
    {
        // Kamera referansýný al
        mainCamera = Camera.main;

        // Duvar referanslarýný bul
        GameObject[] walls = GameObject.FindGameObjectsWithTag("Wall");
        foreach (GameObject wall in walls)
        {
            if (wall.name.Contains("Top")) topWall = wall.transform;
            else if (wall.name.Contains("Bottom")) bottomWall = wall.transform;
            else if (wall.name.Contains("Left")) leftWall = wall.transform;
            else if (wall.name.Contains("Right")) rightWall = wall.transform;
        }

        // Skor panelini bul
        GameObject scoreObj = GameObject.FindGameObjectWithTag("ScorePanel");
        if (scoreObj != null)
        {
            scorePanel = scoreObj.GetComponent<RectTransform>();
        }
    }

    public void AdjustGameArea()
    {
        if (mainCamera == null) mainCamera = Camera.main;

        float vertExtent = mainCamera.orthographicSize;
        float horzExtent = vertExtent * Screen.width / Screen.height;

        AdjustWalls(horzExtent, vertExtent);
        AdjustUI();
    }

    private void AdjustWalls(float horzExtent, float vertExtent)
    {
        // Offset, duvarlarý biraz daha dýþarý taþýr
        float offset = 0.1f;

        Vector3 topPos = new Vector3(0, vertExtent + offset, 0);
        Vector3 bottomPos = new Vector3(0, -vertExtent - offset, 0);
        Vector3 leftPos = new Vector3(-horzExtent - offset, 0, 0);
        Vector3 rightPos = new Vector3(horzExtent + offset, 0, 0);

        Vector3 horizontalScale = new Vector3((horzExtent + offset) * 2, baseWallThickness, 1);
        Vector3 verticalScale = new Vector3(baseWallThickness, (vertExtent + offset) * 2, 1);

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

    public Vector2 GetPlayableArea()
    {
        float vertExtent = mainCamera.orthographicSize;
        float horzExtent = vertExtent * Screen.width / Screen.height;
        return new Vector2(horzExtent, vertExtent);
    }
}
