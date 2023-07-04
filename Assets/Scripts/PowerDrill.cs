using System.Collections;
using System.Collections.Generic;
using Oculus.Interaction.HandGrab;
using UnityEngine;

public class PowerDrill : MonoBehaviour, IHandGrabUseDelegate
{
    private Needle _currentNeedle;
    private DrillState _state;
    private bool _isDrilling = false;

    [SerializeField] private Transform _tip;
    [SerializeField] private Animator _buttonAnimator;

    [SerializeField] private AudioSource _drillingSound;
    [SerializeField] private float _drillingSpeed = 20f;

    [SerializeField, Range(0f, 1f)] private float _maxDrillingVolume;
    [SerializeField, Range(0f, 1f)] private float _maxDrillingPitch;
    [SerializeField, Range(0f, 1f)] private float _maxDrillingVibration;

    [SerializeField] private HandGrabInteractor _controllerInteractorL;
    [SerializeField] private HandGrabInteractor _controllerInteractorR;

    void Start()
    {
        _currentNeedle = null;
        _state = DrillState.RELEASED;
    }

    void Update()
    {
        if(!IsEmpty())
        {
            State needleState = _currentNeedle.GetComponent<NeedleInteraction>().GetCurrentState();
            switch (needleState)
            {
                case State.IN_AIR:
                    _maxDrillingPitch = 1f;
                    _maxDrillingVibration = 0.5f;
                    break;

                case State.IN_SKIN:
                    _maxDrillingPitch = 0.7f;
                    _maxDrillingVibration = 0.4f;
                    break;

                case State.IN_BONE:
                    _maxDrillingPitch = 0.5f;
                    _maxDrillingVibration = 0.2f;
                    break;

                case State.MEDULLARY_CAVITY:
                    _maxDrillingPitch = 0.9f;
                    _maxDrillingVibration = 0.5f;
                    break;
            }
        }
        else
        {
            _maxDrillingPitch = 1f;
            _maxDrillingVibration = 0.5f;
        }

        if (!SimulationManager.IsHandTrackingEnabled() && _state == DrillState.GRABBED) // the drill is grabbed by the user, so the trigger can be pressed
        {
            // Controllers Enabled
            OVRInput.Controller grabbingController;
            float _triggerPressure;

            if( _controllerInteractorL.HasInteractable && _controllerInteractorL.Interactable.RelativeTo == this.transform )
            {
                grabbingController = OVRInput.Controller.LTouch;
                _triggerPressure = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger);
            }
            else
            {
                grabbingController = OVRInput.Controller.RTouch;
                _triggerPressure = OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger);
            }

            // Trigger pressed
            if(_triggerPressure > 0f)
            {
                if(!_isDrilling)
                {
                    _isDrilling = true;
                    _drillingSound.Play();
                }
                else
                {
                    _buttonAnimator.SetFloat("Blend", _triggerPressure);

                    _tip.Rotate(Vector3.up, _drillingSpeed * _triggerPressure * Time.deltaTime, Space.Self);

                    _drillingSound.pitch = _triggerPressure * _maxDrillingPitch;
                    _drillingSound.volume = _triggerPressure * _maxDrillingVolume;

                    OVRInput.SetControllerVibration(1f, _triggerPressure * _maxDrillingVibration, grabbingController);
                }
            }
            // Trigger not pressed
            else
            {
                if(_isDrilling)
                {
                    _isDrilling = false;
                    _buttonAnimator.SetFloat("Blend", 0f);
                    _drillingSound.Stop();
                    OVRInput.SetControllerVibration(0f, 0f, grabbingController);
                }
            }
        }
    }

    public void InsertNeedle(Needle needle)
    {
        _currentNeedle = needle;
        _currentNeedle.gameObject.GetComponent<SnapToTargetPoint>().SnapToPoint();
        Debug.Log("New Needle Snapped!");
    }

    public void RemoveNeedle()
    {
        _currentNeedle.ClearParent();
        _currentNeedle = null;
        Debug.Log("The drill is empty!");
    }

    public bool IsEmpty()
    {
        return _currentNeedle == null;
    }

    public void Grab()
    {
        _state = DrillState.GRABBED;
        Debug.Log("Drill state: "+ _state);
    }

    public void Release()
    {
        _state = DrillState.RELEASED;
        _drillingSound.Stop();
        _isDrilling = false;
        Debug.Log("Drill state: "+ _state);
    }

    public void BeginUse()
    {
        Debug.Log("Interaction Use Begin!");
        _drillingSound.Play();
        _isDrilling = true; 
    }

    public void EndUse()
    {
        Debug.Log("Interaction Use End!");
        _drillingSound.Stop();
        _isDrilling = false;
    }

    public float ComputeUseStrength(float strength)
    {
        _buttonAnimator.SetFloat("Blend", strength);
        _tip.Rotate(Vector3.up, _drillingSpeed * strength * Time.deltaTime, Space.Self);
        _drillingSound.pitch = strength * _maxDrillingPitch;
        _drillingSound.volume = strength * _maxDrillingVolume;
        return strength;
    }

    public bool IsGrabbed()
    {
        return _state == DrillState.GRABBED;
    }

    public bool IsDrilling()
    {
        return _isDrilling; 
    }

    public Needle GetNeedle()
    {
        return _currentNeedle; 
    }
}

enum DrillState
{
    GRABBED,
    RELEASED,
}
