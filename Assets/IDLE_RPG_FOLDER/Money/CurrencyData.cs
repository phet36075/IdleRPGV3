using UnityEngine;

[CreateAssetMenu(fileName = "CurrencyData", menuName = "Game/Currency Data", order = 1)]
public class CurrencyData : ScriptableObject
{
    [SerializeField] private int initialMoney = 0;
    private int currentMoney;

    public int CurrentMoney => currentMoney;

    private void OnEnable()
    {
        // เรียกใช้เมื่อ ScriptableObject ถูกโหลด
        ResetToInitial();
    }

    public void ResetToInitial()
    {
        currentMoney = initialMoney;
    }

    public void AddMoney(int amount)
    {
        if (amount < 0)
        {
            Debug.LogWarning("Attempt to add negative amount of money. Use SpendMoney instead.");
            return;
        }
        currentMoney += amount;
        Debug.Log($"Added {amount} money. Current balance: {currentMoney}");
    }

    public bool SpendMoney(int amount)
    {
        if (amount < 0)
        {
            Debug.LogWarning("Attempt to spend negative amount of money. Use AddMoney instead.");
            return false;
        }

        if (currentMoney >= amount)
        {
            currentMoney -= amount;
            Debug.Log($"Spent {amount} money. Current balance: {currentMoney}");
            return true;
        }
        else
        {
            Debug.Log($"Not enough money to spend {amount}. Current balance: {currentMoney}");
            return false;
        }
    }
}

