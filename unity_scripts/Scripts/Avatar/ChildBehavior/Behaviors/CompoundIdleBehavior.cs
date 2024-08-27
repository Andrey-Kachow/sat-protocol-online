using System.Collections;
using UnityEngine;

public class CompoundIdleBehavior : IChildBehavior
{
    private readonly IChildBehavior[] _constituentBehaviors = new IChildBehavior[2];
    private IChildBehavior _currentBehavior;

    public CompoundIdleBehavior(ChildAvatar childAvatar)
    {
        _constituentBehaviors[0] = new IdleBehavior(childAvatar.AnimationSwitcher);
        _constituentBehaviors[1] = new IdleMessingAroundBehavior(childAvatar);
    }
    public IEnumerator PerformAction()
    {
        _currentBehavior = _constituentBehaviors[CoinToss()];
        yield return _currentBehavior.PerformAction();
    }

    public void StopCoroutinesRelated()
    {
        foreach (IChildBehavior behavior in _constituentBehaviors)
        {
            behavior.StopCoroutinesRelated();
        }
    }

    private int CoinToss()
    {
        return Random.Range(0, 2);
    }

    public override string ToString()
    {
        return $"CompoundBehavior({_currentBehavior})";
    }
}