using UnityEngine;

public class MinionSkill : SlimeSkill
{
    enum Mode { Orbit, Attack }

    Mode _mode = Mode.Orbit;
    float _orbitAngle;
    float _attackCooldown;
    Collider _currentTarget;

    public override void OnActivate(SlimeMono unit)
    {
        _orbitAngle = Random.Range(0f, Mathf.PI * 2f);
    }

    public override void Update(SlimeMono unit, float dt)
    {
        if (SlimePoolMono.Instance.IsControlled(unit))
        {
            IsComplete = true;
            return;
        }

        if (unit.Data.CurrentState == Slime.State.Dead) return;

        _attackCooldown -= dt;

        var controllerTransform = FindNearestController(unit.transform.position);
        if (controllerTransform == null)
        {
            unit.NavigateTo(unit.transform.position, 0f);
            return;
        }

        if (_mode == Mode.Orbit)
            UpdateOrbit(unit, controllerTransform, dt);
        else
            UpdateAttack(unit, controllerTransform);
    }

    void UpdateOrbit(SlimeMono unit, Transform controller, float dt)
    {
        var pool = unit.Pool;

        _orbitAngle += dt * 0.4f;

        var offset = new Vector3(Mathf.Cos(_orbitAngle), 0f, Mathf.Sin(_orbitAngle)) * pool.MinionOrbitRadius;
        var orbitTarget = controller.position + offset;
        unit.NavigateTo(orbitTarget, pool.MoveSpeed * pool.MinionOrbitSpeedMultiplier);

        var enemy = FindNearestEnemy(controller.position, pool.MinionAttackScanRadius);
        if (enemy != null)
        {
            _currentTarget = enemy;
            _mode = Mode.Attack;
        }
    }

    void UpdateAttack(SlimeMono unit, Transform controller)
    {
        var pool = unit.Pool;

        if (_currentTarget == null || !_currentTarget.gameObject.activeInHierarchy)
        {
            _mode = Mode.Orbit;
            return;
        }

        float distToController = Vector3.Distance(_currentTarget.transform.position, controller.position);
        if (distToController > pool.MinionAttackLeashRadius)
        {
            _currentTarget = null;
            _mode = Mode.Orbit;
            return;
        }

        unit.NavigateTo(_currentTarget.transform.position, pool.MoveSpeed);

        if (_attackCooldown <= 0f)
        {
            float dist = Vector3.Distance(unit.transform.position, _currentTarget.transform.position);
            if (dist <= pool.MinionAttackRange && _currentTarget.TryGetComponent<IHittable>(out var hittable))
            {
                hittable.TakeDamage(pool.MinionAttackDamage);
                _attackCooldown = pool.MinionAttackCooldown;
            }
        }
    }

    Collider FindNearestEnemy(Vector3 center, float radius)
    {
        var hits = Physics.OverlapSphere(center, radius, SlimeConfig.Instance.playerTargetLayers);
        if (hits.Length == 0) return null;

        Collider nearest = null;
        float bestSqDist = float.MaxValue;
        foreach (var h in hits)
        {
            float sqDist = (h.transform.position - center).sqrMagnitude;
            if (sqDist < bestSqDist)
            {
                bestSqDist = sqDist;
                nearest = h;
            }
        }
        return nearest;
    }

    Transform FindNearestController(Vector3 position)
    {
        var candidates = SlimePoolMono.Instance.FindControllerTransform();
        if (candidates == null || candidates.Count == 0) return null;

        Transform nearest = null;
        float bestSqDist = float.MaxValue;
        foreach (var t in candidates)
        {
            if (t == null) continue;
            float sqDist = (t.position - position).sqrMagnitude;
            if (sqDist < bestSqDist)
            {
                bestSqDist = sqDist;
                nearest = t;
            }
        }
        return nearest;
    }

    public override void OnDeactivate(SlimeMono unit)
    {
        unit.SetMode(UnitMono.MoveMode.Controller);
    }
}
