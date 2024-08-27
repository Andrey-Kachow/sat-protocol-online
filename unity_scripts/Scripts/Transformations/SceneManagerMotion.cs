using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManagerMotion : MonoBehaviour
{
    public float Amplitude;
    public float Period;

    private float _angularVelocity;
    private float _amplitudeTimesAngularVelocity;

    void Start()
    {
        _angularVelocity = 2 * Mathf.PI / Period;
        _amplitudeTimesAngularVelocity = Amplitude * _angularVelocity;
    }

    void Update()
    {
        float velocity = _amplitudeTimesAngularVelocity * Mathf.Cos(_angularVelocity * Time.timeSinceLevelLoad);
        transform.Translate(Time.deltaTime * velocity * Vector3.up, Space.World);
    }
}
