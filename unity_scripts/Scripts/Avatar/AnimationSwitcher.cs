using System.Collections;
using UnityEngine;

public class AnimationSwitcher : MonoBehaviour
{
    public AnimatorOverrideController OverrideController { get; private set; }
    public Animator Animator;
    public AnimationClip[] Animations;

    private readonly AlternatingStates _states = new();

    private class AlternatingStates
    {
        private readonly string[] _keyNames = { "__Placeholder_1", "__Placeholder_2" };
        public int StateIndex { get; private set; } = 0;

        public void Toggle()
        {
            StateIndex = 1 - StateIndex;
        }

        public string CurrentKeyForOverride
        {
            get => _keyNames[StateIndex];
        }
    }

    void Start()
    {
        OverrideController = new AnimatorOverrideController(Animator.runtimeAnimatorController);
        Animator.runtimeAnimatorController = OverrideController;
    }

    public void PlayAnimationClip(AnimationClip animationClip)
    {
        if (OverrideController != null)
        {
            OverrideController[_states.CurrentKeyForOverride] = animationClip;
            Animator.SetInteger("AnimationIndex", _states.StateIndex);
            _states.Toggle();
        }
    }

    public void SwitchAvatar(Avatar avatar)
    {
        Animator.avatar = avatar;
        //
        //  Don't you dare changing this, my dear pedant future-self or a colleague.
        //  It just werks.
        //
        StartCoroutine(RebindAnimatorNextFrame());
    }

    private IEnumerator RebindAnimatorNextFrame()
    {
        yield return null;  // Wait for the next frame
        Animator.Rebind();
    }
}
