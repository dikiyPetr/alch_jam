using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SlimePoolMono : MonoBehaviour, InputSystem_Actions.IPlayerActions
{
    public static SlimePoolMono Instance { get; private set; }

    [SerializeField] SlimeConfig config;
    [SerializeField] SlimeMono slimePrefab;

    [field: SerializeField] public SlimePool Pool { get; private set; }

    public Vector2 MoveInput { get; private set; }

    readonly List<SlimeMono> _activeMono = new();
    InputSystem_Actions _actions;

    void Awake()
    {
        Instance = this;
        SlimeConfig.Init(config);
        Pool = new SlimePool();
        _actions = new InputSystem_Actions();
        _actions.Player.AddCallbacks(this);
    }

    void OnEnable() => _actions.Player.Enable();
    void OnDisable() => _actions.Player.Disable();
    void OnDestroy() => _actions.Dispose();

    void Start() => SpawnSlime(transform.position);

    // --- Mono registry ---

    public void RegisterMono(SlimeMono mono)
    {
        if (!_activeMono.Contains(mono))
            _activeMono.Add(mono);
    }

    public void UnregisterMono(SlimeMono mono) => _activeMono.Remove(mono);

    // Слайм под управлением — наибольший, который может делиться
    public SlimeMono GetControlledSlime()
    {
        SlimeMono controlled = null;
        float maxHp = float.MinValue;
        foreach (var mono in _activeMono)
        {
            if (mono == null || mono.Data == null) continue;
            if (!mono.Data.CanSplit) continue;
            if (mono.Data.ContainedMaxHp > maxHp ||
                (Mathf.Approximately(mono.Data.ContainedMaxHp, maxHp) &&
                 controlled != null && mono.GetInstanceID() > controlled.GetInstanceID()))
            {
                maxHp = mono.Data.ContainedMaxHp;
                controlled = mono;
            }
        }

        return controlled;
    }

    public SlimeMono FindLargestExcept(SlimeMono exclude)
    {
        SlimeMono largest = null;
        float maxHp = float.MinValue;
        foreach (var mono in _activeMono)
        {
            if (mono == exclude || mono == null) continue;
            if (mono.Data.ContainedMaxHp > maxHp)
            {
                maxHp = mono.Data.ContainedMaxHp;
                largest = mono;
            }
        }

        return largest;
    }

    public SlimeMono FindNearestTo(Vector3 position, SlimeMono exclude)
    {
        SlimeMono nearest = null;
        float minSqDist = float.MaxValue;
        foreach (var mono in _activeMono)
        {
            if (mono == exclude || mono == null) continue;
            float sqDist = (mono.transform.position - position).sqrMagnitude;
            if (sqDist < minSqDist)
            {
                minSqDist = sqDist;
                nearest = mono;
            }
        }

        return nearest;
    }

    // --- Spawn ---

    public void SpawnSlime(Vector3 position)
    {
        var mono = Instantiate(slimePrefab, position, Quaternion.identity);
        RegisterMono(mono);
        mono.Initialize(Pool.CreateSlime(SlimeConfig.Instance.initialMaxHp));
    }

    // --- IPlayerActions ---

    public void OnMove(InputAction.CallbackContext ctx)
        => MoveInput = ctx.ReadValue<Vector2>();

    public void OnAttack(InputAction.CallbackContext ctx)
    {
    }

    public void OnJump(InputAction.CallbackContext ctx)
    {
    }

    public void OnInteract(InputAction.CallbackContext ctx)
    {
    }

    public void OnLook(InputAction.CallbackContext ctx)
    {
    }

    public void OnCrouch(InputAction.CallbackContext ctx)
    {
    }

    public void OnPrevious(InputAction.CallbackContext ctx)
    {
    }

    public void OnNext(InputAction.CallbackContext ctx)
    {
    }

    public void OnSprint(InputAction.CallbackContext ctx)
    {
    }

    public void OnSkill_Merge(InputAction.CallbackContext ctx)
    {
        if (!ctx.started) return;
        var controlled = GetControlledSlime();
        foreach (var mono in new List<SlimeMono>(_activeMono))
            if (mono != controlled)
                mono.ActivateSkill(new MergeSkill());
    }

    public void OnSkill_Following(InputAction.CallbackContext ctx)
    {
        if (!ctx.started) return;
        GetControlledSlime()?.TriggerSkillSplit();
    }

    public void OnSkill_Split(InputAction.CallbackContext ctx)
    {
        if (!ctx.started) return;
        GetControlledSlime()?.TriggerSplit();
    }
}