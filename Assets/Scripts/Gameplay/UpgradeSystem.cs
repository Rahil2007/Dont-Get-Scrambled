using TMPro;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeSystem : MonoBehaviour
{
    private GameObject player;
    private PlayerStats playerStats;
    private WeaponHandler weaponHandler;
    [SerializeField] private MoneyManager moneyManager;

    [SerializeField] private float healthUpgrade, fireRateUpgrade, damageUpgrade, invincibilityUpgrade;
    [SerializeField] private List<TextMeshProUGUI> costText;
    private List<float> costMultiplier = new() { 1.0f, 1.0f, 1.0f, 1.0f };

    [SerializeField] private Weapon Shotgun;
    [SerializeField] private Weapon AssaultRifle;
    [SerializeField] private GameObject weaponInventory;
    [Header("Shop UI")]
    [SerializeField] private GameObject shotgunSoldImg;
    [SerializeField] private UnityEngine.UI.Button shotgunButton;
    [SerializeField] private GameObject assaultRifleSoldImg;
    [SerializeField] private UnityEngine.UI.Button assaultRifleButton;

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerStats = player.GetComponent<PlayerStats>();
            weaponHandler = player.GetComponent<WeaponHandler>();
        }
        
    }  
    public void UpgradeHealth(int cost)
    {
        if (playerStats != null && moneyManager.Money >= cost * costMultiplier[0])
        {
            playerStats.SetMaxHealth(healthUpgrade);
            moneyManager.SpendMoney((int)(cost * costMultiplier[0]));
            costMultiplier[0] += 0.45f;
            costText[0].SetText($"${(int)(cost * costMultiplier[0]):0}");
            moneyManager.UpdateMoneyText();
        } 
    }

    public void UpgradeFireRate(int cost)
    {
        if (playerStats != null && moneyManager.Money >= cost * costMultiplier[1])
        {
            playerStats.SetFireRateMultiplier(fireRateUpgrade);
            moneyManager.SpendMoney((int)(cost * costMultiplier[1]));
            costMultiplier[1] += 0.3f;
            costText[1].SetText($"${(int)(cost * costMultiplier[1]):0}");
            moneyManager.UpdateMoneyText();
        }
    }

    public void UpgradeDamage(int cost)
    {
        if (playerStats != null && moneyManager.Money >= cost * costMultiplier[2])
        {
            playerStats.SetDamageMultiplier(damageUpgrade);
            moneyManager.SpendMoney((int)(cost * costMultiplier[2]));
            costMultiplier[2] += 0.5f;
            costText[2].SetText($"${(int)(cost * costMultiplier[2]):0}");
            moneyManager.UpdateMoneyText();
        }
    }

    public void UpgradeInvincibility(int cost)
    {
        if (playerStats != null && moneyManager.Money >= cost * costMultiplier[3])
        {
            playerStats.SetMaxInvincibility(invincibilityUpgrade);
            moneyManager.SpendMoney((int)(cost * costMultiplier[3]));
            costMultiplier[3] += 0.55f;
            costText[3].SetText($"${(int)(cost * costMultiplier[3]):0}");
            moneyManager.UpdateMoneyText();
        }
    }

    public void PurchaseShotGun(int cost)
    {
        if (weaponHandler == null || moneyManager == null) return;

        if (!weaponHandler.weapons.Exists(w => w is Shotgun) && moneyManager.Money >= cost)
        {
            moneyManager.SpendMoney(cost);
            if (shotgunSoldImg != null) shotgunSoldImg.SetActive(true);
            if (shotgunButton != null) shotgunButton.interactable = false;
            if (Shotgun != null && weaponInventory != null)
            {
                GameObject shotgun = Instantiate(Shotgun.gameObject, weaponInventory.transform);
                shotgun.SetActive(false);
                weaponHandler.weapons.Add(shotgun.GetComponent<Weapon>());
            }
            moneyManager.UpdateMoneyText();
        }
    }

    public void PurchaseAssaultRifle(int cost)
    {
        if (weaponHandler == null || moneyManager == null) return;

        if (!weaponHandler.weapons.Exists(w => w is AssaultRifle) && moneyManager.Money >= cost)
        {
            moneyManager.SpendMoney(cost);
            if (assaultRifleSoldImg != null) assaultRifleSoldImg.SetActive(true);
            if (assaultRifleButton != null) assaultRifleButton.interactable = false;
            if (AssaultRifle != null && weaponInventory != null)
            {
                GameObject assaultRifle = Instantiate(AssaultRifle.gameObject, weaponInventory.transform);
                assaultRifle.SetActive(false);
                weaponHandler.weapons.Add(assaultRifle.GetComponent<Weapon>());
            }
            moneyManager.UpdateMoneyText();
        }
    }
}