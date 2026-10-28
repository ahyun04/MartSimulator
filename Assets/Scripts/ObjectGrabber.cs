
using UnityEngine;

public class ObjectGrabber : MonoBehaviour
{
    private int heldSlotIndex = -1;
    [Header("카메라 설정")]
    public Camera playerCamera;
    public float grabDistance = 3f;
    public float holdDistance = 2f;

    [Header("UI 설정")]
    public bool showCrosshair = true;

    private Rigidbody heldObject;
    private Rigidbody targetObject;
    private DisplayZone targetZone;

    private bool originalUseGravity;
    private bool originalIsKinematic;
    [Header("가격 설정")]
    public DisplayPricePopup pricePopup;
    void Update()
    {
        if (playerCamera == null) return;

        FindTarget();

        // 아이템을 들고 있는 경우
        if (heldObject != null)
        {
            // 진열 구역을 바라보고 F를 누르면 배치
            if (targetZone != null &&
                Input.GetKeyDown(KeyCode.F))
            {
                PlaceObject();
            }

            // G를 누르면 버리기
            if (Input.GetKeyDown(KeyCode.G))
            {
                DropObject();
            }

            return;
        }

        // 아이템을 들고 있지 않을 때 F로 잡기
        if (targetObject != null &&
            Input.GetKeyDown(KeyCode.F))
        {
            GrabObject(targetObject);
        }
    }

    void FindTarget()
    {
        targetObject = null;
        targetZone = null;

        Ray ray = playerCamera.ViewportPointToRay(
            new Vector3(0.5f, 0.5f, 0f)
        );

        RaycastHit[] hits = Physics.RaycastAll(
            ray,
            grabDistance
        );

        // 가까운 오브젝트부터 확인
        System.Array.Sort(hits, (a, b) =>
            a.distance.CompareTo(b.distance));

        foreach (RaycastHit hit in hits)
        {
            // 들고 있는 아이템 자신은 무시
            if (heldObject != null &&
                hit.collider.attachedRigidbody == heldObject)
            {
                continue;
            }

            // 아이템을 들고 있다면 진열 구역 탐색
            if (heldObject != null)
            {
                DisplayZone zone =
                    hit.collider.GetComponentInParent<DisplayZone>();

                if (zone != null)
                {
                    targetZone = zone;
                    return;
                }

                continue;
            }

            // 아이템을 들고 있지 않다면 잡을 대상 탐색
            Rigidbody rb = hit.collider.attachedRigidbody;

            if (rb != null && !rb.isKinematic)
            {
                targetObject = rb;
                return;
            }
        }
    }

    void LateUpdate()
    {
        if (heldObject == null || playerCamera == null)
            return;

        Vector3 holdPosition =
            playerCamera.transform.position +
            playerCamera.transform.forward * holdDistance;

        heldObject.transform.position = holdPosition;
    }

    void GrabObject(Rigidbody rb)
    {
        heldObject = rb;

        originalUseGravity = rb.useGravity;
        originalIsKinematic = rb.isKinematic;

        rb.useGravity = false;
        rb.isKinematic = true;

        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }


    void PlaceObject()
    {
        if (heldObject == null || targetZone == null)
            return;

        Rigidbody rb = heldObject;
        DisplayZone zone = targetZone;

        // 빈 진열 칸 찾기
        if (!zone.TryGetFreeSlot(
            out int slotIndex,
            out Vector3 position,
            out Quaternion rotation))
        {
            Debug.Log("진열 공간이 가득 찼습니다.");
            return;
        }

        // 아이템을 해당 칸에 배치
        rb.transform.SetPositionAndRotation(position, rotation);

        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        rb.useGravity = false;
        rb.isKinematic = true;

        // 해당 칸을 사용 중으로 표시
        zone.SetSlotOccupied(slotIndex, true);

        // 진열 직후 가격 설정 팝업 열기
        if (pricePopup != null)
        {
            pricePopup.Open(rb.gameObject);
        }

        // 잡기 상태 해제
        heldObject = null;
        targetObject = null;
        targetZone = null;
        heldSlotIndex = -1;

        DisplayedItem displayedItem =
             rb.GetComponent<DisplayedItem>();

        if (displayedItem == null)
            displayedItem = rb.gameObject.AddComponent<DisplayedItem>();

        displayedItem.displayZone = zone;
        displayedItem.slotIndex = slotIndex;
    }


    void DropObject()
    {
        if (heldObject == null) return;

        Rigidbody rb = heldObject;

        rb.isKinematic = originalIsKinematic;
        rb.useGravity = originalUseGravity;

        heldObject = null;
        targetObject = null;
        targetZone = null;
    }

    void OnGUI()
    {
        float centerX = Screen.width / 2f;
        float centerY = Screen.height / 2f;

        GUIStyle style = new GUIStyle(GUI.skin.label);
        style.alignment = TextAnchor.MiddleCenter;
        style.fontSize = 20;
        style.normal.textColor = Color.white;

        if (showCrosshair)
        {
            GUI.Label(
                new Rect(centerX - 15, centerY - 15, 30, 30),
                "+",
                style
            );
        }

        string message = "";

        if (heldObject != null)
        {
            if (targetZone != null)
                message = "[F] 진열하기";
            else
                message = "[G] 버리기";
        }
        else if (targetObject != null)
        {
            message = "[F] 잡기";
        }

        if (!string.IsNullOrEmpty(message))
        {
            GUI.Label(
                new Rect(centerX - 100, centerY + 25, 200, 35),
                message,
                style
            );
        }
    }
}
