using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[Serializable]
public class Axle 
{
    public Transform leftWheel;
    public Transform rightWheel;
}

public class CarController : MonoBehaviour
{
    public TMP_Text informationText;

    [Header("Physics")]
    public Rigidbody sphere;
    public float acceleration = 30f;
    public float steering = 80f;
    public float tilting = 1f;
    public float gravity = 10f;
    public float slantingModifier = 1.4f;
    
    [Header("Animation")]
    public float maxSteerAngle = 30f;
    public Transform suspension;
    public List<Axle> axles;

    private float _speed, _currentSpeed;
    private float _rpm;
    private float _yRotate, _currentYRotate;
    private float _zRotate, _currentZRotate;
    
    private bool _drifting;
    private int _driftDirection;

    private Transform _transform;
    
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
        
        // Acceleration
        if (Input.GetAxis("Vertical") != 0)
        {
            if (Input.GetAxis("Vertical") < 0)
            {
                // Hand brake
                _speed = 0;
            }
            else
            {
                _speed = acceleration * Input.GetAxis("Vertical");
            }
        }
        //

        // Steering
        if (Input.GetAxis("Horizontal") != 0)
        {
            int dir = Input.GetAxis("Horizontal") > 0 ? 1 : -1;
            float amount = Mathf.Abs(Input.GetAxis("Horizontal"));
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
            _drifting = false;
        }
        //

        _currentSpeed = Mathf.SmoothStep(_currentSpeed, _speed, Time.deltaTime * 12f); 
        _speed = 0f;
        _currentYRotate = Mathf.Lerp(_currentYRotate, _yRotate, Time.deltaTime * 4f); 
        _yRotate = 0f;
        _currentZRotate = Mathf.Lerp(_currentZRotate, _zRotate, Time.deltaTime * 12f); 
        _zRotate = 0f;
    }
    
    public void FixedUpdate()
    {
        // Forward Acceleration
        sphere.AddForce(_transform.forward * _currentSpeed, ForceMode.Acceleration);
        
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
                                                                _transform.eulerAngles.y + _currentYRotate, 
                                                                _currentZRotate), 
                                             Time.deltaTime * 5f);

        informationText.text = $"Speed: {_currentSpeed}";
    }

    private void Steer(int direction, float amount)
    {
        _zRotate = tilting * direction * amount;
        _yRotate = steering * direction * amount;
    }
}
