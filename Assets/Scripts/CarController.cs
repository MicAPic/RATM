using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    [Space]
    public float maxSteerAngle = 30f;
    public float currentSpeed;
    private float _speed;
    private float _rpm;
    public float currentYRotate;
    private float _yRotate;
    public float currentZRotate;
    private float _zRotate;

    private bool _drifting;
    private int _driftDirection;

    private Transform _transform;
    private float _maxVelocity;
    
    // Start is called before the first frame update
    void Start()
    {
        _transform = transform;
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
            axle.leftWheel.Rotate(rpm);
            axle.rightWheel.Rotate(rpm);
        }
        
        if (!canProcessInput) return;
        
        // Acceleration & relevant audio
        if (Input.GetAxis("Vertical") != 0)
        {
            if (Input.GetAxis("Vertical") < 0)
            {
                if (currentSpeed > 0)
                {
                    // brake
                    _speed = brakeModifier * acceleration * Input.GetAxis("Vertical");

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
                    _speed = reverseModifier * acceleration * Input.GetAxis("Vertical");
                    StartAudioFade(wheelAudioSource, 0.0f, fadeOutDuration);
                }
            }
            else
            {
                _speed = acceleration * Input.GetAxis("Vertical");
            }
        }
        else
        {
            StartAudioFade(engineAudioSource, 0.0f, fadeOutDuration, true);
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            engineAudioSource.PlayOneShot(engineStartClip);
        }
        if (Input.GetKey(KeyCode.W))
        {
            StartAudioFade(engineAudioSource, 1.0f, fadeInDuration, true);
        }
        if (Input.GetKey(KeyCode.S))
        {
            if (currentSpeed < 0.0f)
            {
                backlights.SetActive(false);
                StartAudioFade(engineAudioSource, 0.85f, fadeInDuration, true);
            }
            else
            {
                backlights.SetActive(true);
            }
        }
        else
        {
            backlights.SetActive(false);
        }
        //

        // Steering
        if (Input.GetAxis("Horizontal") != 0 && Mathf.Abs(currentSpeed) - 3.0f > 0) 
        {
            // the second condition is added to prevent steering when the car has stopped
            
            var dir = Input.GetAxis("Horizontal") > 0 ? 1 : -1;
            var amount = Mathf.Abs(Input.GetAxis("Horizontal"));
            Steer(dir, amount);
            {
                var wheelSteer = new Vector3(axles[0].leftWheel.localEulerAngles.x, 
                                             dir * amount * maxSteerAngle, 
                                             0);
                axles[0].leftWheel.localEulerAngles = wheelSteer;
                axles[0].rightWheel.localEulerAngles = wheelSteer;
            }
        }
        //
        
        // Drifting
        if (Input.GetButtonDown("Jump") && !_drifting && Input.GetAxis("Horizontal") != 0)
        {
            wheelAudioSource.clip = screechClips[Random.Range(0, screechClips.Length)];
            wheelAudioSource.Play();
            StartAudioFade(wheelAudioSource, 1.0f, fadeInDuration);
            
            _drifting = true;
            _driftDirection = Input.GetAxis("Horizontal") > 0 ? 1 : -1;
        }
        
        if (_drifting)
        {
            float control = _driftDirection == 1 ? 
                ExtensionMethods.Remap(Input.GetAxis("Horizontal"), -1, 1, 0, 2) : 
                ExtensionMethods.Remap(Input.GetAxis("Horizontal"), -1, 1, 2, 0);
            Steer(_driftDirection, control);
        }
        
        if (Input.GetButtonUp("Jump") && _drifting)
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
