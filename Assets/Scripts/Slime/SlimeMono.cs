using System.Collections;
using UnityEngine;

public class SlimeMono : UnitMono
{
    [SerializeField] SlimeMono slimePrefab;

    [SerializeField] Slime _data;

    public SlimePool Pool => SlimePoolMono.Instance.Pool;
    public Slime Data => _data;

    SlimeSkill _activeSkill;
    public bool HasActiveSkill => _activeSkill != null;

    protected override float MoveSpeed => Pool.MoveSpeed;

    public void Initialize(Slime data)
    {
        _data = data;
        _data.OnStateChanged += OnStateChanged;
        _data.OnDied += _ => Destroy(gameObject);
    }

    void OnDestroy()
    {
        if (SlimePoolMono.Instance != null)
            SlimePoolMono.Instance.UnregisterMono(this);
    }

    void Update()
    {
        if (_data == null || _data.CurrentState == Slime.State.Dead) return;

        if (UpdateSkill(Time.deltaTime))
        {
            UpdateScale();
            return;
        }

        if (SlimePoolMono.Instance.IsControlled(this))
        {
            var input = SlimePoolMono.Instance.MoveInput;
            MoveAsController(new Vector3(input.x, 0f, input.y).normalized);
        }
        else
            StopMovement();

        UpdateScale();
    }

    void UpdateScale()
    {
        float s = Pool.BaseUnitScale * Mathf.Sqrt(_data.ContainedUnitCount);
        transform.localScale = Vector3.one * Mathf.Max(s, 0.1f);
    }

    void OnStateChanged(Slime slime)
    {
        if (slime.CurrentState == Slime.State.Dead)
            Destroy(gameObject);
    }

    // --- Сплит (вызывается из SlimePoolMono) ---

    public void TriggerSplit()
    {
        if (!_data.CanSplit) return;
        var split = _data.Split(1);
        foreach (var s in split)
            KickSlime(SpawnSplitSlime(s));
    }

    // Делит слайм на два примерно равных, новый вылетает в случайную сторону
    public void TriggerEvenSplit()
    {
        if (!_data.CanSplit) return;
        KickSlime(SpawnSplitSlime(_data.SplitHalf()));
    }

    // Разбивает слайм на максимальное количество юнитов с задержкой между каждым
    public void TriggerMaxSplit()
    {
        if (!_data.CanSplit) return;
        StartCoroutine(MaxSplitCoroutine());
    }

    // Поочерёдно выбрасывает по одному юниту с интервалом MaxSplitInterval
    private IEnumerator MaxSplitCoroutine()
    {
        var wait = new WaitForSeconds(Pool.MaxSplitInterval);
        while (_data != null && _data.CanSplit)
        {
            var splits = _data.Split(1);
            KickSlime(SpawnSplitSlime(splits[0]));
            yield return wait;
        }
    }

    public void TriggerSkillSplit()
    {
        if (!_data.CanSplit) return;
        var split = _data.Split(1);
        foreach (var s in split)
            SpawnSplitSlime(s).ActivateSkill(new CursorSkill(Pool.SkillDuration));
    }

    public void TriggerProjectileSplit()
    {
        if (!_data.CanSplit) return;
        // Фиксируем направление от слайма к курсору в момент клика
        var dir = CursorWorldPosition.Instance.Position - transform.position;
        dir.y = 0f;
        dir.Normalize();
        var split = _data.Split(1);
        foreach (var s in split)
            SpawnSplitSlime(s).ActivateSkill(
                new ProjectileSkill(dir, Pool.ProjectileReturnDelay, Pool.ProjectileSpeedMultiplier));
    }

    SlimeMono SpawnSplitSlime(Slime data)
    {
        // Небольшой случайный оффсет чтобы коллайдер нового слайма не перекрывался
        // с родительским — это предотвращает нежелательный overlap resolution от физики
        var offset = Random.insideUnitCircle * 0.1f;
        var pos = transform.position + new Vector3(offset.x, 0f, offset.y);
        var spawned = Instantiate(slimePrefab, pos, Quaternion.identity);
        SlimePoolMono.Instance.RegisterMono(spawned);
        spawned.Initialize(data);
        return spawned;
    }

    // Придаёт слайму случайный горизонтальный импульс и активирует KickSkill,
    // чтобы StopMovement не гасил скорость до завершения полёта
    private void KickSlime(SlimeMono mono)
    {
        var dir = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f)).normalized;
        mono.ApplyImpulse(dir * Pool.SplitImpulseSpeed);
        mono.ActivateSkill(new KickSkill(Pool.SplitKickDuration));
    }

    // --- Скиллы ---

    public void ActivateSkill(SlimeSkill skill)
    {
        _activeSkill = skill;
        skill.OnActivate(this);
    }

    protected bool UpdateSkill(float dt)
    {
        if (_activeSkill == null) return false;
        _activeSkill.Update(this, dt);
        if (_activeSkill.IsComplete)
        {
            _activeSkill.OnDeactivate(this);
            _activeSkill = null;
        }
        return true;
    }
}
