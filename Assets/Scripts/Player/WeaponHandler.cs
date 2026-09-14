using System.Collections.Generic;
using UnityEngine;
public class WeaponHandler : MonoBehaviour
{
    public List<Weapon> weapons;
    private int currentWeaponIndex = 0;
    private Vector2 screenPos;
    private Vector2 worldPos;
    private PlayerStats playerStats;

    void Start()
    {
        playerStats = GetComponent<PlayerStats>();
    }
    void Update()
    {
        //To prevent player from using the gun while paused
        if(Time.timeScale == 0)
            return;

        if (Input.GetButtonDown("Fire1") && !weapons[currentWeaponIndex].holdFire)
            weapons[currentWeaponIndex].Fire(this.GetComponent<FactionMember>().Faction);
        else if(Input.GetButton("Fire1") && weapons[currentWeaponIndex].holdFire)
            weapons[currentWeaponIndex].Fire(this.GetComponent<FactionMember>().Faction);

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            foreach(var weapon in weapons)
                weapon.gameObject.SetActive(false);
            weapons[0].gameObject.SetActive(true);
            currentWeaponIndex = 0;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2) && weapons.Count > 1)
        {
            foreach (var weapon in weapons)
                weapon.gameObject.SetActive(false);
            weapons[1].gameObject.SetActive(true);
            currentWeaponIndex = 1;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3) && weapons.Count > 2)
        {
            foreach (var weapon in weapons)
                weapon.gameObject.SetActive(false);
            weapons[2].gameObject.SetActive(true);
            currentWeaponIndex = 2;
        }

        screenPos = Input.mousePosition;
        worldPos = Camera.main.ScreenToWorldPoint(screenPos);
        Vector2 dirn = worldPos - (Vector2)weapons[currentWeaponIndex].transform.position;
        weapons[currentWeaponIndex].GetComponent<Transform>().rotation = Quaternion.Euler(0,0, Mathf.Atan2(dirn.y, dirn.x) * Mathf.Rad2Deg);
    }

    public void AddWeapon(Weapon weapon)
    {
        weapons.Add(weapon);
    }   
    public void RemoveWeapon(Weapon weapon)
    {
        weapons.Remove(weapon);
    }   
    public void UpdateDamage()
    {
        weapons[currentWeaponIndex].damage *= playerStats.DamageMultiplier;
    }
    public void UpdateFireRate()
    {
        weapons[currentWeaponIndex].fireRate /= playerStats.FireRateMultiplier;
    }
}