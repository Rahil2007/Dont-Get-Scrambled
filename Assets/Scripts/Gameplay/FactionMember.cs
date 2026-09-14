using UnityEngine;

public class FactionMember : MonoBehaviour
{
    [SerializeField] private Faction faction;
    public Faction Faction => faction;

    public void SetMyFaction(Faction faction)
    {
        this.faction = faction;
    }
}
