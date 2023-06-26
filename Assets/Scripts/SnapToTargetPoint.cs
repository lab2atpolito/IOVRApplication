using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnapToTargetPoint : MonoBehaviour
{
    [SerializeField]
    private Transform _targetPoint;
    [SerializeField]
    private float _animationDuration;

    public void SnapToPoint()
    {
        StartCoroutine(SnappingCoroutine());
    }

    IEnumerator SnappingCoroutine()
    {
        Transform transform = this.transform;

        float elapsedTime = 0f;
        while(elapsedTime < _animationDuration)
        {
            float t = elapsedTime / _animationDuration;

            Vector3 finalPosition = _targetPoint.position;
            Quaternion finalRotation = _targetPoint.rotation;

            transform.position = Vector3.Lerp(transform.position, finalPosition, t);
            transform.rotation = Quaternion.Lerp(transform.rotation, finalRotation, t);

            elapsedTime += Time.deltaTime;

            yield return null; 
        }

        transform.rotation = _targetPoint.rotation; 
        transform.position = _targetPoint.position;
        transform.parent = _targetPoint;
    }
}
