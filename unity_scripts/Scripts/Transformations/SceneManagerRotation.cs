using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManagerRotation : MonoBehaviour
{
    private static readonly Vector3 _regularRotateAmount = new (37, 29, 43);
    private static readonly Vector3 _fasterRotateAmount = new (307, 467, 691);

    private const float _fastSpinDuration = 0.3f;

    private Vector3 _currentRotateAmount = _regularRotateAmount;
    private Vector3 _initialScale;

    private void Start()
    {
        _initialScale = transform.localScale;
    }
    void Update()
    {
        transform.Rotate(_currentRotateAmount * Time.deltaTime);
    }

    internal IEnumerator SpinFastAndShrink(float acelerationDuration = 1f, bool backwards = false)
    {
        Vector3 startSpin;
        Vector3 endSpin;
        Vector3 startScale;
        Vector3 endScale;
        if (backwards)
        {
            startSpin = _fasterRotateAmount;
            endSpin = _regularRotateAmount;
            startScale = Vector3.zero;
            endScale = _initialScale;
        }
        else
        {
            startSpin = _regularRotateAmount;
            endSpin = _fasterRotateAmount;
            startScale = _initialScale;
            endScale = Vector3.zero;
        }
        float elapsedTime = 0f;
        while (elapsedTime < acelerationDuration)
        {
            float linearInterpolationRatio = elapsedTime / acelerationDuration;
            _currentRotateAmount = Vector3.Lerp(startSpin, endSpin, linearInterpolationRatio);
            transform.localScale = Vector3.Lerp(startScale, endScale, linearInterpolationRatio);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        _currentRotateAmount = endSpin;
        transform.localScale = endScale;

        yield return new WaitForSeconds(_fastSpinDuration);
    }
}
