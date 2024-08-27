using System.Collections;
using UnityEngine;

class ControlledMotionUtils
{
    private static readonly AnimationClip _inplaceWalkingAnimationClip = ChildBehaviorCache.Instance.InplaceWalking.Get().AnimationClips[0];
    private static readonly AnimationClip _transitionAnimation = ChildBehaviorCache.Instance.InplaceWalking.Get().AnimationClips[1];
    
    public static IEnumerator WalkToTheTargetPosition(Vector3 targetPosition, float duration, ChildAvatar avatar)
    {
        Vector3 requiredVelocity = (targetPosition - avatar.transform.position) / duration;
        yield return FaceTowards(requiredVelocity, 1f, avatar);
        
        avatar.AnimationSwitcher.PlayAnimationClip(_inplaceWalkingAnimationClip);


        requiredVelocity = requiredVelocity.normalized * Mathf.Max(2f, requiredVelocity.magnitude);

        float elapsedTime = 0.0f;
        while (elapsedTime < duration)
        {
            avatar.CharacterController.Move(requiredVelocity * Time.deltaTime);
            Vector3 AvatarHorizontalPosition = avatar.transform.position;
            AvatarHorizontalPosition.y = 0;
            if (Vector3.Distance(targetPosition, AvatarHorizontalPosition) < 0.25f)
            {
                break;
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        avatar.AnimationSwitcher.PlayAnimationClip(_transitionAnimation);
    }

    public static IEnumerator FaceTowards(Vector3 targetFacingDirection, float duration, ChildAvatar avatar)
    {
        // Calculate the target angle in degrees
        float targetAngle = Mathf.Atan2(targetFacingDirection.x, targetFacingDirection.z) * Mathf.Rad2Deg;

        float currentAngle = avatar.transform.eulerAngles.y;
        float angleDifference = Mathf.DeltaAngle(currentAngle, targetAngle);

        targetAngle = currentAngle + angleDifference;

        Quaternion initialRotation = avatar.transform.rotation;
        Quaternion targetRotation = Quaternion.Euler(0, targetAngle, 0);

        float elapsedTime = 0.0f;

        while (elapsedTime < duration)
        {
            avatar.transform.rotation = Quaternion.Slerp(initialRotation, targetRotation, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        avatar.transform.rotation = targetRotation;
    }
}