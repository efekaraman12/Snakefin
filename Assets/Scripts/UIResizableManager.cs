using UnityEngine;
using UnityEngine.UI;

public class UIResizableManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Image backgroundImage;
    [SerializeField] private GameObject buttonsContainer;

    private RectTransform containerRect;
    private ScreenOrientation lastOrientation;

    private void Awake()
    {
        if (buttonsContainer != null)
        {
            containerRect = buttonsContainer.GetComponent<RectTransform>();
        }
    }

    private void Start()
    {
        AdjustUI();
        lastOrientation = Screen.orientation;
    }

    private void Update()
    {
        // Eðer ekran yönü deðiþmiþse
        if (Screen.orientation != lastOrientation)
        {
            Debug.Log("Ekran yönü deðiþti: " + Screen.orientation);
            AdjustUI();
            lastOrientation = Screen.orientation;
        }
    }

    private void AdjustUI()
    {
        if (!containerRect) return;

        // Container'ý ekranýn tam ortasýna hizala
        containerRect.anchorMin = new Vector2(0.5f, 0.5f);
        containerRect.anchorMax = new Vector2(0.5f, 0.5f);
        containerRect.pivot = new Vector2(0.5f, 0.5f);
        containerRect.anchoredPosition = Vector2.zero; // Ortalanmýþ pozisyon

        // Arka planý ekran boyutuna uygun hale getir
        if (backgroundImage != null)
        {
            RectTransform bgRect = backgroundImage.GetComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.sizeDelta = Vector2.zero;
            bgRect.anchoredPosition = Vector2.zero; // Tam ekran
        }
    }
}
