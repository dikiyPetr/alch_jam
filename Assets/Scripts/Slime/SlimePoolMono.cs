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
    public IReadOnlyList<SlimeMono> AllMono => _activeMono;
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

    public List<Transform> FindControllerTransform()
    {
        var controlled = GetControlledSlimes();
        if (controlled.Count > 0) return controlled.ConvertAll((v) => v.transform);

        var controlledFantom = PhantomController.Instance.gameObject;
        return new List<Transform> { controlledFantom.transform };
    }

    // Управляемые слаймы — все с максимальным округлённым HP среди тех, кто может делиться
    public List<SlimeMono> GetControlledSlimes()
    {
        // Первый проход: ищем максимальное округлённое HP
        int maxHp = int.MinValue;
        foreach (var mono in _activeMono)
        {
            if (mono == null || mono.Data == null) continue;
            if (!mono.Data.CanSplit) continue;
            int hp = Mathf.RoundToInt(mono.Data.ContainedMaxHp);
            if (hp > maxHp) maxHp = hp;
        }

        var result = new List<SlimeMono>();
        if (maxHp == int.MinValue) return result;

        // Второй проход: собираем всех слаймов с максимальным HP
        foreach (var mono in _activeMono)
        {
            if (mono == null || mono.Data == null) continue;
            if (!mono.Data.CanSplit) continue;
            if (Mathf.RoundToInt(mono.Data.ContainedMaxHp) == maxHp)
                result.Add(mono);
        }

        return result;
    }

    // Проверяет, входит ли слайм в группу управляемых (с максимальным округлённым HP)
    public bool IsControlled(SlimeMono mono)
    {
        if (mono == null || mono.Data == null || !mono.Data.CanSplit) return false;
        int monoHp = Mathf.RoundToInt(mono.Data.ContainedMaxHp);
        // Если кто-то из активных имеет большее округлённое HP — этот слайм не управляемый
        foreach (var m in _activeMono)
        {
            if (m == null || m.Data == null || !m.Data.CanSplit) continue;
            if (Mathf.RoundToInt(m.Data.ContainedMaxHp) > monoHp) return false;
        }

        return true;
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
        if (!ctx.started) return;
        foreach (var mono in GetControlledSlimes())
            mono.TriggerProjectileSplit();
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

    // Деление на два примерно равных слайма
    public void OnSkill_EvenSplit(InputAction.CallbackContext ctx)
    {
        if (!ctx.started) return;
        foreach (var mono in GetControlledSlimes())
            mono.TriggerEvenSplit();
    }

    // Деление на максимальное количество единичных слаймов
    public void OnSkill_MaxSplit(InputAction.CallbackContext ctx)
    {
        if (!ctx.started) return;
        foreach (var mono in GetControlledSlimes())
            mono.TriggerMaxSplit();
    }

    public void OnSprint(InputAction.CallbackContext ctx)
    {
    }

    public void OnSkill_Merge(InputAction.CallbackContext ctx)
    {
        if (!ctx.started) return;
        // Все слаймы кроме управляемых сливаются
        foreach (var mono in new List<SlimeMono>(_activeMono))
            if (!IsControlled(mono))
                mono.ActivateSkill(new MergeSkill());
    }

    public void OnSkill_Following(InputAction.CallbackContext ctx)
    {
        if (!ctx.started) return;
        foreach (var mono in GetControlledSlimes())
            mono.TriggerSkillSplit();
    }

    public void OnSkill_Split(InputAction.CallbackContext ctx)
    {
        if (!ctx.started) return;
        foreach (var mono in GetControlledSlimes())
            mono.TriggerSplit();
    }
}