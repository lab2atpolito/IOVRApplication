using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class CottonInteraction : MonoBehaviour
{
    [SerializeField] private Material _cottonMaterial;
    [SerializeField] private float _dryValue;
    [SerializeField] private float _wetValue;

    [SerializeField] private float _absorbingDuration;
    [SerializeField] private float _dryingDuration;

    private bool _isDrying = false;
    private bool _isAbsorbing = false;

    public bool isAbsorbing()
    {
        return _isAbsorbing;
    }

    public float _wetness;
    public bool _correctAngle = false;


    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.name == "Cotton Ball" && _correctAngle && !_isAbsorbing)
        {
            _isAbsorbing = true;
            _isDrying = false;
            float currentValue = _cottonMaterial.GetFloat("_Transition");
            StartCoroutine(Absorbing(currentValue, _wetValue, _absorbingDuration));
        }
    }

    private void Start()
    {
        _cottonMaterial.SetFloat("_Transition", _dryValue);
        _wetness = _dryValue;
    }

    private void Update()
    {
        Vector3 bottleUpDirection = this.transform.up;
        Vector3 worldUpDirection = Vector3.up;
        float angle = Vector3.SignedAngle(worldUpDirection, bottleUpDirection, Vector3.up);
        //Debug.Log(angle);

        _correctAngle = angle < 180f && angle > 90f;
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.name == "Cotton Ball" && _isAbsorbing)
        {
            _isAbsorbing = false; 
            _isDrying = true;
            float currentValue = _cottonMaterial.GetFloat("_Transition");
            StartCoroutine(Drying(currentValue, _dryValue, _dryingDuration));
        }
    }

    private IEnumerator Absorbing(float valueA, float valueB, float duration)
    {
        float timer = 0f;
        while (timer < _absorbingDuration && _isAbsorbing)
        {
            float _currentValue = Mathf.Lerp(valueA, valueB, timer / duration);
            _cottonMaterial.SetFloat("_Transition", _currentValue);
            _wetness = _currentValue;
            yield return null;

            timer += Time.deltaTime;
        }
    }

    private IEnumerator Drying(float valueA, float valueB, float duration)
    {
        float timer = 0f;

        while (timer < duration && _isDrying)
        {
            float _currentValue = Mathf.Lerp(valueA, valueB, timer / duration);
            _cottonMaterial.SetFloat("_Transition", _currentValue);
            _wetness = _currentValue;
            yield return null;

            timer += Time.deltaTime;
        }
    }
}
