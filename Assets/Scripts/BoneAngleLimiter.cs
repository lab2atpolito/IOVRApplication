using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoneAngleLimiter : MonoBehaviour
{
    [SerializeField]
    private Transform _legBone;

    [SerializeField]
    private float _maxAngle = 160f;

    [SerializeField]
    private float _minAngle = 45f;

    [SerializeField]
    private bool _obtuseAngle = false;



    private void LateUpdate()
    {
        Vector3 footDirection = this.transform.up;
        Debug.DrawRay(this.transform.position, footDirection, Color.red);

        Vector3 legDirection = _legBone.up;
        legDirection = Vector3.Reflect(legDirection, legDirection.normalized);
        Debug.DrawRay(this.transform.position, legDirection, Color.red);

        // Angle between foot bone and leg bone
        float angle = Vector3.SignedAngle(legDirection, footDirection, Vector3.forward);

        //Debug.Log(angle);

        if(angle > 0)
        {
            angle = 360 - angle;
        }
        else
        {
            angle *= -1;
        }

        if (_obtuseAngle)
        {
            angle = 360 - angle;
        }

        //Debug.Log(angle);

        if (angle > _maxAngle)
        {
            float rotationAngle = angle - _maxAngle;
            this.transform.Rotate(new Vector3(rotationAngle, 0f, 0f), Space.Self);
        }
        else if(angle < _minAngle)
        {
            float rotationAngle = _minAngle - angle;
            this.transform.Rotate(new Vector3(rotationAngle, 0f, 0f), Space.Self);
        }
    }
}
