
using UnityEngine;
using System.Collections;

public class CustomerSpawner : MonoBehaviour
{
    [Header("손님 설정")]
    public GameObject customerPrefab;
    public Transform spawnPoint;
    public Transform exitPoint;

    [Header("스폰 시간")]
    public float minSpawnInterval = 5f;
    public float maxSpawnInterval = 12f;

    [Header("손님 수 제한")]
    public int maxCustomers = 5;

    private int currentCustomers = 0;

    void Start()
    {
        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            float waitTime = Random.Range(
                minSpawnInterval,
                maxSpawnInterval
            );

            yield return new WaitForSeconds(waitTime);

            if (customerPrefab == null || spawnPoint == null)
                continue;

            if (currentCustomers >= maxCustomers)
                continue;

            GameObject customer = Instantiate(
                customerPrefab,
                spawnPoint.position,
                spawnPoint.rotation
            );

            currentCustomers++;

            CustomerAI ai = customer.GetComponent<CustomerAI>();

            if (ai != null)
                ai.Initialize(exitPoint);

            StartCoroutine(TrackCustomer(customer));
        }
    }

    IEnumerator TrackCustomer(GameObject customer)
    {
        while (customer != null)
            yield return null;

        currentCustomers--;
    }
}
