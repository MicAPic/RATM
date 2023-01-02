using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bumper : MonoBehaviour
{
    private Rigidbody _sphere;
    private Vector3 _defaultPos;
    
    // Start is called before the first frame update
    void Start()
    {
        _sphere = GetComponentInParent<CarController>().sphere;
        _defaultPos = transform.localPosition;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(transform.name + " bumped into: " + collision.transform.name);
        
        if (collision.rigidbody == _sphere) return;
        
        _sphere.AddForce(collision.impulse.normalized, ForceMode.Impulse);
        StartCoroutine(ReturnToDefaultPos());
    }

    private IEnumerator ReturnToDefaultPos()
    {
        yield return new WaitForSeconds(1.0f);
        transform.localPosition = _defaultPos;
    }
}
