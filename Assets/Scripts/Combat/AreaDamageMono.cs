using UnityEngine;

// Специализация DamageSourceMono для периодического урона по всем целям в зоне.
// Коллайдер и отслеживание целей — в _rangeFinder (отдельный GameObject).
public abstract class AreaDamageMono : DamageSourceMono
{
    protected override void Tick()
    {
        if (_rangeFinder == null || _rangeFinder.InRange.Count == 0) return;
        _rangeFinder.RemoveDeadEntries();
        float dmg = GetDamage();
        foreach (var target in _rangeFinder.InRange)
            target.TakeDamage(dmg);
    }

    void OnDrawGizmos()
    {
        var col = _rangeFinder?.Collider;
        if (col == null) return;
        Gizmos.color = new Color(1f, 0.2f, 0.2f, 0.25f);
        Gizmos.matrix = col.transform.localToWorldMatrix;

        switch (col)
        {
            case SphereCollider s:
                Gizmos.DrawSphere(s.center, s.radius);
                Gizmos.color = new Color(1f, 0.2f, 0.2f, 0.8f);
                Gizmos.DrawWireSphere(s.center, s.radius);
                break;
            case BoxCollider b:
                Gizmos.DrawCube(b.center, b.size);
                Gizmos.color = new Color(1f, 0.2f, 0.2f, 0.8f);
                Gizmos.DrawWireCube(b.center, b.size);
                break;
            case CapsuleCollider c:
                float r = c.radius;
                float halfH = Mathf.Max(0f, c.height * 0.5f - r);
                var top    = c.center + Vector3.up * halfH;
                var bottom = c.center - Vector3.up * halfH;
                Gizmos.color = new Color(1f, 0.2f, 0.2f, 0.8f);
                Gizmos.DrawWireSphere(top, r);
                Gizmos.DrawWireSphere(bottom, r);
                break;
        }
    }
}
