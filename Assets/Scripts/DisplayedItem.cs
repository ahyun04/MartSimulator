
using UnityEngine;
using TMPro;

public class DisplayedItem : MonoBehaviour
{
    [Header("판매 가격")]
    [Min(0)]
    public int price = 100;

    [Header("가격표 설정")]
    public float labelHeight = 0.3f;
    public float labelScale = 0.01f;

    private GameObject priceLabel;
    private TextMeshPro priceText;
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
        CreatePriceLabel();
        UpdatePriceLabel();
    }

    void LateUpdate()
    {
        if (priceLabel == null) return;

        // 아이템 위에 가격표 배치
        priceLabel.transform.position =
            transform.position + Vector3.up * labelHeight;

        // 카메라를 향하도록 회전
        if (mainCamera == null)
            mainCamera = Camera.main;

        if (mainCamera != null)
        {
            priceLabel.transform.rotation =
                Quaternion.LookRotation(
                    priceLabel.transform.position -
                    mainCamera.transform.position
                );
        }

        UpdatePriceLabel();
    }

    void CreatePriceLabel()
    {
        priceLabel = new GameObject("PriceLabel");
        priceLabel.transform.SetParent(transform, true);
        priceLabel.transform.localScale = Vector3.one * labelScale;

        priceText = priceLabel.AddComponent<TextMeshPro>();
        priceText.alignment = TextAlignmentOptions.Center;

        // 폰트 크기 45로 변경
        priceText.fontSize = 45;

        priceText.color = Color.black;
        priceText.outlineWidth = 0.2f;

        // 글씨 영역을 가로로 넓혀서 줄바꿈 방지
        priceText.rectTransform.sizeDelta = new Vector2(30f, 3f);

        // 한 줄로 표시
        priceText.enableWordWrapping = false;
    }

    void UpdatePriceLabel()
    {
        if (priceText != null)
        {
            priceText.text = "₩" + price.ToString("N0");
        }
    }

    void OnDestroy()
    {
        if (priceLabel != null)
        {
            Destroy(priceLabel);
        }
    }
}
