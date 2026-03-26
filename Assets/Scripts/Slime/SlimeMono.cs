using System.Collections;
using UnityEngine;

public class SlimeMono : UnitMono
{
    [SerializeField] SlimeMono slimePrefab;

    [SerializeField] Slime _data;
    [SerializeField] SlimeStats _stats;

    // Ищем в детях, чтобы не требовать ручной привязки в инспекторе
    SlimeAreaDamageMono _areaDamage;

    public SlimePool Pool => SlimePoolMono.Instance.Pool;
    public Slime Data => _data;
    public SlimeStats Stats => _stats;

    SlimeSkill _activeSkill;
    public bool HasActiveSkill => _activeSkill != null;

    // Транзитный множитель урона — применяется скиллами (например ProjectileSkill)
    [field: SerializeField] public float DamageMultiplier { get; set; } = 1f;

    protected override float MoveSpeed => Pool.MoveSpeed;

    protected override void Awake()
    {
        base.Awake();
        _areaDamage = GetComponentInChildren<SlimeAreaDamageMono>();
    }

    // Первоначальный спавн — stats берутся из инспектора префаба
    public void Initialize(Slime data)
    {
        _data = data;
        _data.OnStateChanged += OnStateChanged;
        _data.OnDied += _ => Destroy(gameObject);
        _areaDamage?.Initialize(this);
    }

    // Спавн при сплите — stats приходят от родительского слайма
    public void Initialize(Slime data, SlimeStats stats)
    {
        _stats = stats;
        Initialize(data);
    }

    // Поглощает другой слайм: суммирует radiusDamage и передаёт HP.
    // Проверяем состояние до прибавления урона — Data.Absorb тоже защищён этой проверкой,
    // но урон мог бы прибавиться несколько раз если несколько слаймов нацелились на одного
    public void AbsorbMono(SlimeMono other)
    {
        if (other.Data.CurrentState == Slime.State.Dead) return;
        _stats.radiusDamage.amount += other.Stats.radiusDamage.amount;
        Data.Absorb(other.Data);
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

        if (SlimePoolMono.Instance.IsControlled(this) && _activeSkill is not MergeSkill)
        {
            var input = SlimePoolMono.Instance.MoveInput;
            MoveAsController(new Vector3(input.x, 0f, input.y).normalized);
        }
        else if (_activeSkill == null)
            ActivateSkill(new MinionSkill());

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
        foreach (var s in _data.Split(1))
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
            KickSlime(SpawnSplitSlime(_data.Split(1)[0]));
            yield return wait;
        }
    }

    public void TriggerSkillSplit()
    {
        if (!_data.CanSplit) return;
        foreach (var s in _data.Split(1))
            SpawnSplitSlime(s).ActivateSkill(new CursorSkill(Pool.SkillDuration));
    }

    public void TriggerProjectileSplit()
    {
        if (!_data.CanSplit) return;
        // Фиксируем направление от слайма к курсору в момент клика
        var dir = CursorWorldPosition.Instance.Position - transform.position;
        dir.y = 0f;
        dir.Normalize();
        foreach (var s in _data.Split(1))
            SpawnSplitSlime(s).ActivateSkill(
                new ProjectileSkill(dir, Pool.ProjectileReturnDelay, Pool.ProjectileSpeedMultiplier));
    }

    // Единственное место расчёта пропорции урона при сплите.
    // После Slime.Split/SplitHalf сумма HP родителя и нового слайма = исходному HP,
    // поэтому originalMaxHp можно восстановить без явного сохранения до деления.
    SlimeMono SpawnSplitSlime(Slime splitData)
    {
        float originalHp = _data.ContainedMaxHp + splitData.ContainedMaxHp;
        float proportion = splitData.ContainedMaxHp / originalHp;
        float splitDamage = _stats.radiusDamage.amount * proportion;
        _stats.radiusDamage.amount -= splitDamage;

        // Небольшой случайный оффсет чтобы коллайдер нового слайма не перекрывался
        // с родительским — это предотвращает нежелательный overlap resolution от физики
        var offset = Random.insideUnitCircle * 0.1f;
        var pos = transform.position + new Vector3(offset.x, 0f, offset.y);
        var spawned = Instantiate(slimePrefab, pos, Quaternion.identity);
        SlimePoolMono.Instance.RegisterMono(spawned);
        // Копируем stats родителя, переопределяя только radiusDamage
        var stats = new SlimeStats
        {
            contactDamage = new Damage { amount = _stats.contactDamage.amount },
            radiusDamage = new Damage { amount = splitDamage },
            radiusRange = _stats.radiusRange,
            radiusTickRate = _stats.radiusTickRate,
        };
        spawned.Initialize(splitData, stats);
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

    public void CancelSkillIfType<T>() where T : SlimeSkill
    {
        if (_activeSkill is not T) return;
        _activeSkill.OnDeactivate(this);
        _activeSkill = null;
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