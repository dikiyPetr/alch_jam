using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Rigidbody))]
public abstract class UnitMono : MonoBehaviour
{
    protected Rigidbody _rb;

    SlimeSkill _activeSkill;
    public Vector3 MoveTarget { get; set; }
    public bool HasActiveSkill => _activeSkill != null;

    protected abstract float MoveSpeed { get; }

    protected virtual void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    // --- Движение ---

    // Движение в направлении — прямое (игрок, направленные скиллы)
    public void MoveInDirection(Vector3 dir, float speed)
        => SetVelocityXZ(dir * speed);

    public void ApplyMovement(Vector3 dir) => MoveInDirection(dir, MoveSpeed);

    protected void StopMovement()
    {
        SetVelocityXZ(Vector3.zero);
    }

    protected void SetVelocityXZ(Vector3 v)
        => _rb.linearVelocity = new Vector3(v.x, _rb.linearVelocity.y, v.z);

    // --- Гизмо ---

    void OnDrawGizmos()
    {
        if (!HasActiveSkill) return;
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(MoveTarget, 0.15f);
        Gizmos.DrawLine(transform.position, MoveTarget);
    }
}