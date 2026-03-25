using UnityEngine;

public class SlimeAreaDamageMono : AreaDamageMono
{
    [SerializeField] private Damage damage;

    protected override float GetDamage()
    {
        return damage.amount;
    }
}