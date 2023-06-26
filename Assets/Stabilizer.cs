using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stabilizer : MonoBehaviour
{
    [SerializeField] private Sticker _sticker_one;
    [SerializeField] private Sticker _sticker_two;

    private bool _isAttached = false;
    private bool _isGrabbed = false;

    void Start()
    {
        // gli sticker non sono grabbable quando lo stabilizer non è afferrato
        _sticker_one.DisableGrabbable();
        _sticker_two.DisableGrabbable();

        // la fisica dello stabilizer è disabilitata finché ci sono gli sticker attaccati
        _sticker_one.DisableGravity();
        _sticker_two.DisableGravity();
    }

    void Update()
    {
        if( !_sticker_one.IsPhysicOn() && !_sticker_one.IsAttached() )
        {
            _sticker_one.EnableGravity();
        }

        if( !_sticker_two.IsPhysicOn() && !_sticker_two.IsAttached() )
        {
            _sticker_two.EnableGravity();
        } 
    }

    public void GrabStabilizer()
    {
        _isGrabbed = true;
        // gli sticker diventano Grabbable, quindi abilito il collider
        if (_sticker_one.IsAttached())
            _sticker_one.EnableGrabbable();
        if (_sticker_two.IsAttached())
            _sticker_two.EnableGrabbable();
    }

    public void ReleaseStabilizer()
    {
        _isGrabbed = false;
        // gli sticker diventano not Grabbable, quindi disabilito il collider
        if (_sticker_one.IsAttached())
            _sticker_one.DisableGrabbable();
        if (_sticker_two.IsAttached())
            _sticker_two.DisableGrabbable();
    }

    public bool IsAttached()
    {
        return _isAttached;
    }

    public void AttachToNeedle()
    {
        _isAttached = true;
    }

    public void DisableGravity()
    {
        _sticker_one.DisableGravity();
        _sticker_two.DisableGravity();
    }

    public bool IsGrabbed()
    {
        return _isGrabbed; 
    }
}
