using System.Collections;
using UnityEngine;

class IdleMessingAroundBehavior : SimpleBehavior
{
    private readonly ChildAvatar _childAvatar;
    private Vector3 _referencePosition;
    private Coroutine _rotating;

    private Vector3 Position
    { 
        get => _childAvatar.transform.position;
        set => _childAvatar.transform.position = value; 
    }
    private Quaternion Rotation 
    { 
        get => _childAvatar.transform.rotation;
        set => _childAvatar.transform.rotation = value;
    }
    public IdleMessingAroundBehavior(ChildAvatar childAvatar)
        : base(childAvatar.AnimationSwitcher, ChildBehaviorCache.Instance.IdleMessingAround.Get())
    {
        _childAvatar = childAvatar;
        _referencePosition = _childAvatar.transform.position;
    }

    public override IEnumerator PerformAction()
    {
        if (_rotating == null)
        {
            _rotating = _childAvatar.StartCoroutine(AdjustMotion());
        }
        yield return base.PerformAction();
    }

    private IEnumerator AdjustMotion()
    {
        float rotationDuration = 3f;
        while (true)
        {      
            if (0.3 < Vector3.Distance(_referencePosition, _childAvatar.transform.position))
            {
                Vector3 directionToTarget = _referencePosition - _childAvatar.transform.position;
                directionToTarget.y = 0; // Keep rotation on the horizontal plane

                // Calculate the target angle in degrees
                float targetAngle = Mathf.Atan2(directionToTarget.x, directionToTarget.z) * Mathf.Rad2Deg;

                float currentAngle = _childAvatar.transform.eulerAngles.y;
                float angleDifference = Mathf.DeltaAngle(currentAngle, targetAngle);

                targetAngle = currentAngle + angleDifference;

                Quaternion initialRotation = Rotation;
                Quaternion targetRotation = Quaternion.Euler(0, targetAngle, 0);

                float elapsedTime = 0.0f;

                while (elapsedTime < rotationDuration)
                {
                    Rotation = Quaternion.Slerp(initialRotation, targetRotation, elapsedTime / rotationDuration);
                    elapsedTime += Time.deltaTime;
                    yield return null;
                }

                Rotation = targetRotation;
                _referencePosition = Position;
            } 
            else 
            {
                yield return new WaitForSeconds(2);
            }
        }
    }

    public override void StopCoroutinesRelated()
    {
        base.StopCoroutinesRelated();
        if (_rotating != null)
        {
            _childAvatar.StopCoroutine(_rotating);
        }
    }
}