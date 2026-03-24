using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SlimePoolMono : MonoBehaviour
{
    public static SlimePoolMono Instance { get; private set; }

    [SerializeField] SlimeConfig config;
    [SerializeField] SlimeMono slimePrefab;

    [field: SerializeField] public SlimePool Pool { get; private set; }

    private readonly List<SlimeMono> _activeMono = new();
    private InputSystem_Actions _actions;

    void Awake()
    {
        Instance = this;
        SlimeConfig.Init(config);
        Pool = new SlimePool();
        _actions = new InputSystem_Actions();
        _actions.Player.Interact.started += _ => Pool.StartMerge();
    }

    void OnEnable() => _actions.Player.Enable();
    void OnDisable() => _actions.Player.Disable();
    void OnDestroy() => _actions.Dispose();

    void Start()
    {
        SpawnSlime(transform.position);
    }

    // --- Mono registry ---

    public void RegisterMono(SlimeMono mono)
    {
        if (!_activeMono.Contains(mono))
            _activeMono.Add(mono);
    }

    public void UnregisterMono(SlimeMono mono) => _activeMono.Remove(mono);

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
}