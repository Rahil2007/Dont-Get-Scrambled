using TMPro;
using UnityEngine;

public class MoneyManager : MonoBehaviour
{
    [SerializeField] int startingMoney = 200;
    private int money;
    public int Money => money;
    [SerializeField] private UIHandler uiHandler;
    [SerializeField] private TextMeshProUGUI moneyText;

    void Awake()
    {
        moneyText.SetText($"${money}");
        money = startingMoney;
    }
    public void AddMoney(int amount)
    {
        money += amount;
        uiHandler.UpdateMoneyText();
    }

    //Bool helps tells Upgrade system if purchase is successful or not
    public bool SpendMoney(int amount)
    {
        if (money >= amount)
        {
            money -= amount;
            uiHandler.UpdateMoneyText();
            return true;
        }
        return false;
    }

    public void UpdateMoneyText()
    {
        moneyText.SetText($"${money}");
    }
}
