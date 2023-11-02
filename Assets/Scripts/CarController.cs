using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

[Serializable]
public class Axle 
{
    public Transform leftWheel;
    public Transform rightWheel;
}

public class CarController : MonoBehaviour
{
    [Header("General")] 
    public bool canProcessInput;

    [Header("Physics")]
    public Rigidbody sphere;
    public float acceleration = 30f;
    public float steering = 80f;
    public float tilting = 1f;
    public float gravity = 10f;
    [Range(0.1f, 2.0f)]
    public float slantingModifier = 1.4f;
    [Range(0.1f, 2.0f)]
    public float brakeModifier = 1.2f;
    [Range(0.1f, 2.0f)]
    public float reverseModifier = 0.5f;

    [Header("Audio")] 
    public float fadeInDuration;
    public float fadeOutDuration;
    [SerializeField] 
    private AudioClip[] screechClips;
    [SerializeField] 
    private AudioClip engineStartClip;
    [SerializeField] 
    private AudioSource wheelAudioSource;
    [SerializeField] 
    private AudioSource engineAudioSource;
    
    private Coroutine _currentEngineCoroutine;
    private Coroutine _currentScreechCoroutine;

    [Header("Animation")] 
    public GameObject backlights;
    public Transform suspension;
    public List<Axle> axles;
    public float wheelRotationSpeed;
    [Space]
    public float maxSteerAngle = 30f;
    public float currentSpeed;
    private float _speed;
    private float _rpm;
    public float currentYRotate;
    private float _yRotate;
    public float currentZRotate;
    private float _zRotate;

    [Header("Input")]
    private float _accelerationInputValue;
    private float _brakingInputValue;
    private float _steeringInputValue;
    private bool _drifting;
    private int _driftDirection;
    private PlayerInput _playerInput;

    private Transform _transform;
    private float _maxVelocity;
    
    void Awake()
    {
        _transform = transform;
        _playerInput = GetComponent<PlayerInput>();
    }
    
    void OnAccelerate(InputValue value)
    {
        _accelerationInputValue = value.Get<float>();
    }
    
    void OnBrakeReverse(InputValue value)
    {
        _brakingInputValue = value.Get<float>();
    }

    void OnSteer(InputValue value)
    {
        _steeringInputValue = value.Get<float>();
    }

