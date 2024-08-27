public class IdleBehavior : SimpleBehavior
{
    public IdleBehavior(AnimationSwitcher animationSwitcher) 
        : base(animationSwitcher, ChildBehaviorCache.Instance.Idle.Get())
    {
        // Parametrized with Idle AnimationClip array.
    }
}
