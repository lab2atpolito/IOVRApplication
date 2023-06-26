using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SnappingWhenNotInUse : MonoBehaviour
{
    [SerializeField] private Transform _snappingTransform;
    [SerializeField] private float _timeOut;

    private float _lastReleasedTime;
    private bool _isGrabbed;
    private bool _isSnapped; 

    void Start()
    {
        _isGrabbed = false;
        _isSnapped = true;
    }

    void Update()
    {
        if (!_isGrabbed && !_isSnapped)
        {
            float timeSinceRelease = Time.time - _lastReleasedTime;

            if(timeSinceRelease >= _timeOut)
            {
                _isSnapped = true;
                this.transform.DOMove(_snappingTransform.position, 1f).SetEase(Ease.InOutSine);
                this.transform.DORotate(_snappingTransform.rotation.eulerAngles, 1f).SetEase(Ease.InOutSine);
            }
        }
        
    }

    public void Grab()
    {
        _isGrabbed = true;
        _isSnapped = false;
    }

    public void Release()
    {
        _isGrabbed = false;
        _lastReleasedTime = Time.time;
    }
}
