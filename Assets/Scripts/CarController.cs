using System;
using System.Collections;
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
    public float gravity = 10f;
    public float slantingModifier = 1.4f;
    
    [Header("Animation")]
    public float maxSteerAngle = 30f;
    public List<Axle> Axles;

    private float _speed, _currentSpeed;
    private float _rpm;
    private float _rotate, _currentRotate;
    
    private bool _drifting;
    private int _driftDirection;
    
    // Start is called before the first frame update
    /*void Start()
    {
        
    }*/

    // Update is called once per frame
    void Update()
    {
        // Animation & Movement
        transform.position = sphere.transform.position - new Vector3(0, 0.57f, 0);

        var rpm = Vector3.right * sphere.velocity.magnitude;
        foreach (var axle in Axles)
        {
            axle.leftWheel.Rotate(rpm);
            axle.rightWheel.Rotate(rpm);
        }
        
        // Acceleration
        if (Input.GetAxis("Vertical") != 0)
        {
            if (_currentSpeed is < 1 and > -1 && Input.GetAxis("Vertical") < 0)
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
                var wheelSteer = new Vector3(Axles[0].leftWheel.localEulerAngles.x, 
                                            dir * amount * maxSteerAngle, 
                                            0);
                Axles[0].leftWheel.localEulerAngles = wheelSteer;
                Axles[0].rightWheel.localEulerAngles = wheelSteer;
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
        _currentRotate = Mathf.Lerp(_currentRotate, _rotate, Time.deltaTime * 4f); 
        _rotate = 0f;
    }
    
    public void FixedUpdate()
    {
        //Forward Acceleration
        sphere.AddForce(transform.forward * _currentSpeed, ForceMode.Acceleration);
        
        // Gravity
        sphere.AddForce(Vector3.down * gravity, ForceMode.Force);
        
        // Steering * Normal Adjustment
        Physics.Raycast(transform.position + Vector3.right * 3.33f, 
            Vector3.down, 
            out var hit, 
            4.0f);

        // i hate this so much, yet it all somehow works:
        var slant = (Quaternion.FromToRotation(transform.up, hit.normal) * 
                         Quaternion.Euler(0, transform.eulerAngles.y, 0)).eulerAngles.x;
        slant -= Mathf.Ceil(slant / 360f - 0.5f) * 360f; // convert to (-180, 180]
        transform.rotation = Quaternion.Lerp(transform.rotation,
                                             Quaternion.Euler(slant * slantingModifier,    
                                                                transform.eulerAngles.y + _currentRotate, 
                                                                0), 
                                             Time.deltaTime * 5f);

        informationText.text = $"Speed: {_currentSpeed}";
    }

    private void Steer(int direction, float amount)
    {
        _rotate = steering * direction * amount;
    }
}
