using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class SlimeMono : MonoBehaviour, InputSystem_Actions.IPlayerActions
{
    [SerializeField] SlimeMono slimePrefab;

    [SerializeField] Slime _data;
    Rigidbody _rb;
    InputSystem_Actions _actions;
    Vector2 _moveInput;
    SlimeSkill _activeSkill;
    public Vector3 MoveTarget { get; set; }

    public SlimePool Pool => SlimePoolMono.Instance.Pool;

    public Slime Data => _data;

    public void Initialize(Slime data)
    {
        _data = data;
        _data.OnStateChanged += OnStateChanged;
        _data.OnDied += _ => Destroy(gameObject);
    }

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _actions = new InputSystem_Actions();
        _actions.Player.AddCallbacks(this);

        int slimeLayer = LayerMask.NameToLayer("Slime");
        if (slimeLayer != -1)
            gameObject.layer = slimeLayer;
    }

    void OnEnable() => _actions.Player.Enable();
    void OnDisable() => _actions.Player.Disable();

    void OnDestroy()
    {
        _actions.Dispose();
        if (SlimePoolMono.Instance != null)
            SlimePoolMono.Instance.UnregisterMono(this);
    }

    void Update()
    {
        if (_data == null || _data.CurrentState == Slime.State.Dead) return;

        if (_activeSkill != null)
        {
            _activeSkill.Update(this, Time.deltaTime);
            if (_activeSkill.IsComplete)
            {
                _activeSkill.OnDeactivate(this);
                _activeSkill = null;
            }
            UpdateScale();
            return;
        }

        switch (_data.CurrentState)
        {
            case Slime.State.Autonomous:
                if (_data.CanSplit)
                    HandleMovement();
                else
                {
                    StopMovement();
                    // TODO: сценарий поведения малого слайма
                }
                break;
            case Slime.State.Merging:
                if (_data.CanSplit && IsLargestInPool())
                    HandleMovement();
                else
                    MoveToMerge();
                break;
        }

        UpdateScale();
    }

    void HandleMovement()
    {
        var dir = new Vector3(_moveInput.x, 0f, _moveInput.y).normalized;
        SetVelocityXZ(dir * Pool.MoveSpeed);
    }

    void StopMovement() => SetVelocityXZ(Vector3.zero);

    void SetVelocityXZ(Vector3 horizontalVelocity)
        => _rb.linearVelocity = new Vector3(horizontalVelocity.x, _rb.linearVelocity.y, horizontalVelocity.z);

    public void ApplyMovement(Vector3 dir) => SetVelocityXZ(dir * Pool.MoveSpeed);

    public void ActivateSkill(SlimeSkill skill)
    {
        _activeSkill = skill;
        skill.OnActivate(this);
    }

    void DoSplit()
    {
        var split = _data.Split(1);
        foreach (var s in split)
            SpawnSplitSlime(s);
    }

    void DoSkillSplit()
    {
        var split = _data.Split(1);
        foreach (var s in split)
            SpawnSplitSlime(s).ActivateSkill(new CursorSkill(Pool.SkillDuration));
    }

    SlimeMono SpawnSplitSlime(Slime data)
    {
        var offset = Random.insideUnitCircle * Pool.SplitSpawnRadius;
        var pos = transform.position + new Vector3(offset.x, 0f, offset.y);
        var spawned = Instantiate(slimePrefab, pos, Quaternion.identity);
        SlimePoolMono.Instance.RegisterMono(spawned);
        spawned.Initialize(data);
        return spawned;
    }

    void MoveToMerge()
    {
        if (_data.CurrentState == Slime.State.Dead) return;

        var nearest = SlimePoolMono.Instance.FindNearestTo(transform.position, this);
        if (nearest == null) { StopMovement(); return; }

        var midpoint = (transform.position + nearest.transform.position) * 0.5f;
        var dir = (midpoint - transform.position);
        dir.y = 0f;
        SetVelocityXZ(dir.normalized * (Pool.MoveSpeed * Pool.MergeSpeedMultiplier));

        if (Vector3.Distance(transform.position, nearest.transform.position) <= Pool.MergeRadius)
        {
                _data.Absorb(nearest.Data);
        }
    }

    void UpdateScale()
    {
        float s = Pool.BaseUnitScale * Mathf.Sqrt(_data.ContainedUnitCount);
        transform.localScale = Vector3.one * Mathf.Max(s, 0.1f);
    }

    void OnDrawGizmos()
    {
        if (_activeSkill == null) return;
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(MoveTarget, 0.15f);
        Gizmos.DrawLine(transform.position, MoveTarget);
    }

    void OnStateChanged(Slime slime)
    {
        if (slime.CurrentState == Slime.State.Dead)
            Destroy(gameObject);
    }

    // --- IPlayerActions ---

    bool IsPlayerControlled =>
        _data != null &&
        _data.CanSplit &&
        (_data.CurrentState == Slime.State.Autonomous ||
         (Pool.IsMerging && IsLargestInPool()));

    bool IsLargestInPool()
    {
        var second = SlimePoolMono.Instance.FindLargestExcept(this);
        if (second == null) return true;
        return _data.ContainedMaxHp > second.Data.ContainedMaxHp ||
               (Mathf.Approximately(_data.ContainedMaxHp, second.Data.ContainedMaxHp) &&
                GetInstanceID() > second.GetInstanceID());
    }

    public void OnMove(InputAction.CallbackContext ctx)
        => _moveInput = ctx.ReadValue<Vector2>();

    public void OnAttack(InputAction.CallbackContext ctx)
    {
        if (!ctx.started || !IsPlayerControlled) return;
        DoSplit();
    }

    public void OnInteract(InputAction.CallbackContext ctx) { }

    public void OnLook(InputAction.CallbackContext ctx) { }
    public void OnCrouch(InputAction.CallbackContext ctx) { }
    public void OnJump(InputAction.CallbackContext ctx)
    {
        if (!ctx.started || !IsPlayerControlled) return;
        DoSkillSplit();
    }
    public void OnPrevious(InputAction.CallbackContext ctx) { }
    public void OnNext(InputAction.CallbackContext ctx) { }
    public void OnSprint(InputAction.CallbackContext ctx) { }
}
