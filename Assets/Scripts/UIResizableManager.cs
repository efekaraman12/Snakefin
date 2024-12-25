using UnityEngine;
using UnityEngine.UI;

public class UIResizableManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Image backgroundImage;
    [SerializeField] private GameObject buttonsContainer;
    [SerializeField] private Button playButton;
    [SerializeField] private Button exitButton;

    [Header("Button Settings")]
    [SerializeField] private float buttonSpacing = 20f;

    [Header("Safe Area Settings")]
    [SerializeField] private float minButtonWidth = 160f;
    [SerializeField] private float maxButtonWidth = 300f;
    [SerializeField] private float minButtonHeight = 50f;
    [SerializeField] private float maxButtonHeight = 80f;

    [Header("Orientation Settings")]
    [SerializeField] private float portraitScale = 1f;
    [SerializeField] private float landscapeScale = 0.7f;

    private RectTransform containerRect;
    private CanvasScaler canvasScaler;

    private float lastScreenWidth;
    private float lastScreenHeight;

    private void Awake()
    {
        if (buttonsContainer != null)
        {
            containerRect = buttonsContainer.GetComponent<RectTransform>();
        }

        canvasScaler = GetComponentInParent<CanvasScaler>();
        if (canvasScaler == null)
        {
            Debug.LogWarning("Canvas Scaler bulunamadý!");
        }

        lastScreenWidth = Screen.width;
        lastScreenHeight = Screen.height;
    }

    private void Start()
    {
        SetupUI();
    }

    private void Update()
    {
        if (lastScreenWidth != Screen.width || lastScreenHeight != Screen.height)
        {
            lastScreenWidth = Screen.width;
            lastScreenHeight = Screen.height;
            SetupUI();
        }
    }

    private void SetupUI()
    {
        if (!containerRect) return;

        bool isPortrait = Screen.height > Screen.width;
        float orientationScale = isPortrait ? portraitScale : landscapeScale;

        float screenWidth = Screen.width;
        float screenHeight = Screen.height;
        float referenceWidth = canvasScaler ? canvasScaler.referenceResolution.x : 1920f;

        float buttonWidth = Mathf.Clamp(screenWidth * 0.25f * orientationScale, minButtonWidth, maxButtonWidth);
        float buttonHeight = Mathf.Clamp(screenHeight * 0.08f * orientationScale, minButtonHeight, maxButtonHeight);

        containerRect.anchorMin = new Vector2(0.5f, 0.5f);
        containerRect.anchorMax = new Vector2(0.5f, 0.5f);
        containerRect.pivot = new Vector2(0.5f, 0.5f);

        float yOffset = isPortrait ? 0f : screenHeight * 0.1f;
        containerRect.anchoredPosition = new Vector2(0, yOffset);

        if (playButton != null)
        {
            RectTransform playRect = playButton.GetComponent<RectTransform>();
            playRect.anchorMin = new Vector2(0.5f, 0.5f);
            playRect.anchorMax = new Vector2(0.5f, 0.5f);
            playRect.pivot = new Vector2(0.5f, 0.5f);
            playRect.sizeDelta = new Vector2(buttonWidth, buttonHeight);
            playRect.anchoredPosition = new Vector2(0, buttonHeight / 2 + buttonSpacing / 2);
        }

        if (exitButton != null)
        {
            RectTransform exitRect = exitButton.GetComponent<RectTransform>();
            exitRect.anchorMin = new Vector2(0.5f, 0.5f);
            exitRect.anchorMax = new Vector2(0.5f, 0.5f);
            exitRect.pivot = new Vector2(0.5f, 0.5f);
            exitRect.sizeDelta = new Vector2(buttonWidth, buttonHeight);
            exitRect.anchoredPosition = new Vector2(0, -buttonHeight / 2 - buttonSpacing / 2);
        }

        float totalHeight = (buttonHeight * 2) + buttonSpacing;
        containerRect.sizeDelta = new Vector2(buttonWidth, totalHeight);
    }

    private void OnValidate()
    {
        if (Application.isPlaying) return;
        if (buttonsContainer != null && containerRect == null)
        {
            containerRect = buttonsContainer.GetComponent<RectTransform>();
        }
        // Editor modunda SetupUI'ý çaðýrma
        if (!Application.isPlaying) return;
        SetupUI();
    }
}