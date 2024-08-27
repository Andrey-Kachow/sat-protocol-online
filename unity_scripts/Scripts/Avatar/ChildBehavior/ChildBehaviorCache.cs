using UnityEngine;

/// <summary>
/// A way to instantiate arrays of animationClips and access them in code at Runtime
/// but without loading all at once for scalability reasons.
/// </summary>
public class ChildBehaviorCache
{
	private ChildBehaviorCache() { }

	private static ChildBehaviorCache _instance;

	/// <summary>
	/// The justification for using singletons is because the lazy evaluation
	/// of static fields in C# does not work nicely with Unity.
	/// </summary>
	public static ChildBehaviorCache Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new ChildBehaviorCache();
			}
			return _instance;
		}
	}

    private static string __(string resourceName)
	{
		return $"AnimationData/{resourceName}";
	}

	public readonly CachedBehaviorAnimationData Idle = new(__("IdleAnimationData"));
    public readonly CachedBehaviorAnimationData IdleMessingAround = new(__("IdleMessingAroundData"));
    public readonly CachedBehaviorAnimationData Sat = new(__("SelfAttachmentAnimationData_v1"));
    public readonly CachedBehaviorAnimationData InplaceWalking = new(__("InplaceWalking"));
    
	/// <summary>
	/// A proxy for the Scriptable data class <see cref="BehaviorAnimationData"/>.
	/// Loads the small resource at runtime once it is first accessed.
	/// </summary>
	public class CachedBehaviorAnimationData
	{
        private readonly string _resourcePath;
		public BehaviorAnimationData BehaviorAnimationData { get; private set; } = null;

		public CachedBehaviorAnimationData(string resourcePath)
		{
            _resourcePath = resourcePath;
		}

		public BehaviorAnimationData Get()
		{
			if (BehaviorAnimationData == null)
			{
				BehaviorAnimationData = Resources.Load<BehaviorAnimationData>(_resourcePath);
            }
            return BehaviorAnimationData;
		}

        public void Invalidate()
        {
            BehaviorAnimationData = null;
        }
    }
}