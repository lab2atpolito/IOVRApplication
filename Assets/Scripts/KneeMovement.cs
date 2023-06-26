using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KneeMovement : MonoBehaviour
{
    [SerializeField]
    private float _maxDistance = 3f;
    [SerializeField]
    private float _rotationThreshold = 90f;

    [SerializeField]
    private Transform _kneeHint;

    [SerializeField]
    private float _initialRotationZ; 

    private void Start()
    {
        Vector3 anglesRotation = this.transform.localRotation.eulerAngles;
        float angleZ = anglesRotation.z;

        if (angleZ > 180f)
        {
            angleZ -= 360f;
        }

        _initialRotationZ = angleZ;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 direction = _kneeHint.position - this.transform.position;
        Debug.DrawRay(this.transform.position, direction, Color.green);

        Vector3 anglesRotation = this.transform.localRotation.eulerAngles;
        float angleZ = anglesRotation.z;

        if (angleZ > 180f)
        {
            angleZ -= 360f;
        }

        //Debug.Log(angleZ);

        float difference = angleZ - _initialRotationZ;

       
        float positionX = difference / _rotationThreshold * _maxDistance;
        _kneeHint.transform.localPosition = new Vector3(positionX, _kneeHint.transform.localPosition.y, _kneeHint.transform.localPosition.z);
    }
}