using UnityEngine;

// Атака лучника: выпускает ArrowProjectile в цель.
// Логика атаки заимствована из ArcherDummy.
public class ArcherAttack : EnemyAttackBase
{
    [SerializeField] ArrowProjectile _arrowPrefab;
    [SerializeField] float _damage = 15f;
    [SerializeField] float _arrowSpeed = 12f;

    protected override void DoAttack(Transform target)
    {
        var arrow = Instantiate(_arrowPrefab, transform.position, Quaternion.identity);
        arrow.Launch(target, _damage, _arrowSpeed);
    }
}
