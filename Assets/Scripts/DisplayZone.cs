
using UnityEngine;

public class DisplayZone : MonoBehaviour
{
    [Header("배치 기준점")]
    public Transform placementPoint;

    [Header("진열 칸 설정")]
    [Min(1)] public int columns = 4;
    [Min(1)] public int rows = 2;

    [Tooltip("좌우 칸 사이 간격")]
    public float spacingX = 0.5f;

    [Tooltip("앞뒤 칸 사이 간격")]
    public float spacingZ = 0.5f;

    private bool[] occupiedSlots;

    void Awake()
    {
        InitializeSlots();
    }

    void OnValidate()
    {
        columns = Mathf.Max(1, columns);
        rows = Mathf.Max(1, rows);
        spacingX = Mathf.Max(0.01f, spacingX);
        spacingZ = Mathf.Max(0.01f, spacingZ);
    }

    void InitializeSlots()
    {
        int slotCount = columns * rows;

        if (occupiedSlots == null ||
            occupiedSlots.Length != slotCount)
        {
            occupiedSlots = new bool[slotCount];
        }
    }

    public bool TryGetFreeSlot(
        out int slotIndex,
        out Vector3 position,
        out Quaternion rotation)
    {
        InitializeSlots();

        slotIndex = -1;
        position = Vector3.zero;
        rotation = Quaternion.identity;

        if (placementPoint == null)
            return false;

        for (int i = 0; i < occupiedSlots.Length; i++)
        {
            if (occupiedSlots[i])
                continue;

            int col = i % columns;
            int row = i / columns;

            position = placementPoint.position
                + placementPoint.right * (col * spacingX)
                + placementPoint.forward * (row * spacingZ);

            rotation = placementPoint.rotation;
            slotIndex = i;

            return true;
        }

        return false;
    }

    public void SetSlotOccupied(int slotIndex, bool occupied)
    {
        InitializeSlots();

        if (slotIndex >= 0 &&
            slotIndex < occupiedSlots.Length)
        {
            occupiedSlots[slotIndex] = occupied;
        }
    }

    public Vector3 GetPlacementPosition()
    {
        return placementPoint != null
            ? placementPoint.position
            : transform.position;
    }

    public Quaternion GetPlacementRotation()
    {
        return placementPoint != null
            ? placementPoint.rotation
            : transform.rotation;
    }
}
