using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrencyManager : MonoBehaviour
{
    private static CurrencyManager _instance;
    public static CurrencyManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<CurrencyManager>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("CurrencyManager");
                    _instance = go.AddComponent<CurrencyManager>();
                }
            }
            return _instance;
        }
    }

    [SerializeField] private CurrencyData currencyData;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    public int CurrentMoney => currencyData.CurrentMoney;

    public void AddMoney(int amount) => currencyData.AddMoney(amount);

    public bool SpendMoney(int amount) => currencyData.SpendMoney(amount);

    public void ResetMoney() => currencyData.ResetToInitial();
}
