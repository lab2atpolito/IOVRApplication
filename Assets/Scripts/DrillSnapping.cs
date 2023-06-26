using System.Collections;
using System.Collections.Generic;
using Oculus.Interaction;
using Oculus.Interaction.HandGrab;
using UnityEngine;

public class DrillSnapping : MonoBehaviour
{
    [SerializeField] private GameObject _needle;

    //[SerializeField] private HandGrabInteractable _needleInteractable;

    // Hand Grab Interactors
    [SerializeField] private HandGrabInteractor _handInteractorL;
    [SerializeField] private HandGrabInteractor _handInteractorR;

    // Controllers Hand Grab Interactors
    [SerializeField] private HandGrabInteractor _controllerInteractorL;
    [SerializeField] private HandGrabInteractor _controllerInteractorR;

    [SerializeField] private PowerDrill _drill;

    private void OnTriggerEnter(Collider other)
    {
        if( other.tag == "Drill" )
        {
            if (_drill.IsEmpty())
            {
                if (SimulationManager.IsHandTrackingEnabled())
                {
                    // Hand Tracking Enabled

                    if (_handInteractorL.HasInteractable
                        && _handInteractorL.Interactable.RelativeTo == _needle.transform)
                    {
                        _handInteractorL.ForceRelease();
                    }
                    else if (_handInteractorR.HasInteractable
                        && _handInteractorR.Interactable.RelativeTo == _needle.transform)
                    {
                        _handInteractorR.ForceRelease();
                    }
                }
                else
                {
                    // Controllers Enabled

                    if (_controllerInteractorL.HasInteractable
                        && _controllerInteractorL.Interactable.RelativeTo == _needle.transform)
                    {
                        _controllerInteractorL.ForceRelease();
                    }
                    else if (_controllerInteractorR.HasInteractable
                        && _controllerInteractorR.Interactable.RelativeTo == _needle.transform)
                    {
                        _controllerInteractorR.ForceRelease();
                    }
                }

                //Needle snapping to drill tip
                _needle.GetComponent<Needle>().Snap();
            }
        }
    }
}
