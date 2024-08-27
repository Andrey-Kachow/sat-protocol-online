using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Main script of child avatar
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class ChildAvatar : MonoBehaviour
{
    private const float GROUNDING_RELIABILITY_Y_SPEED = -2f;

    public float Speed = 2.0f;
    public float Gravity = -9.81f;

    public Vector3 Direction;
    private Vector3 _velocity;
    public CharacterController CharacterController { get; private set; }

    private GameObject _avatarModel;
    public GameObject PlayerPerson;
    public GameObject ModelHolder;
    public AnimationSwitcher AnimationSwitcher;
    private IChildBehavior _currentChildBehavior;
    private IChildBehavior _defaultBehavior;
    private SelfAttachmentExerciseBehavior _selfAttachmentProtocolBehavior;

    private Coroutine _behaviorCoroutine = null;

    public bool IsPerformingExerciseSAT { get; private set; } = false;

    void Start()
    {
        CharacterController = GetComponent<CharacterController>();
     
        _defaultBehavior = new CompoundIdleBehavior(this);
        _selfAttachmentProtocolBehavior = new SelfAttachmentExerciseBehavior(this, PlayerPerson);
        _currentChildBehavior = _defaultBehavior;
        NewBehavior();
    }

    private void Update()
    {
        // Apply gravity
        //
        if (CharacterController.isGrounded && _velocity.y < 0)
        {
            _velocity.y = GROUNDING_RELIABILITY_Y_SPEED;
        }
        _velocity.y += Gravity * Time.deltaTime;

        if (!IsPerformingExerciseSAT)
        {
            CharacterController.Move(_velocity * Time.deltaTime);
        }
    }


    public void SetModel(GameObject model)
    {
        if (model.TryGetComponent(out Animator modelAnimator))
        {
            AnimationSwitcher.SwitchAvatar(modelAnimator.avatar);
            Destroy(modelAnimator);
        }

        _avatarModel = model;
        // Resize to a slightly more adequate size.
        //
        transform.localScale = transform.localScale * GameSceneManager.GAME_OVERSCALE;
    }

    IEnumerator Behave()
    {
        while (true)
        {
            yield return _currentChildBehavior.PerformAction();
        }
    }

    public void ToggleSelfAttachmentExercise()
    {
        IsPerformingExerciseSAT = !IsPerformingExerciseSAT;
        DisposeBehavior();
        if (IsPerformingExerciseSAT)
        {
            _currentChildBehavior = _selfAttachmentProtocolBehavior;
        }
        else
        {
            _currentChildBehavior = _defaultBehavior;
        }
        NewBehavior();
    }
    public void SetEmotion(string stateName)
    {
        _selfAttachmentProtocolBehavior.CurrentState = stateName;
        NewBehavior();
    }

    void DisposeBehavior()
    {
        if (_currentChildBehavior != null)
        {
            _currentChildBehavior.StopCoroutinesRelated();
        }
        if (_behaviorCoroutine != null)
        {
            StopCoroutine(_behaviorCoroutine);
        }
    }

    void NewBehavior()
    {
        DisposeBehavior();
        _behaviorCoroutine = StartCoroutine(Behave());
    }

    public override string ToString()
    {
        return $"( " +
            $"Is in SAT: {IsPerformingExerciseSAT}, \n" +
            $"currentBehavior is: {_currentChildBehavior}, " +
            $"coroutine: {_behaviorCoroutine != null}" +
            $"\n) ";
    }

#if UNITY_EDITOR
    // Debug Area
    void Awake()
    {
        StartCoroutine(DebugDelayedAndTotallyUnexpectedAction(_avatarModel));
    }
    /// <summary>
    /// Coroutine for debug purposes that helps to do something relevant at a time.
    /// </summary>
    /// <param name="objectToPlayWith">Any gameobject of interest in the debugging</param>
    IEnumerator DebugDelayedAndTotallyUnexpectedAction(GameObject objectToPlayWith)
    {
        yield return new WaitForSecondsRealtime(8);
        //
        // Debug Code Here:
        //
    }
#endif
}
