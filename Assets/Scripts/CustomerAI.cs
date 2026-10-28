
using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

public class CustomerAI : MonoBehaviour
{
    [Header("이동 설정")]
    public float stopDistance = 1.5f;
    public float lookTime = 2f;

    [Header("구매 확률 설정")]
    [Range(0f, 1f)]
    public float baseBuyChance = 0.75f;

    public int referencePrice = 100;
    [Range(0.1f, 3f)]
    public float priceSensitivity = 1f;

    private NavMeshAgent agent;
    private Transform exitPoint;
    private bool isShopping = false;

    public void Initialize(Transform exit)
    {
        exitPoint = exit;
    }

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        if (agent == null)
        {
            Debug.LogError("손님에게 NavMeshAgent가 필요합니다.");
            enabled = false;
            return;
        }

        StartCoroutine(ShoppingRoutine());
    }

    IEnumerator ShoppingRoutine()
    {
        if (isShopping) yield break;
        isShopping = true;

        // 현재 진열된 상품 목록 수집
        List<DisplayedItem> items =
            new List<DisplayedItem>(FindObjectsOfType<DisplayedItem>());

        // 가격표가 붙은 상품 중 실제 진열된 상품만 방문
        items.RemoveAll(item =>
            item == null || !item.gameObject.activeInHierarchy);

        // 방문 순서를 무작위로 섞기
        for (int i = items.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            DisplayedItem temp = items[i];
            items[i] = items[j];
            items[j] = temp;
        }

        foreach (DisplayedItem item in items)
        {
            if (item == null) continue;

            // 상품 앞으로 이동
            agent.isStopped = false;
            agent.SetDestination(item.transform.position);

            while (item != null &&
                   agent.pathPending == false &&
                   agent.remainingDistance > stopDistance)
            {
                yield return null;
            }

            if (item == null) continue;

            // 상품 구경
            agent.isStopped = true;
            yield return new WaitForSeconds(lookTime);

            if (item == null) continue;

            // 가격이 낮을수록 구매 확률이 높음
            float priceRatio =
                (float)referencePrice / Mathf.Max(item.price, 1);

            float buyChance = baseBuyChance *
                Mathf.Pow(priceRatio, priceSensitivity);

            buyChance = Mathf.Clamp01(buyChance);

            if (Random.value < buyChance)
            {
                BuyItem(item);
            }

            agent.isStopped = false;
        }

        // 쇼핑이 끝나면 출구로 이동
        if (exitPoint != null)
        {
            agent.SetDestination(exitPoint.position);

            while (agent.pathPending ||
                   agent.remainingDistance > stopDistance)
            {
                yield return null;
            }
        }

        Destroy(gameObject);
    }

    void BuyItem(DisplayedItem item)
    {
        if (item == null) return;

        Debug.Log(
            "손님이 " + item.gameObject.name +
            " 상품을 " + item.price + "원에 구매했습니다!"
        );

        // 진열대 슬롯 반환
        if (item.displayZone != null && item.slotIndex >= 0)
        {
            item.displayZone.SetSlotOccupied(item.slotIndex, false);
        }

        if (PlayerWallet.Instance != null)
        {
            PlayerWallet.Instance.AddMoney(item.price);
        }
        // 상품과 가격표 제거
        Destroy(item.gameObject);
    }
}
