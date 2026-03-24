public abstract class SlimeSkill
{
    public bool IsComplete { get; protected set; }

    public virtual void OnActivate(SlimeMono mono) { }
    public abstract void Update(SlimeMono mono, float dt);
    public virtual void OnDeactivate(SlimeMono mono) { }
}
