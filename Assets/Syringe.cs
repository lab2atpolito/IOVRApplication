using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction.HandGrab;

public class Syringe : MonoBehaviour, IHandGrabUseDelegate
{
    [SerializeField] private Animator _syringeAnimator;
    [SerializeField] private SyringeState _state;

    [SerializeField] private HandGrabInteractor _controllerInteractorL;
    [SerializeField] private HandGrabInteractor _controllerInteractorR;

    [SerializeField] private Material _filling;

    public bool _isConnected = false;
    private bool _isFlushed = false;

    private float _resistanceValue;
    private float _pressingSpeed = 0.2f;
    private float _pressingValue;

    public void Grab()
    {
        _state = SyringeState.GRABBED;
    }

    public void Release()
    {
        _state = SyringeState.RELEASED;
    }

    public bool IsFlushed()
    {
        return _isFlushed;
    }

    public void Connect()
    {
        _isConnected = true;
    }

    public void Disconnect()
    {
        _isConnected = false;
    }

    public void BeginUse()
    {
    }

    public float ComputeUseStrength(float strength)
    {
        //Debug.Log(strength);
        if (_isConnected)
        {
            float resistance = Mathf.Pow(_pressingValue, _resistanceValue);

            _pressingValue += _pressingSpeed * strength * Time.deltaTime;
            _pressingValue *= resistance;
            _pressingValue = Mathf.Clamp01(_pressingValue);

            _filling.SetFloat("_Fill", 1f - _pressingValue);
            _syringeAnimator.SetFloat("Blend", _pressingValue);

            if( _pressingValue >= 1f)
            {
                _isFlushed = true;
            }
        }
        return strength;
    }

    public void EndUse()
    {
    }

    // Start is called before the first frame update
    void Start()
    {
        _filling.SetFloat("_Fill", 1f);
       _state = SyringeState.RELEASED;
        _isFlushed = false;
        _pressingValue = 0f;
        _resistanceValue = 0f;
    }

    void Update()
    {
        if (!SimulationManager.IsHandTrackingEnabled() && _state == SyringeState.GRABBED && _isConnected) // the syringe is grabbed by the user, so the inner part can be pressed
        {
            // Controllers Enabled
            OVRInput.Controller grabbingController;
            float _triggerPressure;

            if (_controllerInteractorL.HasInteractable && _controllerInteractorL.Interactable.RelativeTo == this.transform)
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
            if (_triggerPressure > 0f)
            {
                _resistanceValue += _pressingSpeed * Time.deltaTime;
                _resistanceValue = Mathf.Clamp01(_resistanceValue);
                Debug.Log("Resistance Factor: " + _resistanceValue);

                _pressingValue += _pressingSpeed * _triggerPressure * _resistanceValue * Time.deltaTime;
                _pressingValue = Mathf.Clamp01(_pressingValue);
                Debug.Log("Pressing Value: "+ _pressingValue);

                _syringeAnimator.SetFloat("Blend", _pressingValue);
                _filling.SetFloat("_Fill", 1f - _pressingValue);

                if (_pressingValue < 1f)
                    OVRInput.SetControllerVibration(1f, _triggerPressure * 0.1f, grabbingController);
                else
                {
                    OVRInput.SetControllerVibration(1f, 0f, grabbingController);
                    _isFlushed = true;
                }
            }
        }
    }
}

public enum SyringeState
{
    GRABBED,
    RELEASED,
    PUSHING
}
