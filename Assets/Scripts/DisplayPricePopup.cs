
using UnityEngine;

public class DisplayPricePopup : MonoBehaviour
{
    public bool IsOpen { get; private set; }

    private GameObject currentItem;
    private DisplayedItem displayedItem;

    private string priceInput = "";
    private Rect windowRect = new Rect(0, 0, 320, 190);

    public void Open(GameObject item)
    {
        if (item == null) return;

        currentItem = item;
        displayedItem = item.GetComponent<DisplayedItem>();

        if (displayedItem == null)
            displayedItem = item.AddComponent<DisplayedItem>();

        priceInput = displayedItem.price.ToString();
        IsOpen = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void OnGUI()
    {
        if (!IsOpen) return;

        windowRect.x = (Screen.width - windowRect.width) / 2;
        windowRect.y = (Screen.height - windowRect.height) / 2;

        windowRect = GUI.Window(
            GetInstanceID(),
            windowRect,
            DrawWindow,
            "아이템 가격 설정"
        );
    }

    void DrawWindow(int windowID)
    {
        GUILayout.Space(10);
        GUILayout.Label(
            currentItem != null ? currentItem.name : "아이템"
        );

        GUILayout.Label("판매 가격을 입력하세요.");

        priceInput = GUILayout.TextField(priceInput);

        GUILayout.Space(10);

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("확인"))
        {
            if (int.TryParse(priceInput, out int price) &&
                price >= 0)
            {
                displayedItem.price = price;
                ClosePopup();
            }
            else
            {
                priceInput = "100";
            }
        }

        if (GUILayout.Button("취소"))
        {
            ClosePopup();
        }

        GUILayout.EndHorizontal();

        GUI.DragWindow(
            new Rect(0, 0, 10000, 25)
        );
    }

    void ClosePopup()
    {
        IsOpen = false;
        currentItem = null;
        displayedItem = null;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
