using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class SimpleBehavior : IChildBehavior
{
    private readonly AnimationSwitcher _animationSwitcher;
    private readonly AnimationClip[] _animationClips;
    private int _currentClipIndex;

    public SimpleBehavior(AnimationSwitcher animationSwitcher, AnimationClip[] animationClips)
    {
        _animationSwitcher = animationSwitcher;
        _animationClips = animationClips;
        _currentClipIndex = Random.Range(0, _animationClips.Length);
    }
    public SimpleBehavior(AnimationSwitcher animationSwitcher, BehaviorAnimationData behaviorAnimationData)
        : this(animationSwitcher, behaviorAnimationData.AnimationClips)
    {
    }

    private AnimationClip SwitchToNextDistinctArbitraryAction()
    {
        _currentClipIndex += Random.Range(1, _animationClips.Length);
        _currentClipIndex %= _animationClips.Length;
        return _animationClips[_currentClipIndex];
    }
    public virtual IEnumerator PerformAction()
    {
        // If the current clip is already distinct from the currently played, then no need to sample.
        //
        AnimationClip nextClip = _animationClips[_currentClipIndex];
        AnimationClip nowPlaying = _animationSwitcher.Animator.GetCurrentAnimatorClipInfo(0)[0].clip;
        if (nextClip == nowPlaying)
        {
            nextClip = SwitchToNextDistinctArbitraryAction();
        }
        float processingTime = nextClip.length;
        if (nextClip.isLooping)
        {
            processingTime = Random.Range(2f, 10f);
        } 
        _animationSwitcher.PlayAnimationClip(nextClip);
        yield return new WaitForSeconds(processingTime);
    }

    public virtual void StopCoroutinesRelated()
    { 
        return; // Nothing to stop
    }
}
