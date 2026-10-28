
using UnityEngine;
using TMPro;

public class PlayerWallet : MonoBehaviour
{
    public static PlayerWallet Instance { get; private set; }
    [Header("지갑 설정")]
    public int startingMoney = 0;
    public int money;

    [Header("화면 표시")]
    public TextMeshProUGUI moneyText;

    void Awake()
    {
        Instance = this;
        money = startingMoney;
        UpdateMoneyUI();
    }

    public void AddMoney(int amount)
    {
        if (amount <= 0) return;

        money += amount;
        UpdateMoneyUI();

        Debug.Log("판매 수익: " + amount + "원 / 현재 지갑: " + money + "원");
    }

    void UpdateMoneyUI()
    {
        if (moneyText != null)
        {
            moneyText.text = "지갑: ₩" + money.ToString("N0");
        }
    }
}
