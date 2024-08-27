using System.Collections;
using UnityEngine;

/// <summary>
/// An array of AnimationClips that can be conveniently edited in Unity editor.
/// </summary>
[CreateAssetMenu(fileName = "NewAnimationData", menuName = "Data/BehaviorAnimationData")]
public class BehaviorAnimationData : ScriptableObject
{
    public AnimationClip[] AnimationClips;

    public int ClipsCount
    {
        get => AnimationClips.Length;
    }

    public AnimationClip this[int key]
    {
        get => AnimationClips[key];
    }
}