    // Update is called once per frame
    void Update()
    {
        // Animation & Movement
        _transform.position = sphere.transform.position - new Vector3(0, 0.57f, 0);
        suspension.position = _transform.position;
        suspension.localRotation = Quaternion.Euler(_transform.localEulerAngles.x, 
                                                    _transform.localEulerAngles.y, 
                                                    suspension.localEulerAngles.z);

        var rpm = Vector3.right * sphere.velocity.magnitude;
        foreach (var axle in axles)
        {
            axle.leftWheel.Rotate(rpm * (Time.deltaTime * wheelRotationSpeed));
            axle.rightWheel.Rotate(rpm * (Time.deltaTime * wheelRotationSpeed));
        }
        
        if (!canProcessInput) return;
        
        // Acceleration & relevant audio
        if (_brakingInputValue > 0)
        {
            if (currentSpeed > 0)
            {
                // brake
                _speed = brakeModifier * acceleration * -_brakingInputValue;
                backlights.SetActive(true);

                StartAudioFade(engineAudioSource, 0.0f, fadeOutDuration, true);
                if (Mathf.Abs(sphere.velocity.magnitude) > 25.0f)
                {
                    wheelAudioSource.clip = screechClips[Random.Range(0, screechClips.Length)];
                    wheelAudioSource.Play();
                    StartAudioFade(wheelAudioSource, 0.65f, fadeInDuration);
                }
            }
            else
            {
                // reverse driving
                _speed = reverseModifier * acceleration * -_brakingInputValue;
                backlights.SetActive(false);
                StartAudioFade(engineAudioSource, 0.85f, fadeInDuration, true);
                StartAudioFade(wheelAudioSource, 0.0f, fadeOutDuration);
            }
        }
        else if (_accelerationInputValue > 0)
        {
            _speed = acceleration * _accelerationInputValue;
            StartAudioFade(engineAudioSource, 1.0f, fadeInDuration, true);
            
            if (Time.timeScale > 0.0f)
            {
                backlights.SetActive(false);
            }
        }
        else
        {
            StartAudioFade(engineAudioSource, 0.0f, fadeOutDuration, true);
        }
        

        if (_playerInput.actions["Accelerate"].WasPressedThisFrame())
        {
            engineAudioSource.PlayOneShot(engineStartClip);
        }
        //

        if (_steeringInputValue != 0)
        {
            // Steering
            if (Mathf.Abs(currentSpeed) - 3.0f > 0)
            {
                // the second condition is added to prevent steering when the car has stopped
                var dir = _steeringInputValue > 0 ? 1 : -1;
                if (currentSpeed < 0)
                {
                    // steering controls are reversed
                    dir = -dir;
                }
                var amount = Mathf.Abs(_steeringInputValue);
                Steer(dir, amount);
                if (Time.timeScale > 0)
                {
                    var wheelSteer = new Vector3(axles[0].leftWheel.localEulerAngles.x,
                        dir * amount * maxSteerAngle,
                        0);
                    axles[0].leftWheel.localEulerAngles = wheelSteer;
                    axles[0].rightWheel.localEulerAngles = wheelSteer;
                }
            }

            // Drifting (start)
            if (_playerInput.actions["Drift"].WasPressedThisFrame() && !_drifting)
            {
                wheelAudioSource.clip = screechClips[Random.Range(0, screechClips.Length)];
                wheelAudioSource.Play();
                StartAudioFade(wheelAudioSource, 1.0f, fadeInDuration);

                _drifting = true;
                _driftDirection = _steeringInputValue > 0 ? 1 : -1;
            }
        }
        
        // Drifting (process)
        if (_drifting)
        {
            float control = _driftDirection == 1 ? 
                _steeringInputValue.Remap(-1, 1, 0, 2) : 
                _steeringInputValue.Remap(-1, 1, 2, 0);
            Steer(_driftDirection, control);
        }
        
        // Drifting (end)
        if (_playerInput.actions["Drift"].WasReleasedThisFrame() && _drifting)
        {
            StartAudioFade(wheelAudioSource, 0.0f, fadeOutDuration);
            _drifting = false;
        }
        //

        currentSpeed = Mathf.SmoothStep(currentSpeed, _speed, Time.deltaTime * 12.0f); 
        _speed = 0.0f;
        currentYRotate = Mathf.Lerp(currentYRotate, _yRotate, Time.deltaTime * 4.0f); 
        _yRotate = 0.0f;
        currentZRotate = Mathf.Lerp(currentZRotate, _zRotate, Time.deltaTime * 12.0f); 
        _zRotate = 0.0f;
    }
    
    public void FixedUpdate()
    {
        // Forward Acceleration
        sphere.AddForce(_transform.forward * currentSpeed, ForceMode.Acceleration);
        
        // Gravity
        sphere.AddForce(Vector3.down * gravity, ForceMode.Force);
        
        // Steering * Normal Adjustment
        Physics.Raycast(_transform.position + Vector3.right * 3.33f, 
            Vector3.down, 
            out var hit, 
            4.0f);

        // i hate this so much, yet it all somehow works:
        var slant = (Quaternion.FromToRotation(_transform.up, hit.normal) * 
                         Quaternion.Euler(0, _transform.eulerAngles.y, 0)).eulerAngles.x;
        slant -= Mathf.Ceil(slant / 360f - 0.5f) * 360f; // convert to (-180, 180]
        _transform.rotation = Quaternion.Lerp(_transform.rotation,
                                             Quaternion.Euler(slant * slantingModifier,    
                                                                _transform.eulerAngles.y + currentYRotate, 
                                                                currentZRotate), 
                                             Time.deltaTime * 5f);

        // Debug.Log(currentSpeed);
    }

    private void Steer(int direction, float amount)
    {
        _zRotate = tilting * direction * amount;
        _yRotate = steering * direction * amount;
    }

    private void StartAudioFade(AudioSource source, float targetVolume, float duration, bool isEngine=false)
    {
        if (isEngine)
        {
            if (_currentEngineCoroutine != null)
            {
                StopCoroutine(_currentEngineCoroutine);
            }

            _currentEngineCoroutine = StartCoroutine(CarAudioFade(source, targetVolume, duration));
        }
        else
        {
            if (_currentScreechCoroutine != null)
            {
                StopCoroutine(_currentScreechCoroutine);
            }

            _currentScreechCoroutine = StartCoroutine(CarAudioFade(source, targetVolume, duration));
        }
    }

    private IEnumerator CarAudioFade(AudioSource source, float targetVolume, float duration)
    {
        float time = 0;
        var start = source.volume;
        
        while (time < duration)
        {
            time += Time.deltaTime;
            source.volume = Mathf.Lerp(start, targetVolume, time / duration);
            yield return null;
        }
    }
}
