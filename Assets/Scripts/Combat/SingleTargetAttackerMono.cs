using UnityEngine;

// Абстрактный атакующий, выбирающий одну цель из зоны _rangeFinder по условию.
// SelectTarget — условие выбора, Attack — что сделать с целью.
public abstract class SingleTargetAttackerMono : DamageSourceMono
{
    // Заглушка — урон задаётся в Attack() каждой реализацией отдельно
    protected override float GetDamage() => 0f;

    protected override void Tick()
    {
        if (_rangeFinder == null) return;
        _rangeFinder.RemoveDeadEntries();
        var (target, pos) = SelectTarget();
        if (target != null) Attack(target, pos);
    }

    // Условие выбора цели — переопредели в подклассе
    protected abstract (IHittable target, Vector3 pos) SelectTarget();

    // Действие над выбранной целью
    protected abstract void Attack(IHittable target, Vector3 targetPos);

    // Вспомогательный метод: ближайшая цель к этому объекту
    protected (IHittable target, Vector3 pos) SelectNearest()
    {
        IHittable nearest = null;
        Vector3 nearestPos = default;
        float minSqDist = float.MaxValue;
        foreach (var hittable in _rangeFinder.InRange)
        {
            if (hittable is not Component c) continue;
            float sq = (c.transform.position - transform.position).sqrMagnitude;
            if (sq < minSqDist) { minSqDist = sq; nearest = hittable; nearestPos = c.transform.position; }
        }
        return (nearest, nearestPos);
    }
}
