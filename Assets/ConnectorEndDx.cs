using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction.HandGrab;
using DG.Tweening;

public class ConnectorEndDx : MonoBehaviour
{
    [SerializeField] private Transform _snapPoint;
    [SerializeField] private Transform _connectorEnd;
    [SerializeField] private Rigidbody _rigidbody;
    //[SerializeField] private Collider _collider;

    [SerializeField] private float _duration;
    [SerializeField] private bool _isSnapped;

    [SerializeField] private HandGrabInteractor _controllerInteractorL;
    [SerializeField] private HandGrabInteractor _controllerInteractorR;

    [SerializeField] private HandGrabInteractor _handInteractorL;
    [SerializeField] private HandGrabInteractor _handInteractorR;

    void Start()
    {
        _isSnapped = false;
    }

    IEnumerator SnappingCoroutine()
    {
        _rigidbody.isKinematic = true;

        float elapsedTime = 0f;
        while (elapsedTime < _duration)
        {
            float t = elapsedTime / _duration;

            Vector3 finalPosition = _snapPoint.position;
            Quaternion finalRotation = _snapPoint.rotation;

            _connectorEnd.position = Vector3.Lerp(_connectorEnd.position, finalPosition, t);
            _connectorEnd.rotation = Quaternion.Lerp(_connectorEnd.rotation, finalRotation, t);

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        _connectorEnd.rotation = _snapPoint.rotation;
        _connectorEnd.position = _snapPoint.position;
        _isSnapped = true;

        // The snapping trigger collider is disabled
        this.GetComponent<Collider>().enabled = false;

        _connectorEnd.parent = _snapPoint;
        _connectorEnd.DORotate(new Vector3(0f, 0f, 45f), _duration, RotateMode.LocalAxisAdd);
        _snapPoint.GetComponentInParent<Syringe>().Connect();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Syringe")
        {
            if (!SimulationManager.IsHandTrackingEnabled())
            {
                if (_controllerInteractorL.HasInteractable && _controllerInteractorL.Interactable.RelativeTo == _connectorEnd)
                {
                    _controllerInteractorL.ForceRelease();
                }
                else if(_controllerInteractorR.HasInteractable && _controllerInteractorR.Interactable.RelativeTo == _connectorEnd)
                {
                    _controllerInteractorR.ForceRelease();
                }
            }
            else
            {
                if (_handInteractorL.HasInteractable && _handInteractorL.Interactable.RelativeTo == _connectorEnd)
                {
                    _handInteractorL.ForceRelease();
                }
                else if((_handInteractorR.HasInteractable && _handInteractorR.Interactable.RelativeTo == _connectorEnd))
                {
                    _handInteractorR.ForceRelease();
                }
            }

            StartCoroutine(SnappingCoroutine());
        }
    }

    public void Grab()
    {
        if (_isSnapped) // the connector end is attached to the syringe
        {
            _isSnapped = false;
            //this.GetComponent<Collider>().enabled = true;
            _connectorEnd.parent = null;
            _snapPoint.GetComponentInParent<Syringe>().Disconnect();
        }
    }

    public bool IsSnapped()
    {
        return _isSnapped;
    }
}
