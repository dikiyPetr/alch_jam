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

        if (SlimePoolMono.Instance.GetControlledSlime() == this)
        {
            var input = SlimePoolMono.Instance.MoveInput;
            ApplyMovement(new Vector3(input.x, 0f, input.y).normalized);
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
        else if (slime.CurrentState == Slime.State.Merging)
            ActivateSkill(new MergeSkill());
    }

    // --- Сплит (вызывается из SlimePoolMono) ---

    public void TriggerSplit()
    {
        if (!_data.CanSplit) return;
        var split = _data.Split(1);
        foreach (var s in split)
            SpawnSplitSlime(s);
    }

    public void TriggerSkillSplit()
    {
        if (!_data.CanSplit) return;
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
