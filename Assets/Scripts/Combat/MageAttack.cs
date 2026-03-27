using UnityEngine;

// Атака мага: фаза наводки — индикатор цели следует за целью,
// затем спавнится метеор без фазы предупреждения (визуал уже был показан индикатором).
public class MageAttack : EnemyAttackBase
{
    [SerializeField] MeteorAreaDamage _projectilePrefab;
    [SerializeField] GameObject _targetingIndicatorPrefab;

    GameObject _indicator;

    protected override void OnWindupStart(Transform target)
    {
        if (_targetingIndicatorPrefab != null)
            _indicator = Instantiate(_targetingIndicatorPrefab, GroundPosition(target), Quaternion.identity);
    }

    protected override void OnWindupTick(Transform target)
    {
        if (_indicator != null)
            _indicator.transform.position = GroundPosition(target);
    }

    protected override void OnWindupCancelled()
    {
        DestroyIndicator();
    }

    protected override void DoAttack(Transform target)
    {
        DestroyIndicator();
        var projectile = Instantiate(_projectilePrefab, GroundPosition(target), Quaternion.identity);
        projectile.Launch(GroundPosition(target), skipWarning: true);
    }

    void DestroyIndicator()
    {
        if (_indicator == null) return;
        Destroy(_indicator);
        _indicator = null;
    }

    // Позиция на уровне земли (y=0), как в MeteorAreaDamage.
    static Vector3 GroundPosition(Transform target)
    {
        var pos = target.position;
        pos.y = 0f;
        return pos;
    }
}
