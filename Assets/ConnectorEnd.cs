using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Oculus.Interaction.HandGrab;

public class ConnectorEnd : MonoBehaviour
{
    [SerializeField] private Transform _snapZone;
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

    private void OnTriggerEnter(Collider other)
    {
        if( other.gameObject.tag == "Stabilizer")
        {
            if (!SimulationManager.IsHandTrackingEnabled())
            {
                if (_controllerInteractorL.HasInteractable && _controllerInteractorL.Interactable.RelativeTo == this.transform)
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
                if (_handInteractorL.HasInteractable && _handInteractorL.Interactable.RelativeTo == this.transform)
                {
                    _handInteractorL.ForceRelease();
                }
                else
                {
                    _handInteractorR.ForceRelease();
                }
            }

            this.transform.DOMove(_snapZone.position, _duration);
            Vector3 newRotation = _snapZone.rotation.eulerAngles;
            this.transform.DORotate(newRotation, _duration).OnComplete(() => {
                Snap();
            });
        } 
    }

    private void Snap()
    {
        _isSnapped = true;
        this.GetComponentInChildren<Collider>().enabled = false; //non si può più interagire con l'end connector
        this.transform.parent = _snapZone;
        this.transform.DORotate(new Vector3(0f, 45f, 0f), _duration, RotateMode.LocalAxisAdd);
    }

    public bool IsSnapped()
    {
        return _isSnapped;
    }
}
