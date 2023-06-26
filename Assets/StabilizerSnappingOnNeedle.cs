using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction.HandGrab;

public class StabilizerSnappingOnNeedle : MonoBehaviour
{
    private Collider _collider;
    private NeedleInteraction _needle;

    [SerializeField] private float _duration;
    [SerializeField] private bool _isSnapped;

    [SerializeField] private HandGrabInteractor _controllerInteractorL;
    [SerializeField] private HandGrabInteractor _controllerInteractorR;

    [SerializeField] private HandGrabInteractor _handInteractorL;
    [SerializeField] private HandGrabInteractor _handInteractorR;

    [SerializeField] private Transform _stabilizer;
    [SerializeField] private Transform _snapZone;

    void Start()
    {
        _collider = this.GetComponent<Collider>();
        _needle = this.GetComponentInParent<NeedleInteraction>();
        _isSnapped = false; 
    }

    void Update()
    {
        // Is possible to snap the stabilizer to the top part of the needle only when the needle is not in skin/bone/medullary cavity
        if( _needle.GetCurrentState() == State.IN_AIR)
        {
            _collider.enabled = false;
        }
        else
        {
            _collider.enabled = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if( other.gameObject.tag == "Stabilizer" )
        {
            // Disable gravity for stickers to avoid bugs when colliding with the leg
            _stabilizer.GetComponent<Stabilizer>().DisableGravity();
            Debug.Log("Stabilizer physics disabled!");

            if (!SimulationManager.IsHandTrackingEnabled())
            {
                if (_controllerInteractorL.HasInteractable && _controllerInteractorL.Interactable.RelativeTo == _stabilizer)
                {
                    _controllerInteractorL.ForceRelease();
                }
                else
                {
                    _controllerInteractorR.ForceRelease();
                }
            }
            else
            {
                if (_handInteractorL.HasInteractable && _handInteractorL.Interactable.RelativeTo == _stabilizer)
                {
                    _handInteractorL.ForceRelease();
                }
                else
                {
                    _handInteractorR.ForceRelease();
                }
            }
            StartCoroutine(SnappingCoroutine());
        }
    }

    IEnumerator SnappingCoroutine()
    {
        float elapsedTime = 0f;

        Vector3 finalPosition = _snapZone.position;
        Quaternion finalRotation = _snapZone.rotation;

        while (elapsedTime < _duration)
        {
            float t = elapsedTime / _duration;

            _stabilizer.position = Vector3.Lerp(_stabilizer.position, finalPosition, t);
            _stabilizer.rotation = Quaternion.Lerp(_stabilizer.rotation, finalRotation, t);

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        _stabilizer.rotation = finalRotation;
        _stabilizer.position = finalPosition;
        _stabilizer.parent = this.transform;
        _isSnapped = true;
        _stabilizer.GetComponent<Stabilizer>().AttachToNeedle();
    }
}
