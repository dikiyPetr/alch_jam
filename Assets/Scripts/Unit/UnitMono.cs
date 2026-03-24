using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Rigidbody))]
public abstract class UnitMono : MonoBehaviour
{
    public enum MoveMode { Physics, Navigation, Controller }

    protected Rigidbody _rb;
    protected NavMeshAgent _agent;
    protected CharacterController _controller;

    [SerializeField] protected MoveMode moveMode;

    public Vector3 MoveTarget { get; set; }
    protected abstract float MoveSpeed { get; }

    protected virtual void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        TryGetComponent(out _agent);
        TryGetComponent(out _controller);

        if (_agent != null)
        {
            _agent.updatePosition = false;
            _agent.updateRotation = false;
        }
    }

    // --- Движение ---

    // Physics: прямое управление скоростью Rigidbody
    public void MoveInDirection(Vector3 dir, float speed)
    {
        SetMode(MoveMode.Physics);
        SetVelocityXZ(dir * speed);
    }

    // Navigation: агент считает путь, его желаемая скорость отдаётся Rigidbody
    public void NavigateTo(Vector3 target, float speed)
    {
        SetMode(MoveMode.Navigation);
        MoveTarget = new Vector3(target.x, transform.position.y, target.z);
        _agent.speed = speed;
        _agent.SetDestination(target);
        _agent.nextPosition = transform.position;
        SetVelocityXZ(_agent.desiredVelocity);
    }

    // Controller: движение через CharacterController
    public void MoveAsController(Vector3 motion)
    {
        SetMode(MoveMode.Controller);
        _controller.Move(motion);
    }

    public void ApplyMovement(Vector3 dir) => MoveInDirection(dir, MoveSpeed);

    // Одноразовый импульс: задаёт скорость мгновенно, не переопределяя её каждый кадр
    public void ApplyImpulse(Vector3 impulse)
        => _rb.AddForce(impulse, ForceMode.VelocityChange);

    // Плавный разворот к желаемой скорости через lerp — даёт инерцию при навигации
    public void SteerTowards(Vector3 dir, float targetSpeed, float steeringSpeed, float dt)
    {
        var desired = new Vector3(dir.x, 0f, dir.z) * targetSpeed;
        var current = new Vector3(_rb.linearVelocity.x, 0f, _rb.linearVelocity.z);
        var newVelocity = Vector3.Lerp(current, desired, steeringSpeed * dt);
        SetVelocityXZ(newVelocity);
    }

    // Управление линейным сопротивлением Rigidbody
    public void SetLinearDrag(float drag) => _rb.linearDamping = drag;
    public float GetLinearDrag()          => _rb.linearDamping;

    protected void StopMovement()
    {
        if (moveMode == MoveMode.Navigation && _agent != null && _agent.enabled)
            _agent.ResetPath();
        if (moveMode != MoveMode.Controller)
            SetVelocityXZ(Vector3.zero);
    }

    public void SetMode(MoveMode mode)
    {
        if (moveMode == mode) return;
        moveMode = mode;
        _rb.isKinematic = mode == MoveMode.Controller;
    }

    protected void SetVelocityXZ(Vector3 v)
        => _rb.linearVelocity = new Vector3(v.x, _rb.linearVelocity.y, v.z);

    // --- Гизмо ---

    void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(MoveTarget, 0.15f);
        Gizmos.DrawLine(transform.position, MoveTarget);
    }
}
