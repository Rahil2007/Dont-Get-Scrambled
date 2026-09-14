using UnityEngine;

public class EnemyWeaponHandler : MonoBehaviour
{
    public Weapon weapon;
    private GameObject player;
    private float baseDamage;

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (weapon != null)
            baseDamage = weapon.damage;
    }

    void Update()
    {
        if (weapon != null && player != null)
        {
            Vector2 dirn = player.transform.position - weapon.transform.position;
            weapon.GetComponent<Transform>().rotation = Quaternion.Euler(0, 0, Mathf.Atan2(dirn.y, dirn.x) * Mathf.Rad2Deg);
            weapon.Fire(this.GetComponent<FactionMember>().Faction);
        }
    }

    public void UpdateDamage()
    {
        if (weapon == null) return;
        var enemyComp = GetComponent<Enemy>();
        if (enemyComp == null) return;
        weapon.damage = baseDamage * enemyComp.damageMultiplier;
    }
}
