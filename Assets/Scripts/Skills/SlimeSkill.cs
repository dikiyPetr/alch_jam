public abstract class SlimeSkill
{
    public bool IsComplete { get; protected set; }

    public virtual void OnActivate(SlimeMono unit) { }
    public abstract void Update(SlimeMono unit, float dt);
    public virtual void OnDeactivate(SlimeMono unit) { }
}
