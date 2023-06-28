using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Needle : MonoBehaviour
{

    public NeedleType _type;
    private NeedleState _state;

    private bool _hasSafetyCap = true;

    [SerializeField] private MeshRenderer _meshRenderer;
    public Material _lineMaterial;

    [SerializeField] private PowerDrill _drill;

    private void Start()
    {
        _state = NeedleState.NOTGRABBED;
        Debug.Log(_type + " Needle State: " + _state);

        Material[] materials = _meshRenderer.materials;
        foreach (Material mat in materials)
        {
            if (mat.name == "Ago_2 (Instance)")
            {
                _lineMaterial = mat;
            }
        }
        if (_lineMaterial != null)
            Debug.Log("Materiale trovato!");
        else
            Debug.Log("Materiale non trovato!");
    }

    public void Grab()
    {
        switch (_state)
        {
            case NeedleState.NOTGRABBED:
                _state = NeedleState.GRABBED;
                Debug.Log(_type + " Needle State: "+_state);
                break;
            case NeedleState.SNAPPED:
                _drill.RemoveNeedle();
                _state = NeedleState.GRABBED;
                Debug.Log(_type + " Needle State: " + _state);
                break;
        }
    }

    public void Release()
    {
        if( _state != NeedleState.SNAPPED )
        {
            _state = NeedleState.NOTGRABBED;
            Debug.Log(_type + " Needle State: " + _state);
        }
    }

    public void Snap()
    {
        _drill.InsertNeedle(this);
        _state = NeedleState.SNAPPED;
        Debug.Log(_type + " Needle State: " + _state);
    }

    public void ClearParent()
    {
        this.gameObject.transform.parent = null; 
    }

    internal bool IsGrabbed()
    {
        return _state == NeedleState.GRABBED;
    }

    public NeedleState GetState()
    {
        return _state; 
    }

    public NeedleType GetNeedleType()
    {
        return _type;
    }

    public void RemoveSafetyCap()
    {
        _hasSafetyCap = false;
    }

    public bool HasSafetyCap()
    {
        return _hasSafetyCap;
    }

    public void ShowLine()
    {

        _lineMaterial.SetColor("_Color", Color.white);
        _lineMaterial.EnableKeyword("_EMISSION");
        //_lineMaterial.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
        _lineMaterial.SetColor("_EmissionColor", Color.white);
    }

    public void HideLine()
    {
        _lineMaterial.DisableKeyword("_EMISSION");
        //_lineMaterial.globalIlluminationFlags = MaterialGlobalIlluminationFlags.EmissiveIsBlack;
        _lineMaterial.SetColor("_EmissionColor", Color.black);
        _lineMaterial.SetColor("_Color", Color.white);
    }
}

public enum NeedleType
{
    PINK,
    YELLOW,
    BLUE,
    NULL
}

public enum NeedleState
{
    NOTGRABBED,
    GRABBED,
    SNAPPED,
}
