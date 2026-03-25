using UnityEngine;

[System.Serializable]
public class SlimeStats
{
    [SerializeField] public Damage contactDamage;
    [SerializeField] public Damage radiusDamage;

    [SerializeField] public float radiusRange;
    [SerializeField] public float radiusTickRate;
}
