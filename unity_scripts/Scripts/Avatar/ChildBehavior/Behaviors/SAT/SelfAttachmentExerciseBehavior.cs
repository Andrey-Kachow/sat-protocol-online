using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Behavior that depends on the current active Self attachment state
/// <see cref="SelfAttachmentEmotionalState"/> and calls the delegate
/// <see cref="IChildBehavior"/> for that state. In addition, before
/// doing so at first it makes the child walk towards the middle of the
/// playground. It is the best spot for SAT exercise as the child is visible.
/// Between performing SAT emotional animation, the child is also looking in
/// player's direction.
/// </summary>
public partial class SelfAttachmentExerciseBehavior : IChildBehavior
{
    private static readonly BehaviorAnimationData SatData = ChildBehaviorCache.Instance.Sat.Get();
    private const string StandardState = "None";
    private readonly GameObject _player;
    private readonly ChildAvatar _childAvatar;
    private readonly IChildBehavior _defaultDelegate;
    private readonly Dictionary<string, IChildBehavior> _delegates = new();
    private static readonly Vector3 _initialPosition = Vector3.zero;
    public string CurrentState = StandardState;

    //private Vector3 _satExercisePosition = Vector3.zero;

    /// <summary>
    /// Partitions the Scriptable AnimationClip array items by Self-Attachment emotions.
    /// </summary>
    /// <param name="animationSwitcher">The object that can play the animations</param>
    public SelfAttachmentExerciseBehavior(ChildAvatar childAvatar, GameObject player)
    {
        _player = player;
        _childAvatar = childAvatar;
        _defaultDelegate = new IdleBehavior(childAvatar.AnimationSwitcher);
        foreach (SelfAttachmentEmotionalState state in SelfAttachmentEmotionalState.Table)
        {
            AnimationClip[] animationClips = new AnimationClip[state.AcceptableClipNames.Length];
            int appendIndex = 0;
            foreach (AnimationClip dataClip in SatData.AnimationClips)
            {
                if (state.AcceptableClipNames.Contains(dataClip.name))
                {
                    animationClips[appendIndex] = dataClip;
                    appendIndex++;
                    if (appendIndex >= animationClips.Length)
                    {
                        break;
                    }
                }
            }
            _delegates[state.StateName] = new SimpleBehavior(childAvatar.AnimationSwitcher, animationClips);
        }
    }

    public IEnumerator GoToInitialPosition()
    {
        yield return ControlledMotionUtils.WalkToTheTargetPosition(_initialPosition, 3f, _childAvatar);
        yield return ControlledMotionUtils.FaceTowards(_player.transform.position, 1f, _childAvatar);
        _isInCorrectPosition /* Let's pretend */ = true; 
    }

    private bool _isInCorrectPosition = false;
    public bool Ready
    {
        get => _isInCorrectPosition;
    }

    public IEnumerator PerformAction()
    {
        if (!Ready)
        {
            yield return GoToInitialPosition();
            CurrentState = SelfAttachmentEmotionalState.NEUTRAL_STATE.StateName;
        }
        yield return ControlledMotionUtils.FaceTowards(_player.transform.position, 0.5f, _childAvatar);
        yield return _delegates.GetValueOrDefault(CurrentState, _defaultDelegate).PerformAction();
    }

    public void StopCoroutinesRelated()
    {
        if (Vector3.Distance(_initialPosition, _childAvatar.transform.position) > 2f)
        {
            _isInCorrectPosition = false;
        }
        // Explicitly nothing to dispose;
    }

    public override string ToString()
    {
        return $"SAT Behavior in {CurrentState} state";
    }
}