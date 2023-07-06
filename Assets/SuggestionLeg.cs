using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuggestionLeg : MonoBehaviour
{
    [SerializeField] private GameObject _legGameObject;
    [SerializeField] private Material _legMaterial;

    [SerializeField, Range(0,10)] private float _transitionDuration;
    [SerializeField] private float _alphaCurrentValue;

    [SerializeField] private float _startOpacityValue;
    [SerializeField] private float _endOpacityValue;

    [SerializeField] private TasksManager _simulation;

    private bool _reachedTarget;

    private void Start()
    {
        _legMaterial.SetFloat("_WristOpacity", _endOpacityValue);
        _reachedTarget = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if( other.gameObject.tag == "LegMover")
        {
            _reachedTarget = true;
            if (_simulation.IsGuideActive())
            {
                DisableLeg();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "LegMover")
        {
            _reachedTarget = false;
            if (_simulation.IsGuideActive())
            {
                EnableLeg();
            }
        }
    }

    public bool HasReachedTarget()
    {
        return _reachedTarget;
    }

    public void EnableLeg()
    {
        StartCoroutine(EnableCoroutine(_startOpacityValue, _endOpacityValue, _transitionDuration));
    }

    public void DisableLeg()
    {
        StartCoroutine(DisableCoroutine(_endOpacityValue, _startOpacityValue, _transitionDuration));
    }

    private IEnumerator DisableCoroutine(float startValue, float endValue, float duration)
    {
        float startTime = Time.time;
        float endTime = startTime + duration;

        while (Time.time < endTime)
        {
            float normalizedTime = (Time.time - startTime) / duration;
            _alphaCurrentValue = Mathf.Lerp(startValue, endValue, normalizedTime);
            _legMaterial.SetFloat("_WristOpacity", _alphaCurrentValue);
            yield return null;
        }

        _alphaCurrentValue = endValue;
        _legGameObject.SetActive(false);
    }

    private IEnumerator EnableCoroutine(float startValue, float endValue, float duration)
    {
        _legGameObject.SetActive(true);

        float startTime = Time.time;
        float endTime = startTime + duration;

        while (Time.time < endTime)
        {
            float normalizedTime = (Time.time - startTime) / duration;
            _alphaCurrentValue = Mathf.Lerp(startValue, endValue, normalizedTime);
            _legMaterial.SetFloat("_WristOpacity", _alphaCurrentValue);
            yield return null;
        }

        _alphaCurrentValue = endValue;
    }
}
